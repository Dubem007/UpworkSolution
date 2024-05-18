using AutoMapper;
using Common.Enums;
using Common.Extensions;
using Common.MessageQueue.Interfaces;
using Common.MessageQueue.Services;
using Common.Models;
using Common.Response;
using Common.services.Caching;
using IdentityModel;
using IdentityServer.Models.DTOs;
using JobService.AppCore.Repository.Interface;
using JobService.AppCore.Services.BearerAuth;
using JobService.Domain.Dtos;
using JobService.Domain.Entities;
using JobService.Extension.Middleware;
using JobService.Persistense;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OnaxTools.Dto.Http;
using OnaxTools.Enums.Http;
using System;
using System.Linq;
using System.Net.Mail;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace JobService.AppCore.Repository.Core
{
    public class JobRepository: IJobRepository
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<JobRepository> _logger;
        private readonly JobServiceDbContext _context;
        private readonly IMessageQueueService _messageQueueService;
        private readonly IAppSessionContextRepository _appSessionService;
        private readonly IMapper _mapper;

        public JobRepository(ICacheService cacheService, ILogger<JobRepository> logger, JobServiceDbContext context, IMapper mapper, IMessageQueueService messageQueueService, IAppSessionContextRepository appSessionService)
        {
            _cacheService = cacheService;
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _messageQueueService = _messageQueueService;
            _appSessionService = appSessionService;
        }
        private string AllJobsByUser(string acct, string action = "_AllJobsByUser") => $"{nameof(AllJobsByUser)}{action}:{acct}";

        public async Task<GenResponse<List<UpworkJobsDto>>> GetAllJobsByClientUserId()
        {
            GenResponse<List<UpworkJobsDto>> objResp = new();
            GenResponse<ClaimDetailsDto> appUser = await _appSessionService.GetUserDetails();
            try
            {
                // check cache first if there is any record before calling the database
                var userJobs = await _context.UpworkJobs.Where(m => m.UserId == appUser.Result.UserId && m.IsActive == true).AsNoTracking().ToListAsync();
                if (userJobs != null)
                {
                    var alluserJobs = await MapUpworkJob(userJobs); 
                    objResp.IsSuccess = true;
                    objResp.Result = alluserJobs;
                }
                else
                {
                    return GenResponse<List<UpworkJobsDto>>.Failed($"Jobs with Id {appUser.Result.UserId} not found", StatusCodeEnum.NotFound);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return GenResponse<List<UpworkJobsDto>>.Failed($"An error occured getting jobs for user with Id {appUser.Result.UserId}. Kindly retry", StatusCodeEnum.ServerError);
            }
            return objResp;

        }

        public async Task<GenResponse<List<UpworkJobsDto>>> GetAllJobsPosts()
        {
            GenResponse<List<UpworkJobsDto>> objResp = new();
            GenResponse<ClaimDetailsDto> appUser = await _appSessionService.GetUserDetails();
            try
            {
                // check cache first if there is any record before calling the database
               
                var userJobs = await _context.UpworkJobs.Where(m => m.UserId == appUser.Result.UserId && m.IsActive == true && m.Status == AppConstants.AsPosted).AsNoTracking().ToListAsync();
                if (userJobs.Count() > 0)
                {
                    var alluserJobs = await MapUpworkJob(userJobs);
                    objResp.IsSuccess = true;
                    objResp.Result = alluserJobs;
                }
                else
                {
                    return GenResponse<List<UpworkJobsDto>>.Failed($"All posted Jobs with Id {appUser.Result.UserId} not found", StatusCodeEnum.NotFound);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return GenResponse<List<UpworkJobsDto>>.Failed($"An error occured getting all posted jobs for user with Id {appUser.Result.UserId}. Kindly retry", StatusCodeEnum.ServerError);
            }
            return objResp;

        }

        public async Task<GenResponse<List<UpworkJobsDto>>> GetAllJobsDrafts()
        {
            GenResponse<List<UpworkJobsDto>> objResp = new();
            GenResponse<ClaimDetailsDto> appUser = await _appSessionService.GetUserDetails();
            try
            {
                // check cache first if there is any record before calling the database
                var userJobs = await _context.UpworkJobs.Where(m => m.UserId == appUser.Result.UserId && m.IsActive == true && m.Status == AppConstants.AsDraft).AsNoTracking().ToListAsync();
                if (userJobs.Count() > 0)
                {
                    var alluserJobs = await MapUpworkJob(userJobs);
                    objResp.IsSuccess = true;
                    objResp.Result = alluserJobs;
                }
                else
                {
                    return GenResponse<List<UpworkJobsDto>>.Failed($"All draft Jobs with Id {appUser.Result.UserId} not found", StatusCodeEnum.NotFound);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return GenResponse<List<UpworkJobsDto>>.Failed($"An error occured getting all drafted jobs for user with Id {appUser.Result.UserId}. Kindly retry", StatusCodeEnum.ServerError);
            }
            return objResp;

        }

        public async Task<GenResponse<List<UpworkJobsDto>>> GetAllJobsBySkill()
        {
            GenResponse<List<UpworkJobsDto>> objResp = new();
            GenResponse<ClaimDetailsDto> appUser = await _appSessionService.GetUserDetails();
            try
            {
                var theid = Guid.Parse(appUser.Result.UserId);
                // check cache first if there is any record before calling the database
                var userJobs = await _context.JobUserSkills.Where(m => (m.IsActive == true && m.UserId == theid) && m.IsDeleted == false).Include(m =>m.Job).ToListAsync();
                if (userJobs.Count() > 0)
                {
                    var thejobs = userJobs.Select(x => x.Job).ToList();
                    var alluserJobs = await MapUpworkJob(thejobs);
                    objResp.IsSuccess = true;
                    objResp.Result = alluserJobs;
                }
                else
                {
                    return GenResponse<List<UpworkJobsDto>>.Failed($"All draft Jobs with Id {appUser.Result.UserId} not found", StatusCodeEnum.NotFound);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return GenResponse<List<UpworkJobsDto>>.Failed($"An error occured getting all drafted jobs for user with Id {appUser.Result.UserId}. Kindly retry", StatusCodeEnum.ServerError);
            }
            return objResp;

        }

        public async Task<GenResponse<UpworkJobsDto>> GetJobById(Guid jobId)
        {
            GenResponse<UpworkJobsDto> objResp = new();
            try
            {
                // check cache first if there is any record before calling the database

                var userJobs = await _context.UpworkJobs.Where(m => m.Id == jobId && m.IsActive == true).AsNoTracking().ToListAsync();
                if (userJobs.Count() > 0)
                {
                    var alluserJobs = _mapper.Map<UpworkJobsDto>(userJobs);
                    objResp.IsSuccess = true;
                    objResp.Result = alluserJobs;
                }
                else
                {
                    return GenResponse<UpworkJobsDto>.Failed($"Job with Id {jobId} not found", StatusCodeEnum.NotFound);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return GenResponse<UpworkJobsDto>.Failed($"An error occured getting job with Id {jobId}. Kindly retry", StatusCodeEnum.ServerError);
            }
            return objResp;

        }

        public async Task<GenResponse<JobStatusUpdateDto>> CreateNewJob(CreateJobDto entity)
        {
            GenResponse<JobStatusUpdateDto> objResp = new();
            var quests = new List<Question>();
            var resp = new JobStatusUpdateDto();
            var skills = new List<JobSkill>();
            var thenewjob = new UpworkJobs();
            try
            {
                var appUser = await _appSessionService.GetUserDetails();

                if (entity.Id != null)
                {
                    var theid = Guid.Parse(entity.Id);
                    thenewjob = await _context.UpworkJobs.FirstOrDefaultAsync(x => x.Id == theid);
                    if (thenewjob != null)
                    {
                        thenewjob = await MapUpdateJob(thenewjob, entity);
                        thenewjob.UserId = appUser.Result.UserId;
                        thenewjob.Attachment = "No image";
                        thenewjob.Status = AppConstants.AsPosted;
                    }
                    else 
                    {
                        return GenResponse<JobStatusUpdateDto>.Failed($"{AppConstants.FailedRequest}. Kindly retry", StatusCodeEnum.BadRequest);
                    }

                    
                    //if (entity.Attachment != null) 
                    //{
                    //    thenewjob.Attachment = await UploadAttachment(entity.Attachment);
                    //}

                }
                else 
                {
                    if (entity.IsDraft)
                    {
                        thenewjob = await MapNewJob(entity);
                        thenewjob.UserId = appUser.Result.UserId;
                        thenewjob.Attachment = "No image";
                        thenewjob.Status = AppConstants.AsDraft;
                    }
                    else 
                    {
                        thenewjob = await MapNewJob(entity);
                        thenewjob.UserId = appUser.Result.UserId;
                        thenewjob.Attachment = "No image";
                        thenewjob.Status = AppConstants.AsPosted;
                    }
                    
                    await _context.UpworkJobs.AddAsync(thenewjob);
                }
                
                // Then create the skills

                foreach (var m in entity.Skills)
                {
                    var skill = new JobSkill()
                    {
                        JobId = thenewjob.Id,
                        Name = m,
                    };
                    skills.Add(skill);
                };
                await _context.Questiona.AddRangeAsync(quests);

                // Then create the questions

                foreach (var m in entity.Questions) 
                {
                    var questions = new Question()
                    {
                        JobId = thenewjob.Id,
                        Title = m.Question,
                        Comment = m.Answer
                    };
                    quests.Add(questions);
                }
                await _context.Questiona.AddRangeAsync(quests);


                var saved = await _context.SaveChangesAsync();

                if (saved > 0)
                {
                    resp.JobId = thenewjob.Id.ToString();
                    resp.Title = thenewjob.Title;
                    resp.IsDraft = entity.IsDraft;

                    objResp.IsSuccess = true;
                    objResp.Result = resp;
                    objResp.Message = AppConstants.CreationSuccessResponse;
                }
                else 
                {
                    resp.JobId = null;
                    resp.Title = thenewjob.Title;
                    resp.IsDraft = entity.IsDraft;

                    objResp.IsSuccess = false;
                    objResp.Result = null;
                    objResp.Message = AppConstants.CreationFailedResponse;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return GenResponse<JobStatusUpdateDto>.Failed($"{AppConstants.FailedRequestError}. Kindly retry", StatusCodeEnum.ServerError);
            }
            return objResp;

        }

        public async Task<GenResponse<string>> CreateUsers(AppUserDto entity)
        {
            GenResponse<string> objResp = new();
            try
            {
                var user = new JobUser
                {
                    UserId = entity.UserId,
                    FirstName = entity.FirstName,
                    LastName = entity.LastName,
                    Email = entity.Email,
                    Role = entity.Role,
                    Country = entity.Country,
                    IsActive = entity.IsActive,
                    IsDeleted = entity.IsDeleted,
                    CreatedAt = entity.CreatedAt,
                    SendHelpfulEmails = entity.SendHelpfulEmails,
                    HasReadPolicy = entity.HasReadPolicy
                };

                var result = await _context.JobUsers.AddAsync(user);
                await _context.SaveChangesAsync();

                objResp.IsSuccess = true;
                objResp.Result = AppConstants.CreationSuccessResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return GenResponse<string>.Failed($"An error occured creating job user with Id: {entity.UserId}. Kindly retry", StatusCodeEnum.ServerError);
            }
            return objResp;

        }

        //public async Task<GenResponse<QueryResponse<IEnumerable<UpworkJobsDto>>>> GetAllJobs(string Search)
        //{
        //    GenResponse<QueryResponse<IEnumerable<UpworkJobsDto>>> objResp = new() { Result = new() };
        //    List<UpworkJobs> alljobs = new();
        //    int ItemsPerPage = 0;
        //    int skip = 0;
        //    int Page = 50;
        //    DateTime? startDate = null; 
        //    DateTime? endDate = null;
        //    try
        //    {
        //        GenResponse<ClaimDetailsDto> appUser = await _appSessionService.GetUserDetails();
        //        if (startDate == null || endDate == null)
        //        {
        //            if (Search != null)
        //            {
        //                alljobs = await _context.UpworkJobs.Where(m =>
        //                       (m.IsActive == true && m.UserId == appUser.Result.UserId) && m.Title.ToLower().Contains(Search.ToLower())).ToListAsync();
        //            }
        //            else 
        //            {
        //                alljobs = await _context.UpworkJobs.Where(m =>
        //                       (m.IsActive == true && m.UserId == appUser.Result.UserId && m.IsDeleted == false)).ToListAsync();
        //            }

        //            if (alljobs != null || alljobs.Count > 0)
        //            {
        //                int total = alljobs.Count;

        //                var responses = alljobs.Skip(skip)
        //                                   .Take(ItemsPerPage)
        //                                   .ToList();
 
        //                var sortedRecords = responses.OrderByDescending(b => b.CreatedAt).ToList();
        //                var allsortedJobs = _mapper.Map<List<UpworkJobsDto>>(sortedRecords);

        //                var paginatedResult = QueryResponse<IEnumerable<UpworkJobsDto>>.Generate(allsortedJobs,
        //                  Page,
        //                  ItemsPerPage,
        //                  responses.Count,
        //                  total,
        //                  true,
        //                  "All jobs retrieved successfully");

        //                var result = new QueryResponse<IEnumerable<UpworkJobsDto>>()
        //                {
        //                    Data = paginatedResult.Data.ToList(),
        //                    Count = responses.Count,
        //                    Size = ItemsPerPage,
        //                    Message = paginatedResult.Message,
        //                    Page = Page,
        //                    Success = true,
        //                    Total = total
        //                };
        //                objResp.Result = result;
        //                objResp.IsSuccess = true;

        //            }
        //            else
        //            {
        //                objResp.IsSuccess = false;
        //                objResp.Result = new();
        //            }

        //        }
        //        else
        //        {
                    
        //            if (Search != null)
        //            {
        //                alljobs = await _context.UpworkJobs.Where(m =>
        //                   (m.IsActive == true && m.UserId == appUser.Result.UserId) && m.Title.ToLower().Contains(Search.ToLower()) && (m.CreatedAt >= startDate && m.CreatedAt <= endDate) && m.IsDeleted == false).ToListAsync();
        //            }
        //            else
        //            {
        //                alljobs = await _context.UpworkJobs.Where(m =>
        //                   (m.IsActive == true && m.UserId == appUser.Result.UserId) && (m.CreatedAt >= startDate && m.CreatedAt <= endDate) && m.IsDeleted == false).ToListAsync();
        //            }

        //            if (alljobs != null || alljobs.Count > 0)
        //            {
        //                int total = alljobs.Count;

        //                var responses = alljobs.Skip(skip)
        //                                   .Take(ItemsPerPage)
        //                                   .ToList();

        //                var sortedRecords = responses.OrderByDescending(b => b.CreatedAt).ToList();
        //                var allsortedJobs = _mapper.Map<List<UpworkJobsDto>>(sortedRecords);

        //                var paginatedResult = QueryResponse<IEnumerable<UpworkJobsDto>>.Generate(allsortedJobs,
        //                  Page,
        //                  ItemsPerPage,
        //                  responses.Count,
        //                  total,
        //                  true,
        //                  "All jobs retrieved successfully");

        //                var result = new QueryResponse<IEnumerable<UpworkJobsDto>>()
        //                {
        //                    Data = paginatedResult.Data.ToList(),
        //                    Count = responses.Count,
        //                    Size = ItemsPerPage,
        //                    Message = paginatedResult.Message,
        //                    Page = Page,
        //                    Success = true,
        //                    Total = total
        //                };
        //                objResp.Result = result;
        //                objResp.IsSuccess = true;

        //            }
        //            else
        //            {
        //                objResp.IsSuccess = false;
        //                objResp.Result = new();
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(exception: ex, ex.Message);
        //        objResp.Result = new QueryResponse<IEnumerable<UpworkJobsDto>>();
        //        objResp.IsSuccess = false;
        //        objResp.Message = "Oops! Failed to retrieve pending lc issuance. Kindly retry.";
        //    }
        //    return objResp;
        //}
        //public async Task<GenResponse<QueryResponse<IEnumerable<UpworkJobsDto>>>> GetAllJobsBySkill(string Search)
        //{
        //    GenResponse<QueryResponse<IEnumerable<UpworkJobsDto>>> objResp = new() { Result = new() };
        //    List<JobSkill> allskilljobs = new();
        //    try
        //    {
        //        GenResponse<ClaimDetailsDto> appUser = await _appSessionService.GetUserDetails();
        //        if (requests.startDate == null || requests.endDate == null)
        //        {
        //            allskilljobs = await _context.JobUserSkills.Where(m =>
        //                   (m.IsActive == true && m.Name.ToLower().Contains(requests.Search.ToLower()) && m.IsDeleted == false)).Include(m=>m.Job).ToListAsync();

        //            if (allskilljobs != null || allskilljobs.Count > 0)
        //            {
        //                int total = allskilljobs.Count;

        //                var responses = allskilljobs.Skip(0)
        //                                   .Take(0)
        //                                   .ToList();

        //                var sortedRecords = responses.OrderByDescending(b => b.CreatedAt).ToList();
        //                var allsortedJobs = _mapper.Map<List<UpworkJobsDto>>(sortedRecords);

        //                var paginatedResult = QueryResponse<IEnumerable<UpworkJobsDto>>.Generate(allsortedJobs,
        //                  0,
        //                  0,
        //                  responses.Count,
        //                  total,
        //                  true,
        //                  "All jobs retrieved successfully");

        //                var result = new QueryResponse<IEnumerable<UpworkJobsDto>>()
        //                {
        //                    Data = paginatedResult.Data.ToList(),
        //                    Count = responses.Count,
        //                    Size = 0,
        //                    Message = paginatedResult.Message,
        //                    Page = 0,
        //                    Success = true,
        //                    Total = total
        //                };
        //                objResp.Result = result;
        //                objResp.IsSuccess = true;

        //            }
        //            else
        //            {
        //                objResp.IsSuccess = false;
        //                objResp.Result = new();
        //            }

        //        }
        //        else
        //        {
        //            allskilljobs = await _context.JobUserSkills.Where(m =>
        //                   (m.IsActive == true && m.Name.ToLower().Contains(requests.Search.ToLower()) && m.IsDeleted == false) && (m.CreatedAt >= requests.startDate && m.CreatedAt <= requests.endDate)).Include(m => m.Job).ToListAsync();

        //            if (allskilljobs != null || allskilljobs.Count > 0)
        //            {
        //                int total = allskilljobs.Count;

        //                var responses = allskilljobs.Skip(0)
        //                                   .Take(0)
        //                                   .ToList();

        //                var sortedRecords = responses.OrderByDescending(b => b.CreatedAt).ToList();
        //                var allsortedJobs = _mapper.Map<List<UpworkJobsDto>>(sortedRecords);

        //                var paginatedResult = QueryResponse<IEnumerable<UpworkJobsDto>>.Generate(allsortedJobs,
        //                  0,
        //                  0,
        //                  responses.Count,
        //                  total,
        //                  true,
        //                  "All jobs retrieved successfully");

        //                var result = new QueryResponse<IEnumerable<UpworkJobsDto>>()
        //                {
        //                    Data = paginatedResult.Data.ToList(),
        //                    Count = responses.Count,
        //                    Size = 0,
        //                    Message = paginatedResult.Message,
        //                    Page = 0,
        //                    Success = true,
        //                    Total = total
        //                };
        //                objResp.Result = result;
        //                objResp.IsSuccess = true;

        //            }
        //            else
        //            {
        //                objResp.IsSuccess = false;
        //                objResp.Result = new();
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(exception: ex, ex.Message);
        //        objResp.Result = new QueryResponse<IEnumerable<UpworkJobsDto>>();
        //        objResp.IsSuccess = false;
        //        objResp.Message = "Oops! Failed to retrieve pending lc issuance. Kindly retry.";
        //    }
        //    return objResp;
        //}

        //public async Task<GenResponse<QueryResponse<IEnumerable<UpworkJobsDto>>>> GetAllJobsDrafts(string Search)
        //{
        //    GenResponse<QueryResponse<IEnumerable<UpworkJobsDto>>> objResp = new() { Result = new() };
        //    List<UpworkJobs> alljobs = new();
        //    try
        //    {
        //        GenResponse<ClaimDetailsDto> appUser = await _appSessionService.GetUserDetails();
        //        if (requests.startDate == null || requests.endDate == null)
        //        {
        //            alljobs = await _context.UpworkJobs.Where(m =>
        //                   (m.UserId == appUser.Result.UserId && m.IsActive == true && m.Status == AppConstants.AsDraft) && m.IsDeleted == false).ToListAsync();

        //            if (alljobs != null || alljobs.Count > 0)
        //            {
        //                int total = alljobs.Count;

        //                var responses = alljobs.Skip(0)
        //                                   .Take(0)
        //                                   .ToList();

        //                var sortedRecords = responses.OrderByDescending(b => b.CreatedAt).ToList();
        //                var allsortedJobs = _mapper.Map<List<UpworkJobsDto>>(sortedRecords);

        //                var paginatedResult = QueryResponse<IEnumerable<UpworkJobsDto>>.Generate(allsortedJobs,
        //                  0,
        //                  0,
        //                  responses.Count,
        //                  total,
        //                  true,
        //                  "All jobs retrieved successfully");

        //                var result = new QueryResponse<IEnumerable<UpworkJobsDto>>()
        //                {
        //                    Data = paginatedResult.Data.ToList(),
        //                    Count = responses.Count,
        //                    Size = 0,
        //                    Message = paginatedResult.Message,
        //                    Page = 0,
        //                    Success = true,
        //                    Total = total
        //                };
        //                objResp.Result = result;
        //                objResp.IsSuccess = true;

        //            }
        //            else
        //            {
        //                objResp.IsSuccess = false;
        //                objResp.Result = new();
        //            }

        //        }
        //        else
        //        {
        //            alljobs = await _context.UpworkJobs.Where(m =>
        //                   (m.UserId == appUser.Result.UserId && m.IsActive == true && m.Status == AppConstants.AsDraft) && (m.CreatedAt >= requests.startDate && m.CreatedAt <= requests.endDate) && m.IsDeleted == false).ToListAsync();

        //            if (alljobs != null || alljobs.Count > 0)
        //            {
        //                int total = alljobs.Count;

        //                var responses = alljobs.Skip(0)
        //                                   .Take(0)
        //                                   .ToList();

        //                var sortedRecords = responses.OrderByDescending(b => b.CreatedAt).ToList();
        //                var allsortedJobs = _mapper.Map<List<UpworkJobsDto>>(sortedRecords);

        //                var paginatedResult = QueryResponse<IEnumerable<UpworkJobsDto>>.Generate(allsortedJobs,
        //                  0,
        //                  0,
        //                  responses.Count,
        //                  total,
        //                  true,
        //                  "All jobs retrieved successfully");

        //                var result = new QueryResponse<IEnumerable<UpworkJobsDto>>()
        //                {
        //                    Data = paginatedResult.Data.ToList(),
        //                    Count = responses.Count,
        //                    Size = 0,
        //                    Message = paginatedResult.Message,
        //                    Page = 0,
        //                    Success = true,
        //                    Total = total
        //                };
        //                objResp.Result = result;
        //                objResp.IsSuccess = true;

        //            }
        //            else
        //            {
        //                objResp.IsSuccess = false;
        //                objResp.Result = new();
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(exception: ex, ex.Message);
        //        objResp.Result = new QueryResponse<IEnumerable<UpworkJobsDto>>();
        //        objResp.IsSuccess = false;
        //        objResp.Message = "Oops! Failed to retrieve pending lc issuance. Kindly retry.";
        //    }
        //    return objResp;
        //}

        //public async Task<GenResponse<QueryResponse<IEnumerable<UpworkJobsDto>>>> GetAllJobsPosts(string Search)
        //{
        //    GenResponse<QueryResponse<IEnumerable<UpworkJobsDto>>> objResp = new() { Result = new() };
        //    List<UpworkJobs> alljobs = new();
        //    try
        //    {
        //        GenResponse<ClaimDetailsDto> appUser = await _appSessionService.GetUserDetails();
        //        if (requests.startDate == null || requests.endDate == null)
        //        {
        //            alljobs = await _context.UpworkJobs.Where(m =>
        //                   (m.UserId == appUser.Result.UserId && m.IsActive == true && m.Status == AppConstants.AsPosted) && m.IsDeleted == false).ToListAsync();

        //            if (alljobs != null || alljobs.Count > 0)
        //            {
        //                int total = alljobs.Count;

        //                var responses = alljobs.Skip(0)
        //                                   .Take(0)
        //                                   .ToList();

        //                var sortedRecords = responses.OrderByDescending(b => b.CreatedAt).ToList();
        //                var allsortedJobs = _mapper.Map<List<UpworkJobsDto>>(sortedRecords);

        //                var paginatedResult = QueryResponse<IEnumerable<UpworkJobsDto>>.Generate(allsortedJobs,
        //                  0,
        //                  0,
        //                  responses.Count,
        //                  total,
        //                  true,
        //                  "All jobs retrieved successfully");

        //                var result = new QueryResponse<IEnumerable<UpworkJobsDto>>()
        //                {
        //                    Data = paginatedResult.Data.ToList(),
        //                    Count = responses.Count,
        //                    Size = 0,
        //                    Message = paginatedResult.Message,
        //                    Page = 0,
        //                    Success = true,
        //                    Total = total
        //                };
        //                objResp.Result = result;
        //                objResp.IsSuccess = true;

        //            }
        //            else
        //            {
        //                objResp.IsSuccess = false;
        //                objResp.Result = new();
        //            }

        //        }
        //        else
        //        {
        //            alljobs = await _context.UpworkJobs.Where(m =>
        //                   (m.UserId == appUser.Result.UserId && m.IsActive == true && m.Status == AppConstants.AsDraft) && (m.CreatedAt >= requests.startDate && m.CreatedAt <= requests.endDate) && m.IsDeleted == false).ToListAsync();

        //            if (alljobs != null || alljobs.Count > 0)
        //            {
        //                int total = alljobs.Count;

        //                var responses = alljobs.Skip(0)
        //                                   .Take(0)
        //                                   .ToList();

        //                var sortedRecords = responses.OrderByDescending(b => b.CreatedAt).ToList();
        //                var allsortedJobs = _mapper.Map<List<UpworkJobsDto>>(sortedRecords);

        //                var paginatedResult = QueryResponse<IEnumerable<UpworkJobsDto>>.Generate(allsortedJobs,
        //                  0,
        //                  0,
        //                  responses.Count,
        //                  total,
        //                  true,
        //                  "All jobs retrieved successfully");

        //                var result = new QueryResponse<IEnumerable<UpworkJobsDto>>()
        //                {
        //                    Data = paginatedResult.Data.ToList(),
        //                    Count = responses.Count,
        //                    Size = 0,
        //                    Message = paginatedResult.Message,
        //                    Page = 0,
        //                    Success = true,
        //                    Total = total
        //                };
        //                objResp.Result = result;
        //                objResp.IsSuccess = true;

        //            }
        //            else
        //            {
        //                objResp.IsSuccess = false;
        //                objResp.Result = new();
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(exception: ex, ex.Message);
        //        objResp.Result = new QueryResponse<IEnumerable<UpworkJobsDto>>();
        //        objResp.IsSuccess = false;
        //        objResp.Message = "Oops! Failed to retrieve pending lc issuance. Kindly retry.";
        //    }
        //    return objResp;
        //}

        public async Task<GenResponse<QueryResponse<IEnumerable<UpworkJobsDto>>>> GetAllJobsByUser(GetAllJobsReq entity)
        {
            GenResponse<QueryResponse<IEnumerable<UpworkJobsDto>>> objResp = new() { Result = new() };
            List<UpworkJobs> alljobs = new();
            int ItemsPerPage = 0;
            int skip = 0;
            int Page = 50;
            DateTime? startDate = null;
            DateTime? endDate = null;
            try
            {
                GenResponse<ClaimDetailsDto> appUser = await _appSessionService.GetUserDetails();
                if (startDate == null || endDate == null)
                {
                    if (entity.Search != null)
                    {
                        alljobs = await _context.UpworkJobs.Where(m =>
                               (m.IsActive == true && m.UserId == appUser.Result.UserId) && m.Title.ToLower().Contains(entity.Search.ToLower())).ToListAsync();
                    }
                    else
                    {
                        alljobs = await _context.UpworkJobs.Where(m =>
                               (m.IsActive == true && m.UserId == appUser.Result.UserId && m.IsDeleted == false)).ToListAsync();
                    }

                    if (alljobs != null || alljobs.Count > 0)
                    {
                        int total = alljobs.Count;

                        var responses = alljobs.Skip(skip)
                                           .Take(ItemsPerPage)
                                           .ToList();

                        var sortedRecords = responses.OrderByDescending(b => b.CreatedAt).ToList();
                        var allsortedJobs = _mapper.Map<List<UpworkJobsDto>>(sortedRecords);

                        var paginatedResult = QueryResponse<IEnumerable<UpworkJobsDto>>.Generate(allsortedJobs,
                          Page,
                          ItemsPerPage,
                          responses.Count,
                          total,
                          true,
                          "All jobs retrieved successfully");

                        var result = new QueryResponse<IEnumerable<UpworkJobsDto>>()
                        {
                            Data = paginatedResult.Data.ToList(),
                            Count = responses.Count,
                            Size = ItemsPerPage,
                            Message = paginatedResult.Message,
                            Page = Page,
                            Success = true,
                            Total = total
                        };
                        objResp.Result = result;
                        objResp.IsSuccess = true;

                    }
                    else
                    {
                        objResp.IsSuccess = false;
                        objResp.Result = new();
                    }

                }
                else
                {

                    if (entity.Search != null)
                    {
                        alljobs = await _context.UpworkJobs.Where(m =>
                           (m.IsActive == true && m.UserId == appUser.Result.UserId) && m.Title.ToLower().Contains(entity.Search.ToLower()) && (m.CreatedAt >= startDate && m.CreatedAt <= endDate) && m.IsDeleted == false).ToListAsync();
                    }
                    else
                    {
                        alljobs = await _context.UpworkJobs.Where(m =>
                           (m.IsActive == true && m.UserId == appUser.Result.UserId) && (m.CreatedAt >= startDate && m.CreatedAt <= endDate) && m.IsDeleted == false).ToListAsync();
                    }

                    if (alljobs != null || alljobs.Count > 0)
                    {
                        int total = alljobs.Count;

                        var responses = alljobs.Skip(skip)
                                           .Take(ItemsPerPage)
                                           .ToList();

                        var sortedRecords = responses.OrderByDescending(b => b.CreatedAt).ToList();
                        var allsortedJobs = _mapper.Map<List<UpworkJobsDto>>(sortedRecords);

                        var paginatedResult = QueryResponse<IEnumerable<UpworkJobsDto>>.Generate(allsortedJobs,
                          Page,
                          ItemsPerPage,
                          responses.Count,
                          total,
                          true,
                          "All jobs retrieved successfully");

                        var result = new QueryResponse<IEnumerable<UpworkJobsDto>>()
                        {
                            Data = paginatedResult.Data.ToList(),
                            Count = responses.Count,
                            Size = ItemsPerPage,
                            Message = paginatedResult.Message,
                            Page = Page,
                            Success = true,
                            Total = total
                        };
                        objResp.Result = result;
                        objResp.IsSuccess = true;

                    }
                    else
                    {
                        objResp.IsSuccess = false;
                        objResp.Result = new();
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(exception: ex, ex.Message);
                objResp.Result = new QueryResponse<IEnumerable<UpworkJobsDto>>();
                objResp.IsSuccess = false;
                objResp.Message = "Oops! Failed to retrieve pending lc issuance. Kindly retry.";
            }
            return objResp;

        }
        public async Task<UpworkJobs> MapNewJob(CreateJobDto entity)
        {
            try
            {
                var thejob = new UpworkJobs
                {
                    Title = entity.Title,
                    WorkScope = entity.WorkScope.Scope,
                    WorkDuration = entity.WorkScope.Duration,
                    WorkExperienceLevel = entity.WorkScope.ExperienceLevel,
                    WorkContracToHire = entity.WorkScope.ContracToHire,
                    RateType = entity.WorkBudget.RateType,
                    FromAmount = entity.WorkBudget.FromAmount,
                    ToAmmount = entity.WorkBudget.ToAmmount,
                    Description = entity.Description,
                    EnglishLevel = entity.Preferences.EnglishLevel,
                    HireDate = entity.Preferences.HireDate,
                    HoursPerWeek = entity.Preferences.HoursPerWeek,
                    NoOfProfessionals = entity.Preferences.NoOfProfessionals,
                    TalentType = entity.Preferences.TalentType,
                    CountryCode = entity.Preferences.CountryCode,
                };
                return thejob;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return null;
            }

        }

        public async Task<UpworkJobs> MapUpdateJob(UpworkJobs existJob, CreateJobDto entity)
        {
            try
            {
                existJob.Title = entity.Title;
                existJob.WorkScope = entity.WorkScope.Scope;
                existJob.WorkDuration = entity.WorkScope.Duration;
                existJob.WorkExperienceLevel = entity.WorkScope.ExperienceLevel;
                existJob.WorkContracToHire = entity.WorkScope.ContracToHire;
                existJob.RateType = entity.WorkBudget.RateType;
                existJob.FromAmount = entity.WorkBudget.FromAmount;
                existJob.ToAmmount = entity.WorkBudget.ToAmmount;
                existJob.Description = entity.Description;
                existJob.EnglishLevel = entity.Preferences.EnglishLevel;
                existJob.HireDate = entity.Preferences.HireDate;
                existJob.HoursPerWeek = entity.Preferences.HoursPerWeek;
                existJob.NoOfProfessionals = entity.Preferences.NoOfProfessionals;
                existJob.TalentType = entity.Preferences.TalentType;
                existJob.CountryCode = entity.Preferences.CountryCode;
               
                return existJob;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return null;
            }

        }

        private async Task<string> UploadAttachment(IFormFile attachment)
        {
            if (attachment == null || attachment.Length == 0)
                return null;

            var fileName = Path.GetFileName(attachment.FileName);
            var fileExtension = Path.GetExtension(fileName);
            // Get the directory path for attachments
            var attachmentsDirectory = Path.Combine(Environment.CurrentDirectory, "Attachments");

            // Ensure the directory exists, create if not
            Directory.CreateDirectory(attachmentsDirectory);

            // Combine the directory path with the file name
            var filePath = Path.Combine(attachmentsDirectory, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await attachment.CopyToAsync(stream);
            }

            return $"{fileName}.{fileExtension}";
        }

        public async Task<List<UpworkJobsDto>> MapUpworkJob(List<UpworkJobs> entity)
        {
            try
            {
                var thejob = entity.Select(item => new UpworkJobsDto()
                {
                    Id = item.Id,
                    Title = item.Title,
                    WorkScope = item.WorkScope,
                    WorkDuration = item.WorkDuration,
                    WorkExperienceLevel = item.WorkExperienceLevel,
                    WorkContracToHire = item.WorkContracToHire,
                    RateType = item.RateType,
                    FromAmount = item.FromAmount,
                    ToAmmount = item.ToAmmount,
                    Description = item.Description,
                    EnglishLevel = item.EnglishLevel,
                    HireDate = item.HireDate,
                    HoursPerWeek = item.HoursPerWeek,
                    NoOfProfessionals = item.NoOfProfessionals,
                    TalentType = item.TalentType,
                    CountryCode = item.CountryCode,
                }).ToList();
                
                return thejob;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return null;
            }

        }
    }
}
