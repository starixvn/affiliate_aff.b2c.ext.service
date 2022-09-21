using System.ComponentModel.DataAnnotations;

namespace Fanex.B2C.Ext.Service.Core.Models.MetaModel
{
    public class AffInfoParam
    {
        [Required]
        public string PlayerName { get; set; }

        public DateTime MonthDate { get; set; }

        public string BaseCurrencyName { get; set; }

        public string LicenseeCode { get; set; }

    }
}
