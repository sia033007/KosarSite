using System.ComponentModel.DataAnnotations;

namespace KosarSite.Models
{
    public class PersonModel
    {
        [Key]
        public int Id { get; set; }
        [RegularExpression("^([0-9]){10}$", ErrorMessage ="کد ملی وارد شده اشتباه است")]
        [Required(ErrorMessage ="این فیلد اجباری است")]
        public string IdNumber { get; set; } = string.Empty;
        [MaxLength(500 * 1024, ErrorMessage = "حجم فایل نباید از 500 کیلوبایت بیشتر باشد")]
        [Required(ErrorMessage = "این فیلد اجباری است")]
        public byte[] JobDoc { get; set; }
        [MaxLength(500 * 1024, ErrorMessage = "حجم فایل نباید از 500 کیلوبایت بیشتر باشد")]
        [Required(ErrorMessage = "این فیلد اجباری است")]
        public byte[] BirthDoc { get; set; }
        [MaxLength(500 * 1024, ErrorMessage = "حجم فایل نباید از 500 کیلوبایت بیشتر باشد")]
        [Required(ErrorMessage = "این فیلد اجباری است")]
        public byte[] StudyDoc { get; set; }
    }
}
