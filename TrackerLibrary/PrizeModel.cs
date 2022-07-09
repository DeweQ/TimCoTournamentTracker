using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary;
/// <summary>
/// Represents the prize for a tournament.
/// </summary>
public class PrizeModel
{
    /// <summary>
    /// Represents what place this prize for.
    /// </summary>
    public int PlaceNumber { get; set; }
    /// <summary>
    /// Represents a custom name for this place.
    /// </summary>
    public string PlaceName { get; set; }
    /// <summary>
    /// Represents the amount of prize for this place.
    /// </summary>
    public decimal PrizeAmount { get; set; }
    /// <summary>
    /// Represents the persentage of prize for this place.
    /// </summary>
    public double PrizePresentage { get; set; }
}
