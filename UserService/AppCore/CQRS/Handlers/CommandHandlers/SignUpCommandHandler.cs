using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OnaxTools.Dto.Http;
using OnaxTools.Http;
using System.Security.Claims;
using Common.MessageQueue.Interfaces;
using Common.Extensions;
using UserServices.AppCore.CQRS.Commands;
using UserServices.Models;
using UserServices.Persistense;
using UserServices.Models.DTOs;
using UserServices.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using UserServices.Domain.DTOs;
using Microsoft.Extensions.Options;
using IdentityModel;
using UserServices.Services.AWS;
using UserServices.Models.Enums;
using UserService.Services;

namespace UserServices.AppCore.CQRS.Handlers.CommandHandlers
{
    public class SignUpCommandHandler : IRequestHandler<SignUpCommand, GenResponse<IdentityResult>>
    {
        #region Variable Declarations
        private readonly ILogger<SignUpCommandHandler> logger;
        private readonly IMediator _mediator;
        private readonly AppSettings _appsettings; 
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<UserRole> _roleManager;
        private readonly UserDbContext _dbContext;
        private readonly IUserServicess _UserServices;
        private readonly IAwsS3Client _awsS3Client;
        private readonly string _uploadDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Uploads");

        public SignUpCommandHandler(ILogger<SignUpCommandHandler> logger, IMediator mediator, IOptions<AppSettings> appsettings, UserManager<ApplicationUser> userManager, RoleManager<UserRole> roleManager, UserDbContext dbContext, IUserServicess UserServices, IAwsS3Client awsS3Client)
        {
            this.logger = logger;
            _mediator = mediator;
            _appsettings = appsettings.Value;
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
            _UserServices = UserServices;
            _awsS3Client = awsS3Client;
        }



        #endregion
        public async Task<GenResponse<IdentityResult>> Handle(SignUpCommand request, CancellationToken cancellationToken)
        {
            GenResponse<IdentityResult> objResp = new();
            GenResponse<string> resp = new();
            string profilePictureUrl = string.Empty;
            try
            {
                var roleId = Guid.Parse(request.Entity.RoleId);
                var therole = _roleManager.Roles.FirstOrDefault(X => X.Id == request.Entity.RoleId);

                if (therole == null)
                {
                    return GenResponse<IdentityResult>.Failed("Invalid role Id provided");
                }

                var country = await _dbContext.Countries.FirstOrDefaultAsync(X => X.Code.Trim().ToLower() == request.Entity.CountryCode.Trim().ToLower());

                if (country == null)
                {
                    return GenResponse<IdentityResult>.Failed("Invalid country code provided");
                }

                
                var user = new ApplicationUser
                {
                    Email = request.Entity.Email,
                    FirstName = request.Entity.FirstName,
                    LastName = request.Entity.LastName,
                    CountryCode = request.Entity.CountryCode,
                    SendHelpfulEmails = request.Entity.SendHelpfulEmails,
                    HasReadPolicy = request.Entity.HasReadPolicy,
                    UserName = request.Entity.Email,
                    RoleId = request.Entity.RoleId,
                };
                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, request.Entity.Password);
                var result = await _userManager.CreateAsync(user, request.Entity.Password);

                if (result.Succeeded)
                {
                    var createdUser = await _userManager.FindByNameAsync(user.Email);
                    // Add claims (optional)
                    if (createdUser != null)
                    {
                        createdUser.SubjectId = createdUser.Id;
                        await _userManager.UpdateAsync(user);

                        var addrole = await _UserServices.AddUserToRole(createdUser.Id, request.Entity.RoleId);

                        if (addrole)
                        {
                            var userdto = new AppUserDto
                            {
                                UserId = createdUser.Id,
                                FirstName = createdUser.FirstName,
                                LastName = createdUser.LastName,
                                Email = request.Entity.Email,
                                Country = country.Name,
                                Role = therole.Name,
                                IsActive = createdUser.IsActive,
                                IsDeleted = createdUser.IsDeleted,
                                CreatedAt = createdUser.CreatedAt,
                               
                            };

                            // push the details to rabbitmq for user details creation on ther services
                            var pushCommandRequest = new PushQueueCommand(userdto);
                            var responds = await _mediator.Send(pushCommandRequest);

                            if (responds == null)
                            {
                                return GenResponse<IdentityResult>.Failed("Failed to push registered user detaild to queue");
                            }

                            // Send in app notifcation to all users that a new user just registered and email to user to acknoeldge registration
             
                            var regUser = new EmaiReqsDto()
                            {
                                RecipientEmails = request.Entity.Email,
                                RecipientName = $"{request.Entity.FirstName} {request.Entity.LastName}",
                                SenderName = "donotreply@upwork.com",
                                Subject = "Verify your email address",
                                SenderEmail = "donotreply@upwork.com",
                                EmailType = EmailTypesEnum.VerifyEmail,
                                UserId = createdUser.Id
                            };

                            var notifyjoinedCommandRequest = new UserJoinedNotificationCommand(regUser.RecipientName);
                            var notifyjoined = await _mediator.Send(notifyjoinedCommandRequest);

                            var notifyCommandRequest = new UserNotificationCommand(regUser);
                            var notify = await _mediator.Send(notifyCommandRequest);

                            if (notify == null)
                            {
                                return GenResponse<IdentityResult>.Failed("Failed to notify user to acknoledge registration");
                            }
                        }
                        else
                        {
                            return GenResponse<IdentityResult>.Failed("Failed to add user to role");

                        }
                    }

                    objResp.IsSuccess = true;
                    objResp.Result = result;
                    objResp.Message = AppConstants.CreationSuccessResponse;
                    return objResp;

                }
                else
                {
                    return GenResponse<IdentityResult>.Failed("Failed to register user");
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Error registering user with message:{ex.Message}");
                return GenResponse<IdentityResult>.Failed($"Error registering user with message:{ex.Message}");
            }
        }

        private async Task<GenResponse<string>> UploadProfile(IFormFile request, string Idtype)
        {
            GenResponse<string> objResp = new();
            try
            {

                if (request == null || request.Length == 0)
                {
                    return GenResponse<string>.Failed("No file was uploaded.");
                }
                
                if (!Directory.Exists(_uploadDirectory))
                {
                    Directory.CreateDirectory(_uploadDirectory);
                }

                // Get the content type of the file
                string contentType = request.ContentType;

                // Extract the file extension from the content type
                string fileExtension = "." + contentType.Split('/')[1].Trim();
                string fileName = $"{Idtype}-{request.FileName}_{fileExtension}";

                string filePath = Path.Combine(_uploadDirectory, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.CopyToAsync(stream);
                }

                objResp.IsSuccess = true;
                objResp.Result = fileName;
                objResp.Message = $"File with name {fileName} uploaded successfully.";

            }
            catch (Exception ex)
            {
                this.logger.LogError(exception: ex, ex.Message);
                objResp.IsSuccess = false;
                objResp.Result = null;
                objResp.Message = $" Error uploading file {request.FileName}  with messagr: {ex.Message}";
            }
            return objResp;


        }
    }
}
