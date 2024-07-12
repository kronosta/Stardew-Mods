using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Framework;
using StardewValley;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Reflection;

namespace InlineDGA
{
    public class ModEntry : Mod
    {
        public class DynamicManifestContentPackFor : IManifestContentPackFor
        {
            public string UniqueID { get; set; } = "spacechase0.DynamicGameAssets";
            public ISemanticVersion? MinimumVersion { get; set; } = null;
        }
        public class DynamicManifest : IManifest
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Author { get; set; }
            public ISemanticVersion Version { get; set; }
            public ISemanticVersion? MinimumApiVersion { get; set; } = null;
            public string UniqueID { get; set; }
            public string? EntryDll { get; set; } = null;
            public IManifestContentPackFor ContentPackFor { get; set; } = new DynamicManifestContentPackFor();
            public IManifestDependency[] Dependencies { get; set; } = new IManifestDependency[0];
            public string[] UpdateKeys { get; set; } = new string[0];
            public IDictionary<string, object> ExtraFields { get; set; }

            public DynamicManifest(string name, string description, string author, ISemanticVersion version, string uniqueID, IDictionary<string, object> extraFields)
            {
                Name = name;
                Description = description;
                Author = author;
                Version = version;
                UniqueID = uniqueID;
                ExtraFields = extraFields;
            }
        }

        private Tuple<string, int, int, int, int> _png_startingPatch = null;
        private List<Tuple<string, int, int, int, int, int, int>> _png_imagePatches = null;

        private IDGAApi _DGAApi;
        public override void Entry(IModHelper helper)
        {
            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            Helper.Events.Content.AssetRequested += OnAssetRequested;
            Helper.Events.GameLoop.DayStarted += OnDayStarted;
            Helper.Events.GameLoop.Saving += OnSaving;
        }

        public void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            _DGAApi = Helper.ModRegistry.GetApi<IDGAApi>("spacechase0.DynamicGameAssets");
        }

        public void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Kronosta/InlineDGA/Packs"))
            {
                e.LoadFrom(() => new Dictionary<string, string> { }, AssetLoadPriority.Exclusive);
            }
        }
        public void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            if (!Directory.Exists($"{Helper.DirectoryPath}/Data/{Constants.SaveFolderName}")) 
            {
                Directory.CreateDirectory($"{Helper.DirectoryPath}/Data/{Constants.SaveFolderName}");
            }
            Helper.ConsoleCommands.Trigger("dga_reload", new string[] {});
            foreach (var path in Directory.EnumerateDirectories($"{Helper.DirectoryPath}/Data/{Constants.SaveFolderName}"))
            {
                string[] pathElements = path.Split(Path.DirectorySeparatorChar);
                if (pathElements.Any(x => x.StartsWith("Pack_")))
                {
                    string exactName = pathElements.Where(x => x.StartsWith("Pack_")).ToArray()[0];
                    _DGAApi.AddEmbeddedPack(
                        new DynamicManifest(
                            name: $"[DGA] {exactName.Substring(5)}",
                            description: ".",
                            author: "Kronosta",
                            version: new SemanticVersion(1, 0, 0),
                            uniqueID: $"Kronosta.InlineDGA__{exactName}",
                            new Dictionary<string, object> { ["DGA.FormatVersion"] = 2, ["DGA.ConditionsFormatVersion"] = "1.29.0" }
                        ),
                        $"{Helper.DirectoryPath}/Data/{Constants.SaveFolderName}/{exactName}"
                    );
                }
            }
        }

        public void OnSaving(object sender, SavingEventArgs e)
        {
            string savePath = $"{Helper.DirectoryPath}/Data/{Constants.SaveFolderName}";
            foreach (var i in Game1.content.Load<Dictionary<string,string>>("Data/Kronosta/InlineDGA/Packs"))
            {
                SaveDGAPack(savePath, i.Key, i.Value);
            }
        }

        public void SaveDGAPack(string savePath, string packName, string packValue)
        {
            if (Directory.Exists($"{savePath}/Pack_{packName}"))
            {
                Directory.Delete($"{savePath}/Pack_{packName}", true);
            }
            Directory.CreateDirectory($"{savePath}/Pack_{packName}/i18n");
            ProcessFiles(savePath, packName, packValue);
        }

        public void ProcessFiles(string savePath, string packName, string packValue)
        {
            string[] files = packValue.Split("^");
            foreach (string file in files)
            {
                string filename = file.Substring(0, file.IndexOf(":"));
                if (filename.EndsWith(".json"))
                {
                    string contents = file.Substring(file.IndexOf(":") + 1)
                        .Replace("`", "\"")
                        .Replace("@g", "`")
                        .Replace("@c", "^")
                        .Replace("@(", "{")
                        .Replace("@)", "}")
                        .Replace("@!", $"Kronosta.InlineDGA__Pack_{packName}")
                        .Replace("@a", "@");
                    using (var sw = new StreamWriter($"{savePath}/Pack_{packName}/{filename.Trim()}"))
                    {
                        sw.Write(contents);
                    }
                }
                else if (filename.EndsWith(".png"))
                {
                    string[] contents = file.Substring(file.IndexOf(":") + 1).Split(",");
                    string source = contents[0];
                    Texture2D texture = Game1.content.Load<Texture2D>(source.Trim());
                    using (var stream = new FileStream($"{Helper.DirectoryPath}/Data/{Constants.SaveFolderName}/Pack_{packName}/{filename.Trim()}", FileMode.Create))
                    {
                        texture.SaveAsPng(stream, texture.Width, texture.Height);
                    }

                }
            }
        }
    }
}
