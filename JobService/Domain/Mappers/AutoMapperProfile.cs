
using JobService.Domain.Dtos;
using JobService.Domain.Entities;

namespace JobService.Domain.Configs.Mappers
{
    public class AutoMapperProfile : AutoMapper.Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UpworkJobsDto, UpworkJobs>().ReverseMap();
        }
    }
}
