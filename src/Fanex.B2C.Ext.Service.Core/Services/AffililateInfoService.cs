using Fanex.B2C.Ext.Service.Core.Models.DTO;
using Fanex.B2C.Ext.Service.Core.Models.MetaModel;
using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.B2C.Ext.Service.Core.Services
{
    public class AffililateInfoService(IDynamicRepository repository) : IAffililateInfoService
    {
        public async Task<AffInfo> GetAffInfoAsync(AffInfoCriteria criteria) => await repository.GetAsync<AffInfo>(criteria).ConfigureAwait(false);

        public async Task<IEnumerable<AffDetailInfo>> GetAffDetailInfoListAsync(AffDetailInfoCriteria criteria) => await repository.FetchAsync<AffDetailInfo>(criteria).ConfigureAwait(false);
    }
}
