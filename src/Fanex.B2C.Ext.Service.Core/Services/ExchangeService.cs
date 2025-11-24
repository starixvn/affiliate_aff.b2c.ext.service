using Fanex.B2C.Ext.Service.Core.Models.Criteria;
using Fanex.B2C.Ext.Service.Core.Models.DTO;
using Fanex.B2C.Ext.Service.Core.Models.Param;
using Fanex.Data.Repository;

namespace Fanex.B2C.Ext.Service.Core.Services
{
    public class ExchangeService(IDynamicRepository repository) : IExchangeService
    {
        public async Task<IEnumerable<DailyExchangeRate>> GetDailyExchangeRateAsync(DailyExchangeRateParam param)
        {
            return await GetDailyExchangeRateAsync("Aff_API_ExchangeDailyOanda_Sel", param).ConfigureAwait(false);
        }

        public async Task<IEnumerable<DailyExchangeRate>> GetDailyExchangeBidRateAsync(DailyExchangeRateParam param)
        {
            return await GetDailyExchangeRateAsync("Aff_API_ExchangeDailyOanda_Bid_Sel", param).ConfigureAwait(false);
        }

        private async Task<IEnumerable<DailyExchangeRate>> GetDailyExchangeRateAsync(string spName, DailyExchangeRateParam param)
        {
            var criteria = new DailyExchangeRateCriteria(spName)
            {
                ExchangeIds = param.ExchangeIds,
                ExchangeDate = param.ExchangeDate,
            };

            return await repository.FetchAsync<DailyExchangeRate>(criteria).ConfigureAwait(false);
        }
    }
}
