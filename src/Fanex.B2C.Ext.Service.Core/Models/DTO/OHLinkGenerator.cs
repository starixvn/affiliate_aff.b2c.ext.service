namespace Fanex.B2C.Ext.Service.Core.Models.DTO
{
    public class OHLinkGenerator : OHLinkGeneratorBase
    {
        public int LinkId { get; set; }
        public string Note { get; set; }
        public string DomainName { get; set; }
        public int? SiteId { get; set; }
        public int StatusId { get; set; }
        public string Creator { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? Date { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
