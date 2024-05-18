
using IdentityModel;
using MediatR;
using OnaxTools.Dto.Http;
using UserServices.Models.DTOs;

namespace UserServices.AppCore.CQRS.Commands
{
    public class UserNotificationCommand : IRequest<GenResponse<bool>>
    {
        public UserNotificationCommand(EmaiReqsDto entity)
        {
            this.Entity = entity;
        }

        public EmaiReqsDto Entity { get; private set; }
    }
}
