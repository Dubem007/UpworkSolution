using Common.Extensions;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UserServices.Models
{
    public class SecurityQuestion: CommonProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public bool AcknowledgePolicy { get; set; }
        public bool KeepMeLoggedIn { get; set; }
    }
}
