using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FaxSystem.Data;
using FaxSystem.Models;
using System.Security.Claims;

namespace FaxSystem.Controllers
{
    public class AgenciesController : Controller
    {
        public string userName;
        public User user;
        private IEnumerable<UserRoles> userRoles;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AgenciesController> _logger;

        public AgenciesController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor
            , ILogger<AgenciesController> logger            )
        {
            _context = context;
            userName = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            user = _context.USERS.FirstOrDefault(x => x.UserName == userName);
            userRoles = _context.USER_ROLES.Where(x => x.UserId == user.ID).ToList();
            _logger = logger;
        }

        // GET: Agencies
        public async Task<IActionResult> Index()
        {
            //Check if role is valid to this user
            user = _context.USERS.FirstOrDefault(x => x.UserName == userName);
            var valid = userRoles.Where(x => x.RoleId == 1 || x.RoleId == 1006);
            if (!valid.Any())
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["roles"] = _context.USER_ROLES.Where(x => x.UserId == user.ID).Select(x => x.RoleId).ToList(); ;
            //////
            return View(await _context.AGENCIES.ToListAsync());
        }

        // GET: Agencies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //Check if role is valid to this user
            var valid = userRoles.Where(x => x.RoleId == 1 || x.RoleId == 1006);
            if (!valid.Any())
            {
                return RedirectToAction("Index", "Home");
            }

            User user = _context.USERS.FirstOrDefault(x => x.UserName == userName);
            ViewData["roles"] = _context.USER_ROLES.Where(x => x.UserId == user.ID).Select(x => x.RoleId).ToList();
            //////
            if (id == null || _context.AGENCIES == null)
            {
                return NotFound();
            }

            var agency = await _context.AGENCIES
                .FirstOrDefaultAsync(m => m.ID == id);
            if (agency == null)
            {
                return NotFound();
            }

            return View(agency);
        }

        // GET: Agencies/Create
        public IActionResult Create()
        {
            //Check if role is valid to this user
            var valid = userRoles.Where(x => x.RoleId == 1 || x.RoleId == 1006);
            if (!valid.Any())
            {
                return RedirectToAction("Index", "Home");
            }
            User user = _context.USERS.FirstOrDefault(x => x.UserName == userName);
            ViewData["roles"] = _context.USER_ROLES.Where(x => x.UserId == user.ID).Select(x => x.RoleId).ToList();
            //////
            ViewData["AgenciesData"] = _context.AGENCIES;
            return View();
        }

        // POST: Agencies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name")] Agency agency)
        {
            //Check if role is valid to this user
            var valid = userRoles.Where(x => x.RoleId == 1 || x.RoleId == 1006);
            if (!valid.Any())
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["AgenciesData"] = _context.AGENCIES;
            
            if (ModelState.IsValid)
            {
                _context.Add(agency);
                var temp = $"{user.UserName} قام باضافة الجهة {agency.Name}";
                var log = new Log()
                {
                    ActionTakerName = user.UserName,
                    ActionDescription = temp,
                    date = DateTime.Now
                };
                _context.Add(log);
                await _context.SaveChangesAsync();
                _logger.LogInformation((EventId)200, "{userName} added {agencyName} on {date}", user.UserName, agency.Name, DateTime.Now);
                return RedirectToAction(nameof(Create));
            }
            return View(agency);
        }

        // GET: Agencies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            
            User user = _context.USERS.FirstOrDefault(x => x.UserName == userName);
            ViewData["roles"] = _context.USER_ROLES.Where(x => x.UserId == user.ID).Select(x => x.RoleId).ToList();
            //Check if role is valid to this user
            var valid = userRoles.Where(x => x.RoleId == 1 || x.RoleId == 1006);
            if (!valid.Any())
            {
                return RedirectToAction("Index", "Home");
            }
            //////
            if (id == null || _context.AGENCIES == null)
            {
                return NotFound();
            }

            var agency = await _context.AGENCIES.FindAsync(id);
            if (agency == null)
            {
                return NotFound();
            }
            return View(agency);
        }

        // POST: Agencies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name")] Agency agency)
        {
            if (id != agency.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(agency);
                    var temp = $"{user.UserName} قام بتعديل الجهة {agency.Name}";
                    var log = new Log()
                    {
                        ActionTakerName = user.UserName,
                        ActionDescription = temp,
                        date = DateTime.Now
                    };
                    _context.Add(log);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AgencyExists(agency.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                
                _logger.LogInformation((EventId)200, "{userName} edited {agencyName} on {date}", user.UserName, agency.Name, DateTime.Now);
                return RedirectToAction(nameof(Create));
            }
            return View(agency);
        }

        // GET: Agencies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            //Check if role is valid to this user
            User user = _context.USERS.FirstOrDefault(x => x.UserName == userName);
            ViewData["roles"] = _context.USER_ROLES.Where(x => x.UserId == user.ID).Select(x => x.RoleId).ToList();
            var valid = userRoles.Where(x => x.RoleId == 1);
            if (!valid.Any())
            {
                return RedirectToAction("Index", "Home");
            }
            //////
            if (id == null || _context.AGENCIES == null)
            {
                return NotFound();
            }

            var agency = await _context.AGENCIES
                .FirstOrDefaultAsync(m => m.ID == id);
            if (agency == null)
            {
                return NotFound();
            }

            return View(agency);
        }

        // POST: Agencies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.AGENCIES == null)
            {
                return Problem("Entity set 'ApplicationDbContext.AGENCIES'  is null.");
            }

            var agency = await _context.AGENCIES.FindAsync(id);
            if ( _context.FAXES.FirstOrDefault(x => x.SenderAgencyID == id) != null)
            {
                ModelState.AddModelError("Name", "هذه الجهة تملك العديد من المكاتبات الرجاء الرجوع للمتابعة اولا لحذف المكاتبات");
                return View(agency);
            }
            if (agency != null)
            {
                _context.AGENCIES.Remove(agency);
                var temp = $"{user.UserName} قام بحذف الجهة {agency.Name}";
                var log = new Log()
                {
                    ActionTakerName = user.UserName,
                    ActionDescription = temp,
                    date = DateTime.Now
                };
                _context.Add(log);
            }
            await _context.SaveChangesAsync();
            _logger.LogInformation((EventId)200, "{userName} deleted {agencyName} on {date}", user.UserName, agency.Name, DateTime.Now);
            return RedirectToAction(nameof(Create));
        }

        private bool AgencyExists(int id)
        {
          return _context.AGENCIES.Any(e => e.ID == id);
        }
    }
}
