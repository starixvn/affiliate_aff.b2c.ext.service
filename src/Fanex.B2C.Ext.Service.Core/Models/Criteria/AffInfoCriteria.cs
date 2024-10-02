using Fanex.B2C.Ext.Service.Core.Models.Criteria;
using Fanex.Data;
using Fanex.Data.Repository;
using System.ComponentModel.DataAnnotations;

namespace Fanex.B2C.Ext.Service.Core.Models.MetaModel
{
    public class AffInfoCriteria : CriteriaBase, ILicenseeCode
    {
        [Required]
        [SpParam(SqlParamName = "Player_Username")]
        public string PlayerName { get; set; }

        [Range(typeof(DateTime), "2020-01-01", "2100-01-01", ErrorMessage = "The MonthDate field is invalid!")]
        public DateTime MonthDate { get; set; }

        [Required]
        public string BaseCurrencyName { get; set; }

        [SpParam(Ignored = true)]
        public string LicenseeCode { get; set; }

        public override string GetSettingKey() => $"{LicenseeCode}_Aff_API_AffiliateInfo_Sel";

        public override bool IsValid() => true;
    }
}
