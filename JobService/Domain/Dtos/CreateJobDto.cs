using Common.Models.Paging;

namespace JobService.Domain.Dtos
{
    public class CreateJobDto
    {
        public string? Id { get; set; }
        public string Title { get; set; }
        public List<string> Skills { get; set; }
        public WorkScope WorkScope { get; set; }
        public WorkBudget WorkBudget { get; set; }
        public string Description { get; set; }
        // public IFormFile? Attachment { get; set; }
        public List<ScreeningQuestions> Questions { get; set; }
        public Preference Preferences { get; set; }
        public bool IsDraft { get; set; }
    }

    public class WorkScope
    {
        public string Scope { get; set; }
        public string Duration { get; set; }
        public string ExperienceLevel { get; set; }
        public bool ContracToHire { get; set; }
    }

    public class WorkBudget
    {
        public string RateType { get; set; }
        public Decimal FromAmount { get; set; }
        public Decimal ToAmmount { get; set; }
    }

    public class ScreeningQuestions
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }

    public class Preference
    {
        public string EnglishLevel { get; set; }
        public string HireDate { get; set; }
        public string HoursPerWeek { get; set; }
        public string NoOfProfessionals { get; set; }
        public string TalentType { get; set; }
        public string CountryCode { get; set; }
    }

    public class GetAllJobsReq
    {
        public string? Search { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public int pageIndex { get; set; } = 1;
        public int pageSize { get; set; } = 50;
    }

}
