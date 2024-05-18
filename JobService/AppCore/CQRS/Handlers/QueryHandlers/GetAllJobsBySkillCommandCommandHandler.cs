using Common.Extensions;
using Common.Response;
using IdentityServer.Models.DTOs;
using JobService.AppCore.CQRS.Querys;
using JobService.AppCore.Repository.Interface;
using JobService.Domain.Dtos;
using MediatR;
using OnaxTools.Dto.Http;

namespace JobService.AppCore.CQRS.Handlers.CommandHandlers
{
    public class GetAllJobsBySkillCommandCommandHandler : IRequestHandler<GetAllJobsBySkillCommand, GenResponse<List<UpworkJobsDto>>>
    {
        private readonly ILogger<GetAllJobsBySkillCommandCommandHandler> logger;
        private readonly IJobRepository _jobRepo;
        private readonly IMediator mediator;

        public GetAllJobsBySkillCommandCommandHandler(ILogger<GetAllJobsBySkillCommandCommandHandler> logger, IJobRepository jobRepo, IMediator mediator)
        {
            this.logger = logger;
            this.mediator = mediator;
            _jobRepo = jobRepo;
        }

        public async Task<GenResponse<List<UpworkJobsDto>>> Handle(GetAllJobsBySkillCommand request, CancellationToken cancellationToken)
        {
            GenResponse<List<UpworkJobsDto>> objResp = new();
            try
            {
                var resp = await _jobRepo.GetAllJobsBySkill();
                if (resp.IsSuccess)
                {
                    objResp.IsSuccess = true;
                    objResp.Result = resp.Result;
                    objResp.Message = AppConstants.DataRetrieveSuccessResponse;
                }
                else
                {
                    objResp.IsSuccess = false;
                    objResp.Result = resp.Result;
                    objResp.Message = AppConstants.RecordNotFound;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(exception: ex, ex.Message);
                objResp.IsSuccess = false;
                objResp.Result = new List<UpworkJobsDto>();
                objResp.Message = $"{AppConstants.FailedRequestError} {ex.Message}";
            }
            return objResp;
        }
    }
}
