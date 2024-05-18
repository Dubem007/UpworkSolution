
using MediatR;
using OnaxTools.Dto.Http;
using UserServices.Models.DTOs;

namespace UserServices.AppCore.CQRS.Commands
{
    public class EmailValidateCommand : IRequest<GenResponse<bool>>
    {
        public EmailValidateCommand(string entity)
        {
            this.Entity = entity;
        }

        public string Entity { get; private set; }
    }
}
