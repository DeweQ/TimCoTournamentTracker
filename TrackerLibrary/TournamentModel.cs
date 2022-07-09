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
    /// Represents the name for this tournament.
    /// </summary>
    public string TournamentName { get; set; }
    /// <summary>
    /// Represents the entry fee for this tournament
    /// if it has one.
    /// </summary>
    public decimal EntryFee { get; set; }
    /// <summary>
    /// Represents the list of teams entered this tournament.
    /// </summary>
    public List<TeamModel> EnteredTeams { get; set; } = new();
    /// <summary>
    /// Represents the list of prizes for this tournament,
    /// if it has one.
    /// </summary>
    public List<PrizeModel> Prizes { get; set; } = new();
    /// <summary>
    /// Represents the list of rounds for this tournament.
    /// </summary>
    public List<List<MatchupModel>> Rounds { get; set; } = new();
}
