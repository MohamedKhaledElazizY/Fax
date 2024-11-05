using FaxSystem.ViewModels;
using FaxSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FaxSystem.ViewComponents
{
    public class FaxTableRowViewComponent : ViewComponent
    {
        private FaxTableRowViewModel? faxTableRowViewModel;

        public async Task<IViewComponentResult> InvokeAsync(Fax fax, bool IsSuspended, bool CanEdit, bool CanDecide, bool IsManager = false, bool IsRealTime = false)
        {
            this.faxTableRowViewModel = new FaxTableRowViewModel();
            this.faxTableRowViewModel.fax = fax;
            this.faxTableRowViewModel.IsSuspended = IsSuspended;
            this.faxTableRowViewModel.IsManager = IsManager;
            this.faxTableRowViewModel.IsRealTime = IsRealTime;
            if (IsSuspended)
            {
                this.faxTableRowViewModel.CanEdit = CanEdit;
                this.faxTableRowViewModel.CanDecide = CanDecide;
            }
            else
            {
                this.faxTableRowViewModel.CanEdit = CanEdit;
            }
            return View(this.faxTableRowViewModel);
        }
    }
}
