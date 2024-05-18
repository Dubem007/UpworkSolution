using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Common.Extensions;

namespace JobService.Domain.Entities
{
    public class UpworkJobs: CommonProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? UserId { get; set; }
        public string? WorkScope { get; set; }
        public string? WorkDuration { get; set; }
        public string? WorkExperienceLevel { get; set; }
        public bool? WorkContracToHire { get; set; }
        public string? RateType { get; set; }
        public Decimal? FromAmount { get; set; }
        public Decimal? ToAmmount { get; set; }
        public string? Status { get; set; } = AppConstants.AsPosted;
        public string? Description { get; set; }
        public string? Attachment { get; set; }
        public string? EnglishLevel { get; set; }
        public string? HireDate { get; set; }
        public string? HoursPerWeek { get; set; }
        public string? NoOfProfessionals { get; set; }
        public string? TalentType { get; set; }
        public string? CountryCode { get; set; }
        public List<Question> Question { get; set; }
        public List<JobSkill> JobSkills { get; set;}
    }

}
