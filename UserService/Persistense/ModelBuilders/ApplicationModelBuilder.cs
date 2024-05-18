using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using UserServices.Models;

namespace UserServices.Persistence.ModelBuilders
{
    public static class ApplicationModelBuilder
    {
        public static ModelBuilder AppModelBuilder(this ModelBuilder model)
        {
            model.Entity<UserProfile>()
            .HasOne(p => p.FreeLanceQuestion)
            .WithOne()
            .HasForeignKey<FreelancePrivateQuestions>(p => p.UserId);

            model.Entity<UserProfile>()
                .HasMany(p => p.WorkExperiences)
                .WithOne()
                .HasForeignKey(p => p.UserId);

            model.Entity<UserProfile>()
                .HasMany(p => p.Educations)
                .WithOne()
                .HasForeignKey(p => p.UserId);

            model.Entity<UserProfile>()
                .HasMany(p => p.UserLanguages)
                .WithOne()
                .HasForeignKey(p => p.UserId);

            model.Entity<UserProfile>()
                .HasMany(p => p.UserSkills)
                .WithOne()
                .HasForeignKey(p => p.UserId);

            model.Entity<ApplicationUser>(prop =>
            {
                prop.HasIndex(u => u.Id, "ix_nonclustered_appuser_Id").IsUnique();
            });

            model.Entity<UserProfile>(prop =>
            {
                prop.HasIndex(u => u.Id, "ix_nonclustered_userprofile_Id").IsUnique();
                prop.HasIndex(u => u.UserId, "ix_nonclustered_userprofile_userId").IsUnique();
            });
            model.Entity<ServicesToOffer>(prop =>
            {
                prop.HasIndex(u => u.Id, "ix_nonclustered_servicestooffer_Id").IsUnique();
            });
            model.Entity<FreelancePrivateQuestions>(prop =>
            {
                prop.HasIndex(u => u.Id, "ix_nonclustered_freelanceprivatequestions_id").IsUnique();
            });
            model.Entity<UserSkill>(prop =>
            {
                prop.HasIndex(u => u.Id, "ix_nonclustered_userskill_Id").IsUnique();
            });
            model.Entity<UserLanguage>(prop =>
            {
                prop.HasIndex(u => u.Id, "ix_nonclustered_userlanguage_Id").IsUnique();
            });
            model.Entity<UserEducation>(prop =>
            {
                prop.HasIndex(u => u.Id, "ix_nonclustered_usereducation_Id").IsUnique();
            });
            model.Entity<UserWorkExperience>(prop =>
            {
                prop.HasIndex(u => u.Id, "ix_nonclustered_userworkexperience_Id").IsUnique();
            });

            return model;
        }
    }
}
