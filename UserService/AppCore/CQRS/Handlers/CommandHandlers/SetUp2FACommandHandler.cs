using Common.Extensions;
using MediatR;
using OnaxTools.Dto.Http;
using UserServices.AppCore.CQRS.Commands;
using UserServices.AppCore.Repository.Interfaces;
using UserServices.Models.DTOs;
using UserServices.Services;

namespace UserServices.AppCore.CQRS.Handlers.CommandHandlers
{
    public class SetUp2FACommandHandler : IRequestHandler<Setup2FACommand, GenResponse<string>>
    {
        private readonly ILogger<SetUp2FACommandHandler> logger;
        private readonly IUserRepository _userRepo;
        private readonly IMediator mediator;

        public SetUp2FACommandHandler(ILogger<SetUp2FACommandHandler> logger, IUserRepository userRepo, IMediator mediator)
        {
            this.logger = logger;
            _userRepo = userRepo;
            this.mediator = mediator;
        }

        public async Task<GenResponse<string>> Handle(Setup2FACommand request, CancellationToken cancellationToken)
        {
            GenResponse<string> objResp = new();
            try
            {
                var resp = await _userRepo.Setup2FAForUser();
                if (resp.IsSuccess != null)
                {
                    objResp.IsSuccess = true;
                    objResp.Result = resp.Result;
                    objResp.Message = AppConstants.CreationSuccessResponse;
                }
                else {
                    objResp.IsSuccess = false;
                    objResp.Result = resp.Result;
                    objResp.Message = AppConstants.FailedRequest;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(exception: ex, ex.Message);
                objResp.IsSuccess = false;
                objResp.Result = null;
                objResp.Message = $"{AppConstants.FailedRequestError} {ex.Message}";
            }
            return objResp;
        }
    }
}
