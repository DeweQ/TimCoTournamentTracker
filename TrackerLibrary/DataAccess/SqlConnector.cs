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
    /// <summary>
    /// Saves a new person to the databse.
    /// </summary>
    /// <param name="personModel">The person information.</param>
    /// <returns>The person information, including unique identifier</returns>
    public PersonModel CreatePerson(PersonModel personModel)
    {
        using IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString("Tournaments"));

        var p = new DynamicParameters();
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
        using IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString("Tournaments"));

        var p = new DynamicParameters();
        p.Add("@PlaceNumber", prizeModel.PlaceNumber);
        p.Add("@PlaceName", prizeModel.PlaceName);
        p.Add("@PrizeAmount", prizeModel.PrizeAmount);
        p.Add("@PrizePercentage", prizeModel.PrizePercentage);
        p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

        connection.Execute("dbo.spPrizes_Insert", p, commandType: CommandType.StoredProcedure);

        prizeModel.Id = p.Get<int>("@id");

        return prizeModel;
    }
}
