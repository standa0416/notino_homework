using System.Security.AccessControl;

namespace WebApi.Model
{
    public class SendEmailRequest
    {
        public string From { get; set; }
        public string FromName { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtml { get; set; }
        public string FileHash { get; set; }
    }
}