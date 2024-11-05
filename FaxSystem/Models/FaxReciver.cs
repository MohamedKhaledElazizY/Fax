using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FaxSystem.Models
{
    public class FaxReciver
    {
        
        [ForeignKey("fax")]
        public int FaxID { get; set; }

        [ForeignKey("branch")]
        public int BranchID { get; set; }
        public Fax? fax { get; set; }
        
        public Branch? branch { get; set; }


    }
}
