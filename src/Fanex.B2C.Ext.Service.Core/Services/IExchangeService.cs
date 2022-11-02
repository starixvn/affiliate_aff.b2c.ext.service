using Fanex.B2C.Ext.Service.Core.Models.DTO;
using Fanex.B2C.Ext.Service.Core.Models.MetaModel;

namespace Fanex.B2C.Ext.Service.Core.Services
{
    public interface IExchangeService
    {
        IEnumerable<DailyExchangeRate> GetDailyExchangeRate(DailyExchangeRateParam param);
    }
}
