using System.ComponentModel.DataAnnotations;

namespace Microsis.CWM.Dto.SalesInformation
{
    public class SaleInformationModifyRequest
    {
        //public long WholesaleId { get; set; }
        [Required(ErrorMessage = "شرایط فروش را مشخص نمایید")]
        public string? SaleCondition { get; set; }
        [Required(ErrorMessage = "توضیحات اولیه را مشخص نمایید")]
        public string? OpenningDescription { get; set; }
    }
}
