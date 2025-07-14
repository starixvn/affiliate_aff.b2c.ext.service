using Fanex.B2C.Ext.Service.Core.Models.DTO;
using Fanex.B2C.Ext.Service.Core.Models.Criteria;

namespace Fanex.B2C.Ext.Service.Core.Services
{
    public interface IAffililateInfoService
    {
        public Task<AffInfo> GetAffInfoAsync(AffInfoCriteria criteria);
        public Task<IEnumerable<AffDetailInfo>> GetAffDetailInfoListAsync(AffDetailInfoCriteria criteria);
    }
}
