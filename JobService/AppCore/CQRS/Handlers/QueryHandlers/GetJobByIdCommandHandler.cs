using Common.Extensions;
using IdentityServer.Models.DTOs;
using JobService.AppCore.CQRS.Querys;
using JobService.AppCore.Repository.Interface;
using JobService.Domain.Dtos;
using MediatR;
using OnaxTools.Dto.Http;

namespace JobService.AppCore.CQRS.Handlers.CommandHandlers
{
    public class GetJobByIdCommandHandler : IRequestHandler<GetAllJobsByIdCommand, GenResponse<UpworkJobsDto>>
    {
        private readonly ILogger<GetJobByIdCommandHandler> _logger;
        private readonly IJobRepository _jobRepo;
        private readonly IMediator _mediator;

        public GetJobByIdCommandHandler(ILogger<GetJobByIdCommandHandler> logger, IJobRepository jobRepo, IMediator mediator)
        {
            _logger = logger;
            _jobRepo = jobRepo;
            _mediator = mediator;
        }

        public async Task<GenResponse<UpworkJobsDto>> Handle(GetAllJobsByIdCommand request, CancellationToken cancellationToken)
        {
            GenResponse<UpworkJobsDto> objResp = new();
            try
            {
                var resp = await _jobRepo.GetJobById(request.JobId);
                if (resp.IsSuccess)
                {
                    objResp.IsSuccess = true;
                    objResp.Result = resp.Result;
                    objResp.Message = AppConstants.DataRetrieveSuccessResponse;
                }
                else {
                    objResp.IsSuccess = false;
                    objResp.Result = resp.Result;
                    objResp.Message = AppConstants.RecordNotFound;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(exception: ex, ex.Message);
                objResp.IsSuccess = false;
                objResp.Result = new UpworkJobsDto();
                objResp.Message = $"{AppConstants.FailedRequestError} {ex.Message}";
            }
            return objResp;
        }
    }
}
