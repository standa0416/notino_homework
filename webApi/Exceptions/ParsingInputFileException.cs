using System;

namespace WebApi.Exceptions
{
    public class ParsingInputFileException : Exception
    {
        public ParsingInputFileException(string message) : base(message)
        {
        }
    }
}