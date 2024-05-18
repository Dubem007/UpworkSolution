using NotificationService.Models.DTO;
using OnaxTools.Dto.Http;

namespace NotificationService.Repository
{
    public interface IMessageRepository
    {
        Task<GenResponse<bool>> SendEmailAsync(EmaiReqsDto model);
    }
}
