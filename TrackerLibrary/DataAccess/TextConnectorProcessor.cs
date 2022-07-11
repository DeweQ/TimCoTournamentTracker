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
            {
                t.TeamMembers.Add(persons.Where(x => x.Id == int.Parse(id)).First());
            }

            result.Add(t);
        }
        return result;
    }

    private static string ConvertPeopleListToString(List<PersonModel> people)
    {
        string result = string.Empty;

        if (people.Count == 0) return result;

        foreach (PersonModel p in people)
            result += $"{p.Id}|";

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

        File.WriteAllLines(fileName.FullFilePath(),lines);
    }
}
