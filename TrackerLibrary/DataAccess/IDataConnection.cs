using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess;

public interface IDataConnection
{
    void CreatePrize(PrizeModel prizeModel);
    void CreatePerson(PersonModel personModel);
    void CreateTeam(TeamModel teamModel);
    void CreateTournament(TournamentModel tournamentModel);

    void UpdateMatchup(MatchupModel matchupModel);

    void CompleteTournament(TournamentModel tournamentModel); 

    List<PersonModel> GetPerson_All();
    List<TeamModel> GetTeam_All();
    List<TournamentModel> GetTournament_All();
}
