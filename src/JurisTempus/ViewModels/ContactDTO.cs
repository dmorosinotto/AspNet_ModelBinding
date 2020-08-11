
using System.ComponentModel.DataAnnotations;

namespace JurisTempus.ViewModels
{
    //ESEMPIO VALIDAZIONE CLASSICA FATTA CON ATTRIBUTI
    public class ContactDTO
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(30)]
        public string Subject { get; set; }

        [MaxLength(500)]
        public string Message { get; set; }
    }
}
