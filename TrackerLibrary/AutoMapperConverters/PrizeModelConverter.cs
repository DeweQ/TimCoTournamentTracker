using AutoMapper;
using TrackerLibrary.DataAccess.TextHelpers;
using TrackerLibrary.Models;

namespace TrackerLibrary.AutoMapperConverters;

public class PrizeModelConverter : ITypeConverter<string, List<PrizeModel>>
{
    public List<PrizeModel> Convert(string source, List<PrizeModel> destination, ResolutionContext context)
    {
        if (string.IsNullOrWhiteSpace(source))
            return new List<PrizeModel>();
        int[] ids = source.Split('|').Where(x => int.TryParse(x, out int id)).Select(x => int.Parse(x)).ToArray();

        destination = GlobalConfig.PrizesFile
            .FullFilePath()
            .LoadFile()
            .Where(x => ids.Contains(int.Parse(x.Split(',')[0])))
            .ToList()
            .ConvertToListOfModels<PrizeModel>();

        return destination;
    }
}