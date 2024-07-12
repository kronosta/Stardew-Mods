using System.Collections.Generic;

namespace Kronosta.SeparateFarms
{
    public class ModConfig
    {
        public int MaxExtraFarms { get; set; } = 8;
        public Dictionary<string, string> FarmMapAssetPaths { get; set; } = new Dictionary<string, string>
        {
            { "Farm_Standard", "Maps\\Farm" },
            { "Farm_Beach", "Maps\\Farm_Island" },
            { "Farm_Foraging", "Maps\\Farm_Foraging" },
            { "Farm_FourCorners", "Maps\\Farm_FourCorners" },
            { "Farm_Hilltop", "Maps\\Farm_Mining" },
            { "Farm_Riverland", "Maps\\Farm_Fishing" },
            { "Farm_Wilderness", "Maps\\Farm_Combat" },
        };
        public Dictionary<string, Position> FarmMapWarpTiles { get; set; } = new Dictionary<string, Position>();
    }

    public class Position
    {
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
    }
}
