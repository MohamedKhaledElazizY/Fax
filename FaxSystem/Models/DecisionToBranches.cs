using System.ComponentModel.DataAnnotations;

namespace FaxSystem.Models
{
    public class DecisionToBranches
    {
        public FaxBetweenBranches? faxBetweenBranches { get; set; }
        public Fax? fax { get; set; }
       
        public int[]? Branches { get; set; }
        public int type { get; set; }

    }
}
