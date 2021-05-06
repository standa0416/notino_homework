using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using WebApi.Exceptions;
using WebApi.Interfaces.Services;

namespace WebApi.Services
{
    public class FileStorageService : IFileStorageService
    {
        private const string UploadStoragePathCfg = "UploadStoragePath";
        private const string ResultStoragePathCfg = "ResultStoragePath";
        
        private readonly IConfiguration configuration;
        
        public FileStorageService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        
        public async Task<string> SaveUploadedFile(IFormFile file)
        {
            try
            {
                var directoryPath = configuration[UploadStoragePathCfg];
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                directoryPath = directoryPath.EndsWith('\\') ? directoryPath : $"{directoryPath}\\";
                var fileHash = Guid.NewGuid().ToString("N");
                var filePath = $"{directoryPath}{fileHash}{file.FileName}";

                await using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await file.CopyToAsync(fileStream);
                }

                return fileHash;
            }
            catch (Exception e)
            {
                throw new FileUploadException(e.Message);
            }
        }

        public async Task<(string fileName, string fileData)> ReadUploadedFile(string fileHash)
        {
            try
            {
                var directoryPath = configuration[UploadStoragePathCfg];
                if (!Directory.Exists(directoryPath))
                {
                    throw new ServerConfigurationErrorException("Wrong configuration. Upload directory doesn't exist");
                }

                var filePath = Directory.GetFiles(directoryPath)
                                   .SingleOrDefault(x => Path.GetFileName(x).StartsWith(fileHash))
                               ?? throw new FileNotFoundException();

                var data = await File.ReadAllTextAsync(filePath);

                var fileName = Path.GetFileName(filePath).Substring(fileHash.Length);
                
                return (fileName, data);
            }
            catch (Exception e)
            {
                throw new FileLoadException(e.Message);
            }
        }

        public async Task SaveResultFile(string fileNameWithoutExtension, string fileHash, string fileData)
        {
            try
            {
                var destinationPath = configuration[ResultStoragePathCfg];
                if (!Directory.Exists(destinationPath))
                {
                    Directory.CreateDirectory(destinationPath);
                }

                var destinationFilePath = Path.Combine(destinationPath, $"{fileHash}{fileNameWithoutExtension}.json");

                await using (var fileStream = new StreamWriter(destinationFilePath))
                {
                    await fileStream.WriteAsync(fileData);
                    await fileStream.FlushAsync();
                }
            }
            catch (Exception e)
            {
                throw new FileStorageException(e.Message);
            }
        }

        public (string fileName, Stream stream) GetResultFileStream(string fileHash)
        {
            try
            {
                var directoryPath = configuration[ResultStoragePathCfg];
                if (!Directory.Exists(directoryPath))
                {
                    throw new ServerConfigurationErrorException("Wrong configuration. Result directory doesn't exist");
                }

                var filePath = Directory.GetFiles(directoryPath)
                                   .SingleOrDefault(x => Path.GetFileName(x).StartsWith(fileHash))
                               ?? throw new FileNotFoundException();

                var fileName = Path.GetFileName(filePath).Substring(fileHash.Length);
                
                var stream = new FileStream(filePath, FileMode.Open);
                
                return (fileName, stream);

            }
            catch (Exception e)
            {
                throw new FileStorageException(e.Message);
            }
        }
    }
}