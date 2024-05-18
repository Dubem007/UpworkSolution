using Common.Extensions;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UserServices.Models
{
    public class Connections : CommonProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string AmountToBuy { get; set; }
        public string ChargeAmount { get; set; }
        public string NewConnectsbalance { get; set; }
        public DateTime Expiration { get; set; }
        public string PromoCode { get; set; }
    }
}
