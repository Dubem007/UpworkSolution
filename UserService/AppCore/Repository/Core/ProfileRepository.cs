using AutoMapper;
using Common.Extensions;
using Common.services.Caching;
using Microsoft.EntityFrameworkCore;
using OnaxTools.Dto.Http;
using OnaxTools.Enums.Http;
using UserServices.AppCore.Repository.Interfaces;
using UserServices.Extension.Services.BearerAuth;
using UserServices.Models;
using UserServices.Models.DTOs;
using UserServices.Persistense;

namespace UserServices.AppCore.Repository.Core
{
    public class ProfileRepository: IProfileRepository
    {
        private readonly ILogger<ProfileRepository> _logger;
        private readonly UserDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAppSessionContextRepository _appSessionService;
        private readonly ICacheService _cacheService;
        private readonly string _uploadDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Uploads");
        private readonly string _cachedConnectionHistories = nameof(_cachedConnectionHistories);


        public ProfileRepository(ILogger<ProfileRepository> logger, UserDbContext context, IMapper mapper, IAppSessionContextRepository appSessionService, ICacheService cacheService)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _appSessionService = appSessionService;
            _cacheService = cacheService;
        }

        public async Task<GenResponse<string>> CreateUserProfile(CreateUserProfile entity)
        {
            GenResponse<string> objResp = new();
            GenResponse<string> resp = new();
            string profilePictureUrl = string.Empty;
            var allworks = new List<UserWorkExperience>(); 
            var alledu = new List<UserEducation>();
            var langs = new List<UserLanguage>();
            var skills = new List<UserSkill>();
            var services = new List<ServicesToOffer>();
            try
            {
                var userDetails = await _appSessionService.GetUserDetails();
                if (entity.FreeLanceQuestion != null) 
                {
                    var quests = new FreelancePrivateQuestions()
                    {
                        UserId = userDetails.Result.UserId,
                        FreeLanceQuestion = entity.FreeLanceQuestion.FreeLanceQuestion,
                        FreeLanceGoal = entity.FreeLanceQuestion.FreeLanceGoal,
                        FreeLanceWorkStyle = entity.FreeLanceQuestion.FreeLanceWorkStyle,
                        contracttohireopportunities = entity.FreeLanceQuestion.contracttohireopportunities,
                        WelcomeGreeetings = entity.FreeLanceQuestion.WelcomeGreeetings
                    };
                    await _context.FreelancePrivateQuestions.AddAsync(quests);
                }

                if (entity.WorkExperiences != null)
                {
                    var workExperiences = entity.WorkExperiences.Select(item => new UserWorkExperience()
                    {
                        UserId = userDetails.Result.UserId,
                        Title = item.Title,
                        Company = item.Company,
                        Location = item.Location,
                        CurrentWorkingRole = item.CurrentWorkingRole,
                        Country = item.Country,
                        StartDate = item.StartDate,
                        EndDate = item.EndDate,
                        Description = item.Description
                    });

                    await _context.UserWorkExperiences.AddRangeAsync(workExperiences);
                }

                if (entity.Educations != null)
                {
                    var workeducations = entity.Educations.Select(item => new UserEducation()
                    {
                        UserId = userDetails.Result.UserId,
                        School = item.School,
                        Degree = item.Degree,
                        FieldOfStudy = item.FieldOfStudy,
                        FromDate = item.FromDate,
                        ToDate = item.ToDate,
                        Description = item.Description
                    });
                    await _context.UserEducations.AddRangeAsync(workeducations);
                }

                if (entity.UserLanguages != null)
                {
                    var worklanguages = entity.UserLanguages.Select(item => new UserLanguage()
                    {
                        UserId = userDetails.Result.UserId,
                        Language = item.Language,
                        Proficiency = item.Proficiency,
                    });
                    await _context.UserLanguages.AddRangeAsync(worklanguages);
                }

                if (entity.UserSkills != null)
                {
                    var userskills = entity.UserSkills.Select(item => new UserSkill()
                    {
                        UserId = userDetails.Result.UserId,
                        Name = item.Name,
                    });
                    await _context.UserSkills.AddRangeAsync(userskills);
                }

                if (entity.ServicesToOffer != null)
                {
                    var UserServicess = entity.ServicesToOffer.Select(item => new ServicesToOffer()
                    {
                        UserId = userDetails.Result.UserId,
                        Service = item.Service,
                    });
                    await _context.ServicesToOffers.AddRangeAsync(UserServicess);
                }

                //if (entity.ProfilePicture != null)
                //{
                //    resp = await UploadProfile(entity.ProfilePicture, "ProfilePicture");
                //}
                //profilePictureUrl = resp.Result;

                var userProfile = new UserProfile()
                {
                    UserId = userDetails.Result.UserId,
                    professionalrole = entity.professionalrole,
                    UserBio = entity.UserBio,
                    HourlyRate = entity.HourlyRate,
                    ServiceFee = entity.ServiceFee,
                    YouGet = entity.YouGet,
                    ImageUrl = "no image",
                    DateOfBirth = entity.DateOfBirth,
                    Country = entity.Country,
                    StreetAddress = entity.StreetAddress,
                    AptSuite = entity.AptSuite,
                    City = entity.City,
                    StateOrProvince = entity.StateOrProvince,
                    ZipOrPostalCode = entity.ZipOrPostalCode,
                    PhoneNumber = entity.PhoneNumber
                };
                await _context.UserProfiles.AddRangeAsync(userProfile);

                await _context.SaveChangesAsync();

                objResp.IsSuccess = true;
                objResp.Result = AppConstants.CreationSuccessResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return GenResponse<string>.Failed($"An error occured creating user profile. Kindly retry", StatusCodeEnum.ServerError);
            }
            return objResp;

        }

        public async Task<GenResponse<string>> BoostProfile(BoostProfileDto entity)
        {
            GenResponse<string> objResp = new();
            try
            {
                var userDetails = await _appSessionService.GetUserDetails();
                var req = _mapper.Map<BoostProfile>(entity);
                req.UserId = userDetails.Result.UserId;
                req.BoostEnabled = true;
                
                var result = await _context.BoostProfiles.AddAsync(req);
                await _context.SaveChangesAsync();

                objResp.IsSuccess = true;
                objResp.Result = AppConstants.CreationSuccessResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return GenResponse<string>.Failed($"An error occured creating boost profile. Kindly retry", StatusCodeEnum.ServerError);
            }
            return objResp;

        }

        public async Task<GenResponse<string>> AvailabilityProfile(string connects, bool activateAvailability)
        {
            GenResponse<string> objResp = new();
            try
            {
                var userDetails = await _appSessionService.GetUserDetails();
                var req = await _context.Connections.Where(x=>x.UserId == userDetails.Result.UserId).FirstOrDefaultAsync();
                if (req != null)
                {
                    int newbalance = Convert.ToInt32(req.NewConnectsbalance) - Convert.ToInt32(connects);
                    req.NewConnectsbalance = newbalance.ToString();

                    var conhistroy = new ConnectsHistory()
                    {
                        ConnectUsed = connects,
                        UserId = userDetails.Result.UserId,
                        Action = "Available now Badge",
                        ConnectDate = DateTime.Now,
                        NewConnectBalance = newbalance.ToString(),
                    };
                    await _context.ConnectsHistories.AddAsync(conhistroy);
                }

                var newRequest = new Badge()
                {
                    UserId = userDetails.Result.UserId,
                    AvailabileNow = true,
                    AvailableDate = DateTime.Now
                };
                var result = await _context.Badges.AddAsync(newRequest);

                await _context.SaveChangesAsync();

                objResp.IsSuccess = true;
                objResp.Result = AppConstants.CreationSuccessResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return GenResponse<string>.Failed($"An error occured enabling availability badge. Kindly retry", StatusCodeEnum.ServerError);
            }
            return objResp;

        }

        public async Task<GenResponse<string>> GetConnectionBalanceByUser()
        {
            GenResponse<string> objResp = new();
            try
            {
                var userDetails = await _appSessionService.GetUserDetails();
                var req = await _context.Connections.Where(x => x.UserId == userDetails.Result.UserId).FirstOrDefaultAsync();
                if (req == null)
                {
                    return GenResponse<string>.Failed($"No record of user available connections", StatusCodeEnum.BadRequest);
                }
                objResp.IsSuccess = true;
                objResp.Result = req.NewConnectsbalance;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return GenResponse<string>.Failed($"An error occured enabling availability badge. Kindly retry", StatusCodeEnum.ServerError);
            }
            return objResp;

        }


        public async Task<GenResponse<List<ConnectsHistoryDto>>> GetConnectionHistoriesByUser()
        {
            GenResponse<List<ConnectsHistoryDto>> objResp = new();
            try
            {
                var userDetails = await _appSessionService.GetUserDetails();
                var cachedResult = await _cacheService.GetData<List<ConnectsHistoryDto>>(_cachedConnectionHistories);
                if (cachedResult != null && cachedResult.Any())
                {
                    objResp.IsSuccess = true;
                    objResp.Result = cachedResult;
                }
                else
                {
                    var req = await _context.ConnectsHistories.Where(x => x.UserId == userDetails.Result.UserId).ToListAsync();
                    if (req == null)
                    {
                        return GenResponse<List<ConnectsHistoryDto>>.Failed($"No record of connect histories available connections", StatusCodeEnum.BadRequest);
                    }
                    var resp = _mapper.Map<List<ConnectsHistoryDto>>(req);
                    _ = _cacheService.SetData<List<ConnectsHistoryDto>>(_cachedConnectionHistories, resp, 60 * 10);

                    objResp.IsSuccess = true;
                    objResp.Result = resp;
                }

                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return GenResponse<List<ConnectsHistoryDto>>.Failed($"An error occured getting connect histories. Kindly retry", StatusCodeEnum.ServerError);
            }
            return objResp;

        }

        public async Task<GenResponse<string>> GetAvailabilityProfileByUser()
        {
            GenResponse<string> objResp = new();
            var response = string.Empty;
            try
            {
                var userDetails = await _appSessionService.GetUserDetails();
                var req = await _context.Badges.Where(x => x.UserId == userDetails.Result.UserId).FirstOrDefaultAsync();
                if (req == null)
                {
                    return GenResponse<string>.Failed($"No record of user availability badge", StatusCodeEnum.BadRequest);
                }

                if (req.AvailabileNow == true)
                {
                    response = AppConstants.UserAvailableNow;
                }
                else 
                {
                    response = AppConstants.UserNotAvailable;
                }
                objResp.IsSuccess = true;
                objResp.Result = response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return GenResponse<string>.Failed($"An error occured getting availability badge. Kindly retry", StatusCodeEnum.ServerError);
            }
            return objResp;

        }

        public async Task<GenResponse<List<ApplicationUserDto>>> GetAllOnboardedUsers()
        {
            GenResponse<List<ApplicationUserDto>> objResp = new();
            var response = string.Empty;
            try
            {
                var userDetails = await _appSessionService.GetUserDetails();
                var roles = _context.UserRoles.Where(x=>x.Id == userDetails.Result.Role);

                if (userDetails.Result.Role == AppConstants.ADMIN)
                {
                    var req = await _context.Users.Where(x => x.IsActive == true && x.IsDeleted == false).ToListAsync();
                    if (req.Count() == 0)
                    {
                        return GenResponse<List<ApplicationUserDto>>.Failed($"No record of users onboarded", StatusCodeEnum.BadRequest);
                    }
                    var allusers = await MapUserDetails(req);

                    objResp.IsSuccess = true;
                    objResp.Result = allusers;
                }
                else 
                {
                    return GenResponse<List<ApplicationUserDto>>.Failed($"The user not authorised to view details. Kindly retry", StatusCodeEnum.ServerError);
                }
                
               
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return GenResponse<List<ApplicationUserDto>>.Failed($"An error occured getting all onboarded users. Kindly retry", StatusCodeEnum.ServerError);
            }
            return objResp;

        }

        public async Task<GenResponse<string>> CreateSecurityQuestion(SecurityQuestionDto entity)
        {
            GenResponse<string> objResp = new();
            try
            {
                var userDetails = await _appSessionService.GetUserDetails();
                var req = new SecurityQuestion()
                {
                    UserId = userDetails.Result.UserId,
                    Question = entity.Question,
                    Answer = entity.Answer,
                    AcknowledgePolicy = entity.AcknowledgePolicy,
                    KeepMeLoggedIn = entity.KeepMeLoggedIn
                };

                var result = await _context.SecurityQuestions.AddAsync(req);
                await _context.SaveChangesAsync();

                objResp.IsSuccess = true;
                objResp.Result = AppConstants.CreationSuccessResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return GenResponse<string>.Failed($"An error occured creating boost profile. Kindly retry", StatusCodeEnum.ServerError);
            }
            return objResp;

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
                string fileName = $"{request.FileName}_{fileExtension}";

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
                _logger.LogError(exception: ex, ex.Message);
                objResp.IsSuccess = false;
                objResp.Result = null;
                objResp.Message = $" Error uploading file {request.FileName}  with messagr: {ex.Message}";
            }
            return objResp;


        }

        private async Task<List<ApplicationUserDto>> MapUserDetails(List<ApplicationUser> user) 
        {
            try
            {
                var allCountries = await _context.Countries.ToListAsync();
                var allroles = await _context.UserRoles.ToListAsync();

                var allusers = user.Select(item => new ApplicationUserDto()
                {
                    Id = item.Id,
                    Email = item.Email,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    Role = allroles.Where(x => x.Id == item.RoleId).Select(x=>x.Name).FirstOrDefault(),
                    Country = allCountries.Where(x=>x.Code.ToLower().Trim() == item.CountryCode.ToLower().Trim()).Select(x => x.Name).FirstOrDefault(),
                    IsActive = item.IsActive,
                    CreatedAt = item.CreatedAt,
                }).ToList();


                return allusers;
            }
            catch (Exception ex)
            {

                _logger.LogError(exception: ex, ex.Message);
                return null;
            }
        
        }

        
    }
}
