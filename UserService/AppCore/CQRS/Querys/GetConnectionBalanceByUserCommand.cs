using MediatR;
using Microsoft.AspNetCore.Identity;
using OnaxTools.Dto.Http;
using UserServices.Models.DTOs;

namespace UserService.AppCore.CQRS.Querys
{
    public class GetConnectionBalanceByUserCommand : IRequest<GenResponse<string>>
    {
        public GetConnectionBalanceByUserCommand()
        {
        }

    }
}
