using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models;
/// <summary>
/// Represents the matchup of two teams in a round.
/// </summary>
public class MatchupModel
{
    /// <summary>
    /// The unique identifier for the matchup.
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Two teams in this matchup.
    /// </summary>
    public List<MatchupEntryModel> Entries { get; set; } = new();

    /// <summary>
    /// The ID from the database that will be used to identify the winner
    /// </summary>
    public int WinnerId { get; set; }

    /// <summary>
    /// The winner of this matchup.
    /// </summary>
    public TeamModel Winner { get; set; }
    /// <summary>
    /// The number of a round where this matchup played.
    /// </summary>
    public int MatchupRound { get; set; }

    public string DisplayName
    {
        get
        {
            string result = "Matchup Not Yet Determined";
            if (Entries.All(e => e.TeamCompeting!=null))
                result = string.Join(" vs. ", Entries.Select(e => e.TeamCompeting?.TeamName));
            return result;
        }
    }
}
