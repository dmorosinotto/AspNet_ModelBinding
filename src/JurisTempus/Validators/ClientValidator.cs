
using FluentValidation;
using JurisTempus.ViewModels;
using JurisTempus.Data.Entities;
using JurisTempus.Data;
using Microsoft.EntityFrameworkCore;

namespace JurisTempus.Validators
{
    public class ClientValidator : AbstractValidator<ClientDTO>
    {
        public ClientValidator(BillingContext ctx)
        {
            // REGOLE DI VALIDAZIONE FLUENT PER I VARI CAMPI
            RuleFor(c => c.Name).NotEmpty()
                                .MinimumLength(5).MaximumLength(100)
                                .MustAsync(async (value, cancelToken) =>
                                {
                                    // ESEMPIO LOGICA VALIDAZIONE ASINCRONA X TESTARE UNIQUE
                                    return !(await ctx.Clients.AnyAsync(c => c.Name == value));
                                }).WithMessage("NAME MUST BE UNIQUE!");


            RuleFor(c => c.ContactName).MaximumLength(50)
                                .WithName("Contact Name"); // SPECIFICA NOME PROP DIVERSO X ERRORI


            // ESEMPIO DI REGOLA CONDIZIONALE / BUSINESS LOGIC VALIDAZIONE CUSTOM IN BASE CONDIZIONI
            When(c => !string.IsNullOrEmpty(c.ContactName) || !string.IsNullOrEmpty(c.Phone),
            () =>
            {
                RuleFor(c => c.Phone).NotEmpty()
                                    .WithMessage("CAN'T BE EMPTY IF CONTACT IS SPEIFICED!");
                RuleFor(c => c.ContactName).NotEmpty()
                                    .WithMessage("IF YOU ENTER PHONE PLEASE FILL CONTACT NAME!");
            });
        }
    }
}
