using System;
using Internode.WebTools.Domain.Exceptions;
using Internode.WebTools.Domain.Services;

namespace Internode.WebTools.Console
{
    class Program
    {
        static void Main(string[] args)
        {

            var service = new CustomerApiService("username", "password");

            try
            {
                service.QueryForServices();
            }
            catch (ServiceAuthenticationException)
            {
                System.Console.WriteLine("** Authentication failure **");
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
