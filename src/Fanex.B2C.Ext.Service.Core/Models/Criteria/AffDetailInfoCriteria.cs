using Fanex.Data;
using Fanex.Data.Repository;
using System.ComponentModel.DataAnnotations;

namespace Fanex.B2C.Ext.Service.Core.Models.Criteria
{
    public class AffDetailInfoCriteria : CriteriaBase, ILicenseeCode
    {
        [Range(1, int.MaxValue, ErrorMessage = "PageIndex must be at least 1")]
        public int PageIndex { get; set; } = 1;

        [Range(1, 2000, ErrorMessage = "PageSize must be between 1 and 2000")]
        public int PageSize { get; set; } = 2000;

        [SpParam(Ignored = true)]
        public string LicenseeCode { get; set; }

        public override string GetSettingKey() => $"{LicenseeCode}_Aff_API_AffiliateDetailInfo_Sel";

        public override bool IsValid() => true;
    }
}
