
using FluentValidation;
using JurisTempus.ViewModels;
using JurisTempus.Data.Entities;

namespace JurisTempus.Validators
{
    public class CaseValidator : AbstractValidator<CaseDTO>
    {
        public CaseValidator()
        {
            // REGOLE DI VALIDAZIONE FLUENT PER I VARI CAMPI
            RuleFor(c => c.FileNumber).NotEmpty()
                                    .Matches(@"^\d{10}$") //VALIDA CON RegEx
                                    .WithMessage("must be ten digits"); // CUSTOM ERR MSG
            // DA PROVARE VALIDATORI SU ENUM 
            RuleFor(c => c.Status).NotEmpty().IsEnumName(typeof(CaseStatus));
            RuleFor(c => c.StatusId).NotEqual(0); // .IsInEnum()
        }
    }
}
