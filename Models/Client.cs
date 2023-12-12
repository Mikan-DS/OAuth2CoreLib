using Microsoft.Extensions.Hosting;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OAuth2CoreLib.Models
{
    [Table("OAuth2.Clients")]
    public class Client
    {

        [Key]
        public string ClientId { get; set; }
        public string? Secret { get; set; }
        public bool Enabled { get; set; } = true;
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime Updated { get; set; } = DateTime.UtcNow;
        public DateTime? LastAccessed { get; set; }
        [JsonIgnore]
        [InverseProperty("Client")]
        public List<ClientScope> Scopes { get; set; }

    }

}
