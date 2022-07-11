using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models;
/// <summary>
/// Represents the team competing in tournament.
/// </summary>
public class TeamModel
{
    /// <summary>
    /// The unique identifier for the team.
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// The name of the team.
    /// </summary>
    public string TeamName { get; set; }
    /// <summary>
    /// The list of members in this team.
    /// </summary>
    public List<PersonModel> TeamMembers { get; set; } = new();
}
