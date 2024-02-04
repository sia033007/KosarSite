using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KosarSite.Models
{
    public class Certificate
    {
        [Key]
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public byte[] Content { get; set; } = new byte[0];
        [ForeignKey("Id")]
        public PersonModel PersonModel { get; set; }
    }
}
