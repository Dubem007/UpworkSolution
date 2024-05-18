using OnaxTools.Dto.Http;
using UserServices.Models.DTOs;

namespace UserServices.AppCore.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<GenResponse<ContactDetailsDto>> GetContactDetails();
        Task<GenResponse<string>> Setup2FAForUser();
        Task<GenResponse<bool>> Verify2FAForUser(string otp);
        Task<GenResponse<bool>> CloseMyAccount();
    }
}
