using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using Internode.WebTools.Pcl.Exceptions;

namespace Internode.WebTools.Pcl
{
    public class InternodeCustomerApiClient
    {
        private readonly HttpClient client;

        public InternodeCustomerApiClient(string username, string password)
        {
            var credentials = new NetworkCredential(username, password);
            var handler = new HttpClientHandler { Credentials = credentials };

            client = new HttpClient(handler) {
                BaseAddress = new Uri("https://customer-webtools-api.internode.on.net/")
            };

            // Initialise properties
            Services = new List<InternodeService>();
            ServiceResources = new Dictionary<string, List<InternodeServiceResource>>();

        }

        public List<InternodeService> Services { get; private set; }

        public Dictionary<string, List<InternodeServiceResource>> ServiceResources { get; set; }



        // Public API

        public void QueryForServices()
        {
            //var response = GetAsync("api/v1.5/").Result;

            //OnResponseReceived(response);

            var xdoc = ReadEndpoint("api/v1.5/"); //XDocument.Load(response.Content.ReadAsStreamAsync().Result);

            var services = xdoc.Descendants("service").ToList();

            // ToDo: Extract data from services
            foreach (var service in services)
            {
                Services.Add(new InternodeService(
                    service.Attribute("type").Value,
                    service.Value,
                    service.Attribute("href").Value));
            }
        }

        public AdslServiceInfo GetAdslServiceInfo(string serviceId)
        {
            var adslService = Services.Single(s => s.ServiceId == serviceId && s.ServiceType == ServiceType.PersonalAdsl);
            if (adslService == null)
            {
                throw new ServiceNotFoundException();
            }

            GetServiceResources(adslService);

            var infoEndpoint = ServiceResources[serviceId].Single(sr => sr.Type == "service").Endpoint;

            var xdoc = ReadEndpoint(infoEndpoint);

            var serviceInfo = xdoc.Descendants("service").Single();

            var result = new AdslServiceInfo
            {
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

        private void GetServiceResources(InternodeService service)
        {
            if (ServiceResources.ContainsKey(service.ServiceId)) return;        // Already got the data, don't query again.

            var xdoc = ReadEndpoint(service.ServiceEndpoint);

            var resources = xdoc.Descendants("resource");

            var serviceResources = new List<InternodeServiceResource>();
            foreach (var resource in resources)
            {
                serviceResources.Add(new InternodeServiceResource {
                    Type = resource.Attribute("type").Value,
                    Endpoint = resource.Attribute("href").Value
                });
            }
            
            ServiceResources.Add(service.ServiceId, serviceResources);
        }



        private XDocument ReadEndpoint(string endpointAddress)
        {
            HttpResponseMessage response;
            try
            {
                response = GetAsync(endpointAddress).Result;
                OnResponseReceived(response);
            }
            catch (ServerErrorException)
            {
                // Retry (once)
                response = GetAsync(endpointAddress).Result;
                OnResponseReceived(response);
            }

            return XDocument.Load(response.Content.ReadAsStreamAsync().Result);
        }

        private Task<HttpResponseMessage> GetAsync(string uri)
        {
            return client.GetAsync(uri);
        }

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
