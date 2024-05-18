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
    public class GetAllJobsDraftsCommandHandler : IRequestHandler<GetAllJobsDraftsCommand, GenResponse<List<UpworkJobsDto>>>
    {
        private readonly ILogger<GetAllJobsDraftsCommandHandler> _logger;
        private readonly IJobRepository _jobRepo;
        private readonly IMediator _mediator;

        public GetAllJobsDraftsCommandHandler(ILogger<GetAllJobsDraftsCommandHandler> logger, IJobRepository jobRepo, IMediator mediator)
        {
            _logger = logger;
            _jobRepo = jobRepo;
            _mediator = mediator;
        }

        public async Task<GenResponse<List<UpworkJobsDto>>> Handle(GetAllJobsDraftsCommand request, CancellationToken cancellationToken)
        {
            GenResponse<List<UpworkJobsDto>> objResp = new();
            try
            {
                var resp = await _jobRepo.GetAllJobsDrafts();
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
                _logger.LogError(exception: ex, ex.Message);
                objResp.IsSuccess = false;
                objResp.Result = new List<UpworkJobsDto>();
                objResp.Message = $"{AppConstants.FailedRequestError} {ex.Message}";
            }
            return objResp;
        }
    }
}
