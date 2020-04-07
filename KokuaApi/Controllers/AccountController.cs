using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        private readonly RoleManager<KokuaRole> _roleManager;

        public AccountController(UserManager<KokuaUser> userManager, RoleManager<KokuaRole> roleManager, SignInManager<KokuaUser> signInManager, ILogger<AccountController> logger, IUnitOfWork uow)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _uow = uow;
            _roleManager = roleManager;
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

                string roleName = string.Empty;

                if (model.UserType == UserType.Beneficiary)
                {
                    roleName = "Beneficiary";
                }else
                {
                    roleName = "Volunteer";
                }

                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    var role = new KokuaRole();

                    role.Name = roleName;
                    await _roleManager.CreateAsync(role);
                }


                await _userManager.AddToRoleAsync(user, roleName);
                await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, roleName));

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
            var needs = await _uow.Needs.WhereAsync(a => a.Username == username);

            if (user != null)
            {
                var response = new UserDataResponse
                {
                    Name = user.Name,
                    Surname = user.Surname,
                    WhoAmI = user.WhoAmI,
                    Address = user.Address,
                    Age = user.Age,
                    Needs = needs.Select(a => new
                    {
                        id = a.Id,
                        title = a.Title,
                        username = a.Username,
                        orderStatus = a.OrderStatus,
                        createdAt = a.CreatedAt,
                        acceptedDate = a.AcceptedDate,
                        completedDate = a.CompletedDate,
                        needProducts = a.NeedProducts.Select(a => new { productDescription = a.ProductDescription})
                    }),
                    PhoneNumber = user.PhoneNumber,
                    ProfileImage = user.ProfileImage
                };

                return Ok(response);
            }
            string message = "User is not defined!";

            var statusMessage = new StatusMessageResponseModel();
            statusMessage.IsError = true;
            statusMessage.Messages.Add(message);


            return Ok(statusMessage);



        }

        [HttpPost]
        [Route("Profile")]
        [Authorize]
        public async Task<IActionResult> Profile(UserDataPostResponse model)
        {
            var username = HttpContext.User.Identity.Name;

            var user = await _userManager.FindByNameAsync(username);

            if (!string.IsNullOrWhiteSpace(model.Name))
            {
                user.Name = model.Name;
            }

            if (!string.IsNullOrWhiteSpace(model.Surname))
            {
                user.Surname = model.Surname;
            }

            if (!string.IsNullOrWhiteSpace(model.Age.ToString()))
            {
                user.Age = model.Age;
            }

            if (!string.IsNullOrWhiteSpace(model.WhoAmI))
            {
                user.WhoAmI = model.WhoAmI;
            }

            if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                user.PhoneNumber = model.PhoneNumber;
            }

            if (!string.IsNullOrWhiteSpace(model.Address))
            {
                user.Address = model.Address;
            }

            if (!string.IsNullOrWhiteSpace(model.ProfileImage))
            {
                user.ProfileImage = model.ProfileImage;
            }


            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {

                var response = new UserDataPostResponse
                {
                    Name = user.Name,
                    Surname = user.Surname,
                    WhoAmI = user.WhoAmI,
                    Address = user.Address,
                    Age = user.Age,
                    PhoneNumber = user.PhoneNumber,
                    ProfileImage = user.ProfileImage
                };


                return Ok(response);
            }


            string message = "User is not defined!";

            var statusMessage = new StatusMessageResponseModel();
            statusMessage.IsError = true;
            statusMessage.Messages.Add(message);


            return Ok(statusMessage);
        }


    }
}