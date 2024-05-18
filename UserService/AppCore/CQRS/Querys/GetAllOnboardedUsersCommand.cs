using MediatR;
using Microsoft.AspNetCore.Identity;
using OnaxTools.Dto.Http;
using UserServices.Models;
using UserServices.Models.DTOs;

namespace UserService.AppCore.CQRS.Querys
{
    public class GetAllOnboardedUsersCommand : IRequest<GenResponse<List<ApplicationUserDto>>>
    {
        public GetAllOnboardedUsersCommand()
        {
        }

    }
}
