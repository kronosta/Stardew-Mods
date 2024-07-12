using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeciesOrbs
{
#nullable enable
    public class IL_Instruction
    {
        // ID exists only so that patches to the species' custom event handlers can
        // reorder, remove, and add operations using content patcher.
        public string ID { get; set; }
        public string Opcode { get; set; }
        public byte? Byte { get; set; }
        public string? Type { get; set; }
        public List<string>? ConstructorParams { get; set; }
        public double? Double { get; set; }
        public string? Field { get; set; }
        public Int16? Int16 { get; set; }
        public Int32? Int32 { get; set; }
        public Int64? Int64 { get; set; }
        public List<string>? LabelNames { get; set; }
        public string? MethodName { get; set; }
        public List<string>? MethodParams { get; set; }
        public SByte? SByte { get; set; }
        public Single? Single { get; set; }
        public String? String { get; set; }
    }
#nullable disable
}
