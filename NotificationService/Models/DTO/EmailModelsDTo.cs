using UserService.Models.Enums;

namespace NotificationService.Models.DTO
{
    public class EmailModelsDTo
    {
        public string appId { get; set; }
        public string appReference { get; set; }
        public string senderName { get; set; }
        public string senderEmail { get; set; }
        public string recipientEmails { get; set; }
        public string cc { get; set; }
        public string bcc { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
        public bool IsTransactional { get; set; }
        public Attachments[] attachments { get; set; }
        public bool priority { get; set; }
        public EmailTypesEnum EmailType { get; set; }
    }
    public class Attachments
    {
        public string fileName { get; set; }
        public string base64string { get; set; }
    }


    public class EmailSentResponseDto
    {
        public string status { get; set; }
        public string message { get; set; }
    }


    public class EmaiReqsDto
    {
        public string RecipientEmails { get; set; }
        public string RecipientName { get; set; }
        public string SenderName { get; set; }
        public string Subject { get; set; }
        public string SenderEmail { get; set; }
        public EmailTypesEnum EmailType { get; set; }
        public string? UserId { get; set; }
    }
}
