namespace UserServices.Models.DTOs
{
    public class AppUserDto
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public string Country { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool SendHelpfulEmails { get; set; }
        public bool HasReadPolicy { get; set; }
    }

    public class ClaimDetailsDto
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Country { get; set; }
        public string? FullName { get; set; }
    }

    public class Add2FAResponseDto
    {
        public string Email { get; set; }
        public string SecretKey { get; set; }
    }
}
