using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary;

public class TeamModel
{
    public List<PersonModel> TeamMembers { get; set; } = new();
    public string TeamName { get; set; }
}
