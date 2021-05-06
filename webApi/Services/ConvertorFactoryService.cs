using System;
using System.IO;
using WebApi.Exceptions;
using WebApi.Interfaces.Services;

namespace WebApi.Services
{
    public class ConvertorFactoryService : IConvertorFactoryService
    {
        private readonly IServiceProvider serviceProvider;
        
        public ConvertorFactoryService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        
        public IConvertor GetInstance(string fileName)
        {
            var fileExtension = Path.GetExtension(fileName);

            switch (fileExtension)
            {
                case ".xml":
                    return (IConvertor)serviceProvider.GetService(typeof(XmlToJsonConvertorService));
                case ".bin":
                    throw new NotImplementedException("Conversion from Protobuf is not supported so far");
                default:
                    throw new UnsupportedFileExtension($"Extension '.{fileExtension}' is not supported");
            }
        }
    }
}