using Common.Extensions;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UserServices.Models
{
    public class UserProfile : CommonProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string professionalrole { get; set; }      
        public string? UserBio { get; set; }
        public string HourlyRate { get; set; }
        public string ServiceFee { get; set; }
        public string YouGet { get; set; }
        public string ImageUrl { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Country { get; set; }
        public string StreetAddress { get; set; }
        public string? AptSuite { get; set; }
        public string City { get; set; }
        public string? StateOrProvince { get; set; }
        public string? ZipOrPostalCode { get; set; }
        public string? PhoneNumber { get; set; }
        public ApplicationUser User { get; set; }
        public FreelancePrivateQuestions? FreeLanceQuestion { get; set; }
        public List<UserWorkExperience>? WorkExperiences { get; set; }
        public List<UserEducation> Educations { get; set; }
        public List<UserLanguage>? UserLanguages { get; set; }
        public List<UserSkill>? UserSkills { get; set; }
        public List<ServicesToOffer>? ServicesToOffer { get; set; }
    }
}
