using System.ComponentModel.DataAnnotations;

namespace NotificationService.Models.DTO
{
    public class OnboardingEmailRequestDTO
    {
        [Required]
        [EmailAddress]
        public string CompanyEmail { get; set; }
        [Required]
        public string CompanyName { get; set; }

        [Required]
        public string CorpCode { get; set; }

        public string Subject { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }
    }
}
