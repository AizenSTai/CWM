using System.ComponentModel.DataAnnotations;

namespace Microsis.CWM.Dto.Media
{
    public class MediaModifyRequest
    {
        public long? WholeSaleId { get; set; }
        public int? OrderId { get; set; }
        public string? WholesaleImage { get; set; }
    }
}
