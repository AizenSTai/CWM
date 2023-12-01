using System.ComponentModel.DataAnnotations.Schema;

namespace Microsis.CWM.Model
{
    [Table("MediaSettings")]
    public class MediaSettings
    {
        public long Id { get; set; }
        public int PicMaxQuantity { get; set; }
        public int PicMaxSize { get; set; }
        public string? RecordDate { get; set; }
        public string? RecordTime { get; set; }
        public string? RecordUsername { get; set; }

    }
}
