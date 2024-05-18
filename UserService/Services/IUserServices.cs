using IdentityModel.Client;
using System.Threading.Tasks;
using UserServices.Models.DTOs;

namespace UserService.Services
{
    public interface IUserServicess
    {
        Task<TokenDTO> GetLoginToken(LoginInputDto entity);
        Task<TokenResponse> GetToken(string scope, string Username, string password);
        Task<bool> AddUserToRole(string userId, string roleId);
        Task<bool> ValidateEmail(string token);
    }
}
