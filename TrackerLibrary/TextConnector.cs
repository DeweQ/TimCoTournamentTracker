using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary;

public class TextConnector : IDataConnection
{
    // ToDo - Make the CreatePrize method actually save to text file
    /// <summary>
    /// Saves a new prize to the text file
    /// </summary>
    /// <param name="prizeModel">The prize information.</param>
    /// <returns>The prize information, including the unique identifier.</returns>
    public PrizeModel CreatePrize(PrizeModel prizeModel)
    {
        prizeModel.Id = 1;

        return prizeModel;
    }
}
