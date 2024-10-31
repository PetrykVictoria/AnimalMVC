using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AnimalMVC.Models
{
    public class Animal
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле не може бути порожнім")]
        [Display(Name = "Прізвисько")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Поле не може бути порожнім")]
        [Display(Name = "Власник")]
        public int OwnerId { get; set; } 
        [Required(ErrorMessage = "Поле не може бути порожнім")]
        [Display(Name = "Вид")]
        public int SpeciesId { get; set; } 

        [Required(ErrorMessage = "Поле не може бути порожнім")]
        [Display(Name = "Порода")]
        [RegularExpression(@"^[a-zA-Zа-яА-Я\s]+$", ErrorMessage = "Порода може містити тільки літери та пробіли.")]

        public string Breed { get; set; } = null!;

        [Required(ErrorMessage = "Поле не може бути порожнім")]
        [Display(Name = "Вік")]
        [Range(0, 100, ErrorMessage = "Вік тварини не може бути від'ємним або більше 100 років.")]

        public int Age { get; set; }

        [Display(Name = "Фото")]
        [JsonIgnore]
        public byte[]? Image { get; set; }

        [JsonIgnore]
        public Owner? Owner { get; set; }

        [JsonIgnore]
        public Species? Species { get; set; }

        [JsonIgnore]
        public virtual ICollection<AnimalExhibition> AnimalExhibitions { get; set; } = new List<AnimalExhibition>();

    }
}
