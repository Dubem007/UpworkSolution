using System.ComponentModel.DataAnnotations;

namespace NotificationService.Models.DTO
{
    public class UserCreationNotifyDTO
    {
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        [StringLength(250)]
        public string Fullname { get; set; }

        [Required]
        [StringLength(250)]
        public string Username { get; set; }

        [Required]
        [StringLength(250)]
        public string Password { get; set; }

        [Required]
        public string MailBody { get; set; }
    }

    public class ClaimDetailsDto
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Country { get; set; }
        public bool? FullName { get; set; }
    }
}
