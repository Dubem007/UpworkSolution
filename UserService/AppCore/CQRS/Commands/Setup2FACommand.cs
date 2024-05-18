using MediatR;
using Microsoft.AspNetCore.Identity;
using OnaxTools.Dto.Http;
using UserServices.Models.DTOs;

namespace UserServices.AppCore.CQRS.Commands
{
    public class Setup2FACommand : IRequest<GenResponse<string>>
    {
        public Setup2FACommand()
        {

        }

    }
}
