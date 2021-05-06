using System;

namespace WebApi.Exceptions
{
    public class UnsupportedFileExtension : Exception
    {
        public UnsupportedFileExtension(string message) : base(message)
        {
        }
    }
}