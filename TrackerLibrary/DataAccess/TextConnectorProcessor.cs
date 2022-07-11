using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

//*Load the text file
//*Convert text to List<PrizeModel>
//*Convert the prizes to List<string>
//*Save List<string> to the text file (overwrite)

namespace TrackerLibrary.DataAccess.TextHelpers;

public static class TextConnectorProcessor
{
    public static string FullFilePath(this string fileName)
    {
        return $"{ ConfigurationManager.AppSettings["filePath"] }\\{ fileName }";
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

        foreach(var line in lines)
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

    public static void SaveToPrizeFile(this List<PrizeModel> models, string fileName)
    {
        List<string> lines = new();

        foreach (PrizeModel p in models)
        {
            lines.Add($"{p.Id},{p.PlaceNumber},{p.PlaceName},{p.PrizeAmount},{p.PrizePercentage}");
        }

        File.WriteAllLines(fileName.FullFilePath(), lines);
    }
}
