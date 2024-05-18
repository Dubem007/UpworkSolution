using Common.Extensions;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using UserService.Models.DTOs;

namespace UserServices.Models.DTOs
{
    public class CreateUserProfile
    {
        public FreeLancePrivateQuestionsDto? FreeLanceQuestion { get; set; }
        public string professionalrole { get; set; }
        public List<UserWorkExperienceDto>? WorkExperiences { get; set; }
        public List<UserEducationDto>? Educations { get; set; }
        public List<UserLanguageDto>? UserLanguages { get; set; }
        public List<UserSkillDto>? UserSkills { get; set; }
        public string? UserBio { get; set; }
        public List<ServicesToOfferDto>? ServicesToOffer { get; set; }
        public string HourlyRate { get; set; }
        public string ServiceFee { get; set; }
        public string YouGet { get; set; }
        // public IFormFile? ProfilePicture { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Country { get; set; }
        public string StreetAddress { get; set; }
        public string? AptSuite { get; set; }
        public string City { get; set; }
        public string? StateOrProvince { get; set; }
        public string? ZipOrPostalCode { get; set; }
        public string? PhoneNumber { get; set; }
    }

    public class UserWorkExperienceDto 
    {
        public string Title { get; set; }
        public string Company { get; set; }
        public string? Location { get; set; }
        public string Country { get; set; }
        public bool? CurrentWorkingRole { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
    }

    public class UserEducationDto 
    {
        public string School { get; set; }
        public string Degree { get; set; }
        public string? FieldOfStudy { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Description { get; set; }
    }

    public class UserLanguageDto
    {
        public string Language { get; set; }
        public string Proficiency { get; set; }
    }

    public class UserSkillDto 
    {
        public string Name { get; set; }
    }

    public class ServicesToOfferDto 
    { 
        public string Service { get; set; }
    }
}
