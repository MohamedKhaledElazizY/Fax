using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FaxSystem.Models
{
    public class CreateFaxbetweenbranches
    {
        public FaxBetweenBranches faxBetweenBranches { get; set; }
        [Required(ErrorMessage= "الرجاء اختيار فرع علي الاقل")]
        public int[] Branches { get; set; }

    }
}
