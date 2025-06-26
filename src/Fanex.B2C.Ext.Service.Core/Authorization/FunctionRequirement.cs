using Microsoft.AspNetCore.Authorization;

namespace Fanex.B2C.Ext.Service.Core.Authorization
{
    public class FunctionRequirement(string functionName) : IAuthorizationRequirement
    {
        public string FunctionName { get; } = functionName;
    }
} 