using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Internode.WebTools.Domain.Adapters;
using Internode.WebTools.Domain.Exceptions;
using Internode.WebTools.Domain.Models;
using RestSharp;

namespace Internode.WebTools.Domain.Services
{
    public class CustomerApiService
    {

        private const string BaseUri = "https://customer-webtools-api.internode.on.net/";
        private readonly RestClient _client;

        private List<InternodeService> _services;

        public CustomerApiService() {
            _client = new RestClient {
                BaseUrl = BaseUri,
                UserAgent = "IntranetUsageMeter/0.01 (in development)"
            };
        }

        public CustomerApiService(string username, string password)
            : this() {
            Username = username;
            Password = password;
            _client.Authenticator = new HttpBasicAuthenticator(Username, Password);
        }


        public string Username { get; set; }
        public string Password { get; set; }

        public InternodeService AdslService { get; set; }
        public AdslServiceInfo AdslServiceInfo { get; set; }


        public IList<InternodeService> QueryForServices() {
            var request = new RestRequest { Resource = "api/v1.5/" };
            var response = _client.Execute<InternodeServicesModel>(request);

            OnResponseReceived(response);

            _services = InternodeServicesAdapter.FromInternodeServiceModel(response.Data.Services).ToList();

            AdslService = _services.FirstOrDefault(s => s.ServiceType == "Personal_ADSL");

            return _services;
        }

        public AdslServiceInfo GetAdslPlan()
        {
            if (AdslService == null) return null;

            var request = new RestRequest(AdslService.ServiceEndpoint + "/service");
            var response = _client.Execute<AdslServiceInfoModel>(request);

            OnResponseReceived(response);
            
            AdslServiceInfo = ServiceAdapter.FromAdslServiceModel(response.Data);
            return AdslServiceInfo;
        }

        public ServiceUsage GetServiceUsage(InternodeService service) {
            if (service == null) {
                throw new ArgumentNullException("service", "No service was specified for the usage request.");
            }

            var request = new RestRequest(service.ServiceEndpoint + "/usage");
            var response = _client.Execute<List<Traffic>>(request);

            OnResponseReceived(response);

            return UsageAdapter.FromServiceUsageModel(response.Data);

        }


        private void OnResponseReceived(IRestResponse response) {
            if (response == null) throw new ArgumentNullException("response");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new ServiceAuthenticationException();

            if (response.StatusCode == HttpStatusCode.InternalServerError) {
                // Get more details on the error... Don't appear to get the XML error information as indicated by the Internode specs...
                if (response.Content.Contains("<error>"))
                {
                    // Only throw an exception if there was actually an error being returned... in most(all?) instances
                    // the correct content was still returned even though the '500' InternalServerError was returned.
                    throw new ServiceAccessException("Error calling service");    
                }
            }


            if (response.StatusCode != HttpStatusCode.OK) {
                throw new ApplicationException("Did not get a response from the services endpoint.");
            }
        }

    }
}
