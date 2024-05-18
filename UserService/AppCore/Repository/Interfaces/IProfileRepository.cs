using OnaxTools.Dto.Http;
using UserServices.Models;
using UserServices.Models.DTOs;

namespace UserServices.AppCore.Repository.Interfaces
{
    public interface IProfileRepository
    {
        Task<GenResponse<string>> CreateUserProfile(CreateUserProfile entity);
        Task<GenResponse<string>> BoostProfile(BoostProfileDto entity);
        Task<GenResponse<string>> AvailabilityProfile(string connects, bool activateAvailability);
        Task<GenResponse<string>> GetConnectionBalanceByUser();
        Task<GenResponse<List<ConnectsHistoryDto>>> GetConnectionHistoriesByUser();
        Task<GenResponse<string>> GetAvailabilityProfileByUser();
        Task<GenResponse<string>> CreateSecurityQuestion(SecurityQuestionDto entity);
        Task<GenResponse<List<ApplicationUserDto>>> GetAllOnboardedUsers();
    }
}
