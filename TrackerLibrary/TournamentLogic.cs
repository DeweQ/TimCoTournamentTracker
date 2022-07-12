using System;
using System.Collections.Generic;
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
