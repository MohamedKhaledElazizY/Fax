using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FaxSystem.Data;
using FaxSystem.Models;
using NToastNotify;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace FaxSystem.Controllers
{
    [Authorize]
    public class FaxBetweenBranchesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;
        private readonly IWebHostEnvironment webHostEnvironment;
        public string userName;
        private User user;
        private IEnumerable<UserRoles> userRoles;
        private readonly ILogger<FaxBetweenBranchesController> _logger;

        public FaxBetweenBranchesController(ApplicationDbContext context, IToastNotification toastNotification
            , IWebHostEnvironment hostEnvironment, IHttpContextAccessor httpContextAccessor, ILogger<FaxBetweenBranchesController> logger)
        {
            _context = context;
            _toastNotification = toastNotification;
            webHostEnvironment = hostEnvironment;
            userName = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            user = _context.USERS.FirstOrDefault(x => x.UserName == userName);
            userRoles = _context.USER_ROLES.Where(x => x.UserId == user.ID).ToList();
            _logger = logger;
        }

        // GET: FaxBetweenBranches
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.FAXBRANCHES.Include(f => f.senderBranch);
            var userRoles = _context.USER_ROLES.Where(x => x.UserId == user.ID).ToList();
            ViewData["roles"] = _context.USER_ROLES.Where(x => x.UserId == user.ID).Select(x => x.RoleId).ToList();
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: FaxBetweenBranches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.FAXBRANCHES == null)
            {
                return NotFound();
            }

            var faxBetweenBranches = await _context.FAXBRANCHES
                .Include(f => f.senderBranch)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (faxBetweenBranches == null)
            {
                return NotFound();
            }

            return View(faxBetweenBranches);
        }

        // GET: FaxBetweenBranches/Create
        public IActionResult Create()
        {
            ViewData["SenderBranchName"] = _context.BRANCHES.First(x => x.ID == user.BranchID).Name;
            List<Branch> b = _context.BRANCHES.ToList();
            b.RemoveAll(x => x.Name == "السيد المدير" || x.Name == "السيد نائب المدير" || x.ID == user.BranchID);
            ViewData["ReciverBranchID"] = new SelectList(b, "ID", "Name");
            CreateFaxbetweenbranches f = new CreateFaxbetweenbranches();
            f.faxBetweenBranches = new FaxBetweenBranches();
            f.faxBetweenBranches.SenderBranchID = user.BranchID;
            f.faxBetweenBranches.senderBranch = new Branch();
            f.faxBetweenBranches.Date = DateTime.Now;
            f.faxBetweenBranches.senderBranch.Name = _context.BRANCHES.First(x => x.ID == user.BranchID).Name;
            return View(f);
        }

        // POST: FaxBetweenBranches/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("faxBetweenBranches,Branches,files")] CreateFaxbetweenbranches CreateFaxbetweenbranches, List<IFormFile> files)
        {
            if (_context.FAXBRANCHES.FirstOrDefault(x => x.RegistrationNum == CreateFaxbetweenbranches.faxBetweenBranches.RegistrationNum) != null)
            {
                ModelState.AddModelError("faxBetweenBranches.RegistrationNum", "رقم القيد مكرر");
            }
            else if (ModelState.IsValid)
            {

                _context.Add(CreateFaxbetweenbranches.faxBetweenBranches);
                var temp = $"{user.UserName} قام باضافة الجهة {CreateFaxbetweenbranches.faxBetweenBranches.RegistrationNum}";
                var log = new Log()
                {
                    ActionTakerName = user.UserName,
                    ActionDescription = temp,
                    date = DateTime.Now
                }; 
                _context.Add(log);
                await _context.SaveChangesAsync();

                int id = _context.FAXBRANCHES.
                    First(x => x.RegistrationNum == CreateFaxbetweenbranches
                    .faxBetweenBranches.RegistrationNum).ID;
                foreach (var br in CreateFaxbetweenbranches.Branches)
                {
                    BranchFaxRecivers b = new BranchFaxRecivers();
                    b.BranchID = br;//int.Parse(((SelectListItem)br).Value);
                    b.FaxID = id;
                    //    b.branch.ID = br;
                    //   b.branch.Name=_context.BRANCHES.FirstOrDefault(x=)
                    _context.BRANCH_FAX_RECIVER.Add(b);
                    await _context.SaveChangesAsync();
                }

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
                        // full path to file in temp location
                        string fileName = Path.GetFileName(formFile.FileName);
                        var filePath = Path.Combine(path, fileName);
                        //we are using Temp file name just for the example. Add your own file path.
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
                    flink.FaxBetweenBranchesID = id;
                    _context.FaxLinks.Add(flink);
                    await _context.SaveChangesAsync();
                }
                _toastNotification.AddSuccessToastMessage("تمت اضافة المكاتبة بنجاح");
                TempData["faxid"] = id;
                TempData["faxtype"] = 2;
                return RedirectToAction();
            }
            _toastNotification.AddErrorToastMessage("هناك خطا في المكاتبة");
            ViewData["SenderBranchName"] = _context.BRANCHES.First(x => x.ID == user.BranchID).Name;
            List<Branch> BRANCHES = _context.BRANCHES.ToList();
            BRANCHES.RemoveAll(x => x.Name == "السيد المدير" || x.Name == "السيد نائب المدير" || x.ID == user.BranchID);
            ViewData["ReciverBranchID"] = new SelectList(BRANCHES, "ID", "Name");
            ViewData["ReciverBranchID"] = new SelectList(BRANCHES, "ID", "Name", CreateFaxbetweenbranches.Branches);
            _logger.LogInformation((EventId)200, "{userName} added {agencyName} on {date}", user.UserName, CreateFaxbetweenbranches.faxBetweenBranches.RegistrationNum, DateTime.Now);
            return View(CreateFaxbetweenbranches);
        }

        public IActionResult Archive(ArchPar? arc)
        {
            if (TempData["ID"] != null)
            {
                ViewData["ress"] = TempData["ID"];
            }
            else
            {
                ViewData["ress"] = -1;
            }
            User? UserModel = user;
            user.num_read_faxes_branches = _context.FAXBRANCHES.Count(x=>x.suspend==0);_context.SaveChanges();
            ViewData["BranchID"] = new SelectList(_context.BRANCHES, "ID", "Name");
            List<int> ids = _context.BRANCH_FAX_RECIVER.Where(x => x.BranchID == UserModel.BranchID)
                .ToList().Select(x => x.FaxID).ToList();

            arc.BranchFaxesToBranch = _context.FAXBRANCHES
                .Where(x => ((ids.Contains(x.ID) || x.SenderBranchID == UserModel.BranchID) && x.suspend==0)).ToList();

            if (arc.StartDate != null)
            {
                arc.BranchFaxesToBranch = arc.BranchFaxesToBranch.Where(x => x.Date >= arc.StartDate).ToList();
            }
            if (arc.EndDate != null)
            {
                arc.BranchFaxesToBranch = arc.BranchFaxesToBranch.Where(x => x.Date <= arc.EndDate).ToList();
            }
            if (arc.EntryNumSearch != null)
            {
                arc.BranchFaxesToBranch = arc.BranchFaxesToBranch.Where(x => x.RegistrationNum.Equals(arc.EntryNumSearch)).ToList();
            }
            if (arc.SubjectSearch != null)
            {
                arc.BranchFaxesToBranch = arc.BranchFaxesToBranch.Where(x => x.Subject.Contains(arc.SubjectSearch)).ToList();
            }
            if (arc.BranchID != null)
            {
                ids = _context.BRANCH_FAX_RECIVER.Where(x => x.BranchID == arc.BranchID)
                .ToList().Select(x => x.FaxID).ToList();
                arc.BranchFaxesToBranch = arc.BranchFaxesToBranch
                    .Where(x => ((ids.Contains(x.ID) || x.SenderBranchID == arc.BranchID))).ToList();
            }

            foreach (FaxBetweenBranches f in arc.BranchFaxesToBranch)
            {
                f.senderBranch = _context.BRANCHES.FirstOrDefault(x => x.ID == f.SenderBranchID);
                f.FaxLinks = _context.FaxLinks.Where(x => x.FaxBetweenBranchesID == f.ID).ToList();
                if (f.DecisionID != null)
                {
                    f.decision = _context.DECISIONS.First(x => x.ID == f.DecisionID);
                }
                f.BranchFaxRecivers = _context.BRANCH_FAX_RECIVER.Where(x => x.FaxID == f.ID).ToList();

            }

            arc.StartDate = DateTime.Now;
            arc.EndDate = DateTime.Now;
            ViewData["suspagearc2"] = true;
            ViewData["CanEditFaxBranch"] = userRoles.Where(x => x.RoleId <= 2).Any();
            ViewData["CanEditFaxBranchUserBranchID"] = user.BranchID;
            ViewData["CanSuspendFaxBranch"] = userRoles.Where(x => x.RoleId <= 2).Any();
            return View(arc);
        }

        // GET: FaxBetweenBranches/Edit/5
        public async Task<IActionResult> Suspend(int id,int SuspendStatus)
        {
            User user = _context.USERS.FirstOrDefault(x => x.UserName == userName);
            var userRoles = _context.USER_ROLES.Where(x => x.UserId == user.ID).ToList();
            var valid = userRoles.Where(x => x.RoleId <= 2);
            if (!valid.Any())
            {
                return RedirectToAction("Index", "Home");
            }

            if (id == null || _context.FAXBRANCHES == null)
            {
                return NotFound();
            }

            var faxBetweenBranches = await _context.FAXBRANCHES.FindAsync(id);
            if (faxBetweenBranches == null)
            {
                return NotFound();
            }
            if (faxBetweenBranches.DecisionID == null)
            {
                faxBetweenBranches.suspend = SuspendStatus;
                _context.SaveChanges();
                TempData["ID"] = faxBetweenBranches.ID;
                //StaticData.faxbet = new FaxBetweenBranches();
                //StaticData.faxbet.ID = id;
            }
            else
            {
                return RedirectToAction("Details", id);
            }
            return RedirectToAction("Archive");
        }
        //public async Task<IActionResult> DecisionTaken(ArchPar? arc)
        //{
        //    if (StaticData.id != null)
        //    {
        //        ViewData["ress"] = StaticData.id;
        //        StaticData.id = null;
        //    }
        //    else
        //    {
        //        ViewData["ress"] = -1;
        //    }
        //    User? UserModel = StaticData.User;
        //    ViewData["BranchID"] = new SelectList(_context.BRANCHES, "ID", "Name");
        //    List<int> ids = _context.BRANCH_FAX_RECIVER.Where(x => x.BranchID == UserModel.BranchID)
        //        .ToList().Select(x => x.FaxID).ToList();

        //    arc.BranchFaxesToBranch = _context.FAXBRANCHES
        //        .Where(x => ((ids.Contains(x.ID) || x.SenderBranchID == UserModel.BranchID) && !x.suspend)).ToList();

        //    if (arc.StartDate != null)
        //    {
        //        arc.BranchFaxesToBranch = arc.BranchFaxesToBranch.Where(x => x.Date >= arc.StartDate).ToList();
        //    }
        //    if (arc.EndDate != null)
        //    {
        //        arc.BranchFaxesToBranch = arc.BranchFaxesToBranch.Where(x => x.Date <= arc.EndDate).ToList();
        //    }
        //    if (arc.EntryNumSearch != null)
        //    {
        //        arc.BranchFaxesToBranch = arc.BranchFaxesToBranch.Where(x => x.RegistrationNum.Equals(arc.EntryNumSearch)).ToList();
        //    }
        //    if (arc.SubjectSearch != null)
        //    {
        //        arc.BranchFaxesToBranch = arc.BranchFaxesToBranch.Where(x => x.Subject.Contains(arc.SubjectSearch)).ToList();
        //    }
        //    if (arc.BranchID != null)
        //    {
        //        ids = _context.BRANCH_FAX_RECIVER.Where(x => x.BranchID == arc.BranchID)
        //        .ToList().Select(x => x.FaxID).ToList();
        //        arc.BranchFaxesToBranch = arc.BranchFaxesToBranch
        //            .Where(x => ((ids.Contains(x.ID) || x.SenderBranchID == arc.BranchID))).ToList();
        //    }

        //    foreach (FaxBetweenBranches f in arc.BranchFaxesToBranch)
        //    {
        //        f.senderBranch = _context.BRANCHES.FirstOrDefault(x => x.ID == f.SenderBranchID);
        //        f.FaxLinks = _context.FaxLinks.Where(x => x.FaxBetweenBranchesID == f.ID).ToList();
        //        if (f.DecisionID != null)
        //        {
        //            f.decision = _context.DECISIONS.First(x => x.ID == f.DecisionID);
        //        }
        //        f.BranchFaxRecivers = _context.BRANCH_FAX_RECIVER.Where(x => x.FaxID == f.ID).ToList();

        //    }

        //    arc.StartDate = DateTime.Now;
        //    arc.EndDate = DateTime.Now;
        //    return View(arc);
        //}

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.FAXBRANCHES == null)
            {
                return NotFound();
            }
            CreateFaxbetweenbranches createFaxbetweenbranches = new CreateFaxbetweenbranches();
            var faxBetweenBranches = await _context.FAXBRANCHES.FindAsync(id);
            if (faxBetweenBranches == null)
            {
                return NotFound();
            }
            ViewData["SelectedBranches"] = _context.BRANCH_FAX_RECIVER.Where(x => x.FaxID == id).Select(x => x.BranchID).ToList();
            List<Branch> b = _context.BRANCHES.ToList();
            b.RemoveAll(x => x.Name == "السيد المدير" || x.Name == "السيد نائب المدير" || x.ID == user.BranchID);
            ViewData["ReciverBranchID"] = new SelectList(b, "ID", "Name");
            faxBetweenBranches.senderBranch = _context.BRANCHES.First(x => x.ID == faxBetweenBranches.SenderBranchID);
            createFaxbetweenbranches.faxBetweenBranches = faxBetweenBranches;
            return View(createFaxbetweenbranches);
        }

        // POST: FaxBetweenBranches/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CreateFaxbetweenbranches createFaxbetweenbranches, List<IFormFile> files)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    FaxBetweenBranches f = _context.FAXBRANCHES.First(x => x.ID == createFaxbetweenbranches.faxBetweenBranches.ID);
                    f.Subject = createFaxbetweenbranches.faxBetweenBranches.Subject;
                    f.Notes = createFaxbetweenbranches.faxBetweenBranches.Notes;
                    await _context.SaveChangesAsync();

                    int id = _context.FAXBRANCHES.First(x => x.RegistrationNum == createFaxbetweenbranches.faxBetweenBranches.RegistrationNum).ID;
                    try
                    {
                        var br = _context.BRANCH_FAX_RECIVER.Where(x => x.FaxID == id).ToList();
                        _context.BRANCH_FAX_RECIVER.RemoveRange(br);
                    }
                    catch
                    {

                    }
                    foreach (var br in createFaxbetweenbranches.Branches)
                    {
                        BranchFaxRecivers bb = new BranchFaxRecivers();
                        bb.BranchID = br;
                        bb.FaxID = id;

                        _context.BRANCH_FAX_RECIVER.Add(bb);
                        var temp = $"{user.UserName} قام بتعديل الجهة {createFaxbetweenbranches.faxBetweenBranches.RegistrationNum}";
                        var log = new Log()
                        {
                            ActionTakerName = user.UserName,
                            ActionDescription = temp,
                            date = DateTime.Now
                        };
                        _context.Add(log);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FaxBetweenBranchesExists(createFaxbetweenbranches.faxBetweenBranches.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

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
                foreach (var link in filePaths)
                {
                    FaxLink flink = new FaxLink();
                    flink.link = link;
                    flink.FaxBetweenBranchesID = createFaxbetweenbranches.faxBetweenBranches.ID;
                    _context.FaxLinks.Add(flink);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Archive));
            }
            ViewData["SelectedBranches"] = _context.BRANCH_FAX_RECIVER.Where(x => x.FaxID == createFaxbetweenbranches.faxBetweenBranches.ID).Select(x => x.BranchID).ToList();
            List<Branch> b = _context.BRANCHES.ToList();
            b.RemoveAll(x => x.Name == "السيد المدير" || x.Name == "السيد نائب المدير" || x.ID == user.BranchID);
            ViewData["ReciverBranchID"] =(b, "ID", "Name");
            _logger.LogInformation((EventId)200, "{userName} added {agencyName} on {date}", user.UserName, createFaxbetweenbranches.faxBetweenBranches.RegistrationNum, DateTime.Now);
            return View(createFaxbetweenbranches);
        }

        // GET: FaxBetweenBranches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.FAXBRANCHES == null)
            {
                return NotFound();
            }

            var faxBetweenBranches = await _context.FAXBRANCHES
                .Include(f => f.senderBranch)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (faxBetweenBranches == null)
            {
                return NotFound();
            }

            return View(faxBetweenBranches);
        }

        // POST: FaxBetweenBranches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.FAXBRANCHES == null)
            {
                return Problem("Entity set 'ApplicationDbContext.FAXBRANCHES'  is null.");
            }
            var faxBetweenBranches = await _context.FAXBRANCHES.FindAsync(id);
            if (faxBetweenBranches != null)
            {
                _context.FAXBRANCHES.Remove(faxBetweenBranches);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FaxBetweenBranchesExists(int id)
        {
            return _context.FAXBRANCHES.Any(e => e.ID == id);
        }
    }
}
