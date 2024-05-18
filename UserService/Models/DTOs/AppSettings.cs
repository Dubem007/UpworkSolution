
namespace UserServices.Domain.DTOs
{
    public class AppSettings
    {
        public string AppName { get; set; }
        public string Issuer { get; set; }
        public string IdentityBaseUrl { get; set; }
        public MsgQueue MsgQueue { get; set; }
        public MessagesExpiryDurationInSeconds MessagesExpiryDurationInSeconds { get; set; }
        public bool UseCRMMock { get; set; }
        public string MockEmail { get; set; }
        public bool IsLocalUpload { get; set; }
        public bool IsAWSUpload { get; set; }
        public int SecretKeyLength { get; set; }
        public string SecretKey { get; set; }
        public string Audience { get; set; }
        public string Granttype { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ClientScope { get; set; }
    }


    public class MsgQueue
    {
        public int DelayInMilliseconds { get; set; }
        public bool IsAutoAcknowledged { get; set; }
    }
    public class MessagesExpiryDurationInSeconds
    {
        public int CorpCode { get; set; }
    }

}
