using Fanex.B2C.Ext.Service.Core.Models.DTO;
using Fanex.B2C.Ext.Service.Core.Models.Param;

namespace Fanex.B2C.Ext.Service.Core.Services
{
    public interface IExchangeService
    {
        Task<IEnumerable<DailyExchangeRate>> GetDailyExchangeRateAsync(DailyExchangeRateParam param);
        Task<IEnumerable<DailyExchangeRate>> GetDailyExchangeBidRateAsync(DailyExchangeRateParam param);
    }
}
