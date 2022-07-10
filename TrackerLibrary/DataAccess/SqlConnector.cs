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
    /// Saves a new prize to the database.
    /// </summary>
    /// <param name="prizeModel">The prize information.</param>
    /// <returns>The prize information, including the unique identifier.</returns>
    public PrizeModel CreatePrize(PrizeModel prizeModel)
    {
        using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString("Tournaments")))
        {
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
}
