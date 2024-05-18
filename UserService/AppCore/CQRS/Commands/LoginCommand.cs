
using MediatR;
using OnaxTools.Dto.Http;
using UserServices.Models.DTOs;

namespace UserServices.AppCore.CQRS.Commands
{
    public class LoginCommand : IRequest<GenResponse<TokenDTO>>
    {
        public LoginCommand(LoginInputDto entity)
        {
            this.Entity = entity;
        }

        public LoginInputDto Entity { get; private set; }
    }
}
