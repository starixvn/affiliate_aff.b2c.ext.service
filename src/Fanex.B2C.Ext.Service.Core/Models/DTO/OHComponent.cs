namespace Fanex.B2C.Ext.Service.Core.Models.DTO
{
    public class OHComponent
    {
        public int ComponentId { get; set; }
        public string ComponentName { get; set; }
        public int ComponentTypeId { get; set; }
        public string ComponentTypeName { get; set; }
        public string ComponentCode { get; set; }
        public string Creator { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int StatusId { get; set; }
        public string CategoryType { get; set; }
    }
}

