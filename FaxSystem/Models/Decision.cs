using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FaxSystem.Models
{
    public class Decision
    {
        [Key]
        public int ID { get; set; }
      
        public string? Opinion { get; set; }
        public string? OpinionVoice { get; set; }
        public bool DecisionCheck { get; set; }
        public bool PersonalReview { get; set; }
        public string? DecisionVoice { get; set; }
        public string? DecisionText { get; set; }

       // public Fax Fax { get; set; }
       // public ICollection<FaxReciver> FaxRecivers { get; set; }
    }
}
