using Fanex.B2C.Ext.Service.Core.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Fanex.B2C.Ext.Service.Core.Authorization
{
    public class LicenseeAuthorizationHandler(IOptions<List<User>> userListAccessor) : AuthorizationHandler<LicenseeRequirement>
    {
        private readonly IOptions<List<User>> _userListAccessor = userListAccessor;

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            LicenseeRequirement requirement)
        {
            var username = context.User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var userInfo = _userListAccessor.Value.Find(x => x.UserName.Equals(username));
            if (userInfo?.SupportedLicensees == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var isValidLicensee = userInfo.SupportedLicensees.Exists(licen =>
                string.Equals(requirement.LicenseeCode, licen, StringComparison.OrdinalIgnoreCase));

            if (isValidLicensee)
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