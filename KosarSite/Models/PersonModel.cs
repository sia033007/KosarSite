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
        public byte[] JobDoc { get; set; }
        public byte[] BirthDoc { get; set; }
        public byte[] StudyDoc { get; set; }
    }
}
