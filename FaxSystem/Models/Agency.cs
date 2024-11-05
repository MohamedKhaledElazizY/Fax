using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FaxSystem.Models
{
    public class Agency
    {
        public int ID { get; set; }
        [Required(ErrorMessage="برجاء ادخال اسم الجهة")]
        
        public string Name { get; set; }
        public ICollection<Fax>? SendFaxes { get; set; }
    }
}
