using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unison.Cloud.Core.Data.Entities;
using Unison.Cloud.Core.Exceptions;
using Unison.Cloud.Core.Interfaces.Services;
using Unison.Cloud.Web.Models;
using Unison.Cloud.Web.Utilities;

namespace Unison.Cloud.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(IAccountService accountService, ITokenService tokenService, ILogger<AccountsController> logger)
        {
            _accountService = accountService;
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<AccountDto>> Get()
        {
            IEnumerable<Account> accounts = _accountService.GetAll();
            return Ok(accounts.Select(a => a.ToHttpModel()));
        }

        [HttpGet("{id}")]
        public ActionResult<AccountDto> Get(int id)
        {
            Account account = _accountService.Find(id);
            return Ok(account.ToHttpModel());
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public ActionResult<AuthResponseDto> Authenticate(AuthRequestDto authDto)
        {
            Account account = _accountService.Authenticate(authDto.Username, authDto.Password);

            if (account == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            string token = _tokenService.Create(account);

            AuthResponseDto authResponse = account.ToAuthResponseModel();
            authResponse.Token = token;

            return Ok(authResponse);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public ActionResult Create(RegisterDto createAccountRequest)
        {
            Account account = createAccountRequest.ToBusinessModel();

            try
            {
                _accountService.Create(account, createAccountRequest.Password);
                return Ok();
            }
            catch (InvalidRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public ActionResult Update(int id, RegisterDto updateAccountRequest)
        {
            Account account = updateAccountRequest.ToBusinessModel();
            account.Id = id;

            try
            {
                _accountService.Update(account, updateAccountRequest.Password);
                return Ok();
            }
            catch (InvalidRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _accountService.Remove(id);
            return Ok();
        }
    }
}
