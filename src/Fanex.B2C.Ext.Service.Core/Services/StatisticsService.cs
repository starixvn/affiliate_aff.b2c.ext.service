using Fanex.B2C.Ext.Service.Core.Models.Criteria;
using Fanex.B2C.Ext.Service.Core.Models.DTO;
using Fanex.Data.Repository;

namespace Fanex.B2C.Ext.Service.Core.Services
{
    public class StatisticsService(IDynamicRepository repository) : IStatisticsService
    {
        public async Task<IEnumerable<AffMediaChannel>> GetAffMediaChannelAsync(AffMediaChannelCriteria criteria)
            => await repository.FetchAsync<AffMediaChannel>(criteria).ConfigureAwait(false);

        public async Task<IEnumerable<ClickStatistic>> GetClickStatisticAsync(ClickStatisticCriteria criteria)
            => await repository.FetchAsync<ClickStatistic>(criteria).ConfigureAwait(false);

        // OH Statistics endpoints
        public async Task<IEnumerable<OHCampaign>> GetOHCampaignAsync(OHCampaignCriteria criteria)
            => await repository.FetchAsync<OHCampaign>(criteria).ConfigureAwait(false);

        public async Task<IEnumerable<OHComponent>> GetOHComponentAsync(OHComponentCriteria criteria)
            => await repository.FetchAsync<OHComponent>(criteria).ConfigureAwait(false);

        public async Task<IEnumerable<OHLinkGenerator>> GetOHLinkGeneratorAsync(OHLinkGeneratorCriteria criteria)
            => await repository.FetchAsync<OHLinkGenerator>(criteria).ConfigureAwait(false);

        public async Task<IEnumerable<OHClickStatistics>> GetOHClickStatisticsAsync(OHClickStatisticsCriteria criteria)
            => await repository.FetchAsync<OHClickStatistics>(criteria).ConfigureAwait(false);
    }
}