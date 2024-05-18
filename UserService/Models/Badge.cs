using Common.Extensions;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UserServices.Models
{
    public class Badge : CommonProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public bool AvailabileNow { get; set; }
        public DateTime AvailableDate { get; set; }
    }
}
