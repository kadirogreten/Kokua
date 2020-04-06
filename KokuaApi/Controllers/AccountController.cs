using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KokuaApi.Models;
using KokuaApi.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using ServiceLayer;

namespace KokuaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("MyPolicy")]
    public class AccountController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly UserManager<KokuaUser> _userManager;
        private readonly SignInManager<KokuaUser> _signInManager;
        private readonly ILogger _logger;

        public AccountController(UserManager<KokuaUser> userManager, SignInManager<KokuaUser> signInManager, ILogger<AccountController> logger, IUnitOfWork uow)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _uow = uow;
        }

        [Route("Register")]
        [HttpPost]
        public async Task<StatusMessageResponseModel> Register(RegisterViewModel model)
        {
            KokuaUser user = new KokuaUser
            {
                Email = model.Email,
                UserName = model.Email,
                UserType = model.UserType

            };

            var status = new StatusMessageResponseModel();

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");

                //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                //var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                //await _emailSender.SendEmailAsync(model.Email, callbackUrl, "");

                //await _signInManager.SignInAsync(user, isPersistent: false);
                //_logger.LogInformation("User created a new account with password.");

                status.IsError = false;

                if (status.Messages == null)
                {
                    status.Messages = new List<string>();
                }

                status.Messages.Add("User created a new account with password.");

                return status;
            }

            status.IsError = true;

            if (status.Messages == null)
            {
                status.Messages = new List<string>();
            }

            foreach (var item in result.Errors)
            {
                status.Messages.Add(item.Description);
            }

            return status;
        }


        [HttpGet]
        [Route("Profile")]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var username = HttpContext.User.Identity.Name;

            var user = await _userManager.FindByNameAsync(username);


            return Ok(user);
        }

    }
}