using IdentityServer.Models.DTOs;
using JobService.Domain.Dtos;
using MediatR;
using OnaxTools.Dto.Http;

namespace JobService.AppCore.CQRS.Querys
{
    public class GetAllJobsByIdCommand : IRequest<GenResponse<UpworkJobsDto>>
    {
        public GetAllJobsByIdCommand(Guid jobId)
        {

            this.JobId = jobId;
        }

        public Guid JobId { get; private set; }
    }
}
