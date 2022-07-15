using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary;
//Order list of teams randomly
//Check if it is big enough, if not - add in byes
//Create first round of matchups
//Create every round after that
public static class TournamentLogic
{
    public static List<MatchupModel> GetMatchupsToScore(TournamentModel tournamentModel)
    {
        return tournamentModel.Rounds
                .SelectMany(e => e)
                .Where(m => m.Winner == null && (m.Entries.Count == 1 || m.Entries.Any(e => e.Score != 0)))
                .ToList();
    }

    public static void UpdateTournamentResults(TournamentModel tournamentModel)
    {
        int startingRound = tournamentModel.CheckCurrentRound();
        List<MatchupModel> matchupsToScore = GetMatchupsToScore(tournamentModel);

        ScoreMatchups(matchupsToScore);

        AdvanceWinners(matchupsToScore, tournamentModel);

        matchupsToScore.ForEach(x => GlobalConfig.Connection.UpdateMatchup(x));

        int endingRound = tournamentModel.CheckCurrentRound();
        if (endingRound > startingRound)
        {
            //alert users
            tournamentModel.AlertUsersToNewRound();
        }
    }

    public static void AlertUsersToNewRound(this TournamentModel tournamentModel)
    {
        int currentRoundNumber = tournamentModel.CheckCurrentRound();
        List<MatchupModel> currentRound = tournamentModel.Rounds.Where(r => r.First().MatchupRound == currentRoundNumber).First();

        foreach (MatchupModel matchup in currentRound)
        {
            foreach (MatchupEntryModel entry in matchup.Entries)
            {
                foreach (PersonModel person in entry.TeamCompeting.TeamMembers)
                {
                    AlertPersonToNewRound(person, entry.TeamCompeting.TeamName, matchup.Entries.FirstOrDefault(x => x.TeamCompeting != entry.TeamCompeting));
                }
            }
        }
    }

    private static void AlertPersonToNewRound(PersonModel person, string teamName, MatchupEntryModel competitor)
    {
        if (person.EmailAddress.Length == 0) return;

        string to = person.EmailAddress;
        string subject = "";
        string body = "";
        StringBuilder sb = new();

        if (competitor != null)
        {
            subject = $"You have a new matchup with {competitor.TeamCompeting.TeamName}.";

            sb.AppendLine("<h1>You have a new matchup</h1>");
            sb.Append("<strong>Competitor: </strong>");
            sb.AppendLine(competitor.TeamCompeting.TeamName);
            sb.AppendLine("Have a great time!");
        }
        else
        {
            subject = "You have a bye week this round.";
            sb.AppendLine("Enjoy your free round");
        }

        sb.AppendLine();
        sb.AppendLine();
        sb.AppendLine("~Tournament Tracker");

        body = sb.ToString();

        EmailLogic.SendEmail(to, subject, body);
    }

    private static int CheckCurrentRound(this TournamentModel tournamentModel)
    {
        int result = 1;

        var temp = tournamentModel.Rounds.Where(r => r.All(m => m.Winner != null)).Max(r => r.FirstOrDefault()?.MatchupRound);

        temp = temp == null ? 1 : temp;

        foreach (List<MatchupModel> round in tournamentModel.Rounds)
        {
            if (round.All(m => m.Winner != null))
                result++;
            else
            {
                return result;
            }
        }

        CompleteTournament(tournamentModel);

        return result - 1;
    }

    private static decimal CalculatePrizePayout(this PrizeModel prize, decimal totalIncome)
    {
        decimal result = 0;

        if (prize.PrizeAmount > 0)
        {
            result = prize.PrizeAmount;
        }
        else
        {
            result = Decimal.Multiply(totalIncome, Convert.ToDecimal(prize.PrizePercentage / 100));
        }

        return result;
    }

    private static void CompleteTournament(TournamentModel tournamentModel)
    {

        GlobalConfig.Connection.CompleteTournament(tournamentModel);

        TeamModel winners = tournamentModel.Rounds.Last().First().Winner;
        TeamModel runnerUp = tournamentModel.Rounds.Last().First().Entries.First(x => x.TeamCompeting != winners).TeamCompeting;

        decimal winnerPrize = 0;
        decimal runnerUpPrize = 0;



        if (tournamentModel.Prizes.Count > 0)
        {
            decimal totalIncome = tournamentModel.EnteredTeams.Count * tournamentModel.EntryFee;
            PrizeModel firstPlacePrize = tournamentModel.Prizes.FirstOrDefault(x => x.PlaceNumber == 1);
            PrizeModel secondPlacePrize = tournamentModel.Prizes.FirstOrDefault(x => x.PlaceNumber == 2);
            if (firstPlacePrize != null)
            {
                winnerPrize = firstPlacePrize.CalculatePrizePayout(totalIncome);
            }
            if (secondPlacePrize != null)
            {
                runnerUpPrize = secondPlacePrize.CalculatePrizePayout(totalIncome);
            }
        }

        //Send email to all tournament competitors

        string subject = "";
        string body = "";
        StringBuilder sb = new();

        subject = $"In {tournamentModel.TournamentName}, {winners.TeamName} has won!";

        sb.AppendLine("<h1>WE HAVE A WINNER!</h1>");
        sb.AppendLine("<p>Congratulations to our winner on a great tournament.</p>");
        if(winnerPrize>0)
        {
            sb.AppendLine($"<p>{winners.TeamName} will receive ${winnerPrize}</p>");
        }
        if(runnerUpPrize>0)
        {
            sb.AppendLine($"<p>{runnerUp.TeamName} will receive ${runnerUpPrize}</p>");
        }
        sb.AppendLine(Environment.NewLine);
        sb.AppendLine(Environment.NewLine);
        sb.AppendLine(Environment.NewLine);
        sb.AppendLine("<p>Thanks for a great tournament everyone.</p>");
        sb.AppendLine("~Tournament Tracker");

        body = sb.ToString();

        List<string> bcc = tournamentModel.EnteredTeams.SelectMany(x => x.TeamMembers).Select(x => x.EmailAddress).Where(x => x.Length > 0).ToList();

        EmailLogic.SendEmail(new List<string>(),bcc, subject, body);

        //Complete Tournament
        tournamentModel.CompleteTournament();

    }

    private static void AdvanceWinners(List<MatchupModel> matchups, TournamentModel tournamentModel)
    {
        foreach (MatchupModel matchup in matchups)
        {
            foreach (List<MatchupModel> round in tournamentModel.Rounds)
            {
                foreach (MatchupModel rm in round)
                {
                    foreach (MatchupEntryModel me in rm.Entries)
                    {
                        if (me.ParentMatchup?.Id == matchup.Id)
                        {
                            me.TeamCompeting = matchup.Winner;
                            GlobalConfig.Connection.UpdateMatchup(rm);
                        }
                    }
                }
            }
        }
    }

    private static void ScoreMatchups(List<MatchupModel> matchups)
    {
        matchups.ForEach(matchup =>
        {
            if (matchup.Entries.Count == 2 && matchup.Entries[0].Score == matchup.Entries[1].Score)
                throw new Exception("We do not allow ties in this application.");
            matchup.Winner = matchup.Entries
                        .First(e =>
                        {
                            if (GlobalConfig.Settings.GreaterWins)
                                return e.Score == matchup.Entries.Max(ent => ent.Score);
                            return e.Score == matchup.Entries.Min(ent => ent.Score);

                        }).TeamCompeting;
        });
    }

    public static void CreateRounds(TournamentModel tournamentModel)
    {
        List<TeamModel> randomizedTeams = RandomizeTeamOrder(tournamentModel.EnteredTeams);
        int rounds = FindNumberOfRounds(randomizedTeams.Count);
        int byes = FindNumberOfByes(randomizedTeams.Count, rounds);

        tournamentModel.Rounds.Add(CreateFirstRound(randomizedTeams, byes));
        CreateOtherRounds(tournamentModel, rounds);
    }

    private static void CreateOtherRounds(TournamentModel tournamentModel, int totalRounds)
    {
        int currentRoundNumber = 2;
        List<MatchupModel> previousRound = tournamentModel.Rounds[0];
        List<MatchupModel> currentRound = new();
        MatchupModel currentMatchup = new();

        while (currentRoundNumber <= totalRounds)
        {
            foreach (MatchupModel matchup in previousRound)
            {
                currentMatchup.Entries.Add(new MatchupEntryModel { ParentMatchup = matchup });

                if (currentMatchup.Entries.Count > 1)
                {
                    currentMatchup.MatchupRound = currentRoundNumber;
                    currentRound.Add(currentMatchup);
                    currentMatchup = new();
                }
            }
            currentRoundNumber++;
            tournamentModel.Rounds.Add(currentRound);
            previousRound = currentRound;
            currentRound = new();
        }

    }

    private static List<MatchupModel> CreateFirstRound(List<TeamModel> teams, int byes)
    {
        List<MatchupModel> result = new();
        MatchupModel current = new();

        foreach (TeamModel team in teams)
        {
            MatchupEntryModel entry = new();
            entry.TeamCompeting = team;
            current.Entries.Add(entry);

            if (byes > 0 || current.Entries.Count > 1)
            {
                current.MatchupRound = 1;
                result.Add(current);
                current = new();

                byes = byes > 0 ? byes - 1 : byes;
            }
        }



        return result;
    }

    private static int FindNumberOfByes(int teamCount, int rounds)
    {
        if (rounds <= 0) return 0;
        return (int)Math.Pow(2, rounds) - teamCount;
    }

    private static int FindNumberOfRounds(int teamCount)
    {
        if (teamCount < 2) return 0;
        double result = Math.Log2(teamCount);

        return (int)Math.Ceiling(result);
    }

    private static List<TeamModel> RandomizeTeamOrder(List<TeamModel> teams)
    {
        Random rnd = new Random();
        return teams.OrderBy(x => rnd.Next()).ToList();
    }
}
