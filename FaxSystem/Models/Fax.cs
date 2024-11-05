using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace FaxSystem.Models
{
    public class Fax
    {
        [Key]
        public int ID { get; set; }
        //[Required]
      //  public String FaxNum { get; set; }
        [Required(ErrorMessage = "برجاء ادخال رقم القيد")]
        public String RegistrationNum { get; set; }
        public bool suspend { get; set; }



        [Required(ErrorMessage ="برجاء اختيار الجهة"),Range(1,int.MaxValue,ErrorMessage = "برجاء اختيار الجهة"), ForeignKey("senderAgency")]
        public int SenderAgencyID { get; set; }
        public Agency? senderAgency { get; set; }

        [NotMapped]
        public string? SenderAgencyName { get; set; }




        [Required(ErrorMessage = "برجاء ادخال الموضوع")]
        public string Subject { get; set; }

        [Required, DataType(DataType.Date),Column(TypeName ="date")]
        public DateTime Date { get; set; }
        public List<FaxLink>? FaxLinks { get; set; }
        //public string? AttchementLink { get; set; }
        [ForeignKey("decision")]
        public int? DecisionID { get; set; }

        public Decision? decision { get; set; }
        
        public string? Notes { get; set; }

       

        // public FaxReciver? faxReciver { get; set; }

        public ICollection<FaxReciver>? FaxRecivers { get; set; }

    }
}
