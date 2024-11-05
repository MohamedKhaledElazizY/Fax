using System.ComponentModel.DataAnnotations.Schema;

namespace FaxSystem.Models
{
    public class UserRoles
    {
        [ForeignKey("user")]
        public int UserId { get; set; }
        [ForeignKey("role")]
        public int RoleId { get; set; }

        public User? user { get; set; }
        public Role? role { get; set; }
    }
}
