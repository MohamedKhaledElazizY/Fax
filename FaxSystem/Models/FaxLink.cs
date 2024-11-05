using MessagePack;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FaxSystem.Models
{
    public class FaxLink
    {
        public int ID { get; set; }
        [Required]
        public String link { get; set; }
        [ ForeignKey("Fax")]
        public int? FaxId { get; set; }
        public Fax? Fax { get; set; }
        //   [ ForeignKey("FaxBranches")]
        public int? FaxBetweenBranchesID { get; set; }
     //   public Fax? FaxBranches { get; set; }
    }
}
