using AspNetCore.Yandex.ObjectStorage;
using CommonEnvironment.Elements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using PDS_Server.Repositories;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PDS_Server.Api
{
    [Route("api/application")]
    [ApiController]
    public class AppController : ControllerBase
    {
        private readonly IMongoRepository<DbApplication> _appRepository;
        private readonly YandexStorageService _yandexOptions;
        public AppController(IMongoRepository<DbApplication> appRepository, IOptions<YandexStorageOptions> yandexOptions)
        {
            _appRepository = appRepository;
            _yandexOptions = yandexOptions.Value.CreateYandexObjectService();
        }

        [HttpGet]
        [Route("version")]
        public IActionResult Version()
        {
            try
            {
                return new OkObjectResult(ConfigurationManager.AppSetting.GetValue<string>("version"));
            }
            catch { return BadRequest(); }
        }

        [HttpGet]
        [Authorize]
        [Route("list")]
        public async Task<IActionResult> List()
        {
            try
            {
                return new OkObjectResult(await _appRepository.Get());
            }
            catch { return BadRequest(); }
        }

        [HttpGet]
        [Authorize("Admin,User,TeamOwner")]
        [Route("download/{name?}")]
        public async Task<IActionResult> Download(string name)
        {
            try
            {
                return File(await LoadFile(name), "application/octet-stream");
            }
            catch { return NotFound(); }
        }

        #region Private
        private async Task<Stream> LoadFile(string name)
        {
            return await _yandexOptions.GetAsStreamAsync($"Releases/{name}");
        }
        #endregion
    }
}
