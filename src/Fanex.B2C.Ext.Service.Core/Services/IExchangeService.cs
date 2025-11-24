using Fanex.B2C.Ext.Service.Core.Models.Criteria;
using Fanex.B2C.Ext.Service.Core.Models.DTO;

namespace Fanex.B2C.Ext.Service.Core.Services
{
    public interface IExchangeService
    {
        Task<IEnumerable<DailyExchangeRate>> GetDailyExchangeRateAsync(DailyExchangeRateCriteria criteria);
    }
}
