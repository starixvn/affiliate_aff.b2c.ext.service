using System.Text.Json.Serialization;

namespace Fanex.B2C.Ext.Service.Core.Models.DTO
{
    public class DailyExchangeRate
    {
        [JsonPropertyName("ExchangeId")]
        public int ExchangeId { get; set; }

        [JsonPropertyName("ExchangeCode")]
        public string Code { get; set; }

        [JsonPropertyName("Currency")]
        public string Currency { get; set; }

        [JsonPropertyName("ExchangeRate")]
        public decimal Rate { get; set; }
    }
}
