using System.Net.Mail;

namespace TrackerLibrary;

public class Settings
{
    public string FilePath { get; init; }
    public bool GreaterWins { get; init; }
    public string SenderEmail { get; init; }
    public string SenderDisplayName { get; init; }
    public IReadOnlyDictionary<string,string> ConnectionStrings { get; init; }
    public SmtpClient smtp { get; init; }
    public DatabaseType DatabaseType { get; init; }
}