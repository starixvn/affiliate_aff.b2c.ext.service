namespace Fanex.B2C.Ext.Service.Core.Models.DTO
{
    public class OHCampaign
    {
        public int CampaignId { get; set; }
        public string CampaignName { get; set; }
        public string CampaignCode { get; set; }
        public string Description { get; set; }
        public string Creator { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int StatusId { get; set; }
    }
}

