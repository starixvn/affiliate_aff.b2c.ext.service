using System.Text.Json.Serialization;

namespace Fanex.B2C.Ext.Service.Core.Models.DTO
{
    public class AffDetailInfo
    {

        [JsonPropertyName("AffiliateCode")]
        public string AffCode { get; set; }

        [JsonPropertyName("ParentAffiliateName")]
        public string ParentAff { get; set; }

        [JsonPropertyName("AffiliateName")]
        public string UserName { get; set; }

        [JsonPropertyName("PlayerName")]
        public string Player_Username { get; set; }

        [JsonPropertyName("Currency")]
        public string CurrencyCode { get; set; }

        [JsonPropertyName("RegisteredDate")]
        public DateTime? RegisteredDate { get; set; }

        [JsonPropertyName("LastLoginDate")]
        public DateTime? LastLoginDate { get; set; }

        [JsonPropertyName("LastLoginIP")]
        public string LastLoginIP { get; set; }
    }
}
