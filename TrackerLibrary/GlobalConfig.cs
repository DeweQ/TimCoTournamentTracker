using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.DataAccess;
using AutoMapper;

namespace TrackerLibrary;

public static class GlobalConfig
{
    public const string PrizesFile = "PrizeModels.csv";
    public const string PeopleFile = "PersonModels.csv";
    public const string TeamsFile = "TeamModels.csv";
    public const string TournamentsFile = "TournamentModels.csv";
    public const string MatchupsFile = "MatchupModels.csv";
    public const string MatchupEntriesFile = "MatchupEntryModels.csv";

    private static IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

    public static Settings Settings = config.GetRequiredSection("Settings").Get<Settings>();

    public static IDataConnection Connection { get; private set; }

    private static MapperConfiguration cfg = new(cfg => cfg.AddProfile(new AutoMapperProfile()));

    public static IMapper Mapper = cfg.CreateMapper();


    public static void InitializeConnections(DatabaseType db)
    {
        if (db == DatabaseType.Sql)
        {
            SqlConnector sql = new();
            Connection = sql;
        }

        else if (db == DatabaseType.TextFile)
        {
            TextConnector text = new();
            Connection = text;
        }
    }

    public static string CnnString(string name)
    {
        return Settings.ConnectionStrings[name];

    }
}
