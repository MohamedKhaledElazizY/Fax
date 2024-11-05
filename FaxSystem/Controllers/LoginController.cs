using FaxSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;
using FaxSystem.Data;
using System.Linq;

namespace FaxSystem.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _db;

        public LoginController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(User modeluser)
        {
            if (modeluser.UserName == null || modeluser.UserName.Length < 3 || modeluser.Password == null || modeluser.Password.Length < 4)
            {
                ViewData["ValidationMessage"] = "برجاء إدخال البيانات المطلوبة بشكل صحيح";
                return View();
            }
            var loginFromDb = _db.USERS.Any(u => u.UserName == modeluser.UserName && u.Password == modeluser.Password);
            if (loginFromDb)
            {
                List<Claim> claims = new List<Claim>() {
                new Claim(ClaimTypes.NameIdentifier, modeluser.UserName)
                };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                AuthenticationProperties properties = new AuthenticationProperties()
                {
                    AllowRefresh = true,
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(20)

                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), properties);
                //    TempData["UserModel"] = modeluser;
                var User = _db.USERS.FirstOrDefault(x=>x.UserName== modeluser.UserName);
                ViewData["id"] = User.ID;
                return RedirectToAction("Index", "Home");
            }
            ViewData["ValidationMessage"] = "المستخدم غير موجود";
            return View();
        }
    }
}