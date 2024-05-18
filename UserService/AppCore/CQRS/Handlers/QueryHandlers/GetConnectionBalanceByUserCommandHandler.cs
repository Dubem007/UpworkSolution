using Common.Extensions;
using MediatR;
using OnaxTools.Dto.Http;
using UserService.AppCore.CQRS.Querys;
using UserServices.AppCore.Repository.Interfaces;
using UserServices.Models.DTOs;
using UserServices.Services;

namespace UserServices.AppCore.CQRS.Handlers.QueryHandlers
{
    public class GetConnectionBalanceByUserCommandHandler : IRequestHandler<GetConnectionBalanceByUserCommand, GenResponse<string>>
    {
        private readonly ILogger<GetConnectionBalanceByUserCommandHandler> logger;
        private readonly IProfileRepository _profileRepo;
        private readonly IMediator mediator;

        public GetConnectionBalanceByUserCommandHandler(ILogger<GetConnectionBalanceByUserCommandHandler> logger, IProfileRepository profileRepo, IMediator mediator)
        {
            this.logger = logger;
            _profileRepo = profileRepo;
            this.mediator = mediator;
        }

        public async Task<GenResponse<string>> Handle(GetConnectionBalanceByUserCommand request, CancellationToken cancellationToken)
        {
            GenResponse<string> objResp = new();
            try
            {
                var resp = await _profileRepo.GetConnectionBalanceByUser();
                if (resp.IsSuccess != null)
                {
                    objResp.IsSuccess = true;
                    objResp.Result = resp.Result;
                    objResp.Message = AppConstants.CreationSuccessResponse;
                }
                else
                {
                    objResp.IsSuccess = false;
                    objResp.Result = resp.Result;
                    objResp.Message = AppConstants.CreationFailedResponse;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(exception: ex, ex.Message);
                objResp.IsSuccess = false;
                objResp.Result = null;
                objResp.Message = $"{AppConstants.FailedRequestError} {ex.Message}";
            }
            return objResp;
        }
    }
}
