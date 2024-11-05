using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;

namespace FaxSystem.Models
{
    public class CreateModel : PageModel
    {
        private readonly ILogger<CreateModel> _logger;
        private readonly IToastNotification _toastNotification;
        public static int i = -1;
        public CreateModel(ILogger<CreateModel> logger, IToastNotification toastNotification)
        {
            _logger = logger;
            _toastNotification = toastNotification;
        }

        public void OnGet()
        {
            if (i == 0)
            {
                // Success Toast
                _toastNotification.AddSuccessToastMessage("Woo hoo - it works!");
            }
            else if (i == 1)
            {
                // Info Toast
                _toastNotification.AddInfoToastMessage("Here is some information.");
            }
            else
            {
                // Error Toast
                _toastNotification.AddErrorToastMessage("Woops an error occured.");

                // Warning Toast
                _toastNotification.AddWarningToastMessage("Here is a simple warning!");
            }
        }
    }
}
