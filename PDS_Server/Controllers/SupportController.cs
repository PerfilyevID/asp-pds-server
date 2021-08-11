using AspNetCore.Yandex.ObjectStorage;
using CommonEnvironment.Elements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PDS_Server.Humanity;
using PDS_Server.Repositories;
using PDS_Server.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PDS_Server.Controllers
{
    [Route("support")]
    [ApiController]
    public class SupportController : Controller
    {
        private readonly IMongoRepository<DbReport> _reportRepository;
        private static IMongoRepository<DbAccount> _accountRepository;
        private static IMongoRepository<DbException> _exceptionRepository;
        private readonly YandexStorageService _yandexOptions;
        private static IEmailSender _mailClient;
        public SupportController(IMongoRepository<DbException> exceptionRepository, IMongoRepository<DbReport> reportRepository, IEmailSender mailClient, IMongoRepository<DbAccount> accountRepository, IOptions<YandexStorageOptions> yandexOptions)
        {
            _exceptionRepository = exceptionRepository;
            _reportRepository = reportRepository;
            _accountRepository = accountRepository;
            _mailClient = mailClient;
            _yandexOptions = yandexOptions.Value.CreateYandexObjectService();
        }

        [Route("bug")]
        [HttpPost]
        public async Task<IActionResult> Bug([FromForm] string user, [FromForm] string time, [FromForm] string data)
        {
            try
            {
                var ddd = new HashSet<string>((await _exceptionRepository.Get()).Select(x => x.Data));
                if(!ddd.Contains(data))
                {
                    await _exceptionRepository.Create(new DbException() { User = user, Time = time, Data = data });
                }
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }

        }

        [Route("download/{name?}")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Download(string name)
        {
            try
            {
                Stream stream = await LoadFile(name);
                if (stream != null) { return File(stream, "application/octet-stream"); }
            }
            catch { }
            return NotFound();
        }

        [Route("report/{id?}")]
        [HttpGet]
        public async Task<IActionResult> Report(string id)
        {
            var report = await _reportRepository.Get(new MongoDB.Bson.ObjectId(id));
            if (report == null) return RedirectToAction("index", "home");
            return View(report);
        }

        [Route("delete")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var report = await _reportRepository.Get(new MongoDB.Bson.ObjectId(id));
            if(report != null)
            {
                await _reportRepository.Delete(report.Id);
                await DeleteFile(report);
            }
            return RedirectToAction("console");
        }

        [Route("answer/{id?}")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Answer([FromForm] string comment, string id)
        {
            try
            {
                DbReport report = await _reportRepository.Get(new MongoDB.Bson.ObjectId(id));
                report.Comment = comment;
                report.IsClosed = true;
                await _reportRepository.Update(report.Id, report);
                await _mailClient.SendMessageAsync(Person.GetRandomPerson(), "Поддержка", Local.EmailTemplate.LayoutSupportEvent, $"http://{Request.Host.Value}/support/report/{report.Id}", report.User, "support");
            }
            catch { }
            return RedirectToAction("console");
        }

        [Route("console")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Console()
        {
            var data = (await _reportRepository.Get()).Where(x => x.IsClosed == false).Take(5).ToList();
            if(data.Count == 0)
            {
                data = (await _reportRepository.Get()).ToList();
            }
            return View(data);
        }

        [Route("index")]
        [Route("")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [Route("send")]
        [Route("")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Send([FromForm] string issue, [FromForm] string description, [FromForm] IFormFile file)
        {
            string[] formats = new string[] { "jpeg", "png", "bmp", "gif", "tga", "jpg", "tif", "tiff" };
            if (file != null)
            {
                if (!file.FileName.Contains('.'))
                {
                    ViewBag.Alert = "Неизвестный формат!";
                    return View("Index");
                }
                if (!formats.Contains(file.FileName.Split('.').Last()))
                {
                    ViewBag.Alert = "Формат не поддерживается!";
                    return View("Index");
                }
            }

            DbAccount user = await GetUser(User.Identity.Name);
            DbReport report = new DbReport() { Issue = issue, Description = description, IsClosed = false, User = user.Login };
            report.Id = await _reportRepository.Create(report);
            if (file != null) await SaveFile(file, report);
            await _mailClient.SendMessageAsync(Person.GetRandomPerson(), "Поддержка", Local.EmailTemplate.LayoutSupportCreated, $"http://{Request.Host.Value}/support/report/{report.Id}", user.Login, "support");
            ViewBag.Success = "Обращение отправлено в поддержку!";
            return View("Index");
        }

        [Route("sendguest")]
        [Route("")]
        [HttpPost]
        public async Task<IActionResult> SendGuest([FromForm] string issue, [FromForm] string description, [FromForm] IFormFile file, [FromForm] string email)
        {
            string[] formats = new string[] { "jpeg", "png", "bmp", "gif", "tga", "jpg", "tif", "tiff" };
            if(file != null)
            {
                if (!file.FileName.Contains('.'))
                {
                    ViewBag.Alert = "Неизвестный формат!";
                    return View("Index");
                }
                if (!formats.Contains(file.FileName.Split('.').Last()))
                {
                    ViewBag.Alert = "Формат не поддерживается!";
                    return View("Index");
                }
            }

            DbReport report = new DbReport() { Issue = issue, Description = description, IsClosed = false, User = email };
            report.Id = await _reportRepository.Create(report);
            if (file != null) await SaveFile(file, report);
            try
            {
                await _mailClient.SendMessageAsync(Person.GetRandomPerson(), "Поддержка", Local.EmailTemplate.LayoutSupportCreated, $"http://{Request.Host.Value}/support/report/{report.Id}", email, "support");
                ViewBag.Success = "Обращение отправлено в поддержку!";
                return View("Index");
            }
            catch
            {
                if (file != null) await DeleteFile(report);
                await _reportRepository.Delete(report.Id);
                ViewBag.Alert = "Ошибка";
                return View("Index");
            }
        }
        #region Private
        private async Task<string> SaveFile(IFormFile file, DbReport report)
        {
            string name = string.Format("{0}_{1}", Guid.NewGuid().ToString(), file.FileName);
            var result = await _yandexOptions.PutObjectAsync(file.OpenReadStream(), $"Reports/{name}");
            if (result.IsSuccessStatusCode)
            {
                report.Link = name;
                if (await _reportRepository.Update(report.Id, report))
                {
                    return name;
                }
            }
            return null;
        }
        private async Task<bool> DeleteFile(DbReport report)
        {
            await _yandexOptions.DeleteObjectAsync($"Reports/{report.Link}");
            return true;
        }
        private async Task<Stream> LoadFile(string name)
        {
            return await _yandexOptions.GetAsStreamAsync($"Reports/{name}");
        }
        private static async Task<DbAccount> GetUser(string email)
        {
            string login;
            if (!email.ValidateEmail(out login)) return null;
            List<DbAccount> collection = (await _accountRepository.Get()).Where(x => x.Login == login).ToList();
            if (collection.Count == 1)
            {
                DbAccount person = collection.First();
                if (person != null) return person;
            }
            return null;
        }
        #endregion
    }
}
