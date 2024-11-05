


using System.ComponentModel.DataAnnotations;


namespace FaxSystem.Models
{
    public class Role
    {
        [Key]
        public int RoleID { get; set; }
        [Required(ErrorMessage="بجاء ادخال اسم الصلاحية")]
        public string? RoleName { get; set; }
    }
}
