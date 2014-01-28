using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Internode.WebTools.Domain.Models
{
    /// <summary>
    /// Model to hold error information returned from a Service call when there is a failure.
    /// </summary>
    class ErrorMessageModel
    {
        public string Msg { get; set; }
    }
}
