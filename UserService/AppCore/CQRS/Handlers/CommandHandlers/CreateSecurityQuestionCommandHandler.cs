using Common.Extensions;
using MediatR;
using OnaxTools.Dto.Http;
using UserServices.AppCore.CQRS.Commands;
using UserServices.AppCore.Repository.Interfaces;
using UserServices.Models.DTOs;
using UserServices.Services;

namespace UserServices.AppCore.CQRS.Handlers.CommandHandlers
{
    public class CreateSecurityQuestionCommandHandler : IRequestHandler<CreateSecurityQuestionCommand, GenResponse<string>>
    {
        private readonly ILogger<CreateSecurityQuestionCommandHandler> logger;
        private readonly IProfileRepository _profileRepo;
        private readonly IMediator mediator;

        public CreateSecurityQuestionCommandHandler(ILogger<CreateSecurityQuestionCommandHandler> logger, IProfileRepository profileRepo, IMediator mediator)
        {
            this.logger = logger;
            _profileRepo = profileRepo;
            this.mediator = mediator;
        }

        public async Task<GenResponse<string>> Handle(CreateSecurityQuestionCommand request, CancellationToken cancellationToken)
        {
            GenResponse<string> objResp = new();
            try
            {
                var resp = await _profileRepo.CreateSecurityQuestion(request.Entity);
                if (resp.IsSuccess != null)
                {
                    objResp.IsSuccess = true;
                    objResp.Result = resp.Result;
                    objResp.Message = AppConstants.CreationSuccessResponse;
                }
                else {
                    objResp.IsSuccess = false;
                    objResp.Result = resp.Result;
                    objResp.Message = AppConstants.CreationFailedResponse;
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
