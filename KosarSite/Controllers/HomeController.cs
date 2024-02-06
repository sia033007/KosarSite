using KosarSite.Data;
using KosarSite.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Claims;

namespace KosarSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDb _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDb db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(IFormFile birthFile, IFormFile studyFile, IFormFile jobFile, PersonModel personModel)
        {
            try
            {
                if (birthFile.Length > 500 * 1024 || studyFile.Length > 500 * 1024 || jobFile.Length > 500 * 1024)
                {
                    ViewBag.error = "حجم فایل نباید از 500 کیلوبایت بیشتر باشد";
                    return View();
                }
                else if(await _db.PersonModels.AnyAsync(p=> p.IdNumber == personModel.IdNumber))
                {
                    TempData["warning"] = "این شماره ملی قبلا ثبت شده است";
                    return View();
                }
                personModel.JobDoc = await ConvertFileToByteArray(jobFile);
                personModel.StudyDoc = await ConvertFileToByteArray(studyFile);
                personModel.BirthDoc = await ConvertFileToByteArray(birthFile);
                await _db.PersonModels.AddAsync(personModel);
                await _db.SaveChangesAsync();
                TempData["success"] = "عملیات موفقیت آمیز بود";

            }
            catch(Exception exp)
            {
                TempData["failed"] = "مشکلی رخ داده است";
            }
            return RedirectToAction("Index");
        }
        private async Task<byte[]> ConvertFileToByteArray(IFormFile file)
        {
            using(var stream = new  MemoryStream())
            {
                await file.CopyToAsync(stream);
                return stream.ToArray();

            }
        }

        //public async Task<FileResult> Privacy()
        //{
        //    var file = await _db.PersonModels.FindAsync(3);
        //    return File(file.BirthDoc, "image/jpeg");

        //}
        public IActionResult SearchDoc()
        {
            ViewBag.post = false;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SearchDoc(string idNumber)
        {
            // Verify reCAPTCHA response
            //var recaptchaSecret = "6LeIqmUpAAAAAOlrsuKDfODnFPD8BBLm_-b2aYSt";
            //var client = new HttpClient();
            //var response = client.PostAsync("https://www.google.com/recaptcha/api/siteverify", new FormUrlEncodedContent(new Dictionary<string, string>
            //{

            //            { "secret", recaptchaSecret },
            //            { "response", recaptchaResponse }

            //})).Result;

            //if (!response.IsSuccessStatusCode)
            //{
            //    // Handle error
            //    return BadRequest();
            //}

            //var result = response.Content.ReadAsStringAsync().Result;
            //dynamic jsonResult = JsonConvert.DeserializeObject(result);

            //if (jsonResult.success != "true")
            //{
            //    // reCAPTCHA verification failed
            //    return BadRequest();
            //}
            try
            {
                var person = _db.PersonModels.FirstOrDefault(p => p.IdNumber == idNumber);
                if(person == null)
                {
                    TempData["failed"] = "کاربر یافت نشد";
                    ViewBag.post = true;
                    return View();
                }
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.SerialNumber, idNumber)
                };
                var identity = new ClaimsIdentity(claims, "Cookie");
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync("Cookie", principal);
                ViewBag.person = person;
                ViewBag.post = true;
                TempData["success"] = "عملیات موفقیت آمیز بود";
                return View(person);
            }
            catch(Exception exp)
            {
                TempData["failed"] = "مشکلی رخ داده است";
                return View();
            }
            
        }
        public async Task<IActionResult> ShowJobDoc(string idNumber, string token)
        {
            if (User.Identity.IsAuthenticated)
            {
                var person = await _db.PersonModels.FirstOrDefaultAsync(p => p.IdNumber == idNumber);
                return File(person.JobDoc, "image/jpeg");
            }
            return Unauthorized();
            
        }
        public async Task<IActionResult> ShowStudyDoc(string idNumber, string token)
        {
            if (User.Identity.IsAuthenticated)
            {
                var person = await _db.PersonModels.FirstOrDefaultAsync(p => p.IdNumber == idNumber);
                return File(person.StudyDoc, "image/jpeg");
            }
            return Unauthorized();
        }
        public async Task<IActionResult> ShowBirthDoc(string idNumber, string token)
        {
            if (User.Identity.IsAuthenticated)
            {
                var person = await _db.PersonModels.FirstOrDefaultAsync(p => p.IdNumber == idNumber);
                return File(person.BirthDoc, "image/jpeg");
            }
            return Unauthorized();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}