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
    /// Two teams in this matchup.
    /// </summary>
    public List<MatchupEntryModel> Entries { get; set; } = new();
    /// <summary>
    /// The winner of this matchup.
    /// </summary>
    public TeamModel Winner { get; set; }
    /// <summary>
    /// The number of a round where this matchup played.
    /// </summary>
    public int MatchupRound { get; set; }
}
