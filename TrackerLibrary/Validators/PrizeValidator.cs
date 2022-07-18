using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.Validators;

public class PrizeValidator : AbstractValidator<PrizeModel>
{
    public PrizeValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Continue;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(p => p.PlaceNumber)
            .InclusiveBetween(1, 2);
        RuleFor(p => p.PlaceName)
            .NotEmpty();
        //low: add more informative checks (for negative values)
        RuleFor(p => p.PrizeAmount)
           .GreaterThan(0).Unless(p => p.PrizePercentage > 0).WithMessage("'Prize Amount','Prize Percentage': only one of the fields can be 0.")
           .Equal(0).When(p => p.PrizePercentage > 0,ApplyConditionTo.CurrentValidator).WithMessage("'Prize Amount','Prize Percentage': only one of the fields must have value.");
        RuleFor(p => p.PrizePercentage)
           .GreaterThan(0).Unless(p => p.PrizeAmount > 0).WithMessage(" ")
           .Equal(0).When(p => p.PrizeAmount > 0, ApplyConditionTo.CurrentValidator).WithMessage(" ");
    }
}
