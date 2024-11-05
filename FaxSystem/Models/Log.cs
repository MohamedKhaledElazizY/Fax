using System.ComponentModel.DataAnnotations;

namespace FaxSystem.Models
{
    public class Log
    {
        [Key]
        public int Id { get; set; }
        public string ActionTakerName { get; set; }
        public string ActionDescription { get; set; }
        public DateTime date { get; set; }
    }
}
