
using System;
using FluentValidation;

namespace JurisTempus.ViewModels
{
    public class TimeBillDTO
    {
        public int Id { get; set; }
        public string WorkDateUTC { get; set; }
        public DateTime? WorkDate { get; set; }
        public int TimeSegments { get; set; }
        public decimal Rate { get; set; }
        public string WorkDescription { get; set; }

        public int EmployeeId { get; set; }
        public int CaseId { get; set; }
    }


    public class TimeBillValidator : AbstractValidator<TimeBillDTO>
    {
        public TimeBillValidator()
        {
            RuleFor(f => f.WorkDescription).MinimumLength(25);
            RuleFor(f => f.TimeSegments).GreaterThan(0);
            RuleFor(f => f.Rate).InclusiveBetween(0m, 500m);
            RuleFor(f => f.CaseId).NotEmpty();
            RuleFor(f => f.EmployeeId).NotEmpty();
            RuleFor(f => f.WorkDateUTC).NotNull();
        }
    }
}
