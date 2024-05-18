using Common.Response;
using IdentityServer.Models.DTOs;
using JobService.Domain.Dtos;
using MediatR;
using OnaxTools.Dto.Http;

namespace JobService.AppCore.CQRS.Querys
{
    public class GetAllJobsPostsCommand : IRequest<GenResponse<List<UpworkJobsDto>>>
    {
        public GetAllJobsPostsCommand()
        {
        }

    }
}
