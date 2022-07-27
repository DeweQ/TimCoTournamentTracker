using AutoMapper;
using TrackerLibrary.DataAccess.TextHelpers;
using TrackerLibrary.Models;

namespace TrackerLibrary.AutoMapperConverters;

public class MatchupModelConverter : ITypeConverter<string, List<List<MatchupModel>>>,ITypeConverter<string,MatchupModel>
{
    public List<List<MatchupModel>> Convert(string source, List<List<MatchupModel>> destination, ResolutionContext context)
    {
        string[] rounds = source.Split('|').ToArray();
        var matchupStrings = GlobalConfig.MatchupsFile
            .FullFilePath()
            .LoadFile();

        destination = new();
        foreach (var round in rounds)
        {
            int[] roundIds = round.Split('^').Select(x => int.Parse(x)).ToArray();
            var list = matchupStrings
                .Where(x => roundIds.Contains(int.Parse(x.Split(',')[0])))
                .ToList();
            destination.Add(list
                .ConvertToListOfModels<MatchupModel>());
        }

        return destination;
    }

    public MatchupModel Convert(string source, MatchupModel destination, ResolutionContext context)
    {
        if (string.IsNullOrEmpty(source))
            return null;
        int id = int.Parse(source);
        destination = GlobalConfig.MatchupsFile
            .FullFilePath()
            .LoadFile()
            .Select(x => x.Split(','))
            .First(x => int.Parse(x[0]) == id)
            .ConvertStringToModel<MatchupModel>();

        return destination;
    }
}