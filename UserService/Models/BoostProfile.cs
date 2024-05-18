using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Common.Extensions;

namespace UserServices.Models
{
    public class BoostProfile: CommonProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string Category { get; set; }
        public string Specialty { get; set; }
        public string Bidperclick { get; set; }
        public string Limit { get; set; }
        public string LimitDuration { get; set; }
        public string ProfileClicksDaily { get; set; }
        public DateTime? Enddate { get; set; }
        public bool? BoostEnabled { get; set; }
        public string? TurnOffReason { get; set; }
    }
}
