using AspNetCore.Yandex.ObjectStorage;
using CommonEnvironment.Elements;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PDS_Server.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDS_Server.Controllers
{
    [ApiController]
    [Route("home")]
    [Route("")]
    public class HomeController : Controller
    {
        private readonly IMongoRepository<DbTeam> _pluginRepository;
        private readonly YandexStorageService _yandexOptions;
        public HomeController(IMongoRepository<DbTeam> accountRepository, IOptions<YandexStorageOptions> yandexOptions)
        {
            _pluginRepository = accountRepository;
            _yandexOptions = yandexOptions.Value.CreateYandexObjectService();
        }
        [Route("/")]
        [Route("/index")]
        [Route("/home")]
        [Route("index")]
        [Route("home")]
        public async Task<IActionResult> Index()
        {
            return View("Index", await _pluginRepository.Get());
        }
    }
}
