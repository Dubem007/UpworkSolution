using Common.Extensions;
using MediatR;
using OnaxTools.Dto.Http;
using UserService.Services;
using UserServices.AppCore.CQRS.Commands;
using UserServices.Models.DTOs;
using UserServices.Services;

namespace UserServices.AppCore.CQRS.Handlers.CommandHandlers
{
    public class EmailValidateCommandHandler : IRequestHandler<EmailValidateCommand, GenResponse<bool>>
    {
        private readonly ILogger<EmailValidateCommandHandler> logger;
        private readonly IUserServicess _UserServices;
        private readonly IMediator mediator;

        public EmailValidateCommandHandler(ILogger<EmailValidateCommandHandler> logger, IUserServicess UserServices, IMediator mediator)
        {
            this.logger = logger;
            _UserServices = UserServices;
            this.mediator = mediator;
        }

        public async Task<GenResponse<bool>> Handle(EmailValidateCommand request, CancellationToken cancellationToken)
        {
            GenResponse<bool> objResp = new();
            try
            {
                var resp = await _UserServices.ValidateEmail(request.Entity);
                if (resp)
                {
                    objResp.IsSuccess = true;
                    objResp.Result = resp;
                    objResp.Message = AppConstants.ValidateEmailSuccessResponse;
                }
                else {
                    objResp.IsSuccess = false;
                    objResp.Result = resp;
                    objResp.Message = AppConstants.ValidateEmailFailedResponse;
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
