using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary;
/// <summary>
/// Represents the matchup of two teams in a round.
/// </summary>
public class MatchupModel
{
    /// <summary>
    /// Represents teams in this matchup.
    /// </summary>
    public List<MatchupEntryModel> Entries { get; set; } = new();
    /// <summary>
    /// Represents the winner of a matchup.
    /// </summary>
    public TeamModel Winner { get; set; }
    /// <summary>
    /// Represents what round is this matchup in.
    /// </summary>
    public int MatchupRound { get; set; }
}
