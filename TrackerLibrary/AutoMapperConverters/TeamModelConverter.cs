using AutoMapper;
using TrackerLibrary.DataAccess.TextHelpers;
using TrackerLibrary.Models;

namespace TrackerLibrary.AutoMapperConverters;

public class TeamModelConverter : ITypeConverter<string, List<TeamModel>>,ITypeConverter<string, TeamModel>
{
    public List<TeamModel> Convert(string source, List<TeamModel> destination, ResolutionContext context)
    {
        int[] ids = source.Split('|').Where(x => int.TryParse(x, out int id)).Select(x => int.Parse(x)).ToArray();

        destination = GlobalConfig.TeamsFile
            .FullFilePath()
            .LoadFile()
            .Where(x => ids.Contains(int.Parse(x.Split(',')[0])))
            .ToList()
            .ConvertToListOfModels<TeamModel>();

        return destination;
    }

    public TeamModel Convert(string source, TeamModel destination, ResolutionContext context)
    {
        if (string.IsNullOrEmpty(source))
            return null;
        destination = GlobalConfig.TeamsFile
            .FullFilePath()
            .LoadFile()
            .Select(x => x.Split(','))
            .First(x => x[0] == source)
            .ConvertStringToModel<TeamModel>();

        return destination;
    }
}