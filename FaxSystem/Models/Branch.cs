using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FaxSystem.Models
{
    public class Branch
    {
        [Key]
        public int ID { get; set; }
        [Required(ErrorMessage="برجاء ادخال اسم الفرع")]
        public string Name { get; set; }
        public ICollection<User>? Users { get; set; }
        [NotMapped]
        public ICollection<Fax>? RecivedFaxes { get; set; }
        [NotMapped]
        public ICollection<FaxBetweenBranches>? BranchRecivedFaxes { get; set; }
        [NotMapped]
        public ICollection<FaxBetweenBranches>? BranchSendFaxes { get; set; }
        [NotMapped]
        public ICollection<Decision>? Decisions { get; set; }
        public ICollection<FaxReciver>? FaxRecivers { get; set; }
        public ICollection<BranchFaxRecivers>? BranchFaxRecivers { get; set; }



    }
}
