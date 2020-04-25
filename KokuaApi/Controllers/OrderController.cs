using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class OrderController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly UserManager<KokuaUser> _userManager;
        private readonly SignInManager<KokuaUser> _signInManager;
        private readonly ILogger<OrderController> _logger;

        public OrderController(UserManager<KokuaUser> userManager, SignInManager<KokuaUser> signInManager, ILogger<OrderController> logger, IUnitOfWork uow)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _uow = uow;
        }


        [Route("GetMyOrders")]
        [Authorize]
        [HttpPost]
        [EnableCors("MyPolicy")]
        public async Task<IActionResult> GetMyOrders()
        {

            var username = HttpContext.User.Identity.Name;

            var user = await _userManager.FindByNameAsync(username);
            if (user.UserType == UserType.Volunteer)
            {
                var orders = await _uow.Order.WhereAsync(a => a.OrderStatus == OrderStatus.Completed && a.RequestName == username);
                return Ok(orders);
            }
            else
            {
                var orders = await _uow.Order.WhereAsync(a => a.OrderStatus == OrderStatus.Completed && a.Username == username);
                return Ok(orders);
            }
            
        }



    }
}