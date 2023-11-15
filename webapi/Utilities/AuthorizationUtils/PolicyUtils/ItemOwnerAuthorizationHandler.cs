using Microsoft.AspNetCore.Authorization;
using webapi.Services.Interfaces;

namespace webapi.Utilities.AuthorizationUtils.PolicyUtils
{
    public class ItemOwnerAuthorizationHandler : AuthorizationHandler<ItemOwnerRequirement>
    {
        private readonly IItemService _itemService;
        private readonly IUserService _userService;

        public ItemOwnerAuthorizationHandler(IItemService itemService, IUserService userService)
        {
            _itemService = itemService;
            _userService = userService;
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

                var item = await _itemService.GetById(int.Parse(itemId!));

                if (item is null)
                {
                    context.Fail();
                    return;
                }

                var claimUsername = context.User.Claims.FirstOrDefault(c => c.Type == "username")?.Value;

                if (claimUsername is null)
                {
                    context.Fail();
                    return;
                }

                var user = await _userService.GetByUsername(claimUsername!);

                if (user is null)
                {
                    context.Fail();
                    return;
                }

                if (user!.Id == item!.SellerId)
                {
                    context.Succeed(requirement);
                    return;
                }
            }
            context.Fail();
        }
    }
}
