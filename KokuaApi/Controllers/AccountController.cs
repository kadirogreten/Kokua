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
        public async Task<IActionResult> Register(RegisterViewModel model)
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
                }
                else
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

                return Ok(new { IsSuccess = true, Result = status, Message = "User created a new account with password!" });
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

            return Ok(new { IsSuccess = false, Result = status, Message = "Unexpected Errors!" }); ;
        }


        [HttpGet]
        [Route("Profile")]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var username = HttpContext.User.Identity.Name;

            var user = await _userManager.FindByNameAsync(username);


            if (user.UserType == UserType.Volunteer)
            {

                var needs = await _uow.Needs.WhereAsync(a => a.AcceptedUsername == user.UserName);



                var response = new UserDataResponse
                {
                    Name = user.Name,
                    Surname = user.Surname,
                    WhoAmI = user.WhoAmI,
                    Address = user.Address,
                    Age = user.Age,
                    Needs = needs.Select(a => new UserProfileNeedResponse
                    {
                        Id = a.Id,
                        Title = a.Title,
                        Username = a.Username,
                        OrderStatus = a.OrderStatus,
                        CreatedAt = a.CreatedAt,
                        AcceptedDate = a.AcceptedDate,
                        CompletedDate = a.CompletedDate,
                        Longitude = a.Longitude,
                        Latitude = a.Latitude,
                        NeedProducts = a.NeedProducts == null ? new List<NeedProducts>().Select(b => new UserProfileNeedProductResponse()) : a.NeedProducts.Select(b => new UserProfileNeedProductResponse { Id = b.Id, ProductDescription = b.ProductDescription })
                    }),
                    PhoneNumber = user.PhoneNumber,
                    ProfileImage = user.ProfileImage,
                    NeedsCount = needs.ToList().Count > 0 ? needs.ToList().Count : 0
                };

                return Ok(new { IsSuccess = true, Result = response, Message = "User return a value!" });
            }


            if (user != null)
            {

                var needs = await _uow.Needs.WhereAsync(a => a.Username == user.UserName);

                var response = new UserDataResponse
                {
                    Name = user.Name,
                    Surname = user.Surname,
                    WhoAmI = user.WhoAmI,
                    Address = user.Address,
                    Age = user.Age,
                    Needs = needs.Select(a => new UserProfileNeedResponse
                    {
                        Id = a.Id,
                        Title = a.Title,
                        Username = a.Username,
                        OrderStatus = a.OrderStatus,
                        CreatedAt = a.CreatedAt,
                        AcceptedDate = a.AcceptedDate,
                        CompletedDate = a.CompletedDate,
                        Longitude = a.Longitude,
                        Latitude = a.Latitude,
                        NeedProducts = a.NeedProducts == null ? new List<NeedProducts>().Select(b => new UserProfileNeedProductResponse()) : a.NeedProducts.Select(b => new UserProfileNeedProductResponse { Id = b.Id, ProductDescription = b.ProductDescription })
                    }),
                    PhoneNumber = user.PhoneNumber,
                    ProfileImage = user.ProfileImage,
                    NeedsCount = needs.ToList().Count > 0 ? needs.ToList().Count : 0
                };

                return Ok(new { IsSuccess = true, Result = response, Message = "User return a value!" });
            }
            string message = "User is not defined!";

            var statusMessage = new StatusMessageResponseModel();
            statusMessage.IsError = true;
            statusMessage.Messages.Add(message);


            return Ok(new { IsSuccess = false, Result = statusMessage, Message = "User return null!" });



        }

        [HttpPost]
        [Route("RequestProfile")]
        [Authorize]
        public async Task<IActionResult> Profile(UsernameViewModel model)
        {

            var user = await _userManager.FindByNameAsync(model.Requestname);


            if (user.UserType == UserType.Volunteer)
            {

                var needs = await _uow.Needs.WhereAsync(a => a.AcceptedUsername == user.UserName);



                var response = new UserDataResponse
                {
                    Name = user.Name,
                    Surname = user.Surname,
                    WhoAmI = user.WhoAmI,
                    Address = user.Address,
                    Age = user.Age,
                    Needs = needs.Select(a => new UserProfileNeedResponse
                    {
                        Id = a.Id,
                        Title = a.Title,
                        Username = a.Username,
                        OrderStatus = a.OrderStatus,
                        CreatedAt = a.CreatedAt,
                        AcceptedDate = a.AcceptedDate,
                        CompletedDate = a.CompletedDate,
                        Longitude = a.Longitude,
                        Latitude = a.Latitude,
                        NeedProducts = a.NeedProducts == null ? new List<NeedProducts>().Select(b => new UserProfileNeedProductResponse()) : a.NeedProducts.Select(b => new UserProfileNeedProductResponse { Id = b.Id, ProductDescription = b.ProductDescription })
                    }),
                    PhoneNumber = user.PhoneNumber,
                    ProfileImage = user.ProfileImage,
                    NeedsCount = needs.ToList().Count > 0 ? needs.ToList().Count : 0
                };

                return Ok(new { IsSuccess = true, Result = response, Message = "User return a value!" });
            }


            if (user != null)
            {

                var needs = await _uow.Needs.WhereAsync(a => a.Username == user.UserName);

                var response = new UserDataResponse
                {
                    Name = user.Name,
                    Surname = user.Surname,
                    WhoAmI = user.WhoAmI,
                    Address = user.Address,
                    Age = user.Age,
                    Needs = needs.Select(a => new UserProfileNeedResponse
                    {
                        Id = a.Id,
                        Title = a.Title,
                        Username = a.Username,
                        OrderStatus = a.OrderStatus,
                        CreatedAt = a.CreatedAt,
                        AcceptedDate = a.AcceptedDate,
                        CompletedDate = a.CompletedDate,
                        Longitude = a.Longitude,
                        Latitude = a.Latitude,
                        NeedProducts = a.NeedProducts == null ? new List<NeedProducts>().Select(b => new UserProfileNeedProductResponse()) : a.NeedProducts.Select(b => new UserProfileNeedProductResponse { Id = b.Id, ProductDescription = b.ProductDescription })
                    }),
                    PhoneNumber = user.PhoneNumber,
                    ProfileImage = user.ProfileImage,
                    NeedsCount = needs.ToList().Count > 0 ? needs.ToList().Count : 0
                };

                return Ok(new { IsSuccess = true, Result = response, Message = "User return a value!" });
            }
            string message = "User is not defined!";

            var statusMessage = new StatusMessageResponseModel();
            statusMessage.IsError = true;
            statusMessage.Messages.Add(message);


            return Ok(new { IsSuccess = false, Result = statusMessage, Message = "User return null!" });



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


                return Ok(new { IsSuccess = true, Result = response, Message = "User updated! return a value!" });
            }


            string message = "User is not defined!";

            var statusMessage = new StatusMessageResponseModel();
            statusMessage.IsError = true;
            statusMessage.Messages.Add(message);


            return Ok(statusMessage);
        }



        [Route("LockoutUser")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> LockoutUser(LockoutUserViewModel model)
        {

            var user = await _userManager.FindByNameAsync(model.Username);

            if (user == null)
            {
                return Ok(new { IsSuccess = false, Result = "", Message = "User undefined!" });
            }


            user.LockoutEnabled = true;
            user.LockoutEnd = model.ExpireDate;
            user.IsActive = false;

            await _userManager.UpdateAsync(user);

            return Ok(new { IsSuccess = true, Result = new { NameSurname = $"{user.Name} {user.Surname}", LockoutEnd = user.LockoutEnd, UserActiveStatus = user.IsActive }, Message = "User updated! return a value!" });
        }


    }
}