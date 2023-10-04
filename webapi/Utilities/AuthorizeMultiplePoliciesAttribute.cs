using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace webapi.Utilities
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class AuthorizeMultiplePoliciesAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        private readonly string[] _policies;

        public AuthorizeMultiplePoliciesAttribute(params string[] policies)
        {
            _policies = policies;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var authorizationService = context.HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();

            foreach (var policy in _policies)
            {
                var authorized = await authorizationService.AuthorizeAsync(context.HttpContext.User, context.HttpContext, policy);
                if (authorized.Succeeded)
                {
                    return; // At least one policy was met
                }
            }

            // None of the policies were met
            context.Result = new ForbidResult();
        }
    }


}
