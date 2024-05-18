using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Common.Extensions;

namespace IdentityService.models
{
    public class IdentityDocuments : CommonProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [StringLength(500)]
        public string DocumentName { get; set; }

        [Required]
        [StringLength(500)]
        public string ExtensionType { get; set; }

        [Required]
        [StringLength(500)]
        public string FullDocumentNamePath { get; set; }
    }
}
