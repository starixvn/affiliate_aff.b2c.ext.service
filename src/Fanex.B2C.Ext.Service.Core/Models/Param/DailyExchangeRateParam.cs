using System.ComponentModel.DataAnnotations;

namespace Fanex.B2C.Ext.Service.Core.Models.Param
{
    public class DailyExchangeRateParam
    {
        [Required]
        [RegularExpression("^[0-9,]+$", ErrorMessage = "The ExchangeIds field must be digits and separated by [Comma]!")]
        public string ExchangeIds { get; set; }

        [Range(typeof(DateTime), "2020-01-01", "2100-01-01", ErrorMessage = "The MonthDate field is invalid!")]
        public DateTime ExchangeDate { get; set; }
    }
}
