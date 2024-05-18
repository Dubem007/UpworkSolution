using Common.Extensions;
using MediatR;
using OnaxTools.Dto.Http;
using UserServices.AppCore.CQRS.Commands;
using UserServices.AppCore.Repository.Interfaces;
using UserServices.Models.DTOs;
using UserServices.Services;

namespace UserServices.AppCore.CQRS.Handlers.CommandHandlers
{
    public class Verify2FACommandHandler : IRequestHandler<Verify2FACommand, GenResponse<bool>>
    {
        private readonly ILogger<Verify2FACommandHandler> logger;
        private readonly IUserRepository _userRepo;
        private readonly IMediator mediator;

        public Verify2FACommandHandler(ILogger<Verify2FACommandHandler> logger, IUserRepository userRepo, IMediator mediator)
        {
            this.logger = logger;
            _userRepo = userRepo;
            this.mediator = mediator;
        }

        public async Task<GenResponse<bool>> Handle(Verify2FACommand request, CancellationToken cancellationToken)
        {
            GenResponse<bool> objResp = new();
            try
            {
                var resp = await _userRepo.Verify2FAForUser(request.Otp);
                if (resp.IsSuccess != null)
                {
                    objResp.IsSuccess = true;
                    objResp.Result = resp.Result;
                    objResp.Message = AppConstants.Verify2FASuccessful;
                }
                else {
                    objResp.IsSuccess = false;
                    objResp.Result = resp.Result;
                    objResp.Message = AppConstants.FailedVerify2FA;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(exception: ex, ex.Message);
                objResp.IsSuccess = false;
                objResp.Result = false;
                objResp.Message = $"{AppConstants.FailedRequestError} {ex.Message}";
            }
            return objResp;
        }
    }
}
