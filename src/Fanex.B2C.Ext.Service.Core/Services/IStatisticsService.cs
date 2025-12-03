using Fanex.B2C.Ext.Service.Core.Models.Criteria;
using Fanex.B2C.Ext.Service.Core.Models.DTO;

namespace Fanex.B2C.Ext.Service.Core.Services
{
    public interface IStatisticsService
    {
        public Task<IEnumerable<AffMediaChannel>> GetAffMediaChannelAsync(AffMediaChannelCriteria criteria);
        public Task<IEnumerable<ClickStatistic>> GetClickStatisticAsync(ClickStatisticCriteria criteria);

        // OH Statistics endpoints
        public Task<IEnumerable<OHCampaign>> GetOHCampaignAsync(OHCampaignCriteria criteria);
        public Task<IEnumerable<OHComponent>> GetOHComponentAsync(OHComponentCriteria criteria);
        public Task<IEnumerable<OHLinkGenerator>> GetOHLinkGeneratorAsync(OHLinkGeneratorCriteria criteria);
        public Task<IEnumerable<OHClickStatistics>> GetOHClickStatisticsAsync(OHClickStatisticsCriteria criteria);
    }
}