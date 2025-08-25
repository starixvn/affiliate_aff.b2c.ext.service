using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.B2C.Ext.Service.Core.Models.Criteria
{
    public class AffMediaChannelCriteria : CriteriaBase, ILicenseeCode
    {
        [SpParam(Ignored = true)]
        public string LicenseeCode { get; set; }

        public override string GetSettingKey() => $"{LicenseeCode}_Aff_API_AffMediaChannel_Sel";

        public override bool IsValid() => true;
    }
}