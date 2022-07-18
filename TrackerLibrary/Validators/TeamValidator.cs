using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.Validators;

public class TeamValidator :AbstractValidator<TeamModel>
{
    public TeamValidator()
    {
        RuleFor(team => team.TeamName)
            .NotEmpty()
            .Must(ExcludePipeCharacter)
            .WithMessage("'{PropertyName}' must not contain '|' character.");

        RuleFor(team => team.TeamMembers)
            .NotEmpty();
    }

    private bool ExcludePipeCharacter(string input)
    {
        return !input.Contains('|');
    }
}
