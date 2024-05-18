using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Common.Extensions;

namespace UserServices.Models
{
    public class UserProfileRole
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [Required]
        public string RoleId { get; set; }

        [Required]
        public string UserId { get; set; }

        public bool? IsActive { get; set; } = true;
        public string OperationBy { get; set; } = AppConstants.AppSystem;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool? IsDeleted { get; set; } = false;
    }
}
