using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary;

public static class GlobalConfig
{
    public static List<IDataConnection> Connections { get; private set; } = new();

    public static void InitializeConnections(bool database,bool textFiles)
    {
        if (database)
        {
            // TODO - Set up the sql connector properly
            SqlConnector sql = new();
            Connections.Add(sql);
        }

        if(textFiles)
        {
            // TODO - Set up the text file connector properly
            TextConnector text = new();
            Connections.Add(text);
        }
    }
}
