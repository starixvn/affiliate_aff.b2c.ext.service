using Fanex.B2C.Ext.Service.Core.Constant;
using Fanex.Data;
using Fanex.Data.Repository;
using System.ComponentModel.DataAnnotations;

namespace Fanex.B2C.Ext.Service.Core.Models.Criteria
{
    public class ClickStatisticCriteria : CriteriaBase, ILicenseeCode
    {
        [Required]
        [SpParam(SqlParamName = "FromDate")]
        public DateTime FromDate { get; set; }

        [Required]
        [SpParam(SqlParamName = "ToDate")]
        public DateTime ToDate { get; set; }

        [Range(Pagination.MinPageIndex, Pagination.MaxPageIndex, ErrorMessage = "PageIndex must be between 1 and 1,000,000")]
        [SpParam(SqlParamName = "PageIndex")]
        public int PageIndex { get; set; } = 1;

        [Range(Pagination.MinPageSize, Pagination.MaxPageSizeClickStatistic, ErrorMessage = "PageSize must be between 1 and 1000")]
        [SpParam(SqlParamName = "PageSize")]
        public int PageSize { get; set; } = 1000;

        [SpParam(Ignored = true)]
        public string LicenseeCode { get; set; }

        public override string GetSettingKey() => $"{LicenseeCode}_Aff_API_ClickStatistic_Sel";

        public override bool IsValid() => true;
    }
}