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
using NuGet.Packaging.Signing;
using NToastNotify;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;

namespace FaxSystem.Controllers
{
    
    public class ArchiveController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;
        private User user;
        private IEnumerable<UserRoles> userRoles;

        private readonly IWebHostEnvironment webHostEnvironment;
        public string userName;
        public ArchiveController(ApplicationDbContext context, IToastNotification toastNotification,
            IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment)
        {
            
            _context = context;
            _toastNotification = toastNotification;
            userName = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            user = _context.USERS.FirstOrDefault(x => x.UserName == userName);
            userRoles = _context.USER_ROLES.Where(x => x.UserId == user.ID).ToList();
            this.webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            ViewData["roles"] = _context.USER_ROLES.Where(x => x.UserId == user.ID).Select(x => x.RoleId).ToList();
            return View();
        }

        public IActionResult sendfax(int id, int type)
        {

            if (type == 1)
            {
                Fax fax = _context.FAXES.First(x => x.ID == id);
                fax.senderAgency = _context.AGENCIES.FirstOrDefault(x => x.ID == fax.SenderAgencyID);
                fax.FaxLinks = _context.FaxLinks.Where(x => x.FaxId == fax.ID).ToList();
                if (fax.DecisionID != null)
                {
                    fax.decision = _context.DECISIONS.First(x => x.ID == fax.DecisionID);
                }
                _toastNotification.AddSuccessToastMessage("هناك مكاتبة معلقة جديد" + fax.Subject);
                return ViewComponent("FaxTableRow", new { fax = fax, IsSuspended = true, CanEdit = userRoles.Where(x => x.RoleId == 1 || x.RoleId == 6).Any(), CanDecide = userRoles.Where(x => x.RoleId == 1 || x.RoleId == 4 || x.RoleId == 5).Any(), IsRealTime = true });
            }
            else
            {
                FaxBetweenBranches faxbet = _context.FAXBRANCHES.First(x => x.ID == id);
                faxbet.senderBranch = _context.BRANCHES.FirstOrDefault(x => x.ID == faxbet.SenderBranchID);
                faxbet.FaxLinks = _context.FaxLinks.Where(x => x.FaxBetweenBranchesID == faxbet.ID).ToList();
                if (faxbet.DecisionID != null)
                {
                    faxbet.decision = _context.DECISIONS.First(x => x.ID == faxbet.DecisionID);
                }

                faxbet.BranchFaxRecivers = _context.BRANCH_FAX_RECIVER.Where(x => x.FaxID == faxbet.ID).ToList();
                foreach (BranchFaxRecivers b in faxbet.BranchFaxRecivers)
                {
                    b.branch = _context.BRANCHES.First(x => x.ID == b.BranchID);
                }
                _toastNotification.AddSuccessToastMessage("هناك مكاتبة معلقة جديدة" + faxbet.Subject);
                return ViewComponent("FaxBranchTableRow", new { faxBetweenBranches = faxbet, IsSuspended = true, CanEdit = (userRoles.Where(x => x.RoleId <= 2).Any() && faxbet.SenderBranchID == user.BranchID), CanDecide = userRoles.Where(x => x.RoleId == 1 || x.RoleId == 4 || x.RoleId == 5).Any(), IsRealTime = true });
            }

        }
        public IActionResult sendfaxbranches(int id, int type)
        {
            ViewData["suspagearc"] = true;
            if (type == 1)
            {
                Fax fax = _context.FAXES.First(x => x.ID == id);
                fax.senderAgency = _context.AGENCIES.FirstOrDefault(x => x.ID == fax.SenderAgencyID);
                fax.FaxLinks = _context.FaxLinks.Where(x => x.FaxId == fax.ID).ToList();
                if (fax.DecisionID != null)
                {
                    fax.decision = _context.DECISIONS.First(x => x.ID == fax.DecisionID);
                }
                _toastNotification.AddSuccessToastMessage("هناك مكاتبة جديد" + fax.Subject);
                return ViewComponent("FaxTableRow", new { fax = fax, IsSuspended = false, CanEdit = userRoles.Where(x => x.RoleId == 1 || x.RoleId == 6).Any(), CanDecide = userRoles.Where(x => x.RoleId == 1 || x.RoleId == 4 || x.RoleId == 5).Any(), IsRealTime = true });
            }
            else
            {
                FaxBetweenBranches faxbet = _context.FAXBRANCHES.First(x => x.ID == id);
                faxbet.senderBranch = _context.BRANCHES.FirstOrDefault(x => x.ID == faxbet.SenderBranchID);
                faxbet.FaxLinks = _context.FaxLinks.Where(x => x.FaxBetweenBranchesID == faxbet.ID).ToList();
                if (faxbet.DecisionID != null)
                {
                    faxbet.decision = _context.DECISIONS.First(x => x.ID == faxbet.DecisionID);
                }

                faxbet.BranchFaxRecivers = _context.BRANCH_FAX_RECIVER.Where(x => x.FaxID == faxbet.ID).ToList();
                foreach (BranchFaxRecivers b in faxbet.BranchFaxRecivers)
                {
                    b.branch = _context.BRANCHES.First(x => x.ID == b.BranchID);
                }
                _toastNotification.AddSuccessToastMessage("هناك مكاتبة جديدة" + faxbet.Subject);
                return ViewComponent("FaxBranchTableRow", new { faxBetweenBranches = faxbet, IsSuspended = false, CanEdit = (userRoles.Where(x => x.RoleId <= 2).Any() && faxbet.SenderBranchID == user.BranchID), CanDecide = userRoles.Where(x => x.RoleId == 1 || x.RoleId == 4 || x.RoleId == 5).Any(), IsRealTime = true });
            }

        }
        public IActionResult SuspendeArchive(ArchPar? arc)
        {

            ViewData["roles"] = _context.USER_ROLES.Where(x => x.UserId == user.ID).Select(x => x.RoleId).ToList();
            var valid = userRoles.Where(x => x.RoleId >= 3 && x.RoleId <= 5 || x.RoleId == 1);

            var Boss = userRoles.Where(x => x.RoleId == 4);
            var Vice = userRoles.Where(x => x.RoleId == 5);

            string path = Path.Combine(webHostEnvironment.WebRootPath, "suspend");

            if (userRoles.Where(x => x.RoleId == 4).Any())
            {
                path = Path.Combine(path, "boss");
                FileStream fs = (new FileStream(path, FileMode.Create));
                StreamWriter sr = new StreamWriter(fs);
                sr.WriteLine(_context.FAXES.Where(x => x.suspend).ToList().Count() + _context.FAXBRANCHES.Where(x => (x.suspend==2||x.suspend==3)).ToList().Count());
                sr.Close();
                fs.Close();
            }
            else if (userRoles.Where(x => x.RoleId == 5).Any())
            {
                path = Path.Combine(path, "vice");
                FileStream fs = (new FileStream(path, FileMode.Create));
                StreamWriter sr = new StreamWriter(fs);
                sr.WriteLine(_context.FAXES.Where(x => x.DecisionID == null).ToList().Count() +
                _context.FAXBRANCHES.Where(x => x.DecisionID == null && (x.suspend==1||x.suspend==3)).ToList().Count());
                sr.Close();
                fs.Close();
            }
            if (!valid.Any())
            {
                return RedirectToAction("Index", "Home");
            }
            List<Branch> branches = new List<Branch>();
            List<Agency> agencies = new List<Agency>();
            agencies = _context.AGENCIES.ToList();
            branches = _context.BRANCHES.ToList();
            ViewData["BranchID"] = new SelectList(branches, "ID", "Name");
            ViewData["AgencyID"] = new SelectList(agencies, "ID", "Name");
            List<string> str = new List<string>();
            str.Add("الجهة/الفرع");
            str.Add("الجهة");
            str.Add("الفرع");
            ViewData["from"] = new SelectList(str);
            arc.BranchFaxes = _context.FAXES.ToList();
            arc.BranchFaxesToBranch = _context.FAXBRANCHES.ToList();
            if (Boss.Any()|| userRoles.Where(x => x.RoleId == 1|| x.RoleId == 3|| x.RoleId == 6).Any())
            {

                arc.BranchFaxes = _context.FAXES.Where(x => x.suspend == true).ToList();
                arc.BranchFaxesToBranch = _context.FAXBRANCHES.Where(x => (x.suspend == 2 || x.suspend == 3)).ToList();



            }


            else if (Vice.Any())
            {

                arc.BranchFaxes = _context.FAXES.Where(x => x.decision == null).ToList();
                arc.BranchFaxesToBranch = _context.FAXBRANCHES.Where(x => x.decision == null && (x.suspend == 1 || x.suspend == 3)).ToList();



            }
          

            if (arc.from != null)
            {
                if (arc.from == "الجهة")
                {
                    arc.BranchFaxesToBranch = new List<FaxBetweenBranches>();
                }
                else if (arc.from == "الفرع")
                {
                    arc.BranchFaxes = new List<Fax>();
                }
            }
            if (arc.StartDate != null)
            {
                arc.BranchFaxes = arc.BranchFaxes.Where(x => x.Date >= arc.StartDate).ToList();
                arc.BranchFaxesToBranch = arc.BranchFaxesToBranch.Where(x => x.Date >= arc.StartDate).ToList();
            }
            if (arc.EndDate != null)
            {
                arc.BranchFaxes = arc.BranchFaxes.Where(x => x.Date <= arc.EndDate).ToList();
                arc.BranchFaxesToBranch = arc.BranchFaxesToBranch.Where(x => x.Date <= arc.EndDate).ToList();
            }
            if (arc.EntryNumSearch != null)
            {
                arc.BranchFaxes = arc.BranchFaxes.Where(x => x.RegistrationNum.Equals(arc.EntryNumSearch)).ToList();
                arc.BranchFaxesToBranch = arc.BranchFaxesToBranch.Where(x => x.RegistrationNum.Equals(arc.EntryNumSearch)).ToList();
            }
            if (arc.SubjectSearch != null)
            {
                arc.BranchFaxes = arc.BranchFaxes.Where(x => x.Subject.Contains(arc.SubjectSearch)).ToList();
                arc.BranchFaxesToBranch = arc.BranchFaxesToBranch.Where(x => x.Subject.Contains(arc.SubjectSearch)).ToList();
            }
            if (arc.BranchID != null)
            {
                List<int> ids = _context.FAXERECIVERS.Where(x => x.BranchID == arc.BranchID)
                 .ToList().Select(x => x.FaxID).ToList();
                arc.BranchFaxes = arc.BranchFaxes.Where(x => ((ids.Contains(x.ID)))).ToList();
                ids = _context.BRANCH_FAX_RECIVER.Where(x => x.BranchID == arc.BranchID).ToList().Select(x => x.FaxID).ToList();
                arc.BranchFaxesToBranch = arc.BranchFaxesToBranch.Where(x => ((ids.Contains(x.ID) || x.SenderBranchID == arc.BranchID))).ToList();
            }
            if (arc.AgencyID != null)
            {
                arc.BranchFaxes = arc.BranchFaxes.Where(x => x.SenderAgencyID == arc.AgencyID).ToList();
                arc.BranchFaxesToBranch = new List<FaxBetweenBranches>();
            }

            List<FaxLink> faxlink = _context.FaxLinks.ToList();
            foreach (Fax f in arc.BranchFaxes)
            {
                f.senderAgency = _context.AGENCIES.FirstOrDefault(x => x.ID == f.SenderAgencyID);
                f.FaxLinks = faxlink.Where(x => x.FaxId == f.ID).ToList();

            }

            List<BranchFaxRecivers> b = _context.BRANCH_FAX_RECIVER.ToList();
            foreach (FaxBetweenBranches h in arc.BranchFaxesToBranch)
            {
                h.senderBranch = branches.FirstOrDefault(x => x.ID == h.SenderBranchID);
                h.FaxLinks = faxlink.Where(x => x.FaxBetweenBranchesID == h.ID).ToList();
                h.BranchFaxRecivers = b.Where(x => x.FaxID == h.ID).ToList();
            }

            arc.StartDate = DateTime.Now;
            arc.EndDate = DateTime.Now;
            ViewData["suspage"] = true;

            ViewData["CanEditFax"] = userRoles.Where(x => x.RoleId <= 2).Any();
            ViewData["CanDecideFax"] = userRoles.Where(x =>  x.RoleId == 4 || x.RoleId == 5).Any();

            ViewData["CanEditFaxBranch"] = userRoles.Where(x => x.RoleId <= 2).Any();
            ViewData["CanDecideFaxBranch"] = userRoles.Where(x =>  x.RoleId == 4 || x.RoleId == 5).Any();
            //ViewData["CanSuspendFaxBranch"] = ..
            return View(arc);
        }

        public IActionResult DecisionTokenArchive(ArchPar? arc)
        {
            ViewData["roles"] = _context.USER_ROLES.Where(x => x.UserId == user.ID).Select(x => x.RoleId).ToList();
            var valid = userRoles.Where(x => x.RoleId >= 3 && x.RoleId <= 5 || x.RoleId == 1);

            var Boss = userRoles.Where(x => x.RoleId == 4);
            var admin = userRoles.Where(x => x.RoleId ==1);
            var Vice = userRoles.Where(x => x.RoleId == 5);

            if (!valid.Any())
            {
                return RedirectToAction("Index", "Home");
            }
            List<Branch> branches = new List<Branch>();
            List<Agency> agencies = new List<Agency>();
            agencies = _context.AGENCIES.ToList();
            branches = _context.BRANCHES.ToList();
            ViewData["BranchID"] = new SelectList(branches, "ID", "Name");
            ViewData["AgencyID"] = new SelectList(agencies, "ID", "Name");
            List<string> str = new List<string>();
            str.Add("الجهة/الفرع");
            str.Add("الجهة");
            str.Add("الفرع");
            ViewData["from"] = new SelectList(str);
            User? UserModel = user;
            ViewData["BranchID"] = new SelectList(_context.BRANCHES, "ID", "Name");
            arc.BranchFaxes = _context.FAXES.ToList();
            arc.BranchFaxesToBranch = _context.FAXBRANCHES.ToList();
            //List<int> ids = _context.FAXERECIVERS.Where(x => x.BranchID == UserModel.BranchID)
            //  .ToList().Select(x => x.FaxID).ToList();
            if (Boss.Any()||admin.Any())
            {

                arc.BranchFaxes = _context.FAXES.Where(x => x.suspend == false).ToList();
                arc.BranchFaxesToBranch = _context.FAXBRANCHES.Where(x => x.suspend == 0&&x.DecisionID!=null).ToList();



            }


            else if (Vice.Any())
            {

                arc.BranchFaxes = _context.FAXES.Where(x => x.decision != null).ToList();
                arc.BranchFaxesToBranch = _context.FAXBRANCHES.Where(x => x.decision != null).ToList();



            }


            if (arc.from != null)
            {
                if (arc.from == "الجهة")
                {
                    arc.BranchFaxesToBranch = new List<FaxBetweenBranches>();
                }
                else if (arc.from == "الفرع")
                {
                    arc.BranchFaxes = new List<Fax>();
                }
            }
            if (arc.StartDate != null)
            {
                arc.BranchFaxes = arc.BranchFaxes.Where(x => x.Date >= arc.StartDate).ToList();
                arc.BranchFaxesToBranch = arc.BranchFaxesToBranch.Where(x => x.Date >= arc.StartDate).ToList();
            }
            if (arc.EndDate != null)
            {
                arc.BranchFaxes = arc.BranchFaxes.Where(x => x.Date <= arc.EndDate).ToList();
                arc.BranchFaxesToBranch = arc.BranchFaxesToBranch.Where(x => x.Date <= arc.EndDate).ToList();
            }
            if (arc.EntryNumSearch != null)
            {
                arc.BranchFaxes = arc.BranchFaxes.Where(x => x.RegistrationNum.Equals(arc.EntryNumSearch)).ToList();
                arc.BranchFaxesToBranch = arc.BranchFaxesToBranch.Where(x => x.RegistrationNum.Equals(arc.EntryNumSearch)).ToList();
            }
            if (arc.SubjectSearch != null)
            {
                arc.BranchFaxes = arc.BranchFaxes.Where(x => x.Subject.Contains(arc.SubjectSearch)).ToList();
                arc.BranchFaxesToBranch = arc.BranchFaxesToBranch.Where(x => x.Subject.Contains(arc.SubjectSearch)).ToList();
            }
            if (arc.BranchID != null)
            {
                List<int> ids= _context.FAXERECIVERS.Where(x => x.BranchID == arc.BranchID)
                 .ToList().Select(x => x.FaxID).ToList();
                arc.BranchFaxes = arc.BranchFaxes.Where(x => ((ids.Contains(x.ID)))).ToList();
                ids = _context.BRANCH_FAX_RECIVER.Where(x => x.BranchID == arc.BranchID).ToList().Select(x => x.FaxID).ToList();
                arc.BranchFaxesToBranch = arc.BranchFaxesToBranch.Where(x => ((ids.Contains(x.ID) || x.SenderBranchID == arc.BranchID))).ToList();
            }
            if (arc.AgencyID != null)
            {
                arc.BranchFaxes = arc.BranchFaxes.Where(x => x.SenderAgencyID == arc.AgencyID).ToList();
                arc.BranchFaxesToBranch = new List<FaxBetweenBranches>();
            }
            List<FaxLink> faxlink = _context.FaxLinks.ToList();
            List<FaxReciver> br = _context.FAXERECIVERS.ToList();
            foreach (Fax f in arc.BranchFaxes)
            {
                f.senderAgency = _context.AGENCIES.FirstOrDefault(x => x.ID == f.SenderAgencyID);
                f.FaxLinks = faxlink.Where(x => x.FaxId == f.ID).ToList();
                f.FaxRecivers = br.Where(x => x.FaxID==f.ID).ToList();
                f.decision = _context.DECISIONS.FirstOrDefault(x => x.ID == f.DecisionID);
            }

            List<BranchFaxRecivers> b = _context.BRANCH_FAX_RECIVER.ToList();
            foreach (FaxBetweenBranches h in arc.BranchFaxesToBranch)
            {
                h.senderBranch = branches.FirstOrDefault(x => x.ID == h.SenderBranchID);
                h.FaxLinks = faxlink.Where(x => x.FaxBetweenBranchesID == h.ID).ToList();
                h.BranchFaxRecivers = b.Where(x => x.FaxID == h.ID).ToList();
                h.decision = _context.DECISIONS.FirstOrDefault(x => x.ID == h.DecisionID);
            }
            if (arc.OpinionIsTrue == true)
            {
                arc.BranchFaxes = arc.BranchFaxes.Where(x => (x.decision.Opinion!=null|| x.decision.OpinionVoice != null)).ToList();
                arc.BranchFaxesToBranch = arc.BranchFaxesToBranch.Where(x => (x.decision.Opinion != null|| x.decision.OpinionVoice != null)).ToList();
            }

            arc.StartDate = DateTime.Now;
            arc.EndDate = DateTime.Now;
            return View(arc);
        }

        //public IActionResult OpinionTokenArchive(ArchPar? arc)
        //{
        //    var valid = StaticData.userRoles.Where(x => x.RoleId >= 3 && x.RoleId <= 4 || x.RoleId == 1);
        //    if (!valid.Any())
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //    List<Branch> branches = new List<Branch>();
        //    List<Agency> agencies = new List<Agency>();
        //    agencies = _context.AGENCIES.ToList();
        //    branches = _context.BRANCHES.ToList();
        //    ViewData["BranchID"] = new SelectList(branches, "ID", "Name");
        //    ViewData["AgencyID"] = new SelectList(agencies, "ID", "Name");
        //    List<string> str = new List<string>();
        //    str.Add("الجهة/الفرع");
        //    str.Add("الجهة");
        //    str.Add("الفرع");
        //    ViewData["from"] = new SelectList(str);
        //    User? UserModel = StaticData.User;
        //    ViewData["BranchID"] = new SelectList(_context.BRANCHES, "ID", "Name");
        //    //arc.BranchFaxes = _context.FAXES.Where(x => x.DecisionID != null).ToList();
        //    //arc.BranchFaxesToBranch = _context.FAXBRANCHES.Where(x => x.DecisionID!=null);
        //    //List<int> ids = _context.FAXERECIVERS.Where(x => x.BranchID == UserModel.BranchID)
        //    //  .ToList().Select(x => x.FaxID).ToList();

        //    arc.BranchFaxes = _context.FAXES.Where(x => x.DecisionID != null).ToList();
        //    arc.BranchFaxesToBranch = _context.FAXBRANCHES.Where(x => x.suspend == false).ToList();




        //    if (arc.from != null)
        //    {
        //        if (arc.from == "الجهة")
        //        {
        //            arc.BranchFaxesToBranch = new List<FaxBetweenBranches>();
        //        }
        //        else if (arc.from == "الفرع")
        //        {
        //            arc.BranchFaxes = new List<Fax>();
        //        }
        //    }
        //    if (arc.StartDate != null)
        //    {
        //        arc.BranchFaxes = arc.BranchFaxes.Where(x => x.Date >= arc.StartDate).ToList();
        //        arc.BranchFaxesToBranch = arc.BranchFaxesToBranch.Where(x => x.Date >= arc.StartDate).ToList();
        //    }
        //    if (arc.EndDate != null)
        //    {
        //        arc.BranchFaxes = arc.BranchFaxes.Where(x => x.Date <= arc.EndDate).ToList();
        //        arc.BranchFaxesToBranch = arc.BranchFaxesToBranch.Where(x => x.Date <= arc.EndDate).ToList();
        //    }
        //    if (arc.EntryNumSearch != null)
        //    {
        //        arc.BranchFaxes = arc.BranchFaxes.Where(x => x.RegistrationNum.Equals(arc.EntryNumSearch)).ToList();
        //        arc.BranchFaxesToBranch = arc.BranchFaxesToBranch.Where(x => x.RegistrationNum.Equals(arc.EntryNumSearch)).ToList();
        //    }
        //    if (arc.SubjectSearch != null)
        //    {
        //        arc.BranchFaxes = arc.BranchFaxes.Where(x => x.Subject.Contains(arc.SubjectSearch)).ToList();
        //        arc.BranchFaxesToBranch = arc.BranchFaxesToBranch.Where(x => x.Subject.Contains(arc.SubjectSearch)).ToList();
        //    }
        //    if (arc.BranchID != null)
        //    {
        //        List<int> ids = _context.FAXERECIVERS.Where(x => x.BranchID == arc.BranchID)
        //         .ToList().Select(x => x.FaxID).ToList();
        //        arc.BranchFaxes = arc.BranchFaxes.Where(x => ((ids.Contains(x.ID)))).ToList();
        //        ids = _context.BRANCH_FAX_RECIVER.Where(x => x.BranchID == arc.BranchID).ToList().Select(x => x.FaxID).ToList();
        //        arc.BranchFaxesToBranch = arc.BranchFaxesToBranch.Where(x => ((ids.Contains(x.ID) || x.SenderBranchID == arc.BranchID))).ToList();
        //    }
        //    if (arc.AgencyID != null)
        //    {
        //        arc.BranchFaxes = arc.BranchFaxes.Where(x => x.SenderAgencyID == arc.AgencyID).ToList();
        //    }

        //    List<FaxLink> faxlink = _context.FaxLinks.ToList();
        //    foreach (Fax f in arc.BranchFaxes)
        //    {
        //        f.senderAgency = _context.AGENCIES.FirstOrDefault(x => x.ID == f.SenderAgencyID);
        //        f.FaxLinks = faxlink.Where(x => x.FaxId == f.ID).ToList();
        //        f.decision = _context.DECISIONS.FirstOrDefault(x => x.ID == f.DecisionID);
        //    }

        //    List<BranchFaxRecivers> b = _context.BRANCH_FAX_RECIVER.ToList();
        //    foreach (FaxBetweenBranches h in arc.BranchFaxesToBranch)
        //    {
        //        h.senderBranch = branches.FirstOrDefault(x => x.ID == h.SenderBranchID);
        //        h.FaxLinks = faxlink.Where(x => x.FaxBetweenBranchesID == h.ID).ToList();
        //        h.BranchFaxRecivers = b.Where(x => x.FaxID == h.ID).ToList();
        //        h.decision = _context.DECISIONS.FirstOrDefault(x => x.ID == h.DecisionID);
        //    }

        //    arc.StartDate = DateTime.Now;
        //    arc.EndDate = DateTime.Now;
        //    return View(arc);
        //}
    }
}
