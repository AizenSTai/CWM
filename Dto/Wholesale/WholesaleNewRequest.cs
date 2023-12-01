using System.ComponentModel.DataAnnotations;

namespace Microsis.CWM.Dto.Wholesale
{
    public class WholesaleNewRequest
    {
        public long GuildId { get; set; } 
        [Required(ErrorMessage ="نوع صنف را مشخص نمایید")]
        public string? GuildCode { get; set; }
        [Required(ErrorMessage = "نام منیجر را مشخص نمایید")]
        public string? ManagerNameFa { get; set; }
        [Required(ErrorMessage = "کد ملی منیجر را مشخص نمایید")]
        public string? ManagerNationalCode { get; set; }
        public string? Tel1 { get; set; }
        public string? Tel2 { get; set; }
        [Required(ErrorMessage = "تلفن را پر نمایید")]
        public string? Mobile { get; set; }
        [Required(ErrorMessage = "اطلاعات اضافه را پر نمایید")]
        public string? AdditionalData { get; set; }
        [Required(ErrorMessage = "شماره صنف را مشخص نمایید")]
        public string? GuildNo { get; set; }
        [Required(ErrorMessage = "نام را مشخص نمایید")]
        public string? NameFa { get; set; }
        public string? NameEn { get; set; }
        [Required(ErrorMessage = "شماره ملی را مشخص نمایید")]
        public string? NationalId { get; set; }
        [Required(ErrorMessage = "لوگوی شرکت را مشخص نمایید")]
        public string? WholesaleLogo { get; set; }

    }
}
