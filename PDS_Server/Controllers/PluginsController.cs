﻿using AspNetCore.Yandex.ObjectStorage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PDS_Server.Elements;
using PDS_Server.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PDS_Server.Api
{
    [Route("plugins")]
    [ApiController]
    public class PluginsController : Controller
    {
        private readonly IMongoRepository<DbPlugin> _pluginRepository;
        private readonly YandexStorageService _yandexOptions;
        public PluginsController(IMongoRepository<DbPlugin> accountRepository, IOptions<YandexStorageOptions> yandexOptions)
        {
            _pluginRepository = accountRepository;
            _yandexOptions = yandexOptions.Value.CreateYandexObjectService();
        }

        [HttpGet]
        [Route("plugins")]
        [Authorize(Roles = AccessGroups.APPROVED)]
        public async Task<IActionResult> Plugins()
        {
            var data = new List<object>();
            try
            {
                foreach (var plugin in await _pluginRepository.Get())
                {
                    try
                    {
                        var instance = new
                        {
                            Id = plugin.Id.ToString(),
                            Name = plugin.Name,
                            Description = plugin.Description,
                            Version = plugin.Versions[0].Number,
                            Changelog = plugin.Versions[0].Changelog
                        };
                        data.Add(instance);
                    }
                    catch { }

                }
            }
            catch { }
            return new JsonResult(data);
        }

        [HttpPost]
        [Route("plugin")]
        [Authorize(Roles = AccessGroups.APPROVED)]
        public async Task<IActionResult> Plugin([FromForm] string id, [FromForm] string version, [FromForm] string revitVersion)
        {
            try
            {
                List<object> data = new List<object>();
                var collection = await _pluginRepository.Get();
                DbPlugin plugin = collection.Where(x => x.Id.ToString() == id).First();
                DbVersion targetVersion = plugin.Versions.Where(x => x.Published && x.RevitVersions.Where(z => z.Link != null && z.Number == version).Count() != 0).First();
                DbRevitVersionInstance targetRevitVersion = targetVersion.RevitVersions.Where(x => x.Number == revitVersion).First();
                if (targetVersion != null)
                    return File(await LoadFile(plugin, targetRevitVersion), "application/octet-stream");
            }
            catch { }
            return NotFound();
        }

        #region Private
        private async Task<bool> DeleteFile(DbPlugin plugin, DbRevitVersionInstance revitVersion)
        {
            var result = await _yandexOptions.DeleteObjectAsync($"Plugins/{plugin.Id}/{revitVersion.Number}/{revitVersion.Link}.zip");
            if (result.IsSuccess)
            {
                revitVersion.Link = null;
                if (await _pluginRepository.Update(plugin.Id, plugin))
                {
                    return true;
                }
            }
            return false;
        }
        private async Task<string> SaveFile(IFormFile file, DbPlugin plugin, DbVersion version, DbRevitVersionInstance revitVersion)
        {
            string name = string.Format("{0}_{1}", version.Number, Guid.NewGuid().ToString());
            var result = await _yandexOptions.PutObjectAsync(file.OpenReadStream(), $"Plugins/{plugin.Id}/{revitVersion.Number}/{name}.zip");
            if (result.IsSuccessStatusCode)
            {
                revitVersion.Link = name;
                if (await _pluginRepository.Update(plugin.Id, plugin))
                {
                    return name;
                }
            }
            return null;
        }
        private async Task<Stream> LoadFile(DbPlugin plugin, DbRevitVersionInstance revitVersion)
        {
            return await _yandexOptions.GetAsStreamAsync($"Plugins/{plugin.Id}/{revitVersion.Number}/{revitVersion.Link}.zip");
        }
        #endregion
    }
}
