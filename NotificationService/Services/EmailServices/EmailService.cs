using Microsoft.Extensions.Options;
using NotificationService.Models.DTO;
using NotificationService.Services.Interface;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace NotificationService.Services.EmailServices
{
    public class EmailService: IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly string _fromEmail;
        private readonly IConfiguration _config;
        private readonly AppSettings _appSettings;
        private readonly EmailConfig _emailConfig;

        public EmailService(ILogger<EmailService> logger, IConfiguration config, IOptions<AppSettings> appSettings, IOptions<EmailConfig> emailConfig)
        {
            _logger = logger;
            _config = config;
            _appSettings = appSettings.Value;
            _emailConfig = emailConfig.Value;
        }


        public async Task<bool> MailSendNew(EmailModelsDTo payload)
        {

            if (string.IsNullOrEmpty(payload.recipientEmails) || string.IsNullOrEmpty(payload.subject) || string.IsNullOrEmpty(payload.body))
            {
                _logger.LogInformation($"Parameter cannot be null - | to_address : {payload.recipientEmails} | subject : {payload.subject} | message : {payload.body}");
            }
            else
            {
                try
                {
                    _logger.LogInformation("MailSend");

                    SmtpClient MyServer = new SmtpClient(_appSettings.NetCoreEmailServer, Convert.ToInt32(_appSettings.NetCoreEmailServerPort))
                    {
                        Credentials = new NetworkCredential(_appSettings.NetCoreEmailId, _appSettings.NetCoreEmailPassword),
                        EnableSsl = _appSettings.NetCoreEmailEnableSsl,
                    };


                    if (_appSettings.NetCoreEmailEnableSsl)
                    {
                        var bypass = _appSettings.BypassCertvalidation;
                        var isTest = bypass;
                        var store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
                        store.Open(OpenFlags.ReadOnly);
                        ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) =>
                        {

                            if (isTest) return true; // for development, trust all certificates
                            return sslPolicyErrors == SslPolicyErrors.None
                                 || store.Certificates.Contains(certificate); // Compliant: trust only some certificates
                        };

                    }

                    MailAddress from = new MailAddress("noreply@accessbankplc.com", "Upwork service");
                    MailAddress receiver = new MailAddress(payload.recipientEmails);
                    MailMessage Mymessage = new MailMessage(from, receiver);
                    Mymessage.Subject = payload.subject;
                    Mymessage.Body = payload.body;
                    Mymessage.Sender = new MailAddress("noreply@accessbankplc.com", "Upwork service");
                    //sends the email
                    Mymessage.IsBodyHtml = true;
                    payload.attachments = new Attachments[]
                    {
                    new Attachments { fileName = "", base64string = "" }
                    };
                    payload.priority = true;
                    if (payload.cc != null && payload.cc.Length > 0)
                    {
                        Mymessage.CC.Add(payload.cc);
                    }

                    MyServer.Send(Mymessage);
                    _logger.LogInformation($"Mail Sent to {payload.recipientEmails} successfully- NETCOREMAIL | Subject : {payload.subject}");
                    return true;
                }
                catch (Exception ex)
                {
                    //Error occured, log and return failed response
                    var msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    _logger.LogInformation(ex.Message, ex);
                    _logger.LogInformation($"Failed to send Mail via MailSendNew - NETCOREMAIL to {payload.recipientEmails} | Subject : {payload.subject}, MailSendNew Exception: {msg}");
                    _logger.LogInformation(MethodBase.GetCurrentMethod().Name, ex);
                    return false;
                }

            }
            ////////////////////////////////////////////
            return false;
        }
    }
}
