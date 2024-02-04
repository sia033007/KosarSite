using KosarSite.Data;
using KosarSite.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

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
            personModel.JobDoc = await ConvertFileToByteArray(jobFile);
            personModel.StudyDoc = await ConvertFileToByteArray(studyFile);
            personModel.BirthDoc = await ConvertFileToByteArray(birthFile);
            await _db.PersonModels.AddAsync(personModel);
            await _db.SaveChangesAsync();

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
        public IActionResult Privacy()
        {
            ViewBag.post = false;
            return View();
        }
        [HttpPost]
        public IActionResult Privacy(string idNumber, string recaptchaResponse)
        {
            // Verify reCAPTCHA response
            var recaptchaSecret = "6LeIqmUpAAAAAOlrsuKDfODnFPD8BBLm_-b2aYSt";
            var client = new HttpClient();
            var response = client.PostAsync("https://www.google.com/recaptcha/api/siteverify", new FormUrlEncodedContent(new Dictionary<string, string>
            {

                        { "secret", recaptchaSecret },
                        { "response", recaptchaResponse }
           
            })).Result;

            if (!response.IsSuccessStatusCode)
            {
                // Handle error
                return BadRequest();
            }

            var result = response.Content.ReadAsStringAsync().Result;
            dynamic jsonResult = JsonConvert.DeserializeObject(result);

            if (jsonResult.success != "true")
            {
                // reCAPTCHA verification failed
                return BadRequest();
            }

            var person = _db.PersonModels.FirstOrDefault(p => p.IdNumber == idNumber);
            ViewBag.person = person;
            ViewBag.post = true;

            return View(person);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}