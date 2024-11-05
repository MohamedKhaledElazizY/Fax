using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FaxSystem.Data;
using FaxSystem.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;

namespace FaxSystem.Controllers
{
    public class BranchesController : Controller
    {
        public string userName;
        public User user;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BranchesController> _logger;

        public BranchesController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor
            , ILogger<BranchesController> logger)
        {
            _context = context;
            userName = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            user = _context.USERS.FirstOrDefault(x => x.UserName == userName);
            _logger = logger;
        }


        // GET: Branches/Create
        public IActionResult Create()
        {
            //Check if role is valid to this user
            user = _context.USERS.FirstOrDefault(x => x.UserName == userName);
            var userRoles = _context.USER_ROLES.Where(x => x.UserId == user.ID).ToList();
            ViewData["roles"] = _context.USER_ROLES.Where(x => x.UserId == user.ID).Select(x => x.RoleId).ToList();
            var valid = userRoles.Where(x => x.RoleId == 1 || x.RoleId == 1006);
            if (!valid.Any())
            {
                return RedirectToAction("Index", "Home");
            }
            //////
            ViewData["BranchData"] = _context.BRANCHES;
            return View();
        }

        // POST: Branches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name")] Branch branch)
        {
            ViewData["BranchData"] = _context.BRANCHES;
            //ModelState["ID"].ValidationState= ModelValidationState.Valid;
            if (ModelState.IsValid)
            {
                _context.Add(branch);
                var temp = $"{user.UserName} قام باضافة الفرع {branch.Name}";
                var log = new Log()
                {
                    ActionTakerName = user.UserName,
                    ActionDescription = temp,
                    date = DateTime.Now
                };
                _context.Add(log);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Create));
            }
            _logger.LogInformation((EventId)200, "{userName} added {branchName} on {date}", user.UserName, branch.Name, DateTime.Now);
            return View(branch);
        }



        // GET: Branches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //Check if role is valid to this user
            User user = _context.USERS.FirstOrDefault(x => x.UserName == userName);
            var userRoles = _context.USER_ROLES.Where(x => x.UserId == user.ID).ToList();
            var valid = userRoles.Where(x => x.RoleId == 1 || x.RoleId == 1006);
            if (!valid.Any())
            {
                return RedirectToAction("Index", "Home");
            }
            //////
            if (id == null || _context.BRANCHES == null)
            {
                return NotFound();
            }

            var branch = await _context.BRANCHES.FindAsync(id);
            if (branch == null)
            {
                return NotFound();
            }
            return View(branch);
        }

        // POST: Branches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name")] Branch branch)
        {
            if (id != branch.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(branch);
                    var temp = $"{user.UserName} قام بتعديل الفرع {branch.Name}";
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
                    if (!BranchExists(branch.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Create));
            }
            
            _logger.LogInformation((EventId)200, "{userName} edited {branchName} on {date}", user.UserName, branch.Name, DateTime.Now);
            return View(branch);
        }


        // GET: Branches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            //Check if role is valid to this user
            User user = _context.USERS.FirstOrDefault(x => x.UserName == userName);
            var userRoles = _context.USER_ROLES.Where(x => x.UserId == user.ID).ToList();
            var valid = userRoles.Where(x => x.RoleId == 1 || x.RoleId == 1006);
            if (!valid.Any())
            {
                return RedirectToAction("Index", "Home");
            }
            //////

            if (id == null || _context.BRANCHES == null)
            {
                return NotFound();
            }

            var branch = await _context.BRANCHES
                .FirstOrDefaultAsync(m => m.ID == id);
            
            if (branch == null)
            {
                return NotFound();
            }

            return View(branch);
        }

        // POST: Branches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.BRANCHES == null)
            {
                return Problem("Entity set 'ApplicationDbContext.BRANCHES'  is null.");
            }
            var branch = await _context.BRANCHES.FindAsync(id);
            if (_context.FAXERECIVERS.FirstOrDefault(x => x.BranchID == id) != null || _context.BRANCH_FAX_RECIVER.FirstOrDefault(x => x.BranchID == id) != null
                || _context.FAXBRANCHES.FirstOrDefault(x => x.SenderBranchID == id) != null)
            {
                ModelState.AddModelError("Name", "هذا الفرع يملك العديد من المكاتبات الرجاء الرجوع للفرع اولا لحذف المكاتبات");
                return View(branch);
            }
            if (branch != null)
            {
                _context.BRANCHES.Remove(branch);
                var temp = $"{user.UserName} قام بحذف الفرع {branch.Name}";
                var log = new Log()
                {
                    ActionTakerName = user.UserName,
                    ActionDescription = temp,
                    date = DateTime.Now
                };
                _context.Add(log);
            }
            await _context.SaveChangesAsync();
            _logger.LogInformation((EventId)200, "{userName} deleted {branchName} on {date}", user.UserName, branch.Name, DateTime.Now);
            return RedirectToAction(nameof(Create));
        }

        private bool BranchExists(int id)
        {
          return _context.BRANCHES.Any(e => e.ID == id);
        }
    }
}
