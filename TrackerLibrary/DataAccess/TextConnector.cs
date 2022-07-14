﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;
using TrackerLibrary.DataAccess.TextHelpers;
using System.Configuration;

namespace TrackerLibrary.DataAccess;

public class TextConnector : IDataConnection
{
    private const string PrizesFile = "PrizeModels.csv";
    private const string PeopleFile = "PersonModels.csv";
    private const string TeamsFile = "TeamModels.csv";
    private const string TournamentsFile = "TournamentModels.csv";
    private const string MatchupsFile = "MatchupModels.csv";
    private const string MatchupEntriesFile = "MatchupEntryModels.csv";

    public TextConnector()
    {
        EnsureFolderCreated();
    }

    private static void EnsureFolderCreated()
    {
        string path = ConfigurationManager.AppSettings["filePath"];
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }

    /// <summary>
    /// Saves a new prize to the text file
    /// </summary>
    /// <param name="prizeModel">The prize information.</param>
    /// <returns>The prize information, including the unique identifier.</returns>
    public PrizeModel CreatePrize(PrizeModel prizeModel)
    {
        //Load the text file and convert text to List<PrizeModel>
        var prizes = PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();

        //Find the ID
        int currentId = 1;
        if (prizes.Count > 0)
            currentId = prizes.OrderByDescending(x => x.Id).First().Id + 1;

        //Add the new record with the new ID(max ID +1)
        prizeModel.Id = currentId;

        prizes.Add(prizeModel);

        //Convert the prizes to List<string>
        //Save List<string> to the text file (overwrite)
        prizes.SaveToPrizeFile(PrizesFile);

        return prizeModel;
    }

    /// <summary>
    /// Saves a new person to the text file.
    /// </summary>
    /// <param name="personModel">The person information.</param>
    /// <returns>The person information, includiong the unique identifier.</returns>
    public PersonModel CreatePerson(PersonModel personModel)
    {
        List<PersonModel> persons = PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();

        int currentId = 1;
        if (persons.Count > 0)
            currentId = persons.OrderByDescending(x => x.Id).First().Id + 1;

        personModel.Id = currentId;

        persons.Add(personModel);
        persons.SaveToPeopleFile(PeopleFile);

        return personModel;
    }

    /// <summary>
    /// Saves a new team to the text file.
    /// </summary>
    /// <param name="teamModel">The team information.</param>
    /// <returns>The team information, including the unique identifier.</returns>
    public TeamModel CreateTeam(TeamModel teamModel)
    {
        List<TeamModel> teams = TeamsFile.FullFilePath().LoadFile().ConvertToTeamModels();

        int currentId = 1;
        if (teams.Count > 0) currentId = teams.OrderByDescending(x => x.Id).First().Id + 1;

        teamModel.Id = currentId;

        teams.Add(teamModel);

        teams.SaveToTeamsFile(GlobalConfig.TeamsFile);

        return teamModel;
    }

    /// <summary>
    /// Saves a new tournament to the text file.
    /// </summary>
    /// <param name="tournamentModel"></param>
    public void CreateTournament(TournamentModel tournamentModel)
    {
        List<TournamentModel> tournaments = TournamentsFile
            .FullFilePath()
            .LoadFile()
            .ConvertToTournamentModels();

        int currentId = 1;
        if (tournaments.Count > 0)
            currentId = tournaments.OrderByDescending(x => x.Id).First().Id + 1;

        tournamentModel.Id = currentId;

        tournamentModel.SaveRoundsToFile(MatchupsFile, MatchupEntriesFile);

        tournaments.Add(tournamentModel);

        tournaments.SaveToTournamentsFile(TournamentsFile);
    }

    /// <summary>
    /// Gets all person from the text file.
    /// </summary>
    /// <returns>List of PersonModel containing all entries from the database.</returns>
    public List<PersonModel> GetPerson_All()
    {
        return PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();
    }

    /// <summary>
    /// Gets all teams from the text file.
    /// </summary>
    /// <returns>List of TeamModel containing all entries from the database.</returns>
    public List<TeamModel> GetTeam_All()
    {
        return TeamsFile.FullFilePath().LoadFile().ConvertToTeamModels();
    }

    /// <summary>
    /// Gets all tournaments from the text file.
    /// </summary>
    /// <returns>List of TournamentModel containing all entries from the database.</returns>
    public List<TournamentModel> GetTournament_All()
    {
        return GlobalConfig.TournamentsFile.FullFilePath().LoadFile().ConvertToTournamentModels();
    }

    public void UpdateMatchup(MatchupModel matchupModel)
    {
        matchupModel.UpdateMathcupToFile();
    }
}
