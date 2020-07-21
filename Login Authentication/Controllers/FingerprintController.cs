using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using net_core_fingerprint_authorization.Models;
using net_core_fingerprint_authorization.DataContext;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;

namespace net_core_fingerprint_authorization.Controllers
{
    public class FingerprintController : Controller
    {
        private readonly Databasecontext db = new Databasecontext();

        [BindProperty]
        public FingerprintModel admin { get; set; }

        // GET: FingerprintModels
        [ValidateAntiForgeryToken]
        [AutoValidateAntiforgeryToken]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("userName") == null)
            {
                return RedirectToAction("Login", "Fingerprint");
            }
            else
            {
                var user_check = db.fingerPrintModel.ToList();
                return View(user_check);
            }

        }

        // GET: FingerprintModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fingerprintModel = await db.fingerPrintModel
                .FirstOrDefaultAsync(m => m.ScannedID == id);
            if (fingerprintModel == null)
            {
                return NotFound();
            }

            return View(fingerprintModel);
        }

        // GET: FingerprintModels/Create
        public IActionResult RegisterUser()
        {
            if (HttpContext.Session.GetString("userName") != null)
            {
                TempData.Clear();
                ViewData.Clear();
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Fingerprint");
            }
            return View();
        }

        // POST: FingerprintModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegisterUser(string TocompareFingerPrint, FingerprintModel model)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState != null)
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                Byte[] originalBytes = ASCIIEncoding.Default.GetBytes(model.Password + model.EmailID);
                Byte[] encodedBytes = md5.ComputeHash(originalBytes);

                string hashedPassword = BitConverter.ToString(encodedBytes).Replace("-", "");
                //var nouser = db.fingerPrintModel.Where(u => u.EmailID == model.EmailID && u.Password != hashedPassword).Any();
                //var newudb = db.fingerPrisntModel.Where(u => u.EmailID == model.EmailID && u.Password == hashedPassword).FirstOrDefault();
                model.Password = hashedPassword;
                model.TemplateFormatDB = TocompareFingerPrint;
                db.fingerPrintModel.Add(model);
                db.SaveChanges();

                ModelState.Clear();
                TempData["Success"] = model.Username + " successfully registered.";
                ModelState.Clear();
                return RedirectToAction("Login", "Fingerprint");
            }
            return View();
        }

        // GET: FingerprintModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fingerprintModel = await db.fingerPrintModel.FindAsync(id);
            if (fingerprintModel == null)
            {
                return NotFound();
            }
            return View(fingerprintModel);
        }

        // POST: FingerprintModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ScannedID,Username,EmailID,Password,ComparePassword,TemplateFormatDB")] FingerprintModel fingerprintModel)
        {
            if (id != fingerprintModel.ScannedID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(fingerprintModel);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FingerprintModelExists(fingerprintModel.ScannedID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(fingerprintModel);
        }

        // GET: FingerprintModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fingerprintModel = await db.fingerPrintModel
                .FirstOrDefaultAsync(m => m.ScannedID == id);
            if (fingerprintModel == null)
            {
                return NotFound();
            }

            return View(fingerprintModel);
        }

        // POST: FingerprintModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fingerprintModel = await db.fingerPrintModel.FindAsync(id);
            db.fingerPrintModel.Remove(fingerprintModel);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FingerprintModelExists(int id)
        {
            return db.fingerPrintModel.Any(e => e.ScannedID == id);
        }


        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("userName") != null)
            {
                TempData.Clear();
                ViewData.Clear();
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Fingerprint");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string TocompareFingerPrint, FingerprintModel model, string checkAuthorization)
        {
            var checkExistence = db.fingerPrintModel.Where(x => x.EmailID == model.EmailID).FirstOrDefault();
            if (checkExistence == null)
            {
                ModelState.AddModelError("", "EmailId is not registered!");
                return View();
            }
            MD5 md5 = new MD5CryptoServiceProvider();
            Byte[] originalBytes = ASCIIEncoding.Default.GetBytes(model.Password + model.EmailID);
            Byte[] encodedBytes = md5.ComputeHash(originalBytes);

            string hashedPassword = BitConverter.ToString(encodedBytes).Replace("-", "");
            var checkWrongPassword = db.fingerPrintModel.Where(u => u.EmailID == model.EmailID && u.Password != hashedPassword && TocompareFingerPrint == null).FirstOrDefault();
            if (checkWrongPassword != null)
            {
                ModelState.AddModelError("", "Entered password is incorrect!");
                return View();

            }
            if (model.EmailID != null && model.Password != null && TocompareFingerPrint == null)
            {
                var uniqueUser = db.fingerPrintModel.Where(u => u.EmailID == model.EmailID && u.Password != hashedPassword).Any();
                var checkDB = db.fingerPrintModel.Where(u => u.EmailID == model.EmailID && u.Password == hashedPassword).FirstOrDefault();


                if (checkDB != null)
                {
                    TempData.Clear();
                    HttpContext.Session.SetString("userPresent", "true");
                    ViewBag.UserPrintSuccess = checkDB.TemplateFormatDB;
                    //ViewBag.UserPrint = checkUser.TemplateFormatDB;
                    return View();
                }
                else if (checkExistence == null)
                {

                }
                return RedirectToAction("Login", "Fingerprint");
            }
            if (model.Password == null && model.EmailID != null && TocompareFingerPrint != null)
            {
                var checkEmail = db.fingerPrintModel.Where(x => x.EmailID == model.EmailID).FirstOrDefault();

                if (HttpContext.Session.GetString("userPresent") != null && checkEmail != null)
                {
                    var newudb = db.fingerPrintModel.Where(u => u.EmailID == model.EmailID && TocompareFingerPrint != null).FirstOrDefault();
                    TempData.Clear();
                    ViewData.Clear();
                    HttpContext.Session.Clear();
                    HttpContext.Session.SetString("userName", newudb.Username);

                    //Session["ID"] = newudb.ClientID.ToString();
                    //Session["UserName"] = newudb.UserName.ToString();

                    return RedirectToAction("Index", "FingerPrint");
                }
            }
            return View();
        }

        public IActionResult Signout()
        {
            if (ModelState.IsValid == true)
            {
                TempData.Clear();
                ViewData.Clear();
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Fingerprint");
            }
            return View();
        }

        public IActionResult DownloadWin64()
        {
            var bar64 = "~/downloads/SGI_BWAPI_WIN_64bit.exe";
            return File(bar64, "application/octet-stream",
                "Secugen_WebAPI_64bit.exe");
        }

        public IActionResult DownloadWin32()
        {
            var bar64 = "~/downloads/SGI_BWAPI_Win_32bit.exe";
            return File(bar64, "application/octet-stream",
                "Secugen_WebAPI_32bit.exe");
        }

        public IActionResult read()
        {
            return View();

        }
    }
}
