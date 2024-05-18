namespace UserServices.Models.DTOs
{
    public class SecurityQuestionDto
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public bool AcknowledgePolicy { get; set; }
        public bool KeepMeLoggedIn { get; set; }
    }
}
