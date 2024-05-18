using MediatR;
using Microsoft.AspNetCore.Identity;
using OnaxTools.Dto.Http;
using UserServices.Models.DTOs;

namespace UserServices.AppCore.CQRS.Commands
{
    public class Verify2FACommand : IRequest<GenResponse<bool>>
    {
        public Verify2FACommand(string otp)
        {

            this.Otp = otp;
        }

        public string Otp { get; private set; }
    }
}
