using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FaxSystem.Data;
using FaxSystem.ViewModels;
using FaxSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace FaxSystem.Controllers
{
    [Authorize]
    public class UserRolesController : Controller
    {
        public string userName;
        private IEnumerable<UserRoles> userRoles;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserRolesController> _logger;

        public UserRolesController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, ILogger<UserRolesController> logger)
        {
            _context = context;
            userName = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            User user = _context.USERS.FirstOrDefault(x => x.UserName == userName);
            userRoles = _context.USER_ROLES.Where(x => x.UserId == user.ID).ToList();
            _logger = logger;
        }

        // GET: UserRoles
        public async Task<IActionResult> Index()
        {
            
            var valid = userRoles.Where(x => x.RoleId == 1);
            if (!valid.Any())
            {
                return RedirectToAction("Index", "Home");
            }
            //////
            var applicationDbContext =  _context.USER_ROLES.Include(u => u.role).Include(u => u.user);
            var appUsers = _context.USERS.ToList();


            List<string> usersNames = appUsers.Select(x => x.UserName).ToList();
            ViewData["users"] = usersNames;
            ViewData["count"] = usersNames.Count;
            List<int> usersIDs = appUsers.Select(x => x.ID).ToList();
            ViewData["Ids"] = usersIDs;
            List<string> rolesNames = new List<string>();
             
            foreach (var i in usersIDs)
            {
                var temp = applicationDbContext.Where(x => x.UserId == i).ToList();
                List<string> temp2 = (List<string>)temp.Select(x => x.role.RoleName).ToList();
                var allUserRoles = string.Join("-", temp2);
                rolesNames.Add(allUserRoles);
            }

            ViewData["rolesNames"] = rolesNames;
            User user = _context.USERS.FirstOrDefault(x => x.UserName == userName);
            ViewData["roles"] = _context.USER_ROLES.Where(x => x.UserId == user.ID).Select(x => x.RoleId).ToList();

            return View();
            
        }

        
        public async Task<IActionResult> Edit(int id)
        {
            var applicationDbContext =  _context.USER_ROLES.Include(u => u.role).Include(u => u.user);
            var temp = applicationDbContext.Where(x => x.UserId == id).ToList();
            string tempName = _context.USERS.First(x => x.ID == id).UserName;
            User user = _context.USERS.FirstOrDefault(x => x.UserName == userName);
            ViewData["roles"] = _context.USER_ROLES.Where(x => x.UserId == user.ID).Select(x => x.RoleId).ToList();

            var roles = _context.ROLES.ToList();
            
            var viewmodel = new UserRolesViewModel
            {
                userId = id,
                userName = tempName,
                Roles = roles.Select(role => new RoleViewModel 
                {
                    RoleID = role.RoleID,
                    RoleName = role.RoleName,
                    IsSelected = temp.Where(x => x.RoleId == role.RoleID).Any()

                }).ToList()


            };
            return View(viewmodel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserRolesViewModel model) 
        {
            var user = _context.USERS.First(x => x.ID == model.userId);
            var userRoles = _context.USER_ROLES.Where(u => u.UserId == model.userId).Select(x => x.role.RoleName).ToList();
            

            foreach(var role in model.Roles)
            {
                if(!userRoles.Any(x => x == role.RoleName) && role.IsSelected)
                {
                    UserRoles newUserRoles = new UserRoles();
                    newUserRoles.UserId = model.userId;
                    newUserRoles.RoleId = role.RoleID;
                    _context.USER_ROLES.Add(newUserRoles);
                    var temp = $"{user.UserName} قام بتعديل الجهة {model.userName}";
                    var log = new Log()
                    {
                        ActionTakerName = user.UserName,
                        ActionDescription = temp,
                        date = DateTime.Now
                    };
                    _context.Add(log);
                    _context.SaveChanges();
                }

                if (userRoles.Any(x => x == role.RoleName) && !role.IsSelected)
                {
                    UserRoles temp = _context.USER_ROLES.First(x => x.UserId == model.userId && x.RoleId == role.RoleID);
                    _context.Remove(temp);
                    _context.SaveChanges();
                }
                _logger.LogInformation((EventId)200, "{userName} edited {Name} on {date}", user.UserName, model.userName, DateTime.Now);
            }
                return RedirectToAction("Index");
        }


        
    }
}
