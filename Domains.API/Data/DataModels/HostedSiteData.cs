using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domains.API.Data.DataModels
{
    [Table("HostedSites")]
    public class HostedSiteData
    {
        [Key]
        public int? HostedSiteDetailsId { get; set; }
        [ForeignKey("Domains")]
        public int? DomainId { get; set; }
        [Required]
        [MaxLength(255)]
        public string? HostingProvider { get; set; } = string.Empty;
        [Required]
        public DateTime? RenewalDate { get; set; } = DateTime.MinValue;
        [Required]
        [MaxLength(255)]
        public string? ServerName { get; set; } = string.Empty;
    }
}
