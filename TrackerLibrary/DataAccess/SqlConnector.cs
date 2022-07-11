using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess;

public class SqlConnector : IDataConnection
{
    private const string db = "Tournaments";

    /// <summary>
    /// Saves a new person to the databse.
    /// </summary>
    /// <param name="personModel">The person information.</param>
    /// <returns>The person information, including unique identifier</returns>
    public PersonModel CreatePerson(PersonModel personModel)
    {
        using IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db));

        DynamicParameters p = new();
        p.Add("@FirstName", personModel.FirstName);
        p.Add("@LastName", personModel.LastName);
        p.Add("@EmailAddress", personModel.EmailAdress);
        p.Add(@"CellphoneNumber", personModel.CellphoneNumber);
        p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

        connection.Execute("dbo.spPeople_Insert", p, commandType: CommandType.StoredProcedure);

        personModel.Id = p.Get<int>("@id");

        return personModel;
    }

    /// <summary>
    /// Saves a new prize to the database.
    /// </summary>
    /// <param name="prizeModel">The prize information.</param>
    /// <returns>The prize information, including the unique identifier.</returns>
    public PrizeModel CreatePrize(PrizeModel prizeModel)
    {
        using IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db));

        DynamicParameters p = new();
        p.Add("@PlaceNumber", prizeModel.PlaceNumber);
        p.Add("@PlaceName", prizeModel.PlaceName);
        p.Add("@PrizeAmount", prizeModel.PrizeAmount);
        p.Add("@PrizePercentage", prizeModel.PrizePercentage);
        p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

        connection.Execute("dbo.spPrizes_Insert", p, commandType: CommandType.StoredProcedure);

        prizeModel.Id = p.Get<int>("@id");

        return prizeModel;
    }

    /// <summary>
    /// Saves a new team to the database.
    /// </summary>
    /// <param name="teamModel">The team information.</param>
    /// <returns>The team information, including the unique identifier.</returns>
    public TeamModel CreateTeam(TeamModel teamModel)
    {
        using IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db));
        DynamicParameters p = new();
        p.Add("@TeamName", teamModel.TeamName);
        p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

        connection.Execute("dbo.spTeams_Insert", p, commandType: CommandType.StoredProcedure);
        teamModel.Id = p.Get<int>("@id");

        foreach (PersonModel person in teamModel.TeamMembers)
        {
            p = new DynamicParameters();
            p.Add("@TeamId", teamModel.Id);
            p.Add("@PersonId", person.Id);

            connection.Execute("dbo.spTeamMembers_Insert", p, commandType: CommandType.StoredProcedure);
        }

        return teamModel;
    }

    /// <summary>
    /// Get all people from the database.
    /// </summary>
    /// <returns>List of PersonModel containing all entries from the database.</returns>
    public List<PersonModel> GetPerson_All()
    {
        using IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db));
        List<PersonModel> result = connection.Query<PersonModel>("dbo.spPeople_GetAll").ToList();

        return result;
    }

    /// <summary>
    /// Get all teams from the database.
    /// </summary>
    /// <returns>List of teams containing all entries from the database.</returns>
    public List<TeamModel> GetTeam_All()
    {
        using IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db));
        List<TeamModel> result = connection.Query<TeamModel>("dbo.spTeams_GetAll").ToList();
        foreach (TeamModel team in result)
        {
            DynamicParameters p = new();
            p.Add("@TeamId", team.Id);
            team.TeamMembers = connection.Query<PersonModel>("dbo.spTeamMembers_GetByTeam",p, commandType: CommandType.StoredProcedure).ToList();
        }

        return result;
    }
}
