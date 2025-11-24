using Fanex.B2C.Ext.Service.Core.Models.Criteria;
using Fanex.B2C.Ext.Service.Core.Models.DTO;

namespace Fanex.B2C.Ext.Service.Core.Services
{
    public interface IStatisticsService
    {
        public Task<IEnumerable<AffMediaChannel>> GetAffMediaChannelAsync(AffMediaChannelCriteria criteria);
        public Task<IEnumerable<ClickStatistic>> GetClickStatisticAsync(ClickStatisticCriteria criteria);
    }
}