using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InlineDGA
{
    public interface IDGAApi
    {
        public void AddEmbeddedPack(IManifest manifest, string dir);
    }
}
