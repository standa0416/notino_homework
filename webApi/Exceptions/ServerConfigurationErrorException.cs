using System;

namespace WebApi.Exceptions
{
    public class ServerConfigurationErrorException : Exception
    {
        public ServerConfigurationErrorException(string message) : base(message)
        {
        }
    }
}