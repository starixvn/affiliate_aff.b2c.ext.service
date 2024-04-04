using Fanex.B2C.Ext.Service.Core.Extensions;
using Fanex.B2C.Ext.Service.Core.Models.DTO;
using Microsoft.Extensions.Options;

namespace Fanex.B2C.Ext.Service.Core.Services
{
    public class UserManagementService(IOptions<List<User>> userListAccessor) : IUserManagementService
    {
        public bool IsValidUser(string username, string password)
            => userListAccessor.Value.Exists(x => x.UserName.Equals(username)
            && x.Password.Equals(password.Sha256()));
    }
}
