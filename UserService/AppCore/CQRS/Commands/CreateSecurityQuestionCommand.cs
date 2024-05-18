using MediatR;
using Microsoft.AspNetCore.Identity;
using OnaxTools.Dto.Http;
using UserServices.Models.DTOs;

namespace UserServices.AppCore.CQRS.Commands
{
    public class CreateSecurityQuestionCommand : IRequest<GenResponse<string>>
    {
        public CreateSecurityQuestionCommand(SecurityQuestionDto entity)
        {

            this.Entity = entity;
        }

        public SecurityQuestionDto Entity { get; private set; }
    }
}
