namespace Fanex.B2C.Ext.Service.Core.Models.DTO
{
    public class ApplicationOptions
    {
        public IList<string> WhiteList { get; set; }
        public string LogCategory { get; set; }
        public string SentryDsn { get; set; }
    }
}
