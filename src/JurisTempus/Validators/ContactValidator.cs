
using FluentValidation;
using JurisTempus.ViewModels;

namespace JurisTempus.Validators
{
    /* 
    //HO PREFERITO FARE VALIDAZIONE CLASSICA CON ATTRIBUTI NEL ConcactDTO
    //PER VERIFICARE CHE COMUNQUE IL FLUENT VALIDATOR USA ANCHE QUELLI!!! 
    public class ContactValidator : AbstractValidator<ContactDTO>
    {
        public ContactValidator()
        {
            // REGOLE DI VALIDAZIONE FLUENT EQUIVALENTI A QUELLE CHE HO MESSO SU ATTRIBUTI
            RuleFor(c => c.Email).NotEmpty()
                                 .EmailAddress(); // Controlla che sia una Email valida
            RuleFor(c => c.Subject).NotEmpty()
                                    .MaximumLength(30);
            RuleFor(c => c.Message).MaximumLength(500);
        }
    }
    */
}
