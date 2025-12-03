using Fanex.B2C.Ext.Service.Core.Constant;
using Fanex.Data;
using Fanex.Data.Repository;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Fanex.B2C.Ext.Service.Core.Models.Criteria
{
    public abstract class OHStatisticsCriteriaBase : CriteriaBase
    {
        [Required]
        [SpParam(SqlParamName = "FromDate")]
        public DateTime FromDate { get; set; }

        [Required]
        [SpParam(SqlParamName = "ToDate")]
        public DateTime ToDate { get; set; }

        [DefaultValue(Pagination.MinPageIndex)]
        [Range(Pagination.MinPageIndex, Pagination.MaxPageIndex, ErrorMessage = "PageIndex must be between 1 and 1,000,000")]
        [SpParam(SqlParamName = "PageIndex")]
        public int PageIndex { get; set; }

        [DefaultValue(Pagination.MaxPageSizeClickStatistic)]
        [Range(Pagination.MinPageSize, Pagination.MaxPageSizeClickStatistic, ErrorMessage = "PageSize must be between 1 and 1000")]
        [SpParam(SqlParamName = "PageSize")]
        public int PageSize { get; set; }

        public override bool IsValid() => true;
    }

    public class OHCampaignCriteria : OHStatisticsCriteriaBase
    {
        public override string GetSettingKey() => "OH_API_Campaign_Sel";
    }

    public class OHComponentCriteria : OHStatisticsCriteriaBase
    {
        public override string GetSettingKey() => "OH_API_Component_Sel";
    }

    public class OHLinkGeneratorCriteria : OHStatisticsCriteriaBase
    {
        public override string GetSettingKey() => "OH_API_LinkGenerator_Sel";
    }

    public class OHClickStatisticsCriteria : OHStatisticsCriteriaBase
    {
        public override string GetSettingKey() => "OH_API_ClickStatistics_Sel";
    }
}

