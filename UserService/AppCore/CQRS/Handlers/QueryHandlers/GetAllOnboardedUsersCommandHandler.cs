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
    public class GetAllOnboardedUsersCommandHandler : IRequestHandler<GetAllOnboardedUsersCommand, GenResponse<List<ApplicationUserDto>>>
    {
        private readonly ILogger<GetAllOnboardedUsersCommandHandler> logger;
        private readonly IProfileRepository _profileRepo;
        private readonly IMediator mediator;

        public GetAllOnboardedUsersCommandHandler(ILogger<GetAllOnboardedUsersCommandHandler> logger, IProfileRepository profileRepo, IMediator mediator)
        {
            this.logger = logger;
            _profileRepo = profileRepo;
            this.mediator = mediator;
        }

        public async Task<GenResponse<List<ApplicationUserDto>>> Handle(GetAllOnboardedUsersCommand request, CancellationToken cancellationToken)
        {
            GenResponse<List<ApplicationUserDto>> objResp = new();
            try
            {
                var resp = await _profileRepo.GetAllOnboardedUsers();
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
