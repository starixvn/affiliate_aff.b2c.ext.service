namespace Fanex.B2C.Ext.Service.Core.Models.DTO
{
    public class User
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public List<string> SupportedLicensees { get; set; }
    }
}
