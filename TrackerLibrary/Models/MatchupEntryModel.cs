using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models;
/// <summary>
/// Represents the entry for particular team in a mathcup.
/// </summary>
public class MatchupEntryModel
{
    /// <summary>
    /// The team in a given matchup.
    /// </summary>
    public TeamModel TeamCompeting { get; set; }
    /// <summary>
    /// The score for this team.
    /// </summary>
    public double Score { get; set; }
    /// <summary>
    /// The matchup that this team came
    /// from as a winner.
    /// </summary>
    public MatchupModel ParentMatchup { get; set; }
}
