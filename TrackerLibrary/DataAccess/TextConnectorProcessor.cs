using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess.TextHelpers;
//Low: add comments
public static class TextConnectorProcessor
{
    public static string FullFilePath(this string fileName)
    {
        return $"{ConfigurationManager.AppSettings["filePath"]}\\{fileName}";
    }

    public static List<string> LoadFile(this string file)
    {
        if (!File.Exists(file))
            return new List<string>();
        return File.ReadAllLines(file).ToList();
    }

    public static List<PrizeModel> ConvertToPrizeModels(this List<string> lines)
    {
        List<PrizeModel> result = new();

        foreach (var line in lines)
        {
            string[] columns = line.Split(',');
            PrizeModel p = new();
            p.Id = int.Parse(columns[0]);
            p.PlaceNumber = int.Parse(columns[1]);
            p.PlaceName = columns[2];
            p.PrizeAmount = decimal.Parse(columns[3]);
            p.PrizePercentage = double.Parse(columns[4]);

            result.Add(p);
        }

        return result;
    }

    public static List<PersonModel> ConvertToPersonModels(this List<string> lines)
    {
        List<PersonModel> result = new();

        foreach (var line in lines)
        {
            string[] columns = line.Split(',');
            PersonModel p = new();
            p.Id = int.Parse(columns[0]);
            p.FirstName = columns[1];
            p.LastName = columns[2];
            p.EmailAdress = columns[3];
            p.CellphoneNumber = columns[4];

            result.Add(p);
        }

        return result;
    }

    public static List<TeamModel> ConvertToTeamModels(this List<string> lines)
    {
        List<TeamModel> result = new();
        List<PersonModel> persons = GlobalConfig.PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();

        foreach (string line in lines)
        {
            string[] columns = line.Split(',');
            TeamModel t = new();
            t.Id = int.Parse(columns[0]);
            t.TeamName = columns[1];

            string[] personIds = columns[2].Split('|');

            foreach (string id in personIds)
                t.TeamMembers.Add(persons.Where(x => x.Id == int.Parse(id)).First());

            result.Add(t);
        }
        return result;
    }

    public static List<TournamentModel> ConvertToTournamentModels(
        this List<string> lines)
    {
        //Id,TournamentName,EntryFee,Pipe-separated TeamModel.Id, pipe-Separated PrizeModel.Id,carrot-separated sets of Matchup.Id separated by pipes
        //1,My Tournament,0.00,1|2|3|4,1|3|4,1^2^3^4|5^6|7

        List<TournamentModel> result = new();
        List<TeamModel> teams = GlobalConfig.TeamsFile.FullFilePath().LoadFile().ConvertToTeamModels();
        List<PrizeModel> prizes = GlobalConfig.PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();
        List<MatchupModel> matchups = GlobalConfig.MatchupsFile.FullFilePath().LoadFile().ConvertToMatchupModels();

        foreach (string line in lines)
        {
            string[] columns = line.Split(',');

            TournamentModel tm = new();
            tm.Id = int.Parse(columns[0]);
            tm.TournamentName = columns[1];
            tm.EntryFee = decimal.Parse(columns[2]);

            string[] teamIds = columns[3].Split('|');
            foreach (string id in teamIds)
                tm.EnteredTeams.Add(teams.Where(x => x.Id == int.Parse(id)).First());

            string[] prizeIds = columns[4].Split('|');
            foreach (string id in prizeIds)
                if (int.TryParse(id, out int parsedId))
                    tm.Prizes.Add(prizes.First(e => e.Id == parsedId));

            string[] rounds = columns[5].Split('|');
            foreach (string round in rounds)
            {
                List<MatchupModel> matchupList = new();

                string[] matchupString = round.Split('^');
                foreach (string id in matchupString)
                    matchupList.Add(matchups.First(x => x.Id == int.Parse(id)));

                tm.Rounds.Add(matchupList);
            }

            result.Add(tm);
        }

        return result;
    }

    public static List<MatchupModel> ConvertToMatchupModels(this List<string> lines)
    {
        //id,entries(pipe-separated ids),winnerId,matchupRound
        //1,1|2,2,3
        List<MatchupModel> result = new();

        foreach (var line in lines)
        {
            string[] columns = line.Split(',');
            MatchupModel m = new();

            m.Id = int.Parse(columns[0]);
            m.Entries = ConvertStringToMatchupEntryModels(columns[1]);
            m.Winner = int.TryParse(columns[2],out int winnerId)?LookupTeamById(winnerId):null;
            m.MatchupRound = int.Parse(columns[3]);

            result.Add(m);
        }

        return result;
    }

    private static List<MatchupEntryModel> ConvertToMatchupEntryModels(this List<string> lines)
    {
        List<MatchupEntryModel> result = new();
        foreach (string line in lines)
        {
            string[] columns = line.Split(',');

            MatchupEntryModel me = new();
            me.Id = int.Parse(columns[0]);
            me.TeamCompeting = int.TryParse(columns[1],out int teamCompeting)?
                LookupTeamById(teamCompeting):null;
            me.Score = double.Parse(columns[2]);
            me.ParentMatchup = int.TryParse(columns[3], out int id) ?
                LookupMatchupById(id) : null;

            result.Add(me);
        }

        return result;
    }

    private static List<MatchupEntryModel> ConvertStringToMatchupEntryModels(string input)
    {
        string[] ids = input.Split('|');
        List<string> entries = GlobalConfig.MatchupEntriesFile.FullFilePath().LoadFile();
        List<string> matchingEntries = entries.Where(e => ids.ToList().Contains(e.Split(',')[0])).ToList();
        return matchingEntries.ConvertToMatchupEntryModels();
    }

    private static string ConvertPeopleListToString(List<PersonModel> people)
    {
        //Low: refactor repeating code
        string result = string.Empty;

        if (people.Count == 0) return result;

        foreach (PersonModel p in people)
            result += $"{p.Id}|";

        result = result.TrimEnd('|');

        return result;
    }

    private static string ConvertMatchupListToString(List<MatchupModel> matchups)
    {
        //Low: refactor repeating code

        string result = string.Empty;
        return string.Join('^',matchups.Select(x => x.Id));
    }

    private static string ConvertRoundsToString(List<List<MatchupModel>> rounds)
    {
        //Low: refactor repeating code
        string result = string.Empty;

        if (rounds.Count == 0) return result;

        foreach (List<MatchupModel> r in rounds)
            result += $"{ConvertMatchupListToString(r)}|";

        result = result.TrimEnd('|');

        return result;
    }

    private static string ConvertPrizeListToString(List<PrizeModel> prizes)
    {
        //Low: refactor repeating code
        string result = string.Empty;

        if (prizes.Count == 0) return result;

        foreach (PrizeModel p in prizes)
            result += $"{p?.Id}|";

        result = result.TrimEnd('|');

        return string.Join('|', prizes.Select(p => p.Id));
    }

    private static string ConvertTeamListToString(List<TeamModel> teams)
    {
        //Low: refactor repeating code

        string result = string.Empty;

        if (teams.Count == 0) return result;

        foreach (TeamModel t in teams)
            result += $"{t.Id}|";

        result = result.TrimEnd('|');

        return result;
    }

    private static string ConvertMatchupEntryListToString(List<MatchupEntryModel> entries)
    {
        return string.Join('|', entries.Select(e => e.Id));
    }

    public static void SaveToPrizeFile(this List<PrizeModel> models, string fileName)
    {
        List<string> lines = new();

        foreach (PrizeModel p in models)
            lines.Add($"{p.Id},{p.PlaceNumber},{p.PlaceName},{p.PrizeAmount},{p.PrizePercentage}");

        File.WriteAllLines(fileName.FullFilePath(), lines);
    }

    public static void SaveToPeopleFile(this List<PersonModel> models, string fileName)
    {
        List<string> lines = new();

        foreach (PersonModel p in models)
            lines.Add($"{p.Id},{p.FirstName},{p.LastName},{p.EmailAdress},{p.CellphoneNumber}");

        File.WriteAllLines(fileName.FullFilePath(), lines);
    }

    public static void SaveToTeamsFile(this List<TeamModel> models, string fileName)
    {
        List<string> lines = new();

        foreach (TeamModel t in models)
            lines.Add($"{t.Id},{t.TeamName},{ConvertPeopleListToString(t.TeamMembers)}");

        File.WriteAllLines(fileName.FullFilePath(), lines);
    }

    public static void SaveToTournamentsFile(this List<TournamentModel> models, string fileName)
    {
        List<string> lines = new();

        foreach (TournamentModel tm in models)
            lines.Add($"{tm.Id}," +
                $"{tm.TournamentName}," +
                $"{tm.EntryFee}," +
                $"{ConvertTeamListToString(tm.EnteredTeams)}," +
                $"{ConvertPrizeListToString(tm.Prizes)}," +
                $"{ConvertRoundsToString(tm.Rounds)}");

        File.WriteAllLines(fileName.FullFilePath(), lines);
    }

    public static void SaveRoundsToFile(this TournamentModel tournamentModel, string matchupsFileName, string matchupEntriesFileName)
    {
        //loop through the rounds
        //loop through the matchups and save them
        //loop through the entries and save them

        foreach (List<MatchupModel> round in tournamentModel.Rounds)
            foreach (MatchupModel matchup in round)
                matchup.SaveMatchupToFile();
    }

    public static void SaveMatchupToFile(this MatchupModel matchup)
    {
        List<MatchupModel> matchups = GlobalConfig.MatchupsFile.FullFilePath().LoadFile().ConvertToMatchupModels();

        int currentId = 1;
        if (matchups.Count > 0)
            currentId = matchups.OrderByDescending(x => x.Id).First().Id + 1;

        matchup.Id = currentId;

        matchups.Add(matchup);

        foreach (MatchupEntryModel entry in matchup.Entries)
            entry.SaveEntryToFile();

        //id,entries(pipe-separated ids),winnerId,matchupRound
        List<string> lines = new();
        foreach (MatchupModel m in matchups)
            lines.Add($"{m.Id}," +
                $"{ConvertMatchupEntryListToString(m.Entries)}," +
                $"{m.Winner?.Id}," +
                $"{m.MatchupRound}");

        File.WriteAllLines(GlobalConfig.MatchupsFile.FullFilePath(), lines);
    }

    public static void SaveEntryToFile(this MatchupEntryModel entry)
    {
        List<MatchupEntryModel> entries = GlobalConfig.MatchupEntriesFile.FullFilePath().LoadFile().ConvertToMatchupEntryModels();

        int currentId = 1;
        if (entries.Count > 0)
            currentId = entries.OrderByDescending(x => x.Id).First().Id + 1;

        entry.Id = currentId;

        entries.Add(entry);

        List<string> lines = new();

        foreach (MatchupEntryModel e in entries)
            lines.Add($"{e.Id},{e.TeamCompeting?.Id},{e.Score},{e.ParentMatchup?.Id}");

        File.WriteAllLines(GlobalConfig.MatchupEntriesFile.FullFilePath(), lines);
    }

    private static MatchupModel LookupMatchupById(int id)
    {
        MatchupModel result = GlobalConfig.MatchupsFile.FullFilePath()
            .LoadFile()
            .Where(e => e.Split(',')[0] == id.ToString())
            .ToList()
            .ConvertToMatchupModels()
            .First();
        return result;
    }

    private static TeamModel LookupTeamById(int id)
    {
        TeamModel result = GlobalConfig.TeamsFile.FullFilePath().LoadFile()
            .Where(e => e.Split(',')[0] == id.ToString())
            .ToList()
            .ConvertToTeamModels()
            .First();
            
        return result;
    }
}
