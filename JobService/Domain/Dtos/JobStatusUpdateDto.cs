namespace IdentityServer.Models.DTOs
{
    public class JobStatusUpdateDto
    {
        public string JobId { get; set; }
        public string Title { get; set; }
        public bool IsDraft { get; set; }
    }

}
