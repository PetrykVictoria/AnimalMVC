using System.ComponentModel.DataAnnotations;
namespace AnimalMVC.Models
{
    public class Species
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле не може бути порожнім")]
        [Display(Name = "Вид")]
        [RegularExpression(@"^[a-zA-Zа-яА-Я\s]+$", ErrorMessage = "Вид може містити тільки літери та пробіли.")]
        public string Name { get; set; } = null!;

        public virtual ICollection<Animal> Animals { get; set; } = new List<Animal>();
    }
}
