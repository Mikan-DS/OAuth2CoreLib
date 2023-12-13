using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OAuth2CoreLib.Models
{
    [Table("OAuth2.Users")]
    public class User
    {
        [Key]
        public string UserId { get; set; } = string.Empty;
        public string? Secret { get; set; }

        public bool Enabled { get; set; } = true;
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime Updated { get; set; } = DateTime.UtcNow;
        public DateTime? LastAccessed { get; set; }

        [JsonIgnore]
        [InverseProperty("User")]
        public List<UserAllowedScope> Scopes { get; set; } = new List<UserAllowedScope>();
    }
}
