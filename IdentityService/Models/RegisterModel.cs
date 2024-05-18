using System.ComponentModel.DataAnnotations;

namespace IdentityService.models
{
    public class RegisterModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
    }

    public class RegisterDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public string RoleId { get; set; }
        [Required]
        public string CountryCode { get; set; }
        public bool SendHelpfulEmails { get; set; }
        public bool HasReadPolicy { get; set; }
    }

    public class IdentityDto
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public bool HasReadIdentityPolicy { get; set; }
        [Required]
        public string IdType { get; set; }
        [Required]
        [EmailAddress]
        public IFormFile FrontView { get; set; }
        [Required]
        public IFormFile BackView { get; set; }
        [Required]
        public IFormFile Face { get; set; }
        [Required]
        public string CountryCode { get; set; }
        public bool SendHelpfulEmails { get; set; }
        public bool HasReadPolicy { get; set; }
    }
}
