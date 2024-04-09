using Fanex.B2C.Ext.Service.Core.Models.DTO;
using Fanex.B2C.Ext.Service.Core.Models.MetaModel;
using Fanex.Data.Repository;

namespace Fanex.B2C.Ext.Service.Core.Services
{
    public class ExchangeService(IDynamicRepository repository) : IExchangeService
    {
        public async Task<IEnumerable<DailyExchangeRate>> GetDailyExchangeRateAsync(DailyExchangeRateCriteria criteria) => await repository.FetchAsync<DailyExchangeRate>(criteria).ConfigureAwait(false);
    }
}
