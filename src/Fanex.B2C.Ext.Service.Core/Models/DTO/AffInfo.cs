using System.Text.Json.Serialization;

namespace Fanex.B2C.Ext.Service.Core.Models.DTO
{
    public class AffInfo
    {
        private const int OneHundred = 100;

        [JsonIgnore]
        public string AffId { get; set; }

        [JsonPropertyName("AffiliateName")]
        public string AffName { get; set; }

        [JsonPropertyName("RevenueName")]
        public string GroupName { get; set; }

        [JsonPropertyName("TotalPlayers")]
        public int TotalPlayers { get; set; }

        [JsonIgnore]
        public decimal CommissionRate { get; set; }

        [JsonPropertyName("RevenueShare")]
        public decimal RevenueShare => CommissionRate * OneHundred;

        [JsonPropertyName("TotalEarning")]
        public decimal TotalEarning { get; set; }

        [JsonIgnore]
        public int ErrCode { get; set; }

        [JsonIgnore]
        public string ErrMsg { get; set; }
    }
}
