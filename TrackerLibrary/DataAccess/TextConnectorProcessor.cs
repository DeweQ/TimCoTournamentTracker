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

    public static List<TeamModel> ConvertToTeamModels(this List<string> lines, string peopleFileName)
    {
        List<TeamModel> result = new();
        List<PersonModel> persons = peopleFileName.FullFilePath().LoadFile().ConvertToPersonModels();

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
        this List<string> lines,
        string teamsFileName,
        string peopleFileName,
        string prizesFileName)
    {
        //Id,TournamentName,EntryFee,Pipe-separated TeamModel.Id, pipe-Separated PrizeModel.Id,carrot-separated sets of Matchup.Id separated by pipes
        //1,My Tournament,0.00,1|2|3|4,1|3|4,1^2^3^4|5^6|7

        List<TournamentModel> result = new();
        List<TeamModel> teams = teamsFileName.FullFilePath().LoadFile().ConvertToTeamModels(peopleFileName);
        List<PrizeModel> prizes = prizesFileName.FullFilePath().LoadFile().ConvertToPrizeModels();

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
                tm.Prizes.Add(prizes.Where(x => x.Id == int.Parse(id)).First());

            //TODO: Save Rounds

            result.Add(tm);
        }


        return result;
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

        if (matchups.Count == 0) return result;

        foreach (MatchupModel m in matchups)
            result += $"{m.Id}^";

        result = result.TrimEnd('^');

        return result;
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
            result += $"{p.Id}|";

        result = result.TrimEnd('|');

        return result;
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
                $"{ConvertTeamListToString(tm.EnteredTeams)}," +
                $"{ConvertPrizeListToString(tm.Prizes)}," +
                $"{ConvertRoundsToString(tm.Rounds)}");

        File.WriteAllLines(fileName.FullFilePath(), lines);
    }
}
