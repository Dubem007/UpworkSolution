using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Models.DTO
{
    public class EmailConfig
    {
        public EmailConfig() { }

        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string SmtpService { get; set; }
        public string SmtpEmailId { get; set; }
        public string SmtpPassword { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpHost { get; set; }
        public bool IsDevelopment { get; set; }
    }
}
