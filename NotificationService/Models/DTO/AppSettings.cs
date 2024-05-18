using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Models.DTO
{
    public class AppSettings
    {
        public string AppName { get; set; }
        public string Issuer { get; set; }
        public string AppKey { get; set; }
        public string NetCoreEmailServer { get; set; }
        public string NetCoreEmailServerPort { get; set; }
        public string NetCoreEmailId { get; set; }
        public string NetCoreEmailPassword { get; set; }

        public bool BypassCertvalidation { get; set; }
        public bool NetCoreEmailEnableSsl { get; set; }

        public bool IsNetCoreMail { get; set; }
        public bool ActivateMiddleware { get; set; }
        public MessagesExpiryDurationInSeconds MessagesExpiryDurationInSeconds { get; set; }
        public MsgQueue MsgQueue { get; set; }
        public AccessBankService AccessBankService { get; set; }
    }

    public class MessagesExpiryDurationInSeconds
    {
        public int CorpCode { get; set; }
    }

    public class MsgQueue
    {
        public int DelayInMilliseconds { get; set; }
        public bool IsAutoAcknowledged { get; set; }
    }
    public class AccessBankService{

        public string AppId { get; set; }
        public string AppReference { get; set; }
    }
}
