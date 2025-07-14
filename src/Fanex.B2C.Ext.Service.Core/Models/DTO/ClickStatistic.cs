namespace Fanex.B2C.Ext.Service.Core.Models.DTO
{
    public class ClickStatistic
    {
        public long ClickId { get; set; }
        public int AffId { get; set; }
        public string AffCode { get; set; }
        public string AffUserName { get; set; }
        public DateTime UpdatedTime { get; set; }
        public string IP { get; set; }
        public DateTime Date { get; set; }
        public string Browser { get; set; }
        public string URLReferrer { get; set; }
        public string DomainReferrer { get; set; }
        public string OS { get; set; }
        public string Device { get; set; }
        public string BrowserVersion { get; set; }
        public string BrowserResolution { get; set; }
        public string BrowserLanguage { get; set; }
        public string UserAgent { get; set; }
        public string Country { get; set; }
        public string LanguageCode { get; set; }
        public string SportCode { get; set; }
        public string RedirectURL { get; set; }
        public byte? Status { get; set; }
        public byte? DeviceType { get; set; }
        public string AffMediaLink { get; set; }
        public string ChannelCode { get; set; }
    }
} 