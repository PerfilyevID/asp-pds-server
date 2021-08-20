﻿using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using PDS_Server.Elements;
using PDS_Server.Elements.Communications;
using PDS_Server.Elements.Revit;
using PDS_Server.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDS_Server.Api
{
    [Route("api/chat")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IMongoRepository<DbAccount> _accountRepository;
        private readonly IMongoRepository<DbClashResult> _clashResultRepository;
        private readonly IMongoRepository<DbChat> _chatRepository;
        public ChatController(IMongoRepository<DbChat> chatRepository, IMongoRepository<DbClashResult> documentRepository, IMongoRepository<DbAccount> accountRepository)
        {
            _chatRepository = chatRepository;
            _clashResultRepository = documentRepository;
            _accountRepository = accountRepository;
        }

        [Route("{id?}")]
        [HttpGet]
        public async Task<IActionResult> Chat(string id)
        {
            DbAccount user = await GetUser(User.Identity.Name);
            var result = await _chatRepository.Get(new ObjectId(id), user.Team.ToString());
            if (result != null)
            {
                return Ok(result.ToResponse());
            }
            return NotFound();
        }

        [Route("{id?}")]
        [HttpPost]
        public async Task<IActionResult> PostMessage(string id, [FromForm] string to, [FromForm] string message)
        {
            try
            {
                DbAccount user = await GetUser(User.Identity.Name);
                var result = await _chatRepository.Get(new ObjectId(id), user.Team.ToString());
                if (result != null)
                {
                    var messages = result.Messages;
                    messages.Add(new DbMessage()
                    {
                        From = user.Id,
                        Message = message,
                        Time = DateTime.UtcNow,
                        To = new ObjectId(to)
                    });
                    result.Messages = messages;
                    result.LastChange = DateTime.UtcNow;
                    await _chatRepository.Update(result.Id, result);
                    return Ok();
                }
            }
            catch { }
            return BadRequest();
        }

        [Route("delete/{id?}")]
        [HttpPost]
        public async Task<IActionResult> DeleteMessage(string id, [FromForm] int number)
        {
            try
            {
                DbAccount user = await GetUser(User.Identity.Name);
                var result = await _chatRepository.Get(new ObjectId(id), user.Team.ToString());
                if (result != null)
                {
                    var messages = result.Messages.ToList();
                    if(messages[number].From == user.Id)
                    {
                        messages.RemoveAt(number);
                    }
                    result.Messages = messages;
                    await _chatRepository.Update(result.Id, result);
                    return Ok();
                }
            }
            catch { }
            return BadRequest();
        }
        #region Private
        private async Task<DbAccount> GetUser(string email)
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