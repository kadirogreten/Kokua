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

namespace Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserType
    {
        [Description("Volunteer")]
        Volunteer,
        [Description("Beneficiary")]
        Beneficiary
    }

    public class KokuaUser : MongoUser
    {
        public UserType UserType { get; set; }
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
