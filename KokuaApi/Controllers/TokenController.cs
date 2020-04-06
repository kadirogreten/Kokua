using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using KokuaApi.Helpers;
using KokuaApi.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models;
using ServiceLayer;

namespace KokuaApi.Controllers
{
    [Route("api/")]
    [ApiController]
    [EnableCors("MyPolicy")]
    public class TokenController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly UserManager<KokuaUser> _userManager;
        private readonly RoleManager<KokuaRole> _roleManager;

        public TokenController(IUnitOfWork uow, UserManager<KokuaUser> userManager, RoleManager<KokuaRole> roleManager)
        {
            this._uow = uow;
            this._userManager = userManager;
            this._roleManager = roleManager;
        }


        [Route("Token")]
        [HttpPost]
        public async Task<IActionResult> Token(LoginViewModel model)
        {


            if (model.Grant_Type != "password")
            {
                return BadRequest(new { Error = "Invalid grant type!" });
            }

            if (await IsUsernameAndPassword(model.Email, model.Password))
            {
                return new ObjectResult(await GenerateToken(model.Email));
            }
            else
            {
                return BadRequest(new { Error = "Username password is wrong!" });
            }
        }

        private async Task<bool> IsUsernameAndPassword(string username, string password)
        {
            var user = await this._userManager.FindByEmailAsync(username);

            return await this._userManager.CheckPasswordAsync(user, password);
        }


        private async Task<dynamic> GenerateToken(string username)
        {

            var user = await _userManager.FindByNameAsync(username);

            var userRole = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid ().ToString ()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name,username),
                new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds().ToString())
                };

            RoleExtension.AddRolesToClaims(claims, userRole);

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("OğretenAcademyOğrencileriYapti!"));

            var token = new JwtSecurityToken(
                issuer: "https://localhost:44349/",
                audience: "https://localhost:44349/",
                expires: DateTime.Now.AddDays(1),
                claims: claims,
                signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );


            var response = new
            {
                Access_Token = new JwtSecurityTokenHandler().WriteToken(token),
                UserName = username,
                UserId = user.Id,
                Exp = token.ValidFrom,
                Exp_End = token.ValidTo,
                UserType = user.UserType
            };

            return response;

        }

    }
}