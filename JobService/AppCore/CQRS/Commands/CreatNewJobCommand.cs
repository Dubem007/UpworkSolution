using IdentityServer.Models;
using IdentityServer.Models.DTOs;
using JobService.Domain.Dtos;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OnaxTools.Dto.Http;

namespace JobService.AppCore.CQRS.Commands
{
    public class CreatNewJobCommand : IRequest<GenResponse<string>>
    {
        public CreatNewJobCommand(CreateJobDto entity)
        {

            this.Entity = entity;
        }

        public CreateJobDto Entity { get; private set; }
    }
}
