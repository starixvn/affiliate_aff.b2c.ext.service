using Microsoft.AspNetCore.Authorization;

namespace Fanex.B2C.Ext.Service.Core.Authorization
{
    public class LicenseeRequirement(string licenseeCode) : IAuthorizationRequirement
    {
        public string LicenseeCode { get; } = licenseeCode;
    }
}