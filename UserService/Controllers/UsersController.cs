using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnaxTools.Dto.Http;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using UserServices.AppCore.CQRS.Commands;
using UserServices.Models.DTOs;
using UserServices.Services;

namespace UserServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IMediator _mediator;

        public UsersController(ILogger<UsersController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost(nameof(GetLoginToken))]
        [ProducesResponseType(typeof(GenResponse<TokenDTO>), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Summary = "θD> Generate access token from identity server")]
        public async Task<IActionResult> GetLoginToken(LoginInputDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ModelState);
                }
                var loginCommandRequest = new LoginCommand(model);
                var loginCommandResponse = await _mediator.Send(loginCommandRequest);

                return Ok(loginCommandResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"The get token failed with error: {ex.Message}");
                return Ok(ex);
            }

        }


        [HttpPost("register")]
        [ProducesResponseType(typeof(GenResponse<string>), 200)]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ModelState);
                }

                var signUpCommandRequest = new SignUpCommand(model);
                var signUpCommandResponse = await _mediator.Send(signUpCommandRequest);

                return Ok(signUpCommandResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error registering user with message:{ex.Message}");
                return BadRequest($"Error registering user with message:{ex.Message}");
            }
        }

        
    }
}
