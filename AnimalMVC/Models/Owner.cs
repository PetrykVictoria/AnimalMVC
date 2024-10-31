using System.ComponentModel.DataAnnotations;
namespace AnimalMVC.Models
{
    public class Owner
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле не може бути порожнім")]
        [Display(Name = "ПІБ")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Поле не може бути порожнім")]
        [Display(Name = "Телефон")]
        [RegularExpression(@"^\+?[0-9]{10,15}$", ErrorMessage = "Телефон повинен бути дійсним номером телефону.")]
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "Поле не може бути порожнім")]
        [Display(Name = "Email")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Email повинен бути дійсною електронною адресою.")]

        public string Email { get; set; } = null!;

        public virtual ICollection<Animal> Animals { get; set; } = new List<Animal>();
    }
}
