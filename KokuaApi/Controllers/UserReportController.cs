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
    public class UserReportController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly UserManager<KokuaUser> _userManager;
        private readonly SignInManager<KokuaUser> _signInManager;
        private readonly ILogger<UserReportController> _logger;

        public UserReportController(UserManager<KokuaUser> userManager, SignInManager<KokuaUser> signInManager, ILogger<UserReportController> logger, IUnitOfWork uow)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _uow = uow;
        }



        [Authorize]
        [Route("ReportUser")]
        [HttpPost]
        [EnableCors("MyPolicy")]
        public async Task<IActionResult> ReportUser(UserReportViewModel model)
        {


            var username = HttpContext.User.Identity.Name;

            var requestProfile = await _userManager.FindByNameAsync(model.ReportToUsername);
            var beforeReported = _uow.UserReport
                .Where(a => ((a.ReportDoUsername == username && a.ReportToUsername == requestProfile.UserName)) && a.ReportStatus != ReportStatus.Completed).Any();
            if (beforeReported)
            {
                return Ok(new { IsSuccess = false, Result = "", Message = "You have reported this person before" });
            }

            try
            {
                UserReport report = new UserReport
                {
                    ReportDoUsername = username,
                    ReportToUsername = requestProfile.UserName,
                    ReportType = model.ReportType,
                    ReportSubject = model.ReportSubject,
                    Detail = model.Detail,
                    ReportStatus = ReportStatus.New
                };
                _uow.UserReport.Insert(report);
                await _uow.Complete();



                return Ok(new { IsSuccess = true, Result = report, Message = "User reported successfully!" });

            }
            catch (Exception ex)
            {
                //throw ex;
                return Ok(new { IsSuccess = false, Result = "", Message = "Unexpected Errors!" });
            }


        }


        [Authorize(Roles = "Admin")]
        [Route("GetUserReports")]
        [HttpGet]
        [EnableCors("MyPolicy")]
        public async Task<IActionResult> GetUserReports()
        {

            var response = await _uow.UserReport.WhereAsync(a => a.ReportStatus != ReportStatus.Completed);

            return Ok(new { IsSuccess = true, Result = response, Message = "User reports fetching successfully!" });

        }

        [Authorize(Roles = "Admin")]
        [Route("ChangeReportStatus")]
        [HttpPost]
        [EnableCors("MyPolicy")]
        public async Task<IActionResult> ChangeReportStatus(ChangeUserReportStatusViewModel model)
        {

            var response = await _uow.UserReport.FindAsync(a => a.ID == model.ReportId);

            if (response == null)
            {
                return Ok(new { IsSuccess = false, Result = "", Message = "Unexpected Errors!" });
            }

            response.ReportStatus = model.ReportStatus;

            _uow.UserReport.Update(response);
            await _uow.Complete();

            return Ok(new { IsSuccess = true, Result = response, Message = "User reports fetching successfully!" });

        }


    }
}