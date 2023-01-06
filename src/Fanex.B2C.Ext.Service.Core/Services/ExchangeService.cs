using Fanex.B2C.Ext.Service.Core.Models.DTO;
using Fanex.B2C.Ext.Service.Core.Models.MetaModel;
using Fanex.Data;

namespace Fanex.B2C.Ext.Service.Core.Services
{
    public class ExchangeService : IExchangeService
    {
        public IEnumerable<DailyExchangeRate> GetDailyExchangeRate(DailyExchangeRateParam param)
        {
            const string SpName = "Aff_API_ExchangeDailyOanda_Sel";

            using var db = ObjectDbFactory.CreateInstance(SpName);

            return db.Query<DailyExchangeRate>(param);
        }
    }
}
