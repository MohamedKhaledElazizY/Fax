using FaxSystem.Models;

namespace FaxSystem.ViewModels
{
    public class FaxTableRowViewModel
    {
        public Fax fax { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDecide { get; set; }
        public bool IsSuspended { get; set; }
        public bool IsManager { get; set; }
        public bool IsRealTime { get; set; }
        public FaxTableRowViewModel()
        {
            this.fax = new Fax();
            this.CanEdit = false;
            this.CanDecide = false;
            this.IsSuspended = false;
            this.IsManager = false;
            this.IsRealTime = false;
        }
        public FaxTableRowViewModel(Fax fax, bool edit, bool decide)
        {
            this.fax = fax;
            this.CanEdit = edit;
            this.CanDecide = decide;
        }
    }
}