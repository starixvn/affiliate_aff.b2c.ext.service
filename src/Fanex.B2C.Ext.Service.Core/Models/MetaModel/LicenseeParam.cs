using Fanex.B2C.Ext.Service.Core.Constant;
using Fanex.Data;

namespace Fanex.B2C.Ext.Service.Core.Models.MetaModel
{
    public class LicenseeParam
    {
        [SpParam(Ignored = true)]
        public string LicenseeCode { get; set; } = Licensee.LicenseeDefault;
    }
}
