using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeciesOrbs
{
#nullable enable
    public class Species
    {
        public List<IL_Instruction>? AssetRequested { get; set; }
        public List<IL_Instruction>? AssetsInvalidated { get; set; }
        public List<IL_Instruction>? AssetReady { get; set; }
        public List<IL_Instruction>? LocaleChanged { get; set; }
        public List<IL_Instruction>? MenuChanged { get; set; }
        public List<IL_Instruction>? Rendering { get; set; }
        public List<IL_Instruction>? Rendered { get; set; }
        public List<IL_Instruction>? RenderingWorld { get; set; }
        public List<IL_Instruction>? RenderedWorld { get; set; }
        public List<IL_Instruction>? RenderingActiveMenu { get; set; }
        public List<IL_Instruction>? RenderedActiveMenu { get; set; }
        public List<IL_Instruction>? RenderingHud { get; set; }
        public List<IL_Instruction>? RenderedHud { get; set; }
        public List<IL_Instruction>? WindowResized { get; set; }
        public List<IL_Instruction>? UpdateTicking { get; set; }
        public List<IL_Instruction>? UpdateTicked { get; set; }
        public List<IL_Instruction>? OneSecondUpdateTicking { get; set; }
        public List<IL_Instruction>? OneSecondUpdateTicked { get; set; }
        public List<IL_Instruction>? Saving { get; set; }
        public List<IL_Instruction>? Saved { get; set; }
        public List<IL_Instruction>? DayStarted { get; set; }
        public List<IL_Instruction>? DayEnding { get; set; }
        public List<IL_Instruction>? TimeChanged { get; set; }
        public List<IL_Instruction>? ButtonsChanged { get; set; }
        public List<IL_Instruction>? ButtonPressed { get; set; }
        public List<IL_Instruction>? ButtonReleased { get; set; }
        public List<IL_Instruction>? CursorMoved { get; set; }
        public List<IL_Instruction>? MouseWheelScrolled { get; set; }
        public List<IL_Instruction>? InventoryChanged { get; set; }
        public List<IL_Instruction>? LevelChanged { get; set; }
        public List<IL_Instruction>? Warped { get; set; }
        public List<IL_Instruction>? LocationListChanged { get; set; }
        public List<IL_Instruction>? BuildingListChanged { get; set; }
        public List<IL_Instruction>? ChestInventoryChanged { get; set; }
        public List<IL_Instruction>? DebrisListChanged { get; set; }
        public List<IL_Instruction>? FurnitureListChanged { get; set; }
        public List<IL_Instruction>? LargeTerrainFeatureListChanged { get; set; }
        public List<IL_Instruction>? ObjectListChanged { get; set; }
        public List<IL_Instruction>? NpcListChanged { get; set; }
        public List<IL_Instruction>? TerrainFeatureListChanged { get; set; }
    }
#nullable disable
}
