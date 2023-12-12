using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OAuth2CoreLib.Models
{
    [Table("OAuth2.ClientScopes")]
    public class ClientScope: IScope
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public Client Client { get; set; }
        [JsonIgnore]
        [Required]
        public ResourceScope ResourceScope { get; set; }

    }
}
