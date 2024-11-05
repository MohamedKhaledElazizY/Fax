using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace FaxSystem.Models
{
    public class FaxBetweenBranches
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "برجاء ادخال رقم القيد")]
        public String RegistrationNum { get; set; }

        [ForeignKey("senderBranch")]
        public int SenderBranchID { get; set; }
        public Branch? senderBranch { get; set; }
        //   public Branch reciveBranch { get; set; }
        [Required(ErrorMessage ="برجاء إدخال الموضوع")]
        public string Subject { get; set; }

        public int suspend { get; set; }

        [Required, DataType(DataType.Date), Column(TypeName = "date")]
        public DateTime Date { get; set; }
        public List<FaxLink>? FaxLinks { get; set; }

        public string? Notes { get; set; }
        [ForeignKey("decision")]
        public int? DecisionID { get; set; }

        [NotMapped]
        public List<string>? BranchNames { get; set; }

        public Decision? decision { get; set; }
        
        public ICollection<BranchFaxRecivers>? BranchFaxRecivers { get; set; }

    }
}
