using Common.Extensions;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UserServices.Models
{
    public class ConnectsHistory : CommonProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string ConnectUsed { get; set; }
        public string Action { get; set; }
        public string NewConnectBalance { get; set; }
        public DateTime ConnectDate { get; set; }
    }
}
