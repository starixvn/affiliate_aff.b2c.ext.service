namespace Fanex.B2C.Ext.Service.Core.Models.DTO
{
    /// <summary>
    /// Base class containing shared LinkGenerator properties used by both OHLinkGenerator and OHClickStatistics.
    /// </summary>
    public abstract class OHLinkGeneratorBase
    {
        public string LinkName { get; set; }
        public string IDCode { get; set; }
        public int? CampaignId { get; set; }
        public string CountryCode { get; set; }
        public string MarketId { get; set; }
        public string EventIds { get; set; }
        public int? GroupId { get; set; }
        public int? ChannelId { get; set; }
        public int? AgencyId { get; set; }
        public int? TypeId { get; set; }
        public string PageCode { get; set; }
        public string PageType { get; set; }
        public string ShortDomain { get; set; }
        public string ShortPath { get; set; }
        public string FullLink { get; set; }
    }
}

