namespace Microsis.CWM.Dto.Wholesale
{
    public class WholesaleActivatedListResponse
    {
        public long Id { get; set; } //
        public long GuildId { get; set; }
        public string? GuildCode { get; set; }//
        public string? ManagerNameFa { get; set; }//
        public string? ManagerNationalCode { get; set; } //
        public string? Tel1 { get; set; }
        public string? Tel2 { get; set; }
        public string? Mobile { get; set; } //
        public string? AdditionalData { get; set; } //
        public string? RegisterMiladi { get; set; }
        public string? RegisterDateShamsi { get; set; }
        public string? GuildNo { get; set; } //
        public int RegisterDateInt { get; set; }
        public bool IsActive { get; set; } //
        public bool Delete { get; set; }
        public string? NameFa { get; set; } //
        public string? NameEn { get; set; }
        public string? NationalId { get; set; } //
        public string? WholesaleLogo { get; set; } //
        public long UserId { get; set; } //
        public string? UserKey { get; set; }

    }
}
