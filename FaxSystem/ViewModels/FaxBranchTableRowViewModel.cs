using FaxSystem.Models;

namespace FaxSystem.ViewModels
{
    public class FaxBranchTableRowViewModel
    {
        public FaxBetweenBranches faxBetweenBranches { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDecide { get; set; }
        public bool IsSuspended { get; set; }
        public bool CanSuspend { get; set; }
        public bool IsManager { get; set; }
        public bool IsRealTime{ get; set; }

        public FaxBranchTableRowViewModel()
        {
            this.faxBetweenBranches = new FaxBetweenBranches();
            this.CanEdit = false;
            this.CanDecide = false;
            this.IsSuspended = false;
            this.CanSuspend = false;
            this.IsManager = false;
            this.IsRealTime = false;
        }
        public FaxBranchTableRowViewModel(FaxBetweenBranches faxBetweenBranches, bool edit, bool decide)
        {
            this.faxBetweenBranches = faxBetweenBranches;
            this.CanEdit = edit;
            this.CanDecide = decide;
        }
    }
}