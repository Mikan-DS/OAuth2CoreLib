using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OAuth2CoreLib.Models
{
    [Table("OAuth2.UserAllowedScopes")]
    public class UserAllowedScope: IScope
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public User User { get; set; } = new User();

        [Required]
        public ResourceScope ResourceScope { get; set; } = new ResourceScope();

    }
}
