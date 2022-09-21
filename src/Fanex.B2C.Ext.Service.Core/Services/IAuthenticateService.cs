using Fanex.B2C.Ext.Service.Core.Models;

namespace Fanex.B2C.Ext.Service.Core.Services
{
    public interface IAuthenticateService
    {
        Tuple<bool, Token> IsAuthenticated(TokenRequest request);
    }
}
