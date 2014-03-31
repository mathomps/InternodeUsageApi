using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Internode.WebTools.Pcl
{
    public class ServiceUsage
    {
        public long MeteredBytes { get; set; }
        public long UnmeteredBytes { get; set; }
        public long TotalBytes { get; set; }
        public string PlanInterval { get; set; }
        public long Quota { get; set; }

        // Calculated Properties (for ease of retrieval)

        public int MegabytesUsed
        {
            get { return (int)(TotalBytes / 1000000); }
        }

        public int MegabytesRemaining
        {
            get { return (int)((Quota - TotalBytes) / 1000000); }
        }
        public int PercentUsed
        {
            get { return (int)(TotalBytes * 100 / Quota); }
        }

        public int PercentRemaining
        {
            get { return 100 - PercentUsed; }
        }

    }
}
