using Fanex.B2C.Ext.Service.Core.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Fanex.B2C.Ext.Service.Core.Authorization
{
    public class FunctionAuthorizationHandler(IOptions<List<User>> userListAccessor) : AuthorizationHandler<FunctionRequirement>
    {
        private readonly IOptions<List<User>> _userListAccessor = userListAccessor;

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            FunctionRequirement requirement)
        {
            var username = context.User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var userInfo = _userListAccessor.Value.Find(x => x.UserName.Equals(username));
            if (userInfo?.SupportedFunctions == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var hasRequiredFunction = userInfo.SupportedFunctions.Exists(func => 
                string.Equals(func, requirement.FunctionName, StringComparison.OrdinalIgnoreCase));

            if (hasRequiredFunction)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
} 