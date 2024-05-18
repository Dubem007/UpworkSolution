using NotificationService.Models.DTO;

namespace NotificationService.Services.Interface
{
    public interface IEmailService
    {
        Task<bool> MailSendNew(EmailModelsDTo payload);
    }
}
