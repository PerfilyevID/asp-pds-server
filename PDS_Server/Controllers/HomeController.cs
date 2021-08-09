using AspNetCore.Yandex.ObjectStorage;
using CommonEnvironment.Elements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PDS_Server.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PDS_Server.Controllers
{
    [ApiController]
    [Route("")]
    public class HomeController : Controller
    {
        private readonly IMongoRepository<DbApplication> _applicationRepository;
        private readonly YandexStorageService _yandexOptions;
        public HomeController(IMongoRepository<DbApplication> applicationRepository, IOptions<YandexStorageOptions> yandexOptions)
        {
            _applicationRepository = applicationRepository;
            _yandexOptions = yandexOptions.Value.CreateYandexObjectService();
        }
        [Route("/")]
        [Route("index")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var data = (await _applicationRepository.Get()).ToList();
            data.Reverse();
            return View("Index", data);
        }

        [HttpGet]
        [Authorize]
        [Route("download/{name?}")]
        public async Task<IActionResult> Download(string name)
        {
            try
            {
                Stream stream = await LoadFile(name);
                if(stream != null) { return File(stream, "application/octet-stream"); }
            }
            catch { }
            return NotFound();
        }

        [HttpPost]
        [Route("addversion")]
        public async Task<IActionResult> AddVersion([FromForm] IFormFile file, [FromForm] string version, [FromForm] string changelog)
        {
            if (file != null && !string.IsNullOrEmpty(version) && !string.IsNullOrEmpty(changelog))
            {
                DbApplication application = new DbApplication() { Version = version, Changelog = changelog };
                application.Id = await _applicationRepository.Create(application);
                if (file.FileName.Split('.').Last().ToLower() == "zip")
                    await SaveFile(file, application);
            }
            return RedirectToAction("index", "home");
        }

        [HttpGet]
        [Route("deleteversion/{id?}")]
        public async Task<IActionResult> DeleteVersion(string id)
        {
            DbApplication application = await _applicationRepository.Get(new MongoDB.Bson.ObjectId(id));
            if (application != null)
            {
                await _applicationRepository.Delete(application.Id);
                await DeleteFile(application);
            }   
            return RedirectToAction("index", "home");
        }

        #region Private
        private async Task<bool> DeleteFile(DbApplication application)
        {
            var result = await _yandexOptions.DeleteObjectAsync($"Releases/{application.Link}.zip");
            if (result.IsSuccess)
            {
                application.Link = null;
                if (await _applicationRepository.Update(application.Id, application))
                {
                    return true;
                }
            }
            return false;
        }
        private async Task<string> SaveFile(IFormFile file, DbApplication application)
        {
            string name = string.Format("{0}", Guid.NewGuid().ToString());
            var result = await _yandexOptions.PutObjectAsync(file.OpenReadStream(), $"Releases/{name}.zip");
            if (result.IsSuccessStatusCode)
            {
                application.Link = name;
                if (await _applicationRepository.Update(application.Id, application))
                {
                    return name;
                }
            }
            return null;
        }
        private async Task<Stream> LoadFile(string name)
        {
            try
            {
                return await _yandexOptions.GetAsStreamAsync($"Releases/{name}");
            }
            catch (Exception)
            {
                return null;
            }
            
        }
        #endregion
    }
}
