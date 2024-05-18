using Common.Extensions;
using MediatR;
using OnaxTools.Dto.Http;
using UserService.AppCore.CQRS.Querys;
using UserServices.AppCore.Repository.Interfaces;
using UserServices.Models;
using UserServices.Models.DTOs;
using UserServices.Services;

namespace UserServices.AppCore.CQRS.Handlers.QueryHandlers
{
    public class GetConnectionHistoriesByUserCommandHandler : IRequestHandler<GetConnectionHistoriesByUserCommand, GenResponse<List<ConnectsHistoryDto>>>
    {
        private readonly ILogger<GetConnectionHistoriesByUserCommandHandler> logger;
        private readonly IProfileRepository _profileRepo;
        private readonly IMediator mediator;

        public GetConnectionHistoriesByUserCommandHandler(ILogger<GetConnectionHistoriesByUserCommandHandler> logger, IProfileRepository profileRepo, IMediator mediator)
        {
            this.logger = logger;
            _profileRepo = profileRepo;
            this.mediator = mediator;
        }

        public async Task<GenResponse<List<ConnectsHistoryDto>>> Handle(GetConnectionHistoriesByUserCommand request, CancellationToken cancellationToken)
        {
            GenResponse<List<ConnectsHistoryDto>> objResp = new();
            try
            {
                var resp = await _profileRepo.GetConnectionHistoriesByUser();
                if (resp.IsSuccess != null)
                {
                    objResp.IsSuccess = true;
                    objResp.Result = resp.Result;
                    objResp.Message = AppConstants.DataRetrieveSuccessResponse;
                }
                else
                {
                    objResp.IsSuccess = false;
                    objResp.Result = resp.Result;
                    objResp.Message = AppConstants.FailedRequest;
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
