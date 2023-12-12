using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OAuth2CoreLib.Models
{
    [Table("OAuth2.UserAllowedScopes")]
    public class UserAllowedScope: IScope
    {
        [Key]
        public int key { get; set; }

        [Required]
        public User user { get; set; }

        [Required]
        public ResourceScope ResourceScope { get; set; }

    }
}
