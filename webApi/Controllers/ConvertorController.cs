using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebApi.Exceptions;
using WebApi.Interfaces.Services;
using WebApi.Model;

namespace WebApi.Controllers
{
    // normally I wouldn't put any logic to controllers. In real application I would introduce providers,
    // where I put all the logic, but since this is just demonstration I keep the logic here.

    [ApiController]
    [Route("convertor")]
    public class ConvertorController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IConvertorFactoryService convertorFactoryService;
        private readonly IFileStorageService fileStorageService;
        
        public ConvertorController(IConfiguration configuration, IConvertorFactoryService convertorFactoryService, IFileStorageService fileStorageService)
        {
            this.configuration = configuration;
            this.convertorFactoryService = convertorFactoryService;
            this.fileStorageService = fileStorageService;
        }
        
        [HttpPost("file/upload")]
        public async Task<IActionResult> UploadFile([FromForm]IFormFile file)
        {
            try
            {
                var fileHash = await fileStorageService.SaveUploadedFile(file);
                return Ok(new FileUploadResponse { FileHash = fileHash });
            }
            // Exception and error handling and responses could be more precise, but I hope, that for demonstration purpose it is ok
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut("convert")]
        public async Task<IActionResult> Convert([FromQuery] string fileHash)
        {
            try
            {
                var uploadedFile = await fileStorageService.ReadUploadedFile(fileHash);

                var convertor = convertorFactoryService.GetInstance(uploadedFile.fileName);
                var convertedData = convertor.Convert(uploadedFile.fileData);

                await fileStorageService.SaveResultFile(Path.GetFileNameWithoutExtension(uploadedFile.fileName),
                    fileHash, convertedData);

                return Ok();
            }
            catch (ParsingInputFileException e)
            {
                return BadRequest(e.Message);
            }
            catch (UnsupportedFileExtension)
            {
                return new UnsupportedMediaTypeResult();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        
        [HttpGet("file/download")]
        public IActionResult DownloadFile([FromQuery] string fileHash)
        {
            try
            {
                var result = fileStorageService.GetResultFileStream(fileHash);
                return new FileStreamResult(result.stream, "application/json")
                {
                    FileDownloadName = result.fileName
                };
            }
            // Exception and error handling and responses could be more precise, but I hope, that for demonstration purpose it is ok
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        
        // Just an example, much better would be to create microservice and send messsage with email request to
        // that service, e.g using RabbitMq
        [HttpPost("file/send-email")]
        public IActionResult SendEmail([FromBody] SendEmailRequest request)
        {
            try
            {
                var fileToSend = fileStorageService.GetResultFileStream(request.FileHash);
                
                var server = configuration["Smtp:Server"];
                var port = configuration["Smtp:Port"];
                var fromAddress = configuration["Smtp:FromAddress"];
                var userName = configuration["Smtp:UserName"];
                var password = configuration["Smtp:Password"];
            
                var mailClient = new SmtpClient(server);
                mailClient.Credentials = new NetworkCredential(userName, password);
                mailClient.Port = int.Parse(port);

                var message = new MailMessage();
                if (!string.IsNullOrEmpty(request.From))
                {
                    message.From = new MailAddress(request.From, request.FromName);
                }
                else
                {
                    message.From = new MailAddress("me@you.com");
                }

                message.To.Add(new MailAddress(request.To));

                if (!string.IsNullOrEmpty(request.Cc))
                {
                    message.CC.Add(request.Cc);
                }

                if (!string.IsNullOrEmpty(request.Bcc))
                {
                    message.Bcc.Add(request.Bcc);
                }

                message.Subject = request.Subject;
                message.Body = request.Body;
                message.IsBodyHtml = request.IsHtml;
                message.Attachments.Add(new Attachment(fileToSend.stream, fileToSend.fileName, "application/json"));
                
                mailClient.Send(message);

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            } 
        }
    }
}