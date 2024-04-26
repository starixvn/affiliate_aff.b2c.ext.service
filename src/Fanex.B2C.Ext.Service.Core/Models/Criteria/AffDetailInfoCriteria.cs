using Fanex.B2C.Ext.Service.Core.Models.Criteria;
using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.B2C.Ext.Service.Core.Models.MetaModel
{
    public class AffDetailInfoCriteria : CriteriaBase, ILicenseeCode
    {
        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 2000;

        [SpParam(Ignored = true)]
        public string LicenseeCode { get; set; }

        public override string GetSettingKey() => $"{LicenseeCode}_Aff_API_AffiliateDetailInfo_Sel";

        public override bool IsValid() => true;
    }
}
