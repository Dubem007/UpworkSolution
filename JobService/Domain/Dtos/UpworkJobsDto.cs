namespace JobService.Domain.Dtos
{
    public class UpworkJobsDto
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? UserId { get; set; }
        public string? Skills { get; set; }
        public string? WorkScope { get; set; }
        public string? WorkDuration { get; set; }
        public string? WorkExperienceLevel { get; set; }
        public bool? WorkContracToHire { get; set; }
        public string? RateType { get; set; }
        public Decimal? FromAmount { get; set; }
        public Decimal? ToAmmount { get; set; }
        public string? Description { get; set; }
        public IFormFile? Attachment { get; set; }
        public string? Questions { get; set; }
        public string? EnglishLevel { get; set; }
        public string? HireDate { get; set; }
        public string? HoursPerWeek { get; set; }
        public string? NoOfProfessionals { get; set; }
        public string? TalentType { get; set; }
        public string? CountryCode { get; set; }
    }
}
