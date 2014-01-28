using System;

namespace Internode.WebTools.Domain.Exceptions
{
    /// <summary>
    /// Exception is returned when authentication with the Internode service API fails becaues
    /// of an invalid logon.
    /// </summary>
    public class ServiceAuthenticationException : Exception
    { }
}
