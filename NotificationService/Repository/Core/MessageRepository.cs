using Common.Enums;
using Common.Extensions;
using Common.MessageQueue.Interfaces;
using Common.MessageQueue.Services;
using Common.services.Caching;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using NotificationService.Models.DTO;
using NotificationService.Services.EmailServices;
using NotificationService.Services.Interface;
using OnaxTools.Dto.Http;
using OnaxTools.Enums.Http;
using System.Linq.Expressions;
using System.Net;
using System.Numerics;
using System.Text;
using UserService.Models.Enums;

namespace NotificationService.Repository
{
    public class MessageRepository: IMessageRepository
    {
        private readonly ILogger<MessageRepository> _logger;
        private readonly IMessageQueueService _messageQueueService;
        private readonly IEmailService _emailService;

        public MessageRepository(ILogger<MessageRepository> logger,IMessageQueueService messageQueueService,
            IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
            _messageQueueService = _messageQueueService;
        }
        private string AllJobsByUser(string acct, string action = "_AllJobsByUser") => $"{nameof(AllJobsByUser)}{action}:{acct}";

        public async Task<GenResponse<bool>> SendEmailAsync(EmaiReqsDto model)
        {
            GenResponse<bool> objResp = new();
            var resp = new EmailModelsDTo();
            try
            {
                switch (model.EmailType)
                {
                    case EmailTypesEnum.VerifyEmail:
                        resp = await SendVerifyEmailObjects(model);
                        break;
                    case EmailTypesEnum.VerifyIdentity:
                        resp = await SendVerifyIdentityEmailObjects(model);
                        break;
                    default:
                        // code block
                        break;
                }
                objResp.Result = await _emailService.MailSendNew(resp);
                if (objResp.Result)
                {
                    objResp.IsSuccess = true;
                }
                else 
                {
                    objResp.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return GenResponse<bool>.Failed($"An error occured sending mail, with type {model.EmailType}. Kindly retry", StatusCodeEnum.ServerError);
            }
            return objResp;

        }

        public async Task<EmailModelsDTo> SendVerifyIdentityEmailObjects(EmaiReqsDto info)
        {
            EmailModelsDTo objResp = new();
            try
            {
                var mailBody = GetEmailMessageFromFile("EmailBody.html");
                string mailBody2 = @"<tr>
                    <td class=""yiv4106203373card-row"">As part of our ongoing efforts to promote trust and protect your security, we now require active freelancers to obtain an Identity Verified badge. This badge appears as a checkmark beside your profile name and shows clients that you are a trusted Upwork member who is accurately representing themselves in our marketplace.</td>
                </tr>
                <tr>
                    <td class=""yiv4106203373card-row"">Get your Identity Verified badge</td>
                </tr>
                <tr>
                    <td class=""yiv4106203373card-row"">You will need to complete a two-step verification process to get your badge and start working with clients:</td>
                </tr>
                <tr>
                    <td class=""yiv4106203373card-row"">
                        <table border=""0"" width=""100%"" cellspacing=""0"" cellpadding=""0"">
                            <tbody>
                                <tr>
                                    <td>1.</td>
                                    <td>Provide us with a valid government-issued photo ID</td>
                                </tr>
                                <tr>
                                    <td>2.</td>
                                    <td>Complete visual verification to confirm you are the same person as shown on your ID</td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class=""yiv4106203373card-row"">&nbsp;</td>
                </tr>
                <tr>
                    <td class=""yiv4106203373card-row"">
                        <table border=""0"" width=""100%"" cellspacing=""0"" cellpadding=""0"">
                            <tbody>
                                <tr>
                                    <td>
                                        <div class=""yiv4106203373button-holder""><a href=""https://localhost:7182/identity/UploadIdentity"" target =""_blank"" rel=""nofollow noopener noreferrer"">Verify Your Identity</a></div>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>";

                string footerMessage = @"<tr>
                    <td class=""yiv4106203373card-row"">Please note, you'll need to complete this process before sending new proposals, communicating with clients, obtaining jobs, and withdrawing funds.</td>
                </tr>
                <tr>
                    <td class=""yiv4106203373card-row"">Thanks for helping to keep Upwork secure!</td>
                </tr>
                <tr>
                    <td class=""yiv4106203373card-row"">Regards,<br />The Upwork Team</td>
                </tr>";

                mailBody = mailBody.Replace("{Title}", "Dear " + info.RecipientName + ",");
                mailBody = mailBody.Replace("{MailBody}", mailBody2);
                mailBody = mailBody.Replace("{FooterMessage}", footerMessage);
                mailBody = mailBody.Replace("{Year}", DateTime.Now.Year.ToString());

                objResp.recipientEmails = info.RecipientEmails;
                objResp.senderName = info.SenderName;
                objResp.subject = info.Subject;
                objResp.body = mailBody;
                objResp.cc = "";
                objResp.attachments = new Attachments[]
                {
                    new Attachments { fileName = "", base64string = "" }
                };
                objResp.priority = true;
            }
            catch (Exception ex1)
            {
                _logger.LogError($"Reg-GetOnboardingEmailObject-{info.RecipientEmails}", ex1);
            }
            return objResp;

        }

        public async Task<EmailModelsDTo> SendVerifyEmailObjects(EmaiReqsDto info)
        {
            EmailModelsDTo objResp = new();
            try
            {
                var mailBody = GetEmailMessageFromFile("EmailBody.html");
                string mailBody2 = @"<tr>
                        <td class=""yiv0212603425card-row"">
                            <h2>Verify your email address to complete registration</h2>
                        </td>
                    </tr>
                    <tr>
                        <td class=""yiv0212603425card-row"">Hi Dubem,</td>
                    </tr>
                    <tr>
                        <td class=""yiv0212603425card-row"">Thanks for your interest in joining Upwork! To complete your registration, we need you to verify your email address.</td>
                    </tr>
                    <tr>
                        <td class=""yiv0212603425card-row"">
                            <table border=""0"" width=""100%"" cellspacing=""0"" cellpadding=""0"">
                                <tbody>
                                    <tr>
                                        <td>
                                            <div class=""yiv0212603425button-holder""><a href=""https://localhost:7182/identity/verifyemail/{userId}"" target=""_blank"" rel=""nofollow noopener noreferrer"">Verify Email</a></div>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </td>
                    </tr>";

                string footerMessage = @"<tr>
                            <td class=""yiv0212603425card-row"">Please note that not all applications to join Upwork are accepted. We will notify you of our decision by email within 24 hours.</td>
                        </tr>
                        <tr>
                            <td class=""yiv0212603425card-row"">
                                <div>Thanks for your time,<br />The Upwork Team</div>
                            </td>
                        </tr>";

                mailBody2 = mailBody2.Replace("{userId}", info.UserId);
                mailBody = mailBody.Replace("{Title}", "Dear " + info.RecipientName + ",");
                mailBody = mailBody.Replace("{MailBody}", mailBody2);
                mailBody = mailBody.Replace("{FooterMessage}", footerMessage);
                mailBody = mailBody.Replace("{Year}", DateTime.Now.Year.ToString());

                objResp.recipientEmails = info.RecipientEmails;
                objResp.senderName = info.SenderName;
                objResp.subject = info.Subject;
                objResp.body = mailBody;
                objResp.cc = "";
                objResp.attachments = new Attachments[]
                {
                    new Attachments { fileName = "", base64string = "" }
                };
                objResp.priority = true;
            }
            catch (Exception ex1)
            {
                _logger.LogError($"Reg-GetOnboardingEmailObject-{info.RecipientEmails}", ex1);
            }
            return objResp;

        }

        #region Helpers 
        private string GetEmailMessageFromFile(string filename)
        {
            try
            {
                var emailStatement = new StringBuilder();
                var path = System.IO.Directory.GetCurrentDirectory();
                string message = Path.Combine(path, "EmailTemplate", filename);
                var msgLines = File.ReadAllLines(message);
                foreach (var item in msgLines)
                {
                    emailStatement.Append(item);
                }
                return emailStatement.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return null;
            }
        }
        #endregion
    }
}
