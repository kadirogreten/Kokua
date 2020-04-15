using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KokuaApi.Models;
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
    public class NeedsController : ControllerBase
    {


        private readonly IUnitOfWork _uow;
        private readonly UserManager<KokuaUser> _userManager;
        private readonly SignInManager<KokuaUser> _signInManager;
        private readonly ILogger _logger;

        public NeedsController(UserManager<KokuaUser> userManager, SignInManager<KokuaUser> signInManager, ILogger<NeedsController> logger, IUnitOfWork uow)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _uow = uow;
        }



        [Route("Create-Needs")]
        //[authorize(roles = "beneficiary")]
        [HttpPost]
        [EnableCors("MyPolicy")]
        public async Task<IActionResult> InsertNeed(Needs model)
        {

            if (!ModelState.IsValid)
            {
                return Ok();
            }

            var username = HttpContext.User.Identity.Name;

            if (model != null)
            {

                var needsList = await _uow.Needs.WhereAsync(a => a.Title == model.Title && a.Username == username);

                if (needsList.ToList().Count > 0)
                {
                    return Ok(new { IsError = true, Message = "This title already taken!" });
                }
                var now = DateTime.UtcNow;
                model.CreatedAt = now;
                model.Username = username;

                _uow.Needs.Insert(model);

                foreach (var product in model.NeedProducts)
                {
                    _uow.NeedProducts.Insert(product);

                }
            }
            await _uow.Complete();

            return Ok();

        }

        [Route("Get-Needs")]
        //[Authorize(Roles = "Volunteer")]
        [HttpGet]
        [EnableCors("MyPolicy")]
        public async Task<IActionResult> GetBeneficiaryNeed()
        {
            var x = OrderStatus.Waiting;
            var needsList = await _uow.Needs.WhereAsync(a => a.OrderStatus == OrderStatus.Waiting);
            IList<VolunteerNeedsResponse> response = new List<VolunteerNeedsResponse>();

            if (!needsList.Any())
            {
                return Ok(new { IsSuccess = false, Result = response, Message = "Need list return empty value!" });
            }


            foreach (var item in needsList)
            {
                var data = new VolunteerNeedsResponse
                {
                    Title = item.Title,
                    OrderStatus = item.OrderStatus,
                    BeneficiaryUsername = item.Username,
                    CreatedAt = item.CreatedAt,
                    NeedProducts = item.NeedProducts
                };

                response.Add(data);
            }



            return Ok(new { IsSuccess = true, Result = response, Message = "Need list return value!" });

        }

        [Route("Take-Needs")]
        //[authorize(roles = "volunteer")]
        [HttpPost]
        [EnableCors("MyPolicy")]
        public async Task<IActionResult> InsertNeed(TakeNeedsResponse takenNeed)
        {

            if (!ModelState.IsValid)
            {
                return Ok();
            }

            var username = HttpContext.User.Identity.Name;

            if (takenNeed != null)
            {

                var need = await _uow.Needs.WhereAsync(a => a.Id == takenNeed.Id);

                var user = await _userManager.FindByNameAsync(takenNeed.VolunteerUsername);
                if (user != null && need != null) { 
                
                
                }

            }
            await _uow.Complete();

            return Ok();

        }


    }
}