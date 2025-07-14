using Fanex.B2C.Ext.Service.Core.Models.DTO;
using Fanex.B2C.Ext.Service.Core.Models.Criteria;
using Fanex.Data.Repository;

namespace Fanex.B2C.Ext.Service.Core.Services
{
    public class StatisticsService(IDynamicRepository repository) : IStatisticsService
    {
        public async Task<IEnumerable<AffMediaChannel>> GetAffMediaChannelAsync(AffMediaChannelCriteria criteria) => await repository.FetchAsync<AffMediaChannel>(criteria).ConfigureAwait(false);

        public async Task<IEnumerable<ClickStatistic>> GetClickStatisticAsync(ClickStatisticCriteria criteria) => await repository.FetchAsync<ClickStatistic>(criteria).ConfigureAwait(false);
    }
} 