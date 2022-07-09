using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary;
/// <summary>
/// Represents the team competing in tournament.
/// </summary>
public class TeamModel
{
    /// <summary>
    /// Represents the list of members in this team.
    /// </summary>
    public List<PersonModel> TeamMembers { get; set; } = new();
    /// <summary>
    /// Represents the name of this team.
    /// </summary>
    public string TeamName { get; set; }
}
