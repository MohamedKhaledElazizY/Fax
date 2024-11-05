using FaxSystem.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using FaxSystem.Data;
using NToastNotify;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace FaxSystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;
        public string userName;
        private User user;
        private IEnumerable<UserRoles> userRoles;
        private readonly IWebHostEnvironment webHostEnvironment;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IToastNotification toastNotification,
            IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _toastNotification = toastNotification;
            userName = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            user = _context.USERS.FirstOrDefault(x => x.UserName == userName);
            userRoles = _context.USER_ROLES.Where(x => x.UserId == user.ID).ToList();
            this.webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]

        public IActionResult Index()
        {
            User user = _context.USERS.FirstOrDefault(x => x.UserName == userName);
            var userRoles = _context.USER_ROLES.Where(x => x.UserId == user.ID).ToList();
            var NumberOfSuspendedFaxes = numofsuarc();// _context.FAXES.Where(x => x.DecisionID == null ).ToList().Count() +
          // _context.FAXBRANCHES.Where(x => x.suspend == true).ToList().Count();

           
            TempData["NumberOfSuspendedFaxes"] = NumberOfSuspendedFaxes;
            ViewData["name"] = userName;
            ViewData["roles"] = _context.USER_ROLES.Where(x => x.UserId == user.ID).Select(x => x.RoleId).ToList();
            if(_context.USER_ROLES.Where(x => x.UserId == user.ID).Select(x => x.RoleId).Any())
            {
                TempData["roles"] = _context.USER_ROLES.Where(x => x.UserId == user.ID).Select(x => x.RoleId).ToList();
            }else
            {
                TempData["roles"] = new List<int> { 0};
            }
            ViewData["id"] = user.ID;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Login");
        }
        [HttpPost]
        public int numofsus()
        {
            _toastNotification.AddSuccessToastMessage("هناك مكاتبة معلقة جديدة");
            var NumberOfSuspendedFaxes=0;
            string path = Path.Combine(webHostEnvironment.WebRootPath, "suspend");
            
            if (userRoles.Where(x => x.RoleId == 4).Any())
            {
                path = Path.Combine(path, "boss");
                FileStream fs = (new FileStream(path, FileMode.OpenOrCreate));
                StreamReader sr=new StreamReader(fs);
                string r=sr.ReadToEnd();
                int i;
                if (r == "")
                {
                    i = 0;
                }
                else
                {
                    i=int.Parse(r);
                }
                NumberOfSuspendedFaxes = _context.FAXES.Where(x => x.suspend).ToList().Count() + _context.FAXBRANCHES.Where(x => (x.suspend == 2 || x.suspend == 3)).ToList().Count()-i;
                sr.Close();
                fs.Close();
            }
            else if (userRoles.Where(x => x.RoleId == 5).Any())
            {
                path = Path.Combine(path, "vice");
                FileStream fs = (new FileStream(path, FileMode.OpenOrCreate));
                StreamReader sr = new StreamReader(fs);
                string r = sr.ReadToEnd();
                int i;
                if (r == "")
                {
                    i = 0;
                }
                else
                {
                    i = int.Parse(r);
                }
                NumberOfSuspendedFaxes = _context.FAXES.Where(x => x.DecisionID == null).ToList().Count() +
                _context.FAXBRANCHES.Where(x => x.DecisionID==null &&(x.suspend==1||x.suspend==3)).ToList().Count()-i;
                sr.Close();
                fs.Close();
            }
            TempData["suspendedcount"] = NumberOfSuspendedFaxes;
            return NumberOfSuspendedFaxes;
        }
        [HttpPost]
        public bool checkbranch(int id,int type)
        {
            bool check = false;
            int branchid = _context.USERS.First(x => x.ID == user.ID).BranchID;
            if (type == 1)
            {
               check= _context.FAXERECIVERS.Where(x=>x.BranchID==branchid&&x.FaxID==id).Any();
            }
            if (type == 2)
            {
                check = _context.BRANCH_FAX_RECIVER.Where(x => x.BranchID == branchid && x.FaxID == id).Any()||
                    _context.FAXBRANCHES.Where(x => x.ID == id && x.SenderBranchID==branchid && x.DecisionID != null).Any();
            }

            return check;
        }
        //[HttpPost]
        //public bool checkbranch2(int id,int type)
        //{
        //    bool check = false;
        //    int branchid = _context.USERS.First(x => x.ID == user.ID).BranchID;
        //    if (type == 1)
        //    {
        //       check= _context.FAXERECIVERS.Where(x=>x.BranchID==branchid&&x.FaxID==id).Any();
        //    }
        //    if (type == 2)
        //    {
        //        check = _context.BRANCH_FAX_RECIVER.Where(x => x.BranchID == branchid && x.FaxID == id).Any()||
        //           _context.FAXBRANCHES.Where(x => x.ID == id && x.SenderBranchID==branchid && x.DecisionID != null).Any() ;
        //    }

        //    return check;
        //}
        public int numofsuarc()
        {
            var NumberOfSuspendedFaxes = 0;
            string path = Path.Combine(webHostEnvironment.WebRootPath, "suspend");

            if (userRoles.Where(x => x.RoleId == 4).Any())
            {
                path = Path.Combine(path, "boss");
                FileStream fs = (new FileStream(path, FileMode.OpenOrCreate));
                StreamReader sr = new StreamReader(fs);
                string r = sr.ReadToEnd();
                int i;
                if (r == "")
                {
                    i = 0;
                }
                else
                {
                    i = int.Parse(r);
                }
                NumberOfSuspendedFaxes = _context.FAXES.Where(x => x.suspend).ToList().Count() + _context.FAXBRANCHES.Where(x => (x.suspend == 2 || x.suspend == 3)).ToList().Count() - i;
                sr.Close();
                fs.Close();
            }
            else if (userRoles.Where(x => x.RoleId == 5).Any())
            {
                path = Path.Combine(path, "vice");
                FileStream fs = (new FileStream(path, FileMode.OpenOrCreate));
                StreamReader sr = new StreamReader(fs);
                string r = sr.ReadToEnd();
                int i;
                if (r == "")
                {
                    i = 0;
                }
                else
                {
                    i = int.Parse(r);
                }
                NumberOfSuspendedFaxes = _context.FAXES.Where(x => x.DecisionID == null).ToList().Count() +
                _context.FAXBRANCHES.Where(x => x.DecisionID == null && (x.suspend==1||x.suspend==3)).ToList().Count() - i;
                sr.Close();
                fs.Close();
            }
            TempData["suspendedcount"] = NumberOfSuspendedFaxes;
            return NumberOfSuspendedFaxes;

        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        public int numoffaxes(bool showNotification = true)
        {
            if (showNotification)
            {
                _toastNotification.AddSuccessToastMessage("هناك مكاتبة جديدة");
            }
            int i = user.num_read_faxes;
            int NumberOfSuspendedFaxes = _context.FAXES.Where(x => x.suspend == false).ToList().Count() - i;
            TempData["suspendedcount"] = NumberOfSuspendedFaxes;
            return NumberOfSuspendedFaxes;
        }
        public int numoffaxesbranches(bool showNotification = true)
        {
            if(showNotification)
            {
                _toastNotification.AddSuccessToastMessage("هناك مكاتبة جديدة");
            }
            int ii = user.num_read_faxes_branches;
            int l = _context.FAXBRANCHES.Where(x => x.suspend == 0).ToList().Count() - ii;
            TempData["suspendedcount"] = l;
            return l;
        }
    }

}