using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using PaymentSystem.Domain.Entities;

namespace PaymentSystem.Infrastructure.Identity
{
    public class AdditionalUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<AppUser>
    {
        private readonly UserManager<AppUser> _userManager;

        public AdditionalUserClaimsPrincipalFactory(
            UserManager<AppUser> userManager,
            IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor)
        {
            _userManager = userManager;
        }

        public override async Task<ClaimsPrincipal> CreateAsync(AppUser user)
        {
            var principal = await base.CreateAsync(user);
            var identity = (ClaimsIdentity)principal.Identity!;

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                if (role is "Admins" or "SecondAdmins" or "HelperAdmins" or "Users")
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, role));
                }
            }

            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
            identity.AddClaim(new Claim("UserId", user.Id));

            return principal;
        }
    }
}
