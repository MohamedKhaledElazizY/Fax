namespace FaxSystem.Models
{
    public class DecisionPars
    {
        public Fax? Fax { get; set; }
        public DateTime? FaxDate { get; set; }
        public string? SenderAgency { get; set; }
        public string? Subject { get; set; }
        public string? Opinion { get; set; }
        public Decision? Decision { get; set; }
    }
}
