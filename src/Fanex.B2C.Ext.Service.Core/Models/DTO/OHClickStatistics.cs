namespace Fanex.B2C.Ext.Service.Core.Models.DTO
{
    public class OHClickStatistics : OHLinkGeneratorBase
    {
        // Click columns
        public long ClickId { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public DateTime? Date { get; set; }
        public string IP { get; set; }
        public string DomainReferrer { get; set; }
        public string URLReferrer { get; set; }
        public string RedirectURL { get; set; }
        public string Browser { get; set; }
        public string BrowserVersion { get; set; }
        public string BrowserLanguage { get; set; }
        public string OS { get; set; }
        public string Device { get; set; }
        public byte? DeviceType { get; set; }
        public string UserAgent { get; set; }
        public int? LinkId { get; set; }
        public int? SiteId { get; set; }
        public int? ClickTypeId { get; set; }

        // LinkGenerator columns (with Link prefix from SQL aliases)
        public string LinkNote { get; set; }
        public string LinkDomainName { get; set; }
        public int? LinkStatusId { get; set; }
        public string LinkCreator { get; set; }
        public DateTime? LinkCreatedDate { get; set; }
        public string LinkUpdatedBy { get; set; }
        public DateTime? LinkUpdatedDate { get; set; }
    }
}
