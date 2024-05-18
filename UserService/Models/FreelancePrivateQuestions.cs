using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Common.Extensions;

namespace UserServices.Models
{
    public class FreelancePrivateQuestions : CommonProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string FreeLanceQuestion { get; set; }
        public string FreeLanceGoal { get; set; }
        public string? FreeLanceWorkStyle { get; set; }
        public bool contracttohireopportunities { get; set; }
        public string? WelcomeGreeetings { get; set; }
    }
}
