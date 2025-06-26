using Fanex.Data.Repository;
using System.ComponentModel.DataAnnotations;

namespace Fanex.B2C.Ext.Service.Core.Models.Criteria
{
    public class DailyExchangeRateCriteria : CriteriaBase
    {
        [Required]
        [RegularExpression("^[0-9,]+$", ErrorMessage = "The ExchangeIds field must be digits and separated by [Comma]!")]
        public string ExchangeIds { get; set; }

        [Range(typeof(DateTime), "2020-01-01", "2100-01-01", ErrorMessage = "The MonthDate field is invalid!")]
        public DateTime ExchangeDate { get; set; }

        public override string GetSettingKey() => "Aff_API_ExchangeDailyOanda_Sel";

        public override bool IsValid() => true;
    }
}
