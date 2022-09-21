using Fanex.B2C.Ext.Service.Core.DTO;
using Fanex.B2C.Ext.Service.Core.Models.MetaModel;

namespace Fanex.B2C.Ext.Service.Core.Services
{
    public interface IAffililateInfoService
    {
        public AffInfo GetAffInfo(AffInfoParam param);
    }
}
