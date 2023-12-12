using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OAuth2CoreLib.Models
{
    [Table("OAuth2.AuthorizationCodeScope")]
    public class AuthorizationCodeScope: IScope
    {
        [Key]
        public int Id { get; set; }
        [Required]
        //[Column("AuthorizationCodeId")]
        public AuthorizationCode AuthorizationCode { get; set; }
        [Required]
        public ResourceScope ResourceScope { get; set; }
    }
}
