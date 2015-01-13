using System;
using System.Collections.Generic;
using System.Linq;
using Internode.WebTools.Pcl;
using Internode.WebTools.Pcl.Exceptions;
using Con = System.Console;

namespace Internode.WebTools.Console
{
    class Program
    {

        private static List<InternodeService> services; 

        static void Main(string[] args)
        {
            Con.Write("Username: ");
            var username = System.Console.ReadLine();
            Con.Write("Password: ");
            var password = System.Console.ReadLine();

            IInternodeCustomerApiClient pcl = new InternodeCustomerApiClient();
            pcl.SetCredentials(username, password);

            try
            {
                // Perform a '.Result' here, as we can't perform an 'await' from a Console application.
                // Note that Result will always throw an AggregateException rather than a specific
                // exception - we have to unwrap this exception to find out the actual error.
                services = pcl.QueryForServicesAsync().Result;
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions[0] is AuthenticationException)
                {
                    Con.WriteLine("** Authentication failure **");
                    Con.ReadKey(false);
                    return;
                }
                
            }

            Con.WriteLine("Services for this account:");
            foreach (var internodeService in services)
            {
                Con.WriteLine("Type: {0}, ServiceId: {1}", internodeService.ServiceType, internodeService.ServiceId);
            }


            if (!services.Any())
            {
                Con.WriteLine("No services to enumerate!");
                Con.ReadKey(false);
                return;
            }

            var firstServiceId = services.First().ServiceId;
            try
            {
                var adslService = pcl.GetAdslServiceInfoAsync(firstServiceId).Result;

                if (adslService != null)
                {
                    System.Console.WriteLine("ADSL:\r\n Plan: {0}\r\n Quota: {1:N0}Gb\r\n Rollover: {2:d}\r\n Speed: {3}",
                                             adslService.Plan, adslService.Quota / 1000000000,
                                             adslService.Rollover, adslService.Speed);


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

        }
    }
}
