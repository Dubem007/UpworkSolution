using JobService.Domain.Dtos;
using JobService.Domain.Entities;
using JobService.Models;
using Microsoft.EntityFrameworkCore;

namespace JobService.Persistense
{
    public class JobServiceDbContext : DbContext
    {
        public JobServiceDbContext(DbContextOptions<JobServiceDbContext> options) : base(options)
        {
        }

        public DbSet<UpworkJobs> UpworkJobs { get; set; }
        public DbSet<JobUser> JobUsers { get; set; }
        public DbSet<Question> Questiona { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<JobSkill> JobUserSkills { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        

    }
}
