using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace FaxSystem.Models
{
    public class BranchFaxRecivers
    {
        [ForeignKey("fax")]
        public int FaxID { get; set; }

        [ForeignKey("branch")]
        public int BranchID { get; set; }
        public FaxBetweenBranches? fax { get; set; }

        public Branch? branch { get; set; }
    }
}
