
namespace JobService.Domain.DTOs
{
    public class AppSettings
    {
        public string AppName { get; set; }
        public string Issuer { get; set; }
        public MsgQueue MsgQueue { get; set; }
        public MessagesExpiryDurationInSeconds MessagesExpiryDurationInSeconds { get; set; }
        public bool UseCRMMock { get; set; }
        public string MockEmail { get; set; }
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
