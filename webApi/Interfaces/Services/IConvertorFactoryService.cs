using System.Net.Mime;

namespace WebApi.Interfaces.Services
{
    public interface IConvertorFactoryService
    {
        IConvertor GetInstance(string fileName);
    }
}