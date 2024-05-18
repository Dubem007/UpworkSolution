using MediatR;
using Microsoft.AspNetCore.Identity;
using OnaxTools.Dto.Http;
using UserServices.Models.DTOs;

namespace UserServices.AppCore.CQRS.Commands
{
    public class CreateProfileCommand : IRequest<GenResponse<string>>
    {
        public CreateProfileCommand(CreateUserProfile entity)
        {

            this.Entity = entity;
        }

        public CreateUserProfile Entity { get; private set; }
    }
}
