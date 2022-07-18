using TrackerLibrary;

namespace TrackerUI;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();


        //Initialize the database connection.
        GlobalConfig.InitializeConnections(GlobalConfig.Settings.DatabaseType);


        Application.Run(new TournamentDashboardForm());
        //Application.Run(new TournamentDashboardForm());
    }
}