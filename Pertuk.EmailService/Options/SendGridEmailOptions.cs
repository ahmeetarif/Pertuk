namespace Pertuk.EmailService.Options
{
    public class SendGridEmailOptions
    {
        public string ApiKey { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
    }
}