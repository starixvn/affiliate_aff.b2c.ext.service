using Fanex.B2C.Ext.Service.Core.DTO;
using Fanex.B2C.Ext.Service.Core.Models.MetaModel;
using Fanex.Data;

namespace Fanex.B2C.Ext.Service.Core.Services
{
    public class AffililateInfoService : IAffililateInfoService
    {
        public AffInfo GetAffInfo(AffInfoParam param)
        {
            var UspName = $"Aff_API_AffiliateInfo_Sel_{param.LicenseeCode}";

            using (var db = ObjectDbFactory.CreateInstance(UspName))
            {
                db.SetParameterValue("@MonthDate", param.MonthDate);
                db.SetParameterValue("@Player_Username", param.PlayerName);
                db.SetParameterValue("@BaseCurrencyName", param.BaseCurrencyName);

                return db.QueryEntity<AffInfo>() ?? new AffInfo();
            }
        }
    }
}
