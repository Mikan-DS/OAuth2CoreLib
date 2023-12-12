using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OAuth2CoreLib.Models
{
    [Table("OAuth2.Users")]
    public class User
    {
        [Key]
        public string user_id { get; set; }
        public string? secret { get; set; }

        [JsonIgnore]
        [InverseProperty("user")]
        public List<UserAllowedScope> Scopes { get; set; }
    }
}
