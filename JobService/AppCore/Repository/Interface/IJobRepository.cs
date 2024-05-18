using Common.Response;
using IdentityServer.Models.DTOs;
using JobService.Domain.Dtos;
using JobService.Domain.Entities;
using OnaxTools.Dto.Http;

namespace JobService.AppCore.Repository.Interface
{
    public interface IJobRepository
    {
        Task<GenResponse<List<UpworkJobsDto>>> GetAllJobsByClientUserId();
        Task<GenResponse<JobStatusUpdateDto>> CreateNewJob(CreateJobDto entity);
        Task<GenResponse<string>> CreateUsers(AppUserDto entity);
        Task<GenResponse<List<UpworkJobsDto>>> GetAllJobsPosts();
        Task<GenResponse<List<UpworkJobsDto>>> GetAllJobsDrafts();
        Task<GenResponse<List<UpworkJobsDto>>> GetAllJobsBySkill();
        Task<GenResponse<UpworkJobsDto>> GetJobById(Guid jobId);
        //Task<GenResponse<QueryResponse<IEnumerable<UpworkJobsDto>>>> GetAllJobsBySkill(string Search);

        //Task<GenResponse<QueryResponse<IEnumerable<UpworkJobsDto>>>> GetAllJobs(string Search);
        //Task<GenResponse<QueryResponse<IEnumerable<UpworkJobsDto>>>> GetAllJobsDrafts(string Search);
        //Task<GenResponse<QueryResponse<IEnumerable<UpworkJobsDto>>>> GetAllJobsPosts(string Search);
        Task<GenResponse<QueryResponse<IEnumerable<UpworkJobsDto>>>> GetAllJobsByUser(GetAllJobsReq entity);
    }
}
