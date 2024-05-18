using MediatR;
using OnaxTools.Dto.Http;
using UserServices.Models.DTOs;

namespace UserServices.AppCore.CQRS.Commands
{
    public class PushQueueCommand : IRequest<GenResponse<bool>>
    {
        public PushQueueCommand(AppUserDto entity)
        {

            this.Entity = entity;
        }

        public AppUserDto Entity { get; private set; }
    }
}
