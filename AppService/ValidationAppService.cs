using Microsis.CWM.Dto.Basic;
using Microsis.CWM.Dto.Wholesale;

namespace Microsis.CWM.AppService
{
    public interface IValidationAppService
    {
        List<Error> WholesaleNew(WholesaleNewRequest obj);
    }

    public class ValidationAppService : IValidationAppService
    {

        public ValidationAppService()
        {

        }
        #region ______ Wholesale ______
        public List<Error> WholesaleNew(WholesaleNewRequest obj)
        {
            return null;
        }
        #endregion
    }
}
