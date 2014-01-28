using System;

namespace Internode.WebTools.Domain.Exceptions
{
    /// <summary>
    /// Exception raised when a request to the Intenode API fails. Details of the failure are returned
    /// in the Error property.
    /// </summary>
    public class ServiceAccessException : Exception
    {
        internal ServiceAccessException(string error)
        {
            Error = error;
        }

        public string Error { get; set; }

    }
}
