using System.ComponentModel.DataAnnotations;
using UserServices.Models.Enums;

namespace UserServices.Models.DTOs
{
    public class TokenDTO
    {
        public string? access_token { get; set; }
        public int expires_in { get; set; }
        public string? token_type { get; set; }
        public string? scope { get; set; }
    }

    public class LoginInputDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class EmaiReqsDto
    {
        public string RecipientEmails { get; set; }
        public string RecipientName { get; set; }
        public string SenderName { get; set; }
        public string Subject { get; set; }
        public string SenderEmail { get; set; }
        public EmailTypesEnum EmailType { get; set; }
        public string? UserId { get; set; }
    }
}
