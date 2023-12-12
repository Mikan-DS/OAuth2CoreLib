using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OAuth2CoreLib.Models
{
    [Table("OAuth2.AuthorizationCodes")]
    public class AuthorizationCode
    {
        [Key]
        public string Code { get; set; }

        public User User { get; set; }

        public bool Activated { get; set; }

        [JsonIgnore]
        [InverseProperty("AuthorizationCode")]
        public List<AuthorizationCodeScope> Scopes { get; set; }
    }
}
