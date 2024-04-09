using Fanex.B2C.Ext.Service.Core.Models.DTO;
using Fanex.B2C.Ext.Service.Core.Models.MetaModel;

namespace Fanex.B2C.Ext.Service.Core.Services
{
    public interface IExchangeService
    {
        Task<IEnumerable<DailyExchangeRate>> GetDailyExchangeRateAsync(DailyExchangeRateCriteria criteria);
    }
}
