using CommonEnvironment.Elements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PDS_Server.Repositories;
using PDS_Server.Services;
using RevitTeams_Server.Humanity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PDS_Server.Api
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : Controller
    {
        private static IMongoRepository<DbAccount> _accountRepository;
        private static IEmailSender _mailClient;
        public AccountController(IMongoRepository<DbAccount> accountRepository, IEmailSender mailClient)
        {
            _accountRepository = accountRepository;
            _mailClient = mailClient;
        }

        [HttpGet]
        [Authorize("Admin")]
        [Route("list")]
        public async Task<IActionResult> List()
        {
            try
            {
                return new OkObjectResult(await _accountRepository.Get());
            }
            catch { }
            return NotFound();
        }

        [HttpGet]
        [Authorize]
        [Route("")]
        public async Task<IActionResult> Info()
        {
            try
            {
                return new OkObjectResult((await GetUser(User.Identity.Name)).ToResponse());
            }
            catch { }
            return NotFound();
        }

        [HttpPost]
        [Route("token")]
        public async Task<IActionResult> Token(string email, string password)
        {
            var identity = await GetAccount(email, password);
            if (identity == null) return NotFound();
            return new OkObjectResult(GetToken(identity));
        }

        [HttpGet]
        [Authorize]
        [Route("token")]
        public async Task<IActionResult> Token()
        {
            var identity = await GetAccount(User.Identity.Name);
            if (identity == null) return NotFound();
            return new OkObjectResult(GetToken(identity));
        }

        [HttpGet]
        [Route("verify/{link?}")]
        public async Task<IActionResult> Verify(string link)
        {
            try
            {
                DbAccount account = (await _accountRepository.Get()).Where(x => x.VerifyLink == link).First();
                if (account != null)
                {
                    bool wasVerifyed = !account.IsVerified;
                    if (!account.IsVerified)
                    {
                        account.IsVerified = true;
                        account.Access = DateTime.UtcNow.AddMonths(1);
                        await _accountRepository.Update(account.Id, account);
                        Person person = Person.GetRandomPerson();
                        await _mailClient.SendMessageAsync(Person.GetRandomPerson(), "Wecome", Local.EmailTemplate.LayoutWelcome, account.FirstName, account.Login, "noreply");
                        return View("~/Views/Account/Confirmed.cshtml");
                    }
                }
            }
            catch { }
            return NotFound();
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromForm] string password, [FromForm] string email, [FromForm] string firstName, [FromForm] string lastName)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(password)) return BadRequest();
            if (string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email)) return BadRequest();
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrWhiteSpace(firstName)) return BadRequest();
            if (string.IsNullOrEmpty(lastName) || string.IsNullOrWhiteSpace(lastName)) return BadRequest();
            string normalized;
            if (email.ValidateEmail(out normalized))
            {
                try
                {
                    if (await UserExist(normalized))
                    {
                        return BadRequest();
                    }
                    DbAccount account = new DbAccount()
                    {
                        Login = normalized,
                        Password = password.ConvertToHash(),
                        FirstName = firstName,
                        SecondName = lastName,
                        VerifyLink = Guid.NewGuid().ToString()
                    };
                    await _mailClient.SendMessageAsync(Person.Persons[0], "Please confirm your email", Local.EmailTemplate.LayoutConfirm, $"https://{Request.Host.Value}/api/account/verify/{account.VerifyLink}", account.Login, "noreply");
                    await _accountRepository.Create(account);
                    return Ok();
                }
                catch { return BadRequest(); }
            }
            return BadRequest();
        }

        #region Private
        private static string GetToken(ClaimsIdentity identity)
        {
            var now = DateTime.UtcNow;

            JwtSecurityToken jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
        private static async Task<ClaimsIdentity> GetAccount(string email, string password=null)
        {
            string login;
            List<DbAccount> collection;

            if (!email.ValidateEmail(out login)) return null;

            if (password == null)
                collection = (await _accountRepository.Get()).Where(x => x.Login == login).ToList();

            else
                collection = (await _accountRepository.Get()).Where(x => (x.Password == password.ConvertToHash()) && x.Login == login).ToList();
            
            if (collection.Count == 1)
            {
                DbAccount person = collection.First();
                if (person != null)
                {
                    if (!person.IsVerified) return null;
                    List<Claim> claims = new List<Claim>
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, person.Login),
                        new Claim(ClaimsIdentity.DefaultRoleClaimType, person.Role)
                    };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                    return claimsIdentity;
                }
            }
            return null;
        }
        private async Task<bool> UserExist(string email)
        {
            return (await GetUser(email)) != null;
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
