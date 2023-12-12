using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OAuth2CoreLib.Models
{
    [Table("OAuth2.ResourceScopes")]
    public class ResourceScope
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Scope { get; set; }
    }
}
