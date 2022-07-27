using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.DataAccess.TextHelpers;
using TrackerLibrary.Models;

namespace TrackerLibrary.AutoMapperConverters;

public class MatchupEntryConverter : ITypeConverter<string, List<MatchupEntryModel>>
{
    public List<MatchupEntryModel> Convert(string source, List<MatchupEntryModel> destination, ResolutionContext context)
    {
        int[] ids = source.Split('|').Where(x => int.TryParse(x,out int id)).Select(x => int.Parse(x)).ToArray();

        destination = GlobalConfig.MatchupEntriesFile
            .FullFilePath()
            .LoadFile()
            .Where(x => ids.Contains(int.Parse(x.Split(',')[0])))
            .ToList()
            .ConvertToListOfModels<MatchupEntryModel>();

        return destination;
    }
}