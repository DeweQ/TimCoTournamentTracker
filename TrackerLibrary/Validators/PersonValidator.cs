using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.Validators;

public class PersonValidator :AbstractValidator<PersonModel>
{
    public PersonValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Continue;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(p => p.FirstName)
            .NotEmpty()
            .Length(2, 100)
            .Must(ExludeSpecialCharacters);
        RuleFor(p => p.LastName)
            .NotEmpty()
            .Length(2, 100)
            .Must(ExludeSpecialCharacters);
        RuleFor(p => p.EmailAddress)
            .EmailAddress()
            .When(p => p.EmailAddress != "");
        RuleFor(p => p.CellphoneNumber)
            .NotEmpty()
            .Must(BeAValidCellphoneNumber)
            .WithMessage("'{PropertyName}' must  be a valid number");
    }

    public  bool BeAValidCellphoneNumber(string input) 
    {
        input = input.Replace("(", "");
        input = input.Replace(")", "");
        input = input.Replace("-", "");

        //Regex regex = new Regex(@"\+(9[976]\d | 8[987530]\d | 6[987]\d | 5[90]\d | 42\d | 3[875]\d |2[98654321]\d | 9[8543210] | 8[6421] | 6[6543210] | 5[87654321] |4[987654310] | 3[9643210] | 2[70] | 7 | 1)\d{ 1,14}$");
        Regex regex = new Regex(@"^(\+|00)[1-9][0-9 \-\(\)\.]{7,32}$");

        Match match = regex.Match(input);

        return match.Success;
    }

    private bool ExludeSpecialCharacters(string input)
    {
        input = input.Replace("-","");
        input = input.Replace(" ","");

        return input.All(char.IsLetter);
    }
}
