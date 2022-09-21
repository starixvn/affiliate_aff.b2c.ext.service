using System;
using System.Collections.Generic;
using System.Text;

namespace Fanex.B2C.Ext.Service.Core.Services
{
   public interface IUserManagementService
    {
        bool IsValidUser(string username, string password);
    }
}
