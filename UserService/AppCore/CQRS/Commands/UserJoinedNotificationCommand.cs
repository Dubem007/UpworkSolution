
using IdentityModel;
using MediatR;
using OnaxTools.Dto.Http;
using UserServices.Models.DTOs;

namespace UserServices.AppCore.CQRS.Commands
{
    public class UserJoinedNotificationCommand : IRequest<GenResponse<bool>>
    {
        public UserJoinedNotificationCommand(string entity)
        {
            this.Entity = entity;
        }

        public string Entity { get; private set; }
    }
}
