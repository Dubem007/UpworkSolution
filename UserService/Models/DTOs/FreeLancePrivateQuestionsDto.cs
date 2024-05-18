using Common.Extensions;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UserService.Models.DTOs
{
    public class FreeLancePrivateQuestionsDto
    {
        public string UserId { get; set; }
        public string FreeLanceQuestion { get; set; }
        public string FreeLanceGoal { get; set; }
        public string? FreeLanceWorkStyle { get; set; }
        public bool contracttohireopportunities { get; set; }
        public string? WelcomeGreeetings { get; set; }
    }

}
