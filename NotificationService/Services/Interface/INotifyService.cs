using OnaxTools.Dto.Http;

namespace NotificationService.Services.Interface
{
    public interface INotifyService
    {
        Task<GenResponse<bool>> InAppCommAsync(string payload);
    }
}
