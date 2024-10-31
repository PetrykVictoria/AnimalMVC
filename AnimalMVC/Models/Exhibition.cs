using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace AnimalMVC.Models
{
    public class Exhibition
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле не може бути порожнім")]
        [Display(Name = "Назва")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Поле не може бути порожнім")]
        [Display(Name = "Місце проведення")]
        public string Location { get; set; } = null!;

        [Required(ErrorMessage = "Поле не може бути порожнім")]
        [Display(Name = "Дата проведення")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Поле не може бути порожнім")]
        [Display(Name = "Проведено")]
        public bool IsClosed { get; set; }

        [Required(ErrorMessage = "Поле не може бути порожнім")]
        [Display(Name = "Опис")]
        public string Description { get; set; } = null!;

        [JsonIgnore]
        public virtual ICollection<AnimalExhibition> AnimalExhibitions { get; set; } = new List<AnimalExhibition>();
    }
}
