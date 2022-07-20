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
        ClassLevelCascadeMode = CascadeMode.Continue;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(team => team.TeamName)
            .NotEmpty()
            .Must(ExcludeSpecialCharacters)
            .WithMessage("'{PropertyName}' must not contain characters: ',', '|', '^'.");

        RuleFor(team => team.TeamMembers)
            .NotEmpty();
    }

    private bool ExcludeSpecialCharacters(string name)
    {
        bool result = true;

        if (name.Contains(',') ||
            name.Contains('|') ||
            name.Contains('^'))
            result = false;

        return result;
    }
}
