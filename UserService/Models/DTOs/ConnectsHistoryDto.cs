namespace UserServices.Models
{
    public class ConnectsHistoryDto
    {
        public string UserId { get; set; }
        public string ConnectUsed { get; set; }
        public string Action { get; set; }
        public string NewConnectBalance { get; set; }
        public DateTime ConnectDate { get; set; }
    }
}
