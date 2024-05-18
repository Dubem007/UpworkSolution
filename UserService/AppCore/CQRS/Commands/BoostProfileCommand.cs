using MediatR;
using Microsoft.AspNetCore.Identity;
using OnaxTools.Dto.Http;
using UserServices.Models.DTOs;

namespace UserServices.AppCore.CQRS.Commands
{
    public class BoostProfileCommand : IRequest<GenResponse<string>>
    {
        public BoostProfileCommand(BoostProfileDto entity)
        {

            this.Entity = entity;
        }

        public BoostProfileDto Entity { get; private set; }
    }
}
