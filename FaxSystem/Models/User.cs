using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FaxSystem.Models
{
    public class User
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage="برجاء اختيار فرع المستخدم"), ForeignKey("branch")]
        public int BranchID { get; set; }

        public Branch? branch { get; set; }

        [Required(ErrorMessage = "يجب ألا يقل اسم المستخدم عن 3 حروف"), MinLength(3)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "يجب ألا تقل كلمة المرور عن 4 حروف"), MinLength(4)]
        public string Password { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "يجب تأكيد كلمة المرور")]
        [System.ComponentModel.DataAnnotations.Compare("Password",ErrorMessage ="غير مطابقة لكلمة المرور")]
        public string ConfirmPassword { get; set; }
        public int num_read_faxes { get; set; }
        public int num_read_faxes_branches { get; set; }
    }
}
