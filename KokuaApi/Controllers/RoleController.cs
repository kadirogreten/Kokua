using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
    [Authorize]
    [EnableCors("MyPolicy")]
    public class RoleController : ControllerBase
    {

        private readonly IUnitOfWork _uow;
        private readonly UserManager<KokuaUser> _userManager;
        private readonly SignInManager<KokuaUser> _signInManager;
        private readonly ILogger _logger;
        private readonly RoleManager<KokuaRole> _roleManager;

        public RoleController(UserManager<KokuaUser> userManager, RoleManager<KokuaRole> roleManager, SignInManager<KokuaUser> signInManager, ILogger<AccountController> logger, IUnitOfWork uow)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _uow = uow;
            _roleManager = roleManager;
        }


        [Route("AddToRole")]
        [HttpPost]
        public async Task<StatusMessageResponseModel> AddToRole(string roleName, string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var role = new KokuaRole();

                role.Name = roleName;
                await _roleManager.CreateAsync(role);
            }

            var status = new StatusMessageResponseModel();


            if (user == null) 
            {
                status.IsError = true;

                if (status.Messages == null)
                {
                    status.Messages = new List<string>();

                    status.Messages.Add("User is empty!");

                    return status;
                }

                
            }

            

            await _userManager.AddToRoleAsync(user, roleName);
            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, roleName));

            status.IsError = false;
            if (status.Messages == null)
            {
                status.Messages = new List<string>();
                status.Messages.Add($"User is {roleName} now!");
            }

            return status;
        }
    }
}