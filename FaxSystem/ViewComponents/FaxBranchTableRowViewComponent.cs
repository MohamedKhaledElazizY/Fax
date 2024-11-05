using FaxSystem.ViewModels;
using FaxSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FaxSystem.ViewComponents
{
    public class FaxBranchTableRowViewComponent : ViewComponent
    {
        private FaxBranchTableRowViewModel? faxBranchTableRowViewModel;

        public async Task<IViewComponentResult> InvokeAsync(FaxBetweenBranches faxBetweenBranches, bool IsSuspended, bool CanEdit, bool CanDecide, bool CanSuspend, bool IsManager = false, bool IsRealTime = false)
        {
            this.faxBranchTableRowViewModel = new FaxBranchTableRowViewModel();
            this.faxBranchTableRowViewModel.faxBetweenBranches = faxBetweenBranches;
            this.faxBranchTableRowViewModel.CanEdit = CanEdit;
            this.faxBranchTableRowViewModel.IsSuspended = IsSuspended;
            this.faxBranchTableRowViewModel.IsManager = IsManager;
            this.faxBranchTableRowViewModel.IsRealTime = IsRealTime;
            if (IsSuspended)
            {
                this.faxBranchTableRowViewModel.CanDecide = CanDecide;
            }
            else
            {
                this.faxBranchTableRowViewModel.CanSuspend = CanSuspend;
            }
            return View(this.faxBranchTableRowViewModel);
        }
    }
}
