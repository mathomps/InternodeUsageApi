using System;
using System.Linq;
using Internode.WebTools.Domain.Exceptions;
using Internode.WebTools.Domain.Services;
using Internode.WebTools.Pcl;
using Internode.WebTools.Pcl.Exceptions;
using Con = System.Console;

namespace Internode.WebTools.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Con.Write("Username: ");
            var username = System.Console.ReadLine();
            Con.Write("Password: ");
            var password = System.Console.ReadLine();

            var pcl = new InternodeCustomerApiClient();
            pcl.SetCredentials(username, password);

            try
            {
                // Perform a 'Wait' here, as we can't perform an 'await' from a Console application.
                // Note that Wait() will always throw an AggregateException rather than a specific
                // exception - we have to unwrap this exception to find out the actual error.
                pcl.QueryForServices().Wait();
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions[0].InnerException is AuthenticationException)
                {
                    Con.WriteLine("** Authentication failure **");
                    Con.ReadKey(false);
                    return;
                }
                
            }

            Con.WriteLine("Services for this account:");
            foreach (var internodeService in pcl.Services)
            {
                Con.WriteLine("Type: {0}, ServiceId: {1}", internodeService.ServiceType, internodeService.ServiceId);
            }


            if (!pcl.Services.Any())
            {
                Con.WriteLine("No services to enumerate!");
                Con.ReadKey(false);
                return;
            }

            var firstServiceId = pcl.Services.First().ServiceId;
            try
            {
                var adslService = pcl.GetAdslServiceInfo(firstServiceId).Result;

                if (adslService != null)
                {
                    System.Console.WriteLine("ADSL:: Quota: {0}, Rollover: {1:d}, Speed: {2}",
                                         adslService.Quota / 1000000, adslService.Rollover, adslService.Speed);


                    //var usage = pcl.GetServiceUsage(service.AdslService);
                    //System.Console.WriteLine("Used: {0:n0}Mb, remaining: {1:n0}Mb, {2}% Remaining",
                    //                         usage.MegabytesUsed, usage.MegabytesRemaining, usage.PercentRemaining);

                    //Con.WriteLine("Speed: {0}", adslService.Speed );
                    //Con.WriteLine("Rollover Date: {0:d}", adslService.Rollover);
                }

            }
            catch (Exception ex)
            {
                Con.WriteLine(ex.Data);
            }

            Con.WriteLine("\r\nAll done!");
            Con.ReadKey(false);
            return;


            var service = new CustomerApiService("username", "password");

            try
            {
                service.QueryForServices();
            }
            catch (ServiceAuthenticationException)
            {

            }
            catch (ServiceAccessException)
            {
                System.Console.WriteLine("** Internal Server Error **");
            }


            if (service.AdslService != null)
            {
                System.Console.WriteLine("Service endpoint: {0}, Service ID: {1}", service.AdslService.ServiceEndpoint, service.AdslService.ServiceId);
            }


            var serviceInfo = service.GetAdslPlan();

            if (serviceInfo != null)
            {
                System.Console.WriteLine("ADSL:: Quota: {0}, Rollover: {1:d}, Speed: {2}",
                                         serviceInfo.Quota / 1000000, serviceInfo.Rollover, serviceInfo.Speed);


                var usage = service.GetServiceUsage(service.AdslService);
                System.Console.WriteLine("Used: {0:n0}Mb, remaining: {1:n0}Mb, {2}% Remaining",
                                         usage.MegabytesUsed, usage.MegabytesRemaining, usage.PercentRemaining);

            }

            System.Console.WriteLine("Press any key...");
            System.Console.ReadKey(true);

        }
    }
}
