using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domains.API.Data.DataModels
{
    [Table("Domains")]
    public class DomainData
    {
        [Key]
        public int? DomainId { get; set; }
        [Required]
        [MaxLength(255)]
        public string? DomainName { get; set; } = string.Empty;
        [Required]
        [MaxLength(50)]
        public string? Status { get; set; } = string.Empty;


    }
}
