using Force.DeepCloner;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Locations;

namespace Kronosta.SeparateFarms
{
    public class ModEntry : Mod
    {
        private Dictionary<string, string> extraFarms = new Dictionary<string, string>();
        private ModConfig config;
        public override void Entry(IModHelper helper)
        {
            config = Helper.ReadConfig<ModConfig>();
            Helper.Events.Content.AssetRequested += OnAssetRequested;
            Helper.ConsoleCommands.Add("create_extra_farm", "Creates an extra farm which can be warped to with the warp_extra_farm command.\n\nUsage: create_extra_farm <location id to clone> <name>", CreateExtraFarmCommand);
            Helper.ConsoleCommands.Add("warp_extra_farm", "Warps to an extra farm given its name specified in create_extra_farm.\n\nUsage: warp_extra_farm <name>", WarpExtraFarmCommand);
            Helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
            Helper.Events.Multiplayer.PeerContextReceived += OnPeerContextReceived;
        }

        public void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Locations"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, LocationData>().Data;
                    foreach (var farm in extraFarms)
                    {
                        data[$"{Helper.ModRegistry.ModID}_{farm.Key}"] = asset.AsDictionary<string, LocationData>().Data[farm.Value];
                        data[$"{Helper.ModRegistry.ModID}_{farm.Key}"].CreateOnLoad = new CreateLocationData
                        {
                            AlwaysActive = false,
                            MapPath = config.FarmMapAssetPaths.ContainsKey(farm.Value) ? config.FarmMapAssetPaths[farm.Value] : $"Maps\\{farm.Value}",
                            Type = "StardewValley.GameLocation"
                        };
                    }
                });
                return;
            }
            /*
            var locationDict = Helper.GameContent.Load<Dictionary<string, LocationData>>("Data/Locations");
            foreach (var farm in extraFarms)
            {
                if (e.NameWithoutLocale.IsEquivalentTo(locationDict[farm.Key].CreateOnLoad.MapPath))
                {
                    e.Edit(asset =>
                    {
                        var map = asset.AsMap().Data;
                        map.Properties["CanBuildHere"] = "T";
                    });
                }
            }
            */
        }

        public void CreateExtraFarmCommand(string command, string[] args)
        {
            if (!Game1.player.IsMainPlayer)
            {
                Monitor.Log("You must be the host player to use this command.", LogLevel.Error);
                return;
            }
            if (args.Length < 1)
            {
                Monitor.Log("No farm type was provided as an argument.\nUse Farm_Standard, Farm_Beach, Farm_Riverland, Farm_Wilderness, Farm_Hilltop, Farm_FourCorners, or Farm_Forest for a default farm type, or a location ID for a custom farm type in Data/Locations.", LogLevel.Error);
                return;
            }
            if (args.Length < 2)
            {
                Monitor.Log("No name for the cloned farm was provided as an argument. You must give the cloned farm a name to refer to it in warp_extra_farm.", LogLevel.Error);
                return;
            }
            extraFarms.Add(args[1], args[0]);
            CreateExtraFarmMessage message = new CreateExtraFarmMessage
            {
                Name = args[1],
                CloneFrom = args[0]
            };
            Helper.Multiplayer.SendMessage(message, "CreateExtraFarm");
            Helper.GameContent.InvalidateCache("Data/Locations");
            Helper.GameContent.Load<Dictionary<string,LocationData>>("Data/Locations");
            GameLocation loc = Game1.CreateGameLocation($"{Helper.ModRegistry.ModID}_{args[1]}");
            Game1.locations.Add(loc);
        }

        public void WarpExtraFarmCommand(string command, string[] args)
        {
            if (args.Length < 1)
            {
                Monitor.Log($"No name was given.", LogLevel.Error);
                return;
            }
            Utility.ForEachLocation(location => { Monitor.Log(location.Name, LogLevel.Trace); return true; });
            Game1.getLocationFromName($"{Helper.ModRegistry.ModID}_{args[0]}")?.removeObjectsAndSpawned(8, 8, 1, 1);
            Game1.warpFarmer($"{Helper.ModRegistry.ModID}_{args[0]}", 8, 8, 2);
        }

        public void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID != $"{Helper.ModRegistry.ModID}") return;
            switch (e.Type)
            {
                case "NotifyExtraFarms":
                    extraFarms = e.ReadAs<Dictionary<string, string>>();
                    Helper.GameContent.InvalidateCache("Data/Locations");
                    Helper.GameContent.Load<Dictionary<string, LocationData>>("Data/Locations");
                    foreach (var farm in extraFarms)
                    {
                        GameLocation loc = Game1.getLocationFromName($"{Helper.ModRegistry.ModID}_{farm.Key}") ?? Game1.CreateGameLocation($"{Helper.ModRegistry.ModID}_{farm.Key}");
                    }
                    break;
                case "CreateExtraFarm":
                    {
                        CreateExtraFarmMessage message = e.ReadAs<CreateExtraFarmMessage>();
                        extraFarms.Add(message.Name, message.CloneFrom);
                        Helper.GameContent.InvalidateCache("Data/Locations");
                        Helper.GameContent.Load<Dictionary<string, LocationData>>("Data/Locations");
                        GameLocation loc = Game1.getLocationFromName($"{Helper.ModRegistry.ModID}_{message.Name}") ?? Game1.CreateGameLocation($"{Helper.ModRegistry.ModID}_{message.Name}");
                        break;
                    }
            }
        }

        public void OnPeerContextReceived(object sender, PeerContextReceivedEventArgs e)
        {
            this.Helper.Multiplayer.SendMessage(extraFarms, "NotifyExtraFarms");
        }
    }
}