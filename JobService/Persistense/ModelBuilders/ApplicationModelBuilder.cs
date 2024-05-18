using JobService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobService.Persistence.ModelBuilders
{
    public static class ApplicationModelBuilder
    {
        public static ModelBuilder AppModelBuilder(this ModelBuilder model)
        {

            model.Entity<UpworkJobs>(prop =>
            {
                prop.HasMany<JobSkill>(m => m.JobSkills).WithOne(p => p.Job).HasForeignKey(f => f.JobId).OnDelete(DeleteBehavior.Cascade);
                prop.HasMany<Question>(m => m.Question).WithOne(p => p.Job).HasForeignKey(f => f.JobId).OnDelete(DeleteBehavior.Cascade);
                prop.HasIndex(u => u.Id, "ix_nonclustered_upworkjobs_Id").IsUnique();
                prop.HasIndex(u => u.UserId, "ix_nonclustered_upworkjobs_UserId");
            });

           
            model.Entity<JobUser>(prop =>
            {
                prop.HasIndex(u => u.UserId, "ix_nonclustered_jobuser_Id").IsUnique();
            });

            model.Entity<UpworkJobs>(prop =>
            {
                prop.HasIndex(u => u.UserId, "ix_nonclustered_upworkjob_jobuser_Id").IsUnique();
                prop.HasIndex(u => u.Id, "ix_nonclustered_upworkjob_Id").IsUnique();
            });
            model.Entity<Question>(prop =>
            {
                prop.HasIndex(u => u.JobId, "ix_nonclustered_question_upworkjob_Id").IsUnique();
                prop.HasIndex(u => u.Id, "ix_nonclustered_question_Id").IsUnique();
            });
            model.Entity<JobSkill>(prop =>
            {
                prop.HasIndex(u => u.JobId, "ix_nonclustered_jobskill_jobuser_Id").IsUnique();
                prop.HasIndex(u => u.Id, "ix_nonclustered_jobskill_Id").IsUnique();
            });

            return model;
        }
    }
}
