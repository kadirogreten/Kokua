using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using AspNetCore.Identity.MongoDbCore.Models;
using AspNetCore.Identity.Mongo.Model;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Text.Json.Serialization;
using Models;

namespace Models
{
    

    public class KokuaUser : MongoUser
    {
        public string WhoAmI { get; set; }
        public UserType UserType { get; set; }
        public IEnumerable<Needs> Needs { get; set; }
        public IEnumerable<Order> Orders { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime? Age { get; set; }
        public string Address { get; set; }
        public string ProfileImage { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<KokuaUser>
    {
        public CustomClaimsPrincipalFactory(UserManager<KokuaUser> userManager,
            IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor) { }

        public async override Task<ClaimsPrincipal> CreateAsync(KokuaUser user)
        {
            var principal = await base.CreateAsync(user);

            // Add your claims here
            ((ClaimsIdentity)principal.Identity).AddClaims(
                new[] {
                    new Claim (ClaimTypes.Name, user.UserName),
                        new Claim (CustomClaimTypes.UserId, user.Id.ToString ())
                });

            return principal;
        }
    }

    public static class CustomClaimTypes
    {
        public const string UserId = "UserId";
    }
}
