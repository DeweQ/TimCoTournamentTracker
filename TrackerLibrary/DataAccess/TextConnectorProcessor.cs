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

    public static void SaveRoundsToFile(this TournamentModel tournamentModel)
    {
        //loop through the rounds
        //loop through the matchups and save them
        //loop through the entries and save them

        foreach (List<MatchupModel> round in tournamentModel.Rounds)
            foreach (MatchupModel matchup in round)
                matchup.SaveMatchupToFile();
    }

    private static void SaveMatchupToFile(this MatchupModel matchup)
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

    private static void SaveEntryToFile(this MatchupEntryModel entry) 
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

    private static void UpdateEntryToFile(this MatchupEntryModel entry)
    {
        List<MatchupEntryModel> entries = GlobalConfig.MatchupEntriesFile
            .FullFilePath()
            .LoadFile()
            .ConvertToListOfModels<MatchupEntryModel>();

        entries = entries.Select(e => e.Id == entry.Id ? entry : e).ToList();//swap model to updated in the list.

        entries.SaveModelsToFile(GlobalConfig.MatchupEntriesFile);
    }
}
