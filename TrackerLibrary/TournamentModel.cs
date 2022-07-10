using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary;
/// <summary>
/// Represents the tournament.
/// </summary>
public class TournamentModel
{
    /// <summary>
    /// The name for this tournament.
    /// </summary>
    public string TournamentName { get; set; }
    /// <summary>
    /// The entry fee for this tournament or zero if it has no entry fee.
    /// </summary>
    public decimal EntryFee { get; set; }
    /// <summary>
    /// The list of teams that entered this tournament.
    /// </summary>
    public List<TeamModel> EnteredTeams { get; set; } = new();
    /// <summary>
    /// The list of prizes for this tournament,
    /// if it has any.
    /// </summary>
    public List<PrizeModel> Prizes { get; set; } = new();
    /// <summary>
    /// The list of rounds for this tournament.
    /// </summary>
    public List<List<MatchupModel>> Rounds { get; set; } = new();
}
