using Common.Extensions;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace JobService.Domain.Entities
{
    public class JobSkill : CommonProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid JobId { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public UpworkJobs Job { get; set; }
    }
}
