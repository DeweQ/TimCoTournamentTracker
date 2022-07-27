using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess.TextHelpers;

public static class TextConnectorProcessor
{
    public static string FullFilePath(this string fileName)
    {
        return $"{GlobalConfig.Settings.FilePath}\\{fileName}";
    }

    public static List<string> LoadFile(this string file)
    {
        if (!File.Exists(file))
            return new List<string>();
        return File.ReadAllLines(file).ToList();
    }

    public static T ConvertStringToModel<T>(this string[] input)
        => GlobalConfig.Mapper.Map<string[], T>(input);

    public static string[] ConvertModelToString<T>(this T model)
        => GlobalConfig.Mapper.Map<T, string[]>(model);

    public static List<T> ConvertToListOfModels<T>(this List<string> lines)
        => lines.Select(x => x.Split(',')).Select(x => GlobalConfig.Mapper.Map<string[], T>(x)).ToList();

    public static List<string> ConvertToListOfStrings<T>(this List<T> models)
        => models.Select(x => GlobalConfig.Mapper.Map<T, string[]>(x)).Select(x => string.Join(',', x)).ToList();

    public static void SaveModelsToFile<T>(this List<T> models, string fileName)
    {
        List<string> lines = models.ConvertToListOfStrings();

        File.WriteAllLines(fileName.FullFilePath(), lines);
    }












    #region Obsolete
    public static List<PrizeModel> ConvertToPrizeModels(this List<string> lines)
    {
        return lines.ConvertToListOfModels<PrizeModel>();
    }

    public static List<PersonModel> ConvertToPersonModels(this List<string> lines)
    {
        return lines.ConvertToListOfModels<PersonModel>();
    }

    public static List<TeamModel> ConvertToTeamModels(this List<string> lines)
    {
        return lines.ConvertToListOfModels<TeamModel>();
    }

    public static List<TournamentModel> ConvertToTournamentModels(this List<string> lines)
    {
        //Id,TournamentName,EntryFee,Pipe-separated TeamModel.Id, pipe-Separated PrizeModel.Id,carrot-separated sets of Matchup.Id separated by pipes
        //1,My Tournament,0.00,1|2|3|4,1|3|4,1^2^3^4|5^6|7

        return lines.ConvertToListOfModels<TournamentModel>();

        //List<TournamentModel> result = new();
        //List<TeamModel> teams = GlobalConfig.TeamsFile.FullFilePath().LoadFile().ConvertToTeamModels();
        //List<PrizeModel> prizes = GlobalConfig.PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();
        //List<MatchupModel> matchups = GlobalConfig.MatchupsFile.FullFilePath().LoadFile().ConvertToMatchupModels();

        //foreach (string line in lines)
        //{
        //    string[] columns = line.Split(',');

        //    TournamentModel tm = new()
        //    {
        //        Id = int.Parse(columns[0]),
        //        TournamentName = columns[1],
        //        EntryFee = decimal.Parse(columns[2])
        //    };

        //    //Populate teams.
        //    string[] teamIds = columns[3].Split('|');
        //    foreach (string id in teamIds)
        //        tm.EnteredTeams.Add(teams.Where(x => x.Id == int.Parse(id)).First());

        //    //Populate prizes.
        //    string[] prizeIds = columns[4].Split('|');
        //    foreach (string id in prizeIds)
        //        if (int.TryParse(id, out int parsedId))
        //            tm.Prizes.Add(prizes.First(e => e.Id == parsedId));

        //    //Populate matchups.
        //    string[] rounds = columns[5].Split('|');
        //    foreach (string round in rounds)
        //    {
        //        List<MatchupModel> matchupList = new();

        //        string[] matchupString = round.Split('^');
        //        foreach (string id in matchupString)
        //            matchupList.Add(matchups.First(x => x.Id == int.Parse(id)));

        //        tm.Rounds.Add(matchupList);
        //    }

        //    result.Add(tm);
        //}

        //return result;
    }

    //public static List<MatchupModel> ConvertToMatchupModels(this List<string> lines)
    //{
    //    //id,entries(pipe-separated ids),winnerId,matchupRound
    //    //1,1|2,2,3

    //    return lines.ConvertToListOfModels<MatchupModel>();

    //    //List<MatchupModel> result = new();

    //    //foreach (var line in lines)
    //    //{
    //    //    string[] columns = line.Split(',');
    //    //    MatchupModel m = new();

    //    //    m.Id = int.Parse(columns[0]);
    //    //    m.Entries = ConvertStringToMatchupEntryModels(columns[1]);
    //    //    m.Winner = int.TryParse(columns[2], out int winnerId) ? LookupTeamById(winnerId) : null;
    //    //    m.MatchupRound = int.Parse(columns[3]);

    //    //    result.Add(m);
    //    //}

    //    //return result;
    //}

    //private static List<MatchupEntryModel> ConvertToMatchupEntryModels(this List<string> lines)
    //{
    //    return lines.ConvertToListOfModels<MatchupEntryModel>();
    //    //List<MatchupEntryModel> result = new();
    //    //foreach (string line in lines)
    //    //{
    //    //    string[] columns = line.Split(',');

    //    //    MatchupEntryModel me = new()
    //    //    {
    //    //        Id = int.Parse(columns[0]),
    //    //        TeamCompeting = int.TryParse(columns[1], out int teamCompeting) ?
    //    //        LookupTeamById(teamCompeting) : null,
    //    //        Score = double.Parse(columns[2]),
    //    //        ParentMatchup = int.TryParse(columns[3], out int id) ?
    //    //        LookupMatchupById(id) : null
    //    //    };

    //    //    result.Add(me);
    //    //}

    //    //return result;
    //}

    //private static List<MatchupEntryModel> ConvertStringToMatchupEntryModels(string input)
    //{
    //    string[] ids = input.Split('|');
    //    List<string> entries = GlobalConfig.MatchupEntriesFile.FullFilePath().LoadFile();
    //    List<string> matchingEntries = entries.Where(e => ids.ToList().Contains(e.Split(',')[0])).ToList();
    //    return matchingEntries.ConvertToMatchupEntryModels();
    //}

    //private static string ConvertPeopleListToString(List<PersonModel> people)
    //{
    //    return String.Join('|', people.Select(p => p.Id));
    //}

    //private static string ConvertMatchupListToString(List<MatchupModel> matchups)
    //{
    //    return string.Join('^', matchups.Select(x => x.Id));
    //}

    //private static string ConvertRoundsToString(List<List<MatchupModel>> rounds)
    //{
    //    return String.Join('|', rounds.Select(r => ConvertMatchupListToString(r)));
    //}

    //private static string ConvertPrizeListToString(List<PrizeModel> prizes)
    //{
    //    return string.Join('|', prizes.Select(p => p.Id));
    //}

    //private static string ConvertTeamListToString(List<TeamModel> teams)
    //{
    //    return String.Join('|', teams.Select(t => t.Id));
    //}

    //private static string ConvertMatchupEntryListToString(List<MatchupEntryModel> entries)
    //{
    //    return string.Join('|', entries.Select(e => e.Id));
    //}

    public static void SaveToPrizeFile(this List<PrizeModel> models)
    {
        models.SaveModelsToFile(GlobalConfig.PrizesFile);
    }

    public static void SaveToPeopleFile(this List<PersonModel> models)
    {
        models.SaveModelsToFile(GlobalConfig.PeopleFile);
    }

    public static void SaveToTeamsFile(this List<TeamModel> models)
    {
        models.SaveModelsToFile(GlobalConfig.TeamsFile);
    }

    public static void SaveToTournamentsFile(this List<TournamentModel> models)
    {
        models.SaveModelsToFile(GlobalConfig.TournamentsFile);
    }
    #endregion

    public static void SaveRoundsToFile(this TournamentModel tournamentModel)
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
        List<MatchupModel> matchups = GlobalConfig.MatchupsFile.FullFilePath().LoadFile().ConvertToListOfModels<MatchupModel>();

        int currentId = 1;
        if (matchups.Count > 0)
            currentId = matchups.OrderByDescending(x => x.Id).First().Id + 1;
        matchup.Id = currentId;

        matchups.Add(matchup);

        foreach (MatchupEntryModel entry in matchup.Entries)
            entry.SaveEntryToFile();

        matchups.SaveModelsToFile(GlobalConfig.MatchupsFile);
    }

    public static void SaveEntryToFile(this MatchupEntryModel entry) 
    { 
        List<MatchupEntryModel> entries = GlobalConfig.MatchupEntriesFile
            .FullFilePath()
            .LoadFile()
            .ConvertToListOfModels<MatchupEntryModel>();

        int currentId = 1;
        if (entries.Count > 0)
            currentId = entries.OrderByDescending(x => x.Id).First().Id + 1;
        entry.Id = currentId;

        entries.Add(entry);

        entries.SaveModelsToFile(GlobalConfig.MatchupEntriesFile);
    }

    public static void UpdateMathcupToFile(this MatchupModel matchup)
    {
        List<MatchupModel> matchups = GlobalConfig.MatchupsFile
            .FullFilePath()
            .LoadFile()
            .ConvertToListOfModels<MatchupModel>();

        matchups = matchups.Select(e => e.Id == matchup.Id ? matchup : e).ToList();//swap model to updated in the list.

        foreach (MatchupEntryModel entry in matchup.Entries)
            entry.UpdateEntryToFile();

        matchups.SaveModelsToFile(GlobalConfig.MatchupsFile);
    }

    public static void UpdateEntryToFile(this MatchupEntryModel entry)
    {
        List<MatchupEntryModel> entries = GlobalConfig.MatchupEntriesFile
            .FullFilePath()
            .LoadFile()
            .ConvertToListOfModels<MatchupEntryModel>();

        entries = entries.Select(e => e.Id == entry.Id ? entry : e).ToList();//swap model to updated in the list.

        entries.SaveModelsToFile(GlobalConfig.MatchupEntriesFile);
    }

    //private static MatchupModel LookupMatchupById(int id)
    //{
    //    MatchupModel result = GlobalConfig.MatchupsFile.FullFilePath()
    //        .LoadFile()
    //        .Where(e => e.Split(',')[0] == id.ToString())
    //        .ToList()
    //        .ConvertToMatchupModels()
    //        .First();
    //    return result;
    //}

    //private static TeamModel LookupTeamById(int id)
    //{
    //    TeamModel result = GlobalConfig.TeamsFile.FullFilePath().LoadFile()
    //        .Where(e => e.Split(',')[0] == id.ToString())
    //        .ToList()
    //        .ConvertToTeamModels()
    //        .First();

    //    return result;
    //}
}
