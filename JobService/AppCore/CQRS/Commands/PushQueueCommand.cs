using IdentityServer.Models;
using IdentityServer.Models.DTOs;
using MediatR;
using OnaxTools.Dto.Http;

namespace JobService.AppCore.CQRS.Commands
{
    public class PushQueueCommand : IRequest<GenResponse<bool>>
    {
        public PushQueueCommand(JobStatusUpdateDto entity)
        {

            this.Entity = entity;
        }

        public JobStatusUpdateDto Entity { get; private set; }
    }
}
