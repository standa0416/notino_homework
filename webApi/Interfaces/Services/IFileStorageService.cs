using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebApi.Interfaces.Services
{
    public interface IFileStorageService
    {
        Task<string> SaveUploadedFile(IFormFile file);

        Task<(string fileName, string fileData)> ReadUploadedFile(string fileHash);

        Task SaveResultFile(string fileNameWithoutExtension, string fileHash, string fileData);
        
        (string fileName, Stream stream) GetResultFileStream(string fileHash);
    }
}