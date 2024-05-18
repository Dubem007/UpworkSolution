
using MediatR;
using OnaxTools.Dto.Http;
using UserServices.Models.DTOs;

namespace UserServices.AppCore.CQRS.Commands
{
    public class UploadIdentityCommand : IRequest<GenResponse<string>>
    {
        public UploadIdentityCommand(IdentityDto entity)
        {
            this.Entity = entity;
        }

        public IdentityDto Entity { get; private set; }
    }
}
