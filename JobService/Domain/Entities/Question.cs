using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Common.Extensions;

namespace JobService.Domain.Entities
{
    public class Question : CommonProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid JobId { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }
        public UpworkJobs Job { get; set; }
    }
}
