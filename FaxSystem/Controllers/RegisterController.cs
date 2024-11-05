using FaxSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;
using FaxSystem.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace FaxSystem.Controllers
{
    [Authorize]
    public class RegisterController : Controller
    {
        public string userName;
        private readonly ApplicationDbContext _db;
        public User currentUser = new User();
        private readonly ILogger<RegisterController> _logger;
        public RegisterController(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor, ILogger<RegisterController> logger)
        {
            _db = db;
            userName = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            _logger = logger;
        }
        //GET: Register

        public IActionResult Register()
        {
            //Check if role is valid to this user
            currentUser = _db.USERS.FirstOrDefault(x => x.UserName == userName);
            var userRoles = _db.USER_ROLES.Where(x => x.UserId == currentUser.ID).ToList();
            var valid = userRoles.Where(x => x.RoleId == 1);
            if (!valid.Any()) 
            {
                return RedirectToAction("Index", "Home");
            }
            //////
            ViewBag.ListOfBranches = new SelectList(_db.BRANCHES, "ID", "Name");
            ViewData["roles"] = _db.USER_ROLES.Where(x => x.UserId == currentUser.ID).Select(x => x.RoleId).ToList();
            return View();
        }
        //POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> Register(User _user)
        {
            if (_user.BranchID == -1)
            {
                ModelState.AddModelError("BranchID", "برجاء اختيار فرع المستخدم");
            }
            ViewBag.ListOfBranches = new SelectList(_db.BRANCHES, "ID", "Name", _user.BranchID);
            _user.branch = _db.BRANCHES.FirstOrDefault(x => x.ID == _user.BranchID);
            if (ModelState.IsValid)
            {
                var check = _db.USERS.Any(s => s.UserName == _user.UserName);
                var tempName = _user.UserName;
                var temp = $"{userName} قام باضافة المستخدم {tempName}";
                if (!check)
                {
                    _db.USERS.Add(_user);
                    var log = new Log()
                    {
                        ActionTakerName = userName,
                        ActionDescription = temp,
                        date = DateTime.Now
                    };
                    _db.Add(log);
                    _db.SaveChanges();
                    var newUser = _db.USERS.Where(x => x.UserName == _user.UserName && x.Password == _user.Password).First();
                    int newUserId = newUser.ID;
                    return RedirectToAction("Edit", "UserRoles",new {id = newUserId } );
                }
                else
                {
                    ViewData["ValidationMessage"] = "اسم المستخدم موجود بالفعل";
                    return View();
                }
            }
          //  _logger.LogInformation((EventId)200, "{userName} added {Name} on {date}", user.UserName, _user.UserName, DateTime.Now);
            return View();
        }
    }
}
