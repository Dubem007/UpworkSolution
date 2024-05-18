using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnaxTools.Dto.Http;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using UserService.AppCore.CQRS.Querys;
using UserServices.AppCore.CQRS.Commands;
using UserServices.Extension.Middleware;
using UserServices.Models;
using UserServices.Models.DTOs;
using UserServices.Models.Enums;
using UserServices.Services;

namespace UserServices.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize("ApiScope")]
    public class ProfileController : ControllerBase
    {
        private readonly ILogger<ProfileController> _logger;
        private readonly IMediator _mediator;

        public ProfileController(ILogger<ProfileController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        

        [HttpGet(nameof(GetAvailabilityProfileByUser))]
        [AuthAttribute(nameof(UserRolesEnum.Freelancer), nameof(UserRolesEnum.Client))]
        [ProducesResponseType(typeof(GenResponse<string>), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Summary = "θD> Get Availability user profile")]
        public async Task<IActionResult> GetAvailabilityProfileByUser()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ModelState);
                }
                var getAvailabilityProfileByUserCommandRequest = new GetAvailabilityProfileByUserCommand();
                var getAvailabilityProfileByUserCommandResponse = await _mediator.Send(getAvailabilityProfileByUserCommandRequest);

                return Ok(getAvailabilityProfileByUserCommandResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"The get availability user profile with error: {ex.Message}");
                return Ok(ex);
            }

        }

        [HttpGet(nameof(GetAllOnboardedUsers))]
        [AuthAttribute(nameof(UserRolesEnum.Admin))]
        [ProducesResponseType(typeof(GenResponse<List<ApplicationUserDto>>), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Summary = "θD> Get all onboarded users")]
        public async Task<IActionResult> GetAllOnboardedUsers()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ModelState);
                }
                var getAllOnboardedUsersCommandRequest = new GetAllOnboardedUsersCommand();
                var getAllOnboardedUsersCommandResponse = await _mediator.Send(getAllOnboardedUsersCommandRequest);

                return Ok(getAllOnboardedUsersCommandResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"The get all onboarded users with error: {ex.Message}");
                return Ok(ex);
            }

        }

        [HttpGet(nameof(GetConnectionBalanceByUser))]
        [AuthAttribute(nameof(UserRolesEnum.Freelancer), nameof(UserRolesEnum.Client))]
        [ProducesResponseType(typeof(GenResponse<string>), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Summary = "θD> Get connection balance by user")]
        public async Task<IActionResult> GetConnectionBalanceByUser()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ModelState);
                }
                var getConnectionBalanceByUserCommandRequest = new GetConnectionBalanceByUserCommand();
                var getConnectionBalanceByUserCommandResponse = await _mediator.Send(getConnectionBalanceByUserCommandRequest);

                return Ok(getConnectionBalanceByUserCommandResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"The get connection balance by user with error: {ex.Message}");
                return Ok(ex);
            }

        }

        [HttpGet(nameof(GetConnectionHistoriesByUser))]
        [AuthAttribute(nameof(UserRolesEnum.Freelancer), nameof(UserRolesEnum.Client))]
        [ProducesResponseType(typeof(GenResponse<string>), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Summary = "θD> Get connection histories by user")]
        public async Task<IActionResult> GetConnectionHistoriesByUser()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ModelState);
                }
                var getConnectionHistoriesByUserCommandRequest = new GetConnectionHistoriesByUserCommand();
                var getConnectionHistoriesByUserCommandResponse = await _mediator.Send(getConnectionHistoriesByUserCommandRequest);

                return Ok(getConnectionHistoriesByUserCommandResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"The get connection histories by user with error: {ex.Message}");
                return Ok(ex);
            }

        }

        [HttpPost(nameof(UploadIdentity))]
        [AuthAttribute(nameof(UserRolesEnum.Freelancer), nameof(UserRolesEnum.Client))]
        [ProducesResponseType(typeof(GenResponse<string>), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Summary = "θΞ> To upload identity files for user profile")]
        public async Task<IActionResult> UploadIdentity(IdentityDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ModelState);
                }
                var identityuploadCommandRequest = new UploadIdentityCommand(model);
                var identityCommandResponse = await _mediator.Send(identityuploadCommandRequest);

                return Ok(identityCommandResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"The get token failed with error: {ex.Message}");
                return Ok(ex);
            }

        }

        [HttpGet(nameof(verifyemail))]
        [AuthAttribute(nameof(UserRolesEnum.Freelancer), nameof(UserRolesEnum.Client))]
        [ProducesResponseType(typeof(GenResponse<string>), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Summary = "θΞ> To verify email ")]
        public async Task<IActionResult> verifyemail([FromQuery]string model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ModelState);
                }
                var emailValidateCommandRequest = new EmailValidateCommand(model);
                var emailValidateCommandResponse = await _mediator.Send(emailValidateCommandRequest);

                return Ok(emailValidateCommandResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"The validate email  failed with error: {ex.Message}");
                return Ok(ex);
            }

        }

        [HttpPost(nameof(setup2FA))]
        [AuthAttribute(nameof(UserRolesEnum.Freelancer), nameof(UserRolesEnum.Client))]
        [ProducesResponseType(typeof(GenResponse<string>), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Summary = "θΞ> To Setup 2FA")]
        public async Task<IActionResult> setup2FA()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ModelState);
                }
                var setup2FACommandRequest = new Setup2FACommand();
                var setup2FACommandResponse = await _mediator.Send(setup2FACommandRequest);

                return Ok(setup2FACommandResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"The setup 2fa with error: {ex.Message}");
                return Ok(ex);
            }

        }

        [HttpPost(nameof(Verify2FA))]
        [AuthAttribute(nameof(UserRolesEnum.Freelancer), nameof(UserRolesEnum.Client))]
        [ProducesResponseType(typeof(GenResponse<bool>), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Summary = "θΞ> To Verify 2FA")]
        public async Task<IActionResult> Verify2FA(string otp)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ModelState);
                }
                var verify2FACommandRequest = new Verify2FACommand(otp);
                var verify2FACommandResponse = await _mediator.Send(verify2FACommandRequest);

                return Ok(verify2FACommandResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"The verify 2fa with error: {ex.Message}");
                return Ok(ex);
            }

        }

        [HttpPost(nameof(CreateUserProfile))]
        [AuthAttribute(nameof(UserRolesEnum.Freelancer), nameof(UserRolesEnum.Client))]
        [ProducesResponseType(typeof(GenResponse<string>), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Summary = "θD> Create user profile")]
        public async Task<IActionResult> CreateUserProfile(CreateUserProfile model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ModelState);
                }
                var createProfileCommandRequest = new CreateProfileCommand(model);
                var createProfileCommandResponse = await _mediator.Send(createProfileCommandRequest);

                return Ok(createProfileCommandResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"The get token failed with error: {ex.Message}");
                return Ok(ex);
            }

        }

        [HttpPost(nameof(BoostProfile))]
        [AuthAttribute(nameof(UserRolesEnum.Freelancer), nameof(UserRolesEnum.Client))]
        [ProducesResponseType(typeof(GenResponse<string>), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Summary = "θD> Boost user profile")]
        public async Task<IActionResult> BoostProfile(BoostProfileDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ModelState);
                }
                var boostProfileCommandRequest = new BoostProfileCommand(model);
                var boostProfileCommandResponse = await _mediator.Send(boostProfileCommandRequest);

                return Ok(boostProfileCommandResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"The boost user profile failed with error: {ex.Message}");
                return Ok(ex);
            }

        }

        [HttpPost(nameof(AvailabilityProfile))]
        [AuthAttribute(nameof(UserRolesEnum.Freelancer), nameof(UserRolesEnum.Client))]
        [ProducesResponseType(typeof(GenResponse<string>), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Summary = "θD> create avaialability profile")]
        public async Task<IActionResult> AvailabilityProfile(string connects, bool activateAvailability)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ModelState);
                }
                var availabilityProfileCommandRequest = new AvailabilityProfileCommand(connects, activateAvailability);
                var availabilityProfileCommandResponse = await _mediator.Send(availabilityProfileCommandRequest);

                return Ok(availabilityProfileCommandResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"The create availability profile failed with error: {ex.Message}");
                return Ok(ex);
            }

        }

        [HttpPost(nameof(CreateSecurityQuestion))]
        [AuthAttribute(nameof(UserRolesEnum.Freelancer), nameof(UserRolesEnum.Client))]
        [ProducesResponseType(typeof(GenResponse<string>), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Summary = "θD> create security question")]
        public async Task<IActionResult> CreateSecurityQuestion(SecurityQuestionDto entity)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ModelState);
                }
                var createSecurityQuestionCommandRequest = new CreateSecurityQuestionCommand(entity);
                var createSecurityQuestionCommandResponse = await _mediator.Send(createSecurityQuestionCommandRequest);

                return Ok(createSecurityQuestionCommandResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"The create secuirty question failed with error: {ex.Message}");
                return Ok(ex);
            }

        }
    }
}
