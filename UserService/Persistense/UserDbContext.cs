using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserServices.Models;

namespace UserServices.Persistense
{
    public class UserDbContext : IdentityDbContext<ApplicationUser>
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<IdentityDocuments> IdentityDocuments { get; set; }
        public DbSet<UserProfileRole> UserProfileRoles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<Connections> Connections { get; set; }
        public DbSet<BoostProfile> BoostProfiles { get; set; }
        public DbSet<Badge> Badges { get; set; }
        public DbSet<SecurityQuestion> SecurityQuestions { get; set; }
        public DbSet<ConnectsHistory> ConnectsHistories { get; set; }
        public DbSet<MembershipPlan> MembershipPlans { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<UserLanguage> UserLanguages { get; set; }
        public DbSet<Proficiency> Proficiencies { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<FreelancePrivateQuestions> FreelancePrivateQuestions { get; set; }
        public DbSet<UserWorkExperience> UserWorkExperiences { get; set; }
        public DbSet<UserEducation> UserEducations { get; set; }
        public DbSet<UserSkill> UserSkills { get; set; }
        public DbSet<ServicesToOffer> ServicesToOffers { get; set; }






    }
}
