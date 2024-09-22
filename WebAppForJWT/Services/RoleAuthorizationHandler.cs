using Microsoft.AspNetCore.Authorization;

namespace WebAppForJWT.Services
{
    public class RoleAuthorizationHandler : AuthorizationHandler<RoleRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
        {
            var jwtToken = context.User.Claims.FirstOrDefault(c => c.Type == "UserRole"); //см AythRegController как там назван Claim: new Claim("UserRole", user.Role)
            if (jwtToken != null && jwtToken.Value == requirement.Role)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
