using System.Runtime.ConstrainedExecution;

namespace UserServices.Models.DTOs
{
    public class BoostProfileDto
    {
        public string Category { get; set; }
        public string Specialty { get; set; }
        public string Bidperclick { get; set; }
        public string Limit { get; set; }
        public string LimitDuration { get; set; }
        public string ProfileClicksDaily { get; set; }
        public DateTime? Enddate { get; set; }
    }

    public class BuyConnectsDto
    {
        public string Category { get; set; }
        public string Specialty { get; set; }
        public string Bidperclick { get; set; }
        public string Limit { get; set; }
        public string LimitDuration { get; set; }
        public DateTime? Enddate { get; set; }
    }
}
