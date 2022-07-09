using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary;

public class TournamentModel
{
    public string TournamentName { get; set; }
    public decimal EntryFee { get; set; }
    public List<TeamModel> EnteredTeams { get; set; } = new();
    public List<PrizeModel> Prizes { get; set; } = new();
    public List<List<MatchupModel>> Rounds { get; set; } = new();
}
