using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace AnimalMVC.Models
{
    public class AnimalExhibition
    {
        [Key]
        [Required(ErrorMessage = "Поле не може бути порожнім")]
        [Display(Name = "Тварина")]
        public int AnimalId { get; set; }

        [Key]
        [Required(ErrorMessage = "Поле не може бути порожнім")]
        [Display(Name = "Виставка")]
        public int ExhibitionId { get; set; }

        [Required(ErrorMessage = "Поле не може бути порожнім")]
        [Display(Name = "Дата реєстрації")]
        public DateTime RegistrationDate { get; set; }

        [JsonIgnore]
        public Animal? Animal { get; set; }

        [JsonIgnore]
        public Exhibition? Exhibition { get; set; } 
    }
}
