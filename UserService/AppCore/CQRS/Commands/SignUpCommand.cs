using MediatR;
using Microsoft.AspNetCore.Identity;
using OnaxTools.Dto.Http;
using UserServices.Models.DTOs;

namespace UserServices.AppCore.CQRS.Commands
{
    public class SignUpCommand : IRequest<GenResponse<IdentityResult>>
    {
        public SignUpCommand(RegisterDto entity)
        {

            this.Entity = entity;
        }

        public RegisterDto Entity { get; private set; }
    }
}
