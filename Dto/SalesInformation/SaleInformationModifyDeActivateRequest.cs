using System.ComponentModel.DataAnnotations;

namespace Microsis.CWM.Dto.SalesInformation
{
    public class SaleInformationModifyDeActivateRequest
    {
        [Required(ErrorMessage = "آیدی فروشگاه نیاز است")]
        public long WholesaleId { get; set; }
    }
}
