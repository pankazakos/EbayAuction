using Microsoft.AspNetCore.Authorization;
using webapi.Models;
using webapi.Services;

namespace webapi.Utilities.AuthorizationUtils.PolicyUtils
{
    public class ItemOwnerAuthorizationHandler : AuthorizationHandler<ItemOwnerRequirement>
    {
        private readonly IItemService _itemService;

        public ItemOwnerAuthorizationHandler(IItemService itemService)
        {
            _itemService = itemService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ItemOwnerRequirement requirement)
        {
            if (context.Resource is DefaultHttpContext httpContext)
            {
                var itemId = httpContext.Request.RouteValues["id"]?.ToString();

                if (string.IsNullOrEmpty(itemId))
                {
                    context.Fail();
                    return;
                }

                var item = await _itemService.GetById(int.Parse(itemId));

                if (item is null)
                {
                    context.Fail();
                    return;
                }

                var claimUsername = context.User.Claims.FirstOrDefault(c => c.Type == "username")?.Value;

                if (claimUsername == item.Seller.Username)
                {
                    context.Succeed(requirement);
                    return;
                }
            }
            context.Fail();
        }
    }
}
