using System.ComponentModel.DataAnnotations;

namespace Microsis.CWM.Dto.Media
{
    public class MediaNewRequest
    {
        //public long WholesaleId { get; set; }
        [Required(ErrorMessage = "عکس را مشخص نمایید")]
        public string? WholesaleImage { get; set; }
        [Required(ErrorMessage = "ترتیب را مشخص نمایید")]
        public int? OrderId { get; set; }
    }
}
