using Internode.WebTools.Pcl.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Internode.WebTools.Pcl
{
    public class InternodeCustomerApiClient : IInternodeCustomerApiClient
    {
        private HttpClient client;

        public InternodeCustomerApiClient()
        {
            

            // Initialise properties
            Services = new List<InternodeService>();
            ServiceResources = new Dictionary<string, IEnumerable<InternodeServiceResource>>();

        }

        public void SetCredentials(string username, string password)
        {
            var credentials = new NetworkCredential(username, password);
            var handler = new HttpClientHandler { Credentials = credentials };

            client = new HttpClient(handler) {
                BaseAddress = new Uri("https://customer-webtools-api.internode.on.net/")
            };
        }

        public List<InternodeService> Services { get; private set; }

        public Dictionary<string, IEnumerable<InternodeServiceResource>> ServiceResources { get; set; }



        // Public API

        public async Task QueryForServices()
        {
            var xdoc = await ReadEndpoint("api/v1.5/");

            var services = xdoc.Descendants("service").ToList();

            foreach (var service in services)
            {
                Services.Add(new InternodeService(
                    service.Attribute("type").Value,
                    service.Value,
                    service.Attribute("href").Value));
            }
        }

        public async Task<AdslServiceInfo> GetAdslServiceInfo(string serviceId)
        {
            var adslService = Services.Single(s => s.ServiceId == serviceId &&
                                                   s.ServiceType == ServiceType.PersonalAdsl);
            if (adslService == null)
            {
                throw new ServiceNotFoundException();
            }

            await GetServiceResources(adslService);

            var infoEndpoint = ServiceResources[serviceId].Single(sr => sr.Type == "service").Endpoint;

            var xdoc = await ReadEndpoint(infoEndpoint);

            var serviceInfo = xdoc.Descendants("service").Single();

            var result = new AdslServiceInfo {
                Id = int.Parse(serviceInfo.Descendants("id").Single().Value),
                Username = serviceInfo.Descendants("username").Single().Value,
                Quota = long.Parse(serviceInfo.Descendants("quota").Single().Value),
                Plan = serviceInfo.Descendants("plan").Single().Value,
                Carrier = serviceInfo.Descendants("carrier").Single().Value,
                Speed = serviceInfo.Descendants("speed").Single().Value,
                UsageRating = serviceInfo.Descendants("usage-rating").Single().Value,
                Rollover = DateTime.Parse(serviceInfo.Descendants("rollover").Single().Value),

                ExcessCharged = serviceInfo.Descendants("excess-charged").Single().Value,
                ExcessShaped = serviceInfo.Descendants("excess-shaped").Single().Value,
                ExcessRestrictAccess = serviceInfo.Descendants("excess-restrict-access").Single().Value,
                PlanInterval = serviceInfo.Descendants("plan-interval").Single().Value,
                PlanCost = serviceInfo.Descendants("plan-cost").Single().Value,
            };

            // 'excess-cost' will only be present if an excess is charged!
            if (result.ExcessCharged == "yes")
            {
                result.ExcessCost = serviceInfo.Descendants("excess-cost").Single().Value;
            }

            return result;

        }

        private async Task GetServiceResources(InternodeService service)
        {
            if (ServiceResources.ContainsKey(service.ServiceId)) return;        // Already got the data, don't query again.

            var xdoc = await ReadEndpoint(service.ServiceEndpoint);

            var resources = xdoc.Descendants("resource");

            var serviceResources = resources.Select(resource => new InternodeServiceResource {
                Type = resource.Attribute("type").Value,
                Endpoint = resource.Attribute("href").Value
            });

            ServiceResources.Add(service.ServiceId, serviceResources);
        }



        private async Task<XDocument> ReadEndpoint(string endpointAddress)
        {
            HttpResponseMessage response;
            try
            {
                response = await GetResponseMessageAsync(endpointAddress);
            }
            catch (ServerErrorException)
            {
                // Retry (once)
                var re = new ManualResetEvent(initialState: true);
                re.WaitOne(5000);
                response = GetResponseMessageAsync(endpointAddress).Result;
            }

            var resultStream = await response.Content.ReadAsStreamAsync();
            return XDocument.Load(resultStream);
        }

        private async Task<HttpResponseMessage> GetResponseMessageAsync(string uri)
        {
            var response = await client.GetAsync(uri);
            OnResponseReceived(response);
            return response;
        }

        /// <summary>
        /// Automatically throw exceptions if the response is not valid for any reason.
        /// </summary>
        /// <param name="response"></param>
        private void OnResponseReceived(HttpResponseMessage response)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    throw new AuthenticationException("Authorisation failed");

                case HttpStatusCode.InternalServerError:
                    throw new ServerErrorException("Internal Server error");

            }

            // Generic catch-all for status codes other than 200 (OK)
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }

        }

    }

    public class InternodeServiceResource
    {
        public string Type { get; set; }
        public string Endpoint { get; set; }
    }
}
