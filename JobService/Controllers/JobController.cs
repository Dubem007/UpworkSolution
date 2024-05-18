using Common.Response;
using JobService.AppCore.CQRS.Commands;
using JobService.AppCore.CQRS.Querys;
using JobService.Domain.Dtos;
using JobService.Extension.Middleware;
using JobService.Models.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnaxTools.Dto.Http;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace JobService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly ILogger<JobController> _logger;
        private readonly IMediator _mediator;

        public JobController(ILogger<JobController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }



        [HttpPost(nameof(GetAllJobsByClientUserId))]
        [AuthAttribute(nameof(UserRolesEnum.Freelancer), nameof(UserRolesEnum.Client))]
        [ProducesResponseType(typeof(GenResponse<List<UpworkJobsDto>>), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Summary = "θD> Get all jobs by user")]
        public async Task<IActionResult> GetAllJobsByClientUserId()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ModelState);
                }
                var getAllJobsByClientUserIdCommandRequest = new GetAllJobsByClientUserIdCommand();
                var getAllJobsByClientUserIdCommandResponse = await _mediator.Send(getAllJobsByClientUserIdCommandRequest);

                return Ok(getAllJobsByClientUserIdCommandResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"The get job by user with error: {ex.Message}");
                return Ok(ex);
            }

        }

        [HttpPost(nameof(GetAllJobsBySkill))]
        [AuthAttribute(nameof(UserRolesEnum.Freelancer), nameof(UserRolesEnum.Client))]
        [ProducesResponseType(typeof(GenResponse<List<UpworkJobsDto>>), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Summary = "θD> Get all jobs by skill")]
        public async Task<IActionResult> GetAllJobsBySkill()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ModelState);
                }
                var getAllJobsBySkillCommandRequest = new GetAllJobsBySkillCommand();
                var getAllJobsBySkillCommandResponse = await _mediator.Send(getAllJobsBySkillCommandRequest);

                return Ok(getAllJobsBySkillCommandResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"The get all job by skill with error: {ex.Message}");
                return Ok(ex);
            }

        }

        [HttpPost(nameof(GetAllJobsDrafts))]
        [AuthAttribute(nameof(UserRolesEnum.Freelancer), nameof(UserRolesEnum.Client))]
        [ProducesResponseType(typeof(GenResponse<List<UpworkJobsDto>>), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Summary = "θD> Get all drafts by user")]
        public async Task<IActionResult> GetAllJobsDrafts()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ModelState);
                }
                var gGetAllJobsDraftsCommandRequest = new GetAllJobsDraftsCommand();
                var getAllJobsDraftsCommandRequestResponse = await _mediator.Send(gGetAllJobsDraftsCommandRequest);

                return Ok(getAllJobsDraftsCommandRequestResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"The get drafts by user with error: {ex.Message}");
                return Ok(ex);
            }

        }

        [HttpPost(nameof(GetAllJobsPosts))]
        [AuthAttribute(nameof(UserRolesEnum.Freelancer), nameof(UserRolesEnum.Client))]
        [ProducesResponseType(typeof(GenResponse<List<UpworkJobsDto>>), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Summary = "θD> Get all job posts by user")]
        public async Task<IActionResult> GetAllJobsPosts()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ModelState);
                }
                var getAllJobsPostsCommandRequest = new GetAllJobsPostsCommand();
                var getAllJobsPostsCommandResponse = await _mediator.Send(getAllJobsPostsCommandRequest);

                return Ok(getAllJobsPostsCommandResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"The get job posts by user with error: {ex.Message}");
                return Ok(ex);
            }

        }

        [HttpGet(nameof(GetAllJobsById))]
        [AuthAttribute(nameof(UserRolesEnum.Freelancer), nameof(UserRolesEnum.Client))]
        [ProducesResponseType(typeof(GenResponse<QueryResponse<IEnumerable<UpworkJobsDto>>>), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Summary = "θD> Get job by Id")]
        public async Task<IActionResult> GetAllJobsById(Guid jobId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ModelState);
                }
                var getAllJobsByIdCommandRequest = new GetAllJobsByIdCommand(jobId);
                var getAllJobsByIdCommandResponse = await _mediator.Send(getAllJobsByIdCommandRequest);

                return Ok(getAllJobsByIdCommandResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"The get job by id with error: {ex.Message}");
                return Ok(ex);
            }

        }

        [HttpPost(nameof(CreatNewJob))]
        [AuthAttribute(nameof(UserRolesEnum.Freelancer), nameof(UserRolesEnum.Client))]
        [ProducesResponseType(typeof(GenResponse<QueryResponse<string>>), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Summary = "θD> Create new job")]
        public async Task<IActionResult> CreatNewJob(CreateJobDto entity)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ModelState);
                }
                var creatNewJobCommandRequest = new CreatNewJobCommand(entity);
                var creatNewJobCommandResponse = await _mediator.Send(creatNewJobCommandRequest);

                return Ok(creatNewJobCommandResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"The create new job  with error: {ex.Message}");
                return Ok(ex);
            }

        }
    }
}

