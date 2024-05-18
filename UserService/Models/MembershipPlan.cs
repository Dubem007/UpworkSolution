using Common.Extensions;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UserServices.Models
{
    public class MembershipPlan : CommonProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string SubscriptionPlan { get; set; }
        public string PlanType { get; set; }
    }
}
