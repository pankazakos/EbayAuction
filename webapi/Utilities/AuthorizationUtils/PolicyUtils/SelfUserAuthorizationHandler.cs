using Microsoft.AspNetCore.Authorization;
using webapi.Services;

namespace webapi.Utilities.AuthorizationUtils.PolicyUtils
{
    public class SelfUserAuthorizationHandler : AuthorizationHandler<SelfUserRequirement>
    {
        private readonly IUserService _userService;

        public SelfUserAuthorizationHandler(IUserService userService)
        {
            _userService = userService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SelfUserRequirement requirement)
        {
            if (context.Resource is DefaultHttpContext httpContext)
            {
                var userId = httpContext.Request.RouteValues["id"]?.ToString();

                if (string.IsNullOrEmpty(userId))
                {
                    context.Fail();
                    return;
                }

                var user = await _userService.GetById(int.Parse(userId));

                if (user is null)
                {
                    context.Fail();
                    return;
                }

                var claimUsername = context.User.Claims.FirstOrDefault(c => c.Type == "username")?.Value;

                if (user.Username == claimUsername)
                {
                    context.Succeed(requirement);
                    return;
                }
            }

            context.Fail();
        }
    }

}
