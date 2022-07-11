using System;
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
    public TextConnector()
    {
        EnsureFolderCreated();
    }

    private const string PrizesFile = "PrizeModels.csv";

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
        {
            currentId = prizes.OrderByDescending(x => x.Id).First().Id + 1;
        }

        //Add the new record with the new ID(max ID +1)
        prizeModel.Id = currentId;

        prizes.Add(prizeModel);

        //Convert the prizes to List<string>
        //Save List<string> to the text file (overwrite)
        prizes.SaveToPrizeFile(PrizesFile);

        return prizeModel;
    }

    private void EnsureFolderCreated()
    {
        string path = ConfigurationManager.AppSettings["filePath"];
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }
}
