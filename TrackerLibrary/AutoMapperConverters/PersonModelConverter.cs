using AutoMapper;
using TrackerLibrary.DataAccess.TextHelpers;
using TrackerLibrary.Models;

namespace TrackerLibrary.AutoMapperConverters;

class PersonModelConverter : ITypeConverter<string, List<PersonModel>>,ITypeConverter<string, PersonModel>
{
    public List<PersonModel> Convert(string source, List<PersonModel> destination, ResolutionContext context)
    {
        int[] ids = source.Split('|').Where(x => int.TryParse(x, out int id)).Select(x => int.Parse(x)).ToArray();

        destination = GlobalConfig.PeopleFile
            .FullFilePath()
            .LoadFile()
            .Where(x => ids.Contains(int.Parse(x.Split(',')[0])))
            .ToList()
            .ConvertToListOfModels<PersonModel>();

        return destination;
    }

    public PersonModel Convert(string source, PersonModel destination, ResolutionContext context)
    {
        if (string.IsNullOrEmpty(source))
            return null;
        destination = GlobalConfig.PeopleFile
            .FullFilePath()
            .LoadFile()
            .Select(x => x.Split(','))
            .First(x => x[0] == source)
            .ConvertStringToModel<PersonModel>();

        return destination;
    }
}
