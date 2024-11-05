using FaxSystem.Data;
using FaxSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Protocol.Plugins;
using NuGet.Protocol.Core.Types;
using Microsoft.AspNetCore.Authorization;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Security.Claims;

namespace FaxSystem.Controllers
{
    [Authorize]
    public class DecisionController : Controller
    {
        public string userName;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment webHostEnvironment;
        public DecisionController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            webHostEnvironment = hostEnvironment;
            userName = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        public IActionResult Create(int id, int type)
        {
            User user = _context.USERS.FirstOrDefault(x => x.UserName == userName);
            var userRoles = _context.USER_ROLES.Where(x => x.UserId == user.ID).ToList();
            var RolesIds = _context.USER_ROLES.Where(x => x.UserId == user.ID).Select(x => x.RoleId).ToList();
            var valid = userRoles.Where(x => x.RoleId == 1 || x.RoleId == 4 || x.RoleId == 5);
            if (!valid.Any())
            {
                return View();
            }
            ViewData["roles"] = RolesIds;
            DecisionToBranches? decisionToBranches = new DecisionToBranches();
            decisionToBranches.type = type;
            if (type == 1)
            {
                FaxBetweenBranches? faxBetweenBranches = _context.FAXBRANCHES.FirstOrDefault(x => x.ID == id);
                Branch? branch = new Branch();
                branch.ID = faxBetweenBranches.SenderBranchID;
                branch = _context.BRANCHES.FirstOrDefault(x => x.ID == branch.ID);
                faxBetweenBranches.senderBranch = branch;
                faxBetweenBranches.BranchFaxRecivers = _context.BRANCH_FAX_RECIVER.Where(x => x.FaxID == faxBetweenBranches.ID).ToList();
                foreach(BranchFaxRecivers r in faxBetweenBranches.BranchFaxRecivers)
                {
                    r.branch= _context.BRANCHES.FirstOrDefault(x => x.ID == r.BranchID);
                }
                faxBetweenBranches.FaxLinks = _context.FaxLinks.Where(x => x.FaxBetweenBranchesID == faxBetweenBranches.ID).ToList();
                decisionToBranches.faxBetweenBranches = faxBetweenBranches;
                if (decisionToBranches.faxBetweenBranches.DecisionID != null)
                {
                    decisionToBranches.faxBetweenBranches.decision = _context.DECISIONS.FirstOrDefault(x => x.ID == decisionToBranches.faxBetweenBranches.DecisionID);
                }
                
            }

            if (type == 2)
            {
                Fax? fax = _context.FAXES.FirstOrDefault(x => x.ID == id);
                Agency? agency = new Agency();
                agency.ID = fax.SenderAgencyID;
                agency = _context.AGENCIES.FirstOrDefault(x => x.ID == agency.ID);
                fax.SenderAgencyName = agency.Name;
                List<Branch> b = _context.BRANCHES.ToList();
                b.RemoveAll(x => x.Name == "السيد المدير" || x.Name == "السيد نائب المدير");
                ViewData["FaxRecivers"] = new SelectList(b, "ID", "Name");
                decisionToBranches.fax = fax;
                if (decisionToBranches.fax.DecisionID != null)
                {
                    decisionToBranches.fax.decision = _context.DECISIONS.FirstOrDefault(x => x.ID == decisionToBranches.fax.DecisionID);
                }
            }
            return View(decisionToBranches);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DecisionToBranches decisionToBranches)
        {
            User user = _context.USERS.FirstOrDefault(x => x.UserName == userName);
            var userRoles = _context.USER_ROLES.Where(x => x.UserId == user.ID).ToList();
            if (decisionToBranches.type == 1)
            {
                
                //ModelValidationState=

                //   ModelValidationState["Branches"].ValidationState=ModelState.val
                //      ModelState[" decisionToBranches.faxBetweenBranches.BranchNames"].ValidationState = ModelValidationState.Valid;
                // if (ModelState.IsValid)
                {
                    if (decisionToBranches.faxBetweenBranches.DecisionID == null)
                    {
                        _context.DECISIONS.Add(decisionToBranches.faxBetweenBranches.decision);
                        _context.SaveChanges();
                        FaxBetweenBranches branchFax = _context.FAXBRANCHES.First(x => x.ID == decisionToBranches.faxBetweenBranches.ID);
                        branchFax.DecisionID = decisionToBranches.faxBetweenBranches.decision.ID;
                        if (userRoles.Where(x => x.RoleId == 4).Any()|| branchFax.suspend == 1)
                        {
                            branchFax.suspend = 0;
                            TempData["faxid"]= branchFax.ID;
                        TempData["faxtype"] = 2;
                        }
                        _context.SaveChanges();
                        
                    }
                    else
                    {

                        FaxBetweenBranches branchFax = _context.FAXBRANCHES.First(x => x.ID == decisionToBranches.faxBetweenBranches.ID);
                        branchFax.suspend = 0;
                        _context.SaveChanges();

                        Decision d = _context.DECISIONS.First(x => x.ID == decisionToBranches.faxBetweenBranches.DecisionID);
                        d.DecisionCheck = decisionToBranches.faxBetweenBranches.decision.DecisionCheck;
                        d.PersonalReview = decisionToBranches.faxBetweenBranches.decision.PersonalReview;
                        d.DecisionVoice = decisionToBranches.faxBetweenBranches.decision.DecisionVoice;
                        d.DecisionText = decisionToBranches.faxBetweenBranches.decision.DecisionText;
                        _context.SaveChanges();
                        TempData["faxid"] = branchFax.ID;
                        TempData["faxtype"] = 2;
                    }



                    var Boss = userRoles.Where(x => x.RoleId == 4);
                    var Vice = userRoles.Where(x => x.RoleId == 5);


                    if (userRoles.Where(x => x.RoleId == 4).Any())
                    {

                        string path = Path.Combine(webHostEnvironment.WebRootPath, "suspend");
                        path = Path.Combine(path, "boss");
                        FileStream fs = (new FileStream(path, FileMode.Create));
                        StreamWriter sr = new StreamWriter(fs);
                        sr.WriteLine(_context.FAXES.Where(x => x.suspend).ToList().Count() + _context.FAXBRANCHES.Where(x => (x.suspend==2||x.suspend==3)).ToList().Count());
                        sr.Close();
                        fs.Close();
                    }
                    if (userRoles.Where(x => x.RoleId == 5).Any() || userRoles.Where(x => x.RoleId == 4).Any())
                    {

                        string path = Path.Combine(webHostEnvironment.WebRootPath, "suspend");
                        path = Path.Combine(path, "vice");
                        FileStream fs = (new FileStream(path, FileMode.Create));
                        StreamWriter sr = new StreamWriter(fs);
                        sr.WriteLine(_context.FAXES.Where(x => x.DecisionID == null).ToList().Count() +
                        _context.FAXBRANCHES.Where(x => x.DecisionID == null && (x.suspend==1||x.suspend==3  )).ToList().Count());
                        sr.Close();
                        fs.Close();
                    }
                    return RedirectToAction("Index", "Home");
                }
                //    else
                {
                    ViewData["ValidationMessage"] = "Errrrrrrrrooooooooooooooooorrrrrrrrrrrrrr";
                    return View(decisionToBranches);

                }

            }


            if (decisionToBranches.type == 2)
            {
                FaxReciver faxReciver = new FaxReciver();
                faxReciver.FaxID = decisionToBranches.fax.ID;
                List<FaxReciver> faxRecivers = new List<FaxReciver>();

                if (decisionToBranches.Branches == null && userRoles.Where(x => x.RoleId == 4).Any())
                {
                    var RolesIds = _context.USER_ROLES.Where(x => x.UserId == user.ID).Select(x => x.RoleId).ToList();
                    ViewData["roles"] = RolesIds;
                    List<Branch> b = _context.BRANCHES.ToList();
                    b.RemoveAll(x => x.Name == "السيد المدير" || x.Name == "السيد نائب المدير");
                    ViewData["FaxRecivers"] = new SelectList(b, "ID", "Name");
                    decisionToBranches.fax.senderAgency = _context.AGENCIES.First(x => x.ID == decisionToBranches.fax.SenderAgencyID);
                    ModelState.AddModelError("Branches", "الرجاء اختيار فرع علي الاقل");
                }
                else if (ModelState.IsValid)
                {
                    if (decisionToBranches.fax.DecisionID == null)
                    {
                        _context.DECISIONS.Add(decisionToBranches.fax.decision);
                        _context.SaveChanges();
                        Fax fax = _context.FAXES.First(x => x.RegistrationNum == decisionToBranches.fax.RegistrationNum);
                        fax.DecisionID = decisionToBranches.fax.decision.ID;
                        if(userRoles.Where(x =>x.RoleId == 4).Any())
                        {
                            fax.suspend = false;
                            

                            foreach (var branch in decisionToBranches.Branches)
                            {
                                faxReciver.BranchID = branch;
                                faxReciver.FaxID = fax.ID;
                                faxRecivers.Add(faxReciver);
                                _context.FAXERECIVERS.Add(faxReciver);
                                await _context.SaveChangesAsync();
                            }
                            TempData["faxid"] = fax.ID;
                            TempData["faxtype"] = 1;
                        }
                        _context.SaveChanges();
                    }
                    else 
                    {
                        Fax fax = _context.FAXES.First(x => x.RegistrationNum == decisionToBranches.fax.RegistrationNum);
                        fax.suspend = false;
                        _context.SaveChanges();
                        Decision d=_context.DECISIONS.First(x=>x.ID==decisionToBranches.fax.DecisionID);
                        d.DecisionCheck = decisionToBranches.fax.decision.DecisionCheck;
                        d.PersonalReview = decisionToBranches.fax.decision.PersonalReview;
                        d.DecisionVoice = decisionToBranches.fax.decision.DecisionVoice;
                        d.DecisionText = decisionToBranches.fax.decision.DecisionText;
                        _context.SaveChanges();

                        foreach (var branch in decisionToBranches.Branches)
                        {
                            faxReciver.BranchID = branch;
                            faxReciver.FaxID = fax.ID;
                            faxRecivers.Add(faxReciver);
                            _context.FAXERECIVERS.Add(faxReciver);
                             _context.SaveChanges();
                        }
                        TempData["faxid"] = fax.ID;
                        TempData["faxtype"] = 1;
                    }
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
                    path = Path.Combine(webHostEnvironment.WebRootPath, "suspend");
                    if (userRoles.Where(x => x.RoleId == 5).Any() || userRoles.Where(x => x.RoleId == 4).Any())
                    {
                        path = Path.Combine(path, "vice");
                        FileStream fs = (new FileStream(path, FileMode.Create));
                        StreamWriter sr = new StreamWriter(fs);
                        sr.WriteLine(_context.FAXES.Where(x => x.DecisionID == null).ToList().Count() +
                        _context.FAXBRANCHES.Where(x => x.DecisionID == null && (x.suspend==1||x.suspend==3)).ToList().Count());
                        sr.Close();
                        fs.Close();
                    }
                    return RedirectToAction("Index", "Home");
                }
            }
            ViewData["ValidationMessage"] = "Errrrrrrrrooooooooooooooooorrrrrrrrrrrrrr";
            return View(decisionToBranches);
        }

        public string UploadVoice(string fileName, string chunks)
        {
            string path, filePath;
            byte[] data;
            try
            {
                path = Path.Combine("\\\\192.168.1.252\\Share Folder", "Uploads");
                filePath = Path.Combine(path, fileName);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch (System.Exception)
            {
                System.Console.WriteLine("[Error]-----------: failed to create file in the path");
                throw;
            }

            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        data = Convert.FromBase64String(chunks);
                        bw.Write(data);
                        bw.Close();
                    }
                    fs.Close();
                }
                System.IO.File.WriteAllBytes(filePath, data);
            }
            catch (System.Exception)
            {
                System.Console.WriteLine("[Error]-----------: failed to write file");
                throw;
            }
            return filePath;
        }
    }
}
