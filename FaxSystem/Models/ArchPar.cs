namespace FaxSystem.Models
{
    public class ArchPar
    {
        public IEnumerable<Fax> BranchFaxes { get; set; }
        public IEnumerable<FaxBetweenBranches> BranchFaxesToBranch { get; set; }
        public string? EntryNumSearch { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? SubjectSearch { get; set; }
        public bool OpinionIsTrue { get; set; }
        public int? BranchID { get; set; }
        public string from { get; set; }
        public int? AgencyID { get; set; }
      
    }
}
