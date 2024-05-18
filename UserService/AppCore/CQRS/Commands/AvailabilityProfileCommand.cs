using MediatR;
using Microsoft.AspNetCore.Identity;
using OnaxTools.Dto.Http;
using UserServices.Models.DTOs;

namespace UserServices.AppCore.CQRS.Commands
{
    public class AvailabilityProfileCommand : IRequest<GenResponse<string>>
    {
        public AvailabilityProfileCommand(string connects, bool activateAvailability)
        {

            this.Connects = connects;
            this.ActivateAvailability = activateAvailability;
        }

        public string Connects { get; private set; }
        public bool ActivateAvailability { get; private set; }
    }
}
