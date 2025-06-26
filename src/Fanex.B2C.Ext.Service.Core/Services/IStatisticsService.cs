using Fanex.B2C.Ext.Service.Core.Models.DTO;
using Fanex.B2C.Ext.Service.Core.Models.Criteria;

namespace Fanex.B2C.Ext.Service.Core.Services
{
    public interface IStatisticsService
    {
        public Task<IEnumerable<AffMediaChannel>> GetAffMediaChannelAsync(AffMediaChannelCriteria criteria);
        public Task<IEnumerable<ClickStatistic>> GetClickStatisticAsync(ClickStatisticCriteria criteria);
    }
} 