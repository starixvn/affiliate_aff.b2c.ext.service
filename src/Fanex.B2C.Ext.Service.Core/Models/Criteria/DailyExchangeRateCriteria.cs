using Fanex.Data.Repository;

namespace Fanex.B2C.Ext.Service.Core.Models.Criteria
{
    public class DailyExchangeRateCriteria(string spName) : CriteriaBase
    {
        private readonly string _spName = spName;
        public override string GetSettingKey() => _spName;

        public string ExchangeIds { get; set; }

        public DateTime ExchangeDate { get; set; }

        public override bool IsValid() => !string.IsNullOrEmpty(_spName);
    }
}
