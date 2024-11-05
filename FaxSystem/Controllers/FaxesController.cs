using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FaxSystem.Data;
using FaxSystem.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NToastNotify;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using System.Security.Claims;

namespace FaxSystem.Controllers
{
    [Authorize]
    public class FaxesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;
        private readonly IWebHostEnvironment webHostEnvironment;
        public string userName;
        private IEnumerable<UserRoles> userRoles;
        private User user;
        ILogger<FaxesController> _logger;

        public FaxesController(ApplicationDbContext context, IToastNotification toastNotification
            , IWebHostEnvironment hostEnvironment, IHttpContextAccessor httpContextAccessor, ILogger<FaxesController> logger)
        {
            _context = context;
            _toastNotification = toastNotification;
            webHostEnvironment = hostEnvironment;
            userName = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            user = _context.USERS.FirstOrDefault(x => x.UserName == userName);
            userRoles = _context.USER_ROLES.Where(x => x.UserId == user.ID).ToList();
            _logger= logger;
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.FAXES.Include(f => f.decision).Include(f => f.senderAgency);
            ViewData["roles"] = _context.USER_ROLES.Where(x => x.UserId == user.ID).Select(x => x.RoleId).ToList(); ;
            return View(await applicationDbContext.ToListAsync());
        }

        public IActionResult fileurl(String a)
        {
            string path = Path.Combine("\\\\192.168.1.252\\Share Folder", "Uploads");
            path = Path.Combine(path, a);
            FileStream fs = (new FileStream(path, FileMode.Open));
            var mem = new MemoryStream();
            mem.Position = 0;
            mem.SetLength(0);
            fs.CopyTo(mem);
            mem.Close();
            fs.Close();
            byte[] buffer = mem.GetBuffer();
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fs.Name).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return File(buffer, mimeType);
        }

        public IActionResult Archive(ArchPar? arc)
        {
            User? UserModel = user;user.num_read_faxes = _context.FAXES.Count(x => x.suspend == false);
            _context.SaveChanges();
            ViewData["BranchID"] = new SelectList(_context.BRANCHES, "ID", "Name");
            List<int> ids = _context.FAXERECIVERS.Where(x => x.BranchID == UserModel.BranchID)
                .ToList().Select(x => x.FaxID).ToList();

            arc.BranchFaxes = _context.FAXES
                .Where(x => ids.Contains(x.ID) && x.DecisionID != null).ToList();

            if (arc.StartDate != null)
            {
                arc.BranchFaxes = arc.BranchFaxes.Where(x => x.Date >= arc.StartDate).ToList();
            }
            if (arc.EndDate != null)
            {
                arc.BranchFaxes = arc.BranchFaxes.Where(x => x.Date <= arc.EndDate).ToList();
            }
            if (arc.EntryNumSearch != null)
            {
                arc.BranchFaxes = arc.BranchFaxes.Where(x => x.RegistrationNum.Equals(arc.EntryNumSearch)).ToList();
            }
            if (arc.SubjectSearch != null)
            {
                arc.BranchFaxes = arc.BranchFaxes.Where(x => x.Subject.Contains(arc.SubjectSearch)).ToList();
            }
            if (arc.BranchID != null)
            {
                ids = _context.FAXERECIVERS.Where(x => x.BranchID == arc.BranchID)
                .ToList().Select(x => x.FaxID).ToList();
                arc.BranchFaxes = arc.BranchFaxes
                    .Where(x => ((ids.Contains(x.ID)))).ToList();
            }

            foreach (Fax f in arc.BranchFaxes)
            {
                f.senderAgency = _context.AGENCIES.FirstOrDefault(x => x.ID == f.SenderAgencyID);
                f.FaxLinks = _context.FaxLinks.Where(x => x.FaxId == f.ID).ToList();
                if (f.DecisionID != null)
                {
                    f.decision = _context.DECISIONS.First(x => x.ID == f.DecisionID);
                }
            }
            arc.StartDate = DateTime.Now;
            arc.EndDate = DateTime.Now;
            ViewData["suspagearc1"] = true;
            ViewData["CanEditFax"] = userRoles.Where(x => x.RoleId == 1 || x.RoleId == 6).Any();
           // ViewData["CanDecideFax"] = userRoles.Where(x => x.RoleId == 1 || x.RoleId == 4 || x.RoleId == 5).Any();
            return View(arc);
        }
        //public IActionResult DecisionTaken(ArchPar? arc)
        //{
        //    User? UserModel = StaticData.User;
        //    ViewData["BranchID"] = new SelectList(_context.BRANCHES, "ID", "Name");
        //    List<int> ids = _context.FAXERECIVERS.Where(x => x.BranchID == UserModel.BranchID)
        //        .ToList().Select(x => x.FaxID).ToList();

        //    arc.BranchFaxes = _context.FAXES
        //        .Where(x => ids.Contains(x.ID) && x.DecisionID != null).ToList();
        //    arc.BranchFaxesToBranch=_context.FAXBRANCHES.Where(x=>ids.Contains(x.ID)&&x.DecisionID != null).ToList();

        //    if (arc.StartDate != null)
        //    {
        //        arc.BranchFaxes = arc.BranchFaxes.Where(x => x.Date >= arc.StartDate).ToList();
        //    }
        //    if (arc.EndDate != null)
        //    {
        //        arc.BranchFaxes = arc.BranchFaxes.Where(x => x.Date <= arc.EndDate).ToList();
        //    }
        //    if (arc.EntryNumSearch != null)
        //    {
        //        arc.BranchFaxes = arc.BranchFaxes.Where(x => x.RegistrationNum.Equals(arc.EntryNumSearch)).ToList();
        //    }
        //    if (arc.SubjectSearch != null)
        //    {
        //        arc.BranchFaxes = arc.BranchFaxes.Where(x => x.Subject.Contains(arc.SubjectSearch)).ToList();
        //    }
        //    if (arc.BranchID != null)
        //    {
        //        ids = _context.FAXERECIVERS.Where(x => x.BranchID == arc.BranchID)
        //        .ToList().Select(x => x.FaxID).ToList();
        //        arc.BranchFaxes = arc.BranchFaxes
        //            .Where(x => ((ids.Contains(x.ID)))).ToList();
        //    }

        //    foreach (Fax f in arc.BranchFaxes)
        //    {
        //        f.senderAgency = _context.AGENCIES.FirstOrDefault(x => x.ID == f.SenderAgencyID);
        //        f.FaxLinks = _context.FaxLinks.Where(x => x.FaxId == f.ID).ToList();
        //        if (f.DecisionID != null)
        //        {
        //            f.decision = _context.DECISIONS.First(x => x.ID == f.DecisionID);
        //        }
        //    }
        //    arc.StartDate = DateTime.Now;
        //    arc.EndDate = DateTime.Now;
        //    return View(arc);
        //}

        // GET: Faxes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.FAXES == null)
            {
                return NotFound();
            }

            var fax = await _context.FAXES
                .Include(f => f.decision)
                .Include(f => f.senderAgency)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (fax == null)
            {
                return NotFound();
            }
            return View(fax);
        }

        // GET: Faxes/Create
        public IActionResult Create()
        {
            User user = _context.USERS.FirstOrDefault(x => x.UserName == userName);
            var userRoles = _context.USER_ROLES.Where(x => x.UserId == user.ID).ToList();
            ViewData["roles"] = _context.USER_ROLES.Where(x => x.UserId == user.ID).Select(x => x.RoleId).ToList();
            var valid = userRoles.Where(x => x.RoleId >= 3 && x.RoleId <= 5 || x.RoleId == 1);
            if (!valid.Any())
            {
                return RedirectToAction("Index", "Home");
            }
            if (TempData["ID"] != null)
            {
                ViewData["ress"] = TempData["ID"];
            }
            else
            {
                ViewData["ress"] = -1;
            }
            ViewData["SenderBranchID"] = new SelectList(_context.AGENCIES, "ID", "Name");
            Fax f = new Fax();
            f.Date = DateTime.Now;
            return View(f);
        }

        // POST: Faxes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RegistrationNum,SenderAgencyID,ReciverBranchID,Subject,Date,FaxLink,DecisionID,Notes,files")] Fax fax, List<IFormFile> files)
        {
            if (_context.FAXES.FirstOrDefault(x => x.RegistrationNum == fax.RegistrationNum) != null)
            {
                ModelState.AddModelError("RegistrationNum", "رقم القيد مكرر");
            }

            else if (ModelState.IsValid)
            {
                long size = files.Sum(f => f.Length);
                var filePaths = new List<string>();
                string path = Path.Combine("\\\\192.168.1.252\\Share Folder", "Uploads");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        string fileName = Path.GetFileName(formFile.FileName);
                        var filePath = Path.Combine(path, fileName);
                        filePaths.Add(fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                    }
                }
                fax.Date = DateTime.Now;
                _context.Add(fax);
                var temp = $"{user.UserName} قام باضافة الجهة {fax.RegistrationNum}";
                var log = new Log()
                {
                    ActionTakerName = user.UserName,
                    ActionDescription = temp,
                    date = DateTime.Now
                };
                _context.Add(log);
                await _context.SaveChangesAsync();
                fax.ID = _context.FAXES.First(x => x.RegistrationNum == fax.RegistrationNum).ID;

                foreach (var link in filePaths)
                {
                    FaxLink flink = new FaxLink();
                    flink.link = link;
                    flink.FaxId = fax.ID;
                    _context.FaxLinks.Add(flink);
                    await _context.SaveChangesAsync();
                }
                _toastNotification.AddSuccessToastMessage("تمت اضافة المكاتبة بنجاح");
                ViewData["SenderBranchID"] = new SelectList(_context.AGENCIES, "ID", "Name");
                TempData["ID"] = fax.ID;
                //StaticData.fax = new Fax();
                //StaticData.fax = fax;
                return RedirectToAction("Create");
            }
            _toastNotification.AddErrorToastMessage("هناك خطا في المكاتبة");
            ViewData["SenderBranchID"] = new SelectList(_context.AGENCIES, "ID", "Name", fax.SenderAgencyID);
            _logger.LogInformation((EventId)200, "{userName} added {agencyName} on {date}", user.UserName, fax.RegistrationNum, DateTime.Now);
            return View(fax);
        }

        // GET: Faxes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            User user = _context.USERS.FirstOrDefault(x => x.UserName == userName);
            var userRoles = _context.USER_ROLES.Where(x => x.UserId == user.ID).ToList();
            var valid = userRoles.Where(x => x.RoleId == 1 || x.RoleId == 6);
            if (!valid.Any())
            {
                return RedirectToAction("Index", "Home");
            }

            if (id == null || _context.FAXES == null)
            {
                return NotFound();
            }

            var fax = await _context.FAXES.FindAsync(id);
            if (fax == null)
            {
                return NotFound();
            }
            ViewData["SenderBranchID"] = new SelectList(_context.BRANCHES, "ID", "Name", fax.SenderAgencyID);
            fax.senderAgency = _context.AGENCIES.First(x => x.ID == fax.SenderAgencyID);
            return View(fax);
        }

        // POST: Faxes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("ID,RegistrationNum,Subject,Date,FaxLink,Notes,senderAgency,files")] Fax fax, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Fax f = _context.FAXES.First(x => x.ID == fax.ID);
                    f.Subject = fax.Subject;
                    f.Notes = fax.Notes;
                    var temp = $"{user.UserName} قام تعديل الجهة {fax.RegistrationNum}";
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
                    if (!FaxExists(fax.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                var filePaths = new List<string>();

                string path = Path.Combine("\\\\192.168.1.252\\Share Folder","Uploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        string fileName = Path.GetFileName(formFile.FileName);
                        var filePath = Path.Combine(path, fileName);
                        filePaths.Add(fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                    }
                }
                foreach (var link in filePaths)
                {
                    FaxLink flink = new FaxLink();
                    flink.link = link;
                    flink.FaxId = fax.ID;
                    _context.FaxLinks.Add(flink);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Archive));
            }
            ViewData["SenderBranchID"] = new SelectList(_context.BRANCHES, "ID", "Name", fax.SenderAgencyID);
            _logger.LogInformation((EventId)200, "{userName} edited {agencyName} on {date}", user.UserName, fax.RegistrationNum, DateTime.Now);
            return View(fax);
        }

        // GET: Faxes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.FAXES == null)
            {
                return NotFound();
            }

            var fax = await _context.FAXES
                .Include(f => f.decision)
                .Include(f => f.senderAgency)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (fax == null)
            {
                return NotFound();
            }

            return View(fax);
        }

        // POST: Faxes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.FAXES == null)
            {
                return Problem("Entity set 'ApplicationDbContext.FAXES'  is null.");
            }
            var fax = await _context.FAXES.FindAsync(id);
            if (fax != null)
            {
                _context.FAXES.Remove(fax);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FaxExists(int id)
        {
            return _context.FAXES.Any(e => e.ID == id);
        }
    }
}
