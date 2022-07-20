using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.Validators;

public class TournamentValidator : AbstractValidator<TournamentModel>
{
    public TournamentValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Continue;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(t => t.EnteredTeams.Count)
            .GreaterThanOrEqualTo(2).WithMessage("Tournament must have at least 2 teams.");
        RuleFor(t => t.Prizes.Count)
            .LessThanOrEqualTo(2).WithMessage("Tournament can handle only 2 winning places(first and/or second.");
        RuleFor(t => t)
            .Must(NotExceedTournamentBalance)
            .WithMessage("Total prize amount exceeds tournament balance.");
        RuleFor(t => t.Prizes)
            .Must(ContainNonRecurringPlaces)
            .WithMessage("There could be only one prize per place.");
        RuleFor(t => t.EntryFee)
            .GreaterThanOrEqualTo(0);
        RuleFor(t => t.TournamentName)
            .NotEmpty()
            .Must(ExcludeSpecialCharacters)
            .WithMessage("'{PropertyName}' must not contain characters: ',', '|', '^'.");

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

    private bool ContainNonRecurringPlaces(List<PrizeModel> prizes)
    {
        bool result = true;

        int distinctPrizesCount = prizes.DistinctBy(p => p.PlaceNumber).Count();

        if (distinctPrizesCount < prizes.Count) result = false;

        return result;
    }

    private bool NotExceedTournamentBalance(TournamentModel tournament)
    {
        bool result = true;

        decimal balance = decimal.Multiply(tournament.EntryFee, tournament.EnteredTeams.Count);

        decimal totalPrizeAmount = tournament.Prizes.Select(p => p.PrizeAmount).Sum();
        double totalPrizePercentage = tournament.Prizes.Select(p => p.PrizePercentage).Sum();

        if (totalPrizePercentage > 100) result = false;

        decimal totalPrizePercentageAmount = decimal.Multiply(balance, Convert.ToDecimal(totalPrizePercentage/100));

        decimal totalPrize = decimal.Add(totalPrizeAmount, totalPrizePercentageAmount);

        if (totalPrize > balance) result = false;

        return result;
    }
}
