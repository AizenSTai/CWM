namespace Microsis.CWM.Dto.MediaSettings
{
    public class MediaSettingsGetResponse
    {
        public int PicMaxQuantity { get; set; }
        public int PicMaxSize { get; set; }
        public string? RecordDate { get; set; }
        public string? RecordTime { get; set; }
        public string? RecordUsername { get; set; }
    }
}
