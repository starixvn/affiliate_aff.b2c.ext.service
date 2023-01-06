using Fanex.B2C.Ext.Service.Core.Models.DTO;
using Fanex.B2C.Ext.Service.Core.Models.MetaModel;
using Fanex.Data;

namespace Fanex.B2C.Ext.Service.Core.Services
{
    public class AffililateInfoService : IAffililateInfoService
    {
        public AffInfo GetAffInfo(AffInfoParam param)
        {
            var spName = $"{param.LicenseeCode}_Aff_API_AffiliateInfo_Sel";

            using var db = ObjectDbFactory.CreateInstance(spName);
            db.SetParameterValue("@MonthDate", param.MonthDate);
            db.SetParameterValue("@Player_Username", param.PlayerName);
            db.SetParameterValue("@BaseCurrencyName", param.BaseCurrencyName);

            return db.QueryEntity<AffInfo>();
        }

        public IEnumerable<AffDetailInfo> GetAffDetailInfoList(AffDetailInfoParam param)
        {
            var spName = $"{param.LicenseeCode}_Aff_API_AffiliateDetailInfo_Sel";

            using var db = ObjectDbFactory.CreateInstance(spName);

            return db.Query<AffDetailInfo>(param);
        }
    }
}
