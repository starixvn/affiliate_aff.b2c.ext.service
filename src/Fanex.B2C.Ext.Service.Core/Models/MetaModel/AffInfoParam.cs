using System.ComponentModel.DataAnnotations;

namespace Fanex.B2C.Ext.Service.Core.Models.MetaModel
{
    public class AffInfoParam : LicenseeParam
    {
        [Required]
        public string PlayerName { get; set; }

        [Range(typeof(DateTime), "2020-01-01", "2100-01-01", ErrorMessage = "The MonthDate field is invalid!")]
        public DateTime MonthDate { get; set; }

        [Required]
        public string BaseCurrencyName { get; set; }
    }
}
