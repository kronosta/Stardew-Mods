using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Logging;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.GameData.Buildings;
using StardewValley.GameData.Locations;
using StardewValley.SaveMigrations;
using System.Runtime.CompilerServices;
using HarmonyLib;
using xTile;
using Force.DeepCloner;

namespace Kronosta.NestedBuildings
{
    public class ModEntry : Mod
    {
        private List<GameLocation> buildingLocations = new List<GameLocation>();
        private List<GameLocation> toAdd = new List<GameLocation>();
        private static IMonitor staticMonitor;
        private static IModHelper staticHelper;
        public override void Entry(IModHelper helper)
        {
            staticMonitor = Monitor;
            staticHelper = Helper;
            Helper.Events.World.BuildingListChanged += OnBuildingListChanged;
            Helper.Events.GameLoop.DayStarted += OnDayStarted;
            Helper.Events.GameLoop.DayEnding += OnDayEnding;
            Harmony harmony = new Harmony(Helper.ModRegistry.ModID);
            harmony.Patch(
                original: AccessTools.Method(typeof(SaveGame), nameof(SaveGame.loadDataToLocations)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.loadDataToLocations_Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Building), "createIndoors"),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.createIndoors_Prefix))
            );
            /*
            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.findStructure)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.findStructure_Prefix))
            );
            */
        }

        public void OnBuildingListChanged(object sender, BuildingListChangedEventArgs e)
        {
            foreach (var building in e.Added)
            {
                building.indoors?.Value?.isAlwaysActive?.Set(true);
                if (building.indoors?.Value != null && !Game1.locations.Contains(building.indoors?.Value))
                {
                    Game1.locations.Add(building.indoors.Value);
                    buildingLocations.Add(building.indoors.Value);
                }
            }
        }

        public void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            Utility.ForEachLocation(location =>
            {
                RecurseActivateLocation(location);
                return true;
            }, false, false);
            foreach (GameLocation location in toAdd)
            {
                Game1.locations.Add(location);
            }
            toAdd.Clear();
        }

        public void RecurseActivateLocation(GameLocation location)
        {
            foreach (Building building in location.buildings)
            {
                if (building?.indoors?.Value == null) continue;
                toAdd.Add(building.indoors.Value);
                buildingLocations.Add(building.indoors.Value);
                RecurseActivateLocation(building.indoors.Value);
            }
        }

        public void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            buildingLocations.ForEach(l => Game1.locations.Remove(l));
        }

        public static bool loadDataToLocations_Prefix(List<GameLocation> fromLocations)
        {
            //IGameLogger gamelog = (StardewValley.Logging.IGameLogger)typeof(Game1).("log").GetValue(null);
            Dictionary<string, string> formerLocationNames = SaveGame.GetFormerLocationNames();
            if (formerLocationNames.Count > 0)
            {
                foreach (GameLocation fromLocation2 in fromLocations)
                {
                    foreach (NPC npc in fromLocation2.characters)
                    {
                        string curHome = npc.DefaultMap;
                        if (curHome != null && formerLocationNames.TryGetValue(curHome, out var newHome))
                        {
                            //IGameLogger log = gamelog;
                            DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 3);
                            defaultInterpolatedStringHandler.AppendLiteral("Updated ");
                            defaultInterpolatedStringHandler.AppendFormatted(npc.Name);
                            defaultInterpolatedStringHandler.AppendLiteral("'s home from '");
                            defaultInterpolatedStringHandler.AppendFormatted(curHome);
                            defaultInterpolatedStringHandler.AppendLiteral("' to '");
                            defaultInterpolatedStringHandler.AppendFormatted(newHome);
                            defaultInterpolatedStringHandler.AppendLiteral("'.");
                            //log.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
                            npc.DefaultMap = newHome;
                        }
                    }
                }
            }
            Game1.netWorldState.Value.ParrotPlatformsUnlocked = SaveGame.loaded.parrotPlatformsUnlocked;
            Game1.player.team.farmPerfect.Value = SaveGame.loaded.farmPerfect;
            List<GameLocation> loadedLocations = new List<GameLocation>();
            foreach (GameLocation fromLocation in fromLocations)
            {
                GameLocation realLocation2 = Game1.getLocationFromName(fromLocation.name);
                if (realLocation2 == null)
                {
                    if (fromLocation is Cellar)
                    {
                        realLocation2 = Game1.CreateGameLocation("Cellar");
                        if (realLocation2 == null)
                        {
                            //gamelog.Error("Couldn't create 'Cellar' location. Was it removed from Data/Locations?");
                            continue;
                        }
                        realLocation2.name.Value = fromLocation.name.Value;
                        Game1.locations.Add(realLocation2);
                    }
                    /*
                    if (realLocation2 == null)
                    {
                        realLocation2 = Game1.CreateGameLocation(fromLocation.name);
                        Game1.locations.Add(realLocation2);
                    }
                    */
                    if (realLocation2 == null && formerLocationNames.TryGetValue(fromLocation.name, out var realLocationName))
                    {
                        //IGameLogger log2 = gamelog;
                        DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(32, 2);
                        defaultInterpolatedStringHandler.AppendLiteral("Mapped legacy location '");
                        defaultInterpolatedStringHandler.AppendFormatted(fromLocation.Name);
                        defaultInterpolatedStringHandler.AppendLiteral("' to '");
                        defaultInterpolatedStringHandler.AppendFormatted(realLocationName);
                        defaultInterpolatedStringHandler.AppendLiteral("'.");
                        //log2.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
                        realLocation2 = Game1.getLocationFromName(realLocationName);
                    }
                    if (realLocation2 == null)
                    {
                        //gamelog.Warn("Ignored unknown location '" + fromLocation.NameOrUniqueName + "' in save data.");
                        continue;
                    }
                }
                if (!(realLocation2 is Farm farm))
                {
                    if (!(realLocation2 is FarmHouse farmHouse))
                    {
                        if (!(realLocation2 is Forest forest))
                        {
                            if (!(realLocation2 is MovieTheater theater))
                            {
                                if (!(realLocation2 is Town town))
                                {
                                    if (!(realLocation2 is Beach beach))
                                    {
                                        if (!(realLocation2 is Woods woods))
                                        {
                                            if (!(realLocation2 is CommunityCenter communityCenter))
                                            {
                                                if (realLocation2 is ShopLocation shopLocation && fromLocation is ShopLocation fromShopLocation)
                                                {
                                                    shopLocation.itemsFromPlayerToSell.MoveFrom(fromShopLocation.itemsFromPlayerToSell);
                                                    shopLocation.itemsToStartSellingTomorrow.MoveFrom(fromShopLocation.itemsToStartSellingTomorrow);
                                                }
                                            }
                                            else if (fromLocation is CommunityCenter fromCommunityCenter)
                                            {
                                                communityCenter.areasComplete.Set(fromCommunityCenter.areasComplete);
                                            }
                                        }
                                        else if (fromLocation is Woods fromWoods)
                                        {
                                            woods.hasUnlockedStatue.Value = fromWoods.hasUnlockedStatue.Value;
                                        }
                                    }
                                    else if (fromLocation is Beach fromBeach)
                                    {
                                        beach.bridgeFixed.Value = fromBeach.bridgeFixed;
                                    }
                                }
                                else if (fromLocation is Town fromTown)
                                {
                                    town.daysUntilCommunityUpgrade.Value = fromTown.daysUntilCommunityUpgrade;
                                }
                            }
                            else if (fromLocation is MovieTheater fromTheater)
                            {
                                theater.dayFirstEntered.Set(fromTheater.dayFirstEntered);
                            }
                        }
                        else if (fromLocation is Forest fromForest)
                        {
                            forest.obsolete_log = fromForest.obsolete_log;
                        }
                    }
                    else if (fromLocation is FarmHouse fromFarmHouse)
                    {
                        farmHouse.setMapForUpgradeLevel(farmHouse.upgradeLevel);
                        farmHouse.fridge.Value = fromFarmHouse.fridge.Value;
                        farmHouse.ReadWallpaperAndFloorTileData();
                    }
                }
                else if (fromLocation is Farm fromFarm)
                {
                    farm.greenhouseUnlocked.Value = fromFarm.greenhouseUnlocked.Value;
                    farm.greenhouseMoved.Value = fromFarm.greenhouseMoved.Value;
                    farm.hasSeenGrandpaNote = fromFarm.hasSeenGrandpaNote;
                    farm.grandpaScore.Value = fromFarm.grandpaScore;
                    farm.UpdatePatio();
                }
                if (realLocation2 != null && fromLocation != null) RecurseLoadLocation(realLocation2, fromLocation);
                
                if (!SaveGame.loaded.HasSaveFix(SaveFixes.MigrateBuildingsToData))
                {
                    SaveMigrator_1_6.ConvertBuildingsToData(realLocation2);
                }
                
                loadedLocations.Add(realLocation2);
            }
            SaveGame.MigrateVillagersByFormerName();
            foreach (GameLocation realLocation in loadedLocations)
            {
                realLocation.AddDefaultBuildings(load: false);
                foreach (Building b in realLocation.buildings)
                {
                    b.load();
                    if (b.GetIndoorsType() == IndoorsType.Instanced)
                    {
                        b.GetIndoors()?.addLightGlows();
                    }
                }
                foreach (FarmAnimal value in realLocation.animals.Values)
                {
                    value.reload(null);
                }
                foreach (Furniture item in realLocation.furniture)
                {
                    item.updateDrawPosition();
                }
                foreach (LargeTerrainFeature largeTerrainFeature in realLocation.largeTerrainFeatures)
                {
                    largeTerrainFeature.Location = realLocation;
                    largeTerrainFeature.loadSprite();
                }
                foreach (TerrainFeature value2 in realLocation.terrainFeatures.Values)
                {
                    value2.Location = realLocation;
                    value2.loadSprite();
                }
                foreach (KeyValuePair<Microsoft.Xna.Framework.Vector2, StardewValley.Object> v in realLocation.objects.Pairs)
                {
                    v.Value.initializeLightSource(v.Key);
                    v.Value.reloadSprite();
                }
                realLocation.addLightGlows();
                if (!(realLocation is IslandLocation islandLocation))
                {
                    if (realLocation is FarmCave farmCave)
                    {
                        farmCave.UpdateReadyFlag();
                    }
                }
                else
                {
                    islandLocation.AddAdditionalWalnutBushes();
                }
            }
            Utility.ForEachLocation(delegate (GameLocation location)
            {
                if (location.characters.Count > 0)
                {
                    NPC[] array = location.characters.ToArray();
                    foreach (NPC obj in array)
                    {
                        SaveGame.initializeCharacter(obj, location);
                        obj.reloadSprite();
                    }
                }
                return true;
            });
            Game1.player.currentLocation = Utility.getHomeOfFarmer(Game1.player);
            return false;
        }

        public static void RecurseLoadLocation(GameLocation realLocation2, GameLocation fromLocation, string buildingType = null)
        {
            if (buildingType != null)
            {
                string buildingMapAsset = staticHelper.GameContent.Load<Dictionary<string, BuildingData>>("Data/Buildings")[buildingType].IndoorMap;
                realLocation2.map = staticHelper.GameContent.Load<Map>("Maps/" + buildingMapAsset.Replace("\\", "/"));
            }
            //realLocation2.TransferDataFromSavedLocation(fromLocation);
            realLocation2.animals.MoveFrom(fromLocation.animals);
            realLocation2.buildings.Set(
                new Netcode.NetCollection<Building>(fromLocation.buildings.Select<Building,Building>(
                    b => b.DeepClone()
                ))
            );
            foreach (var building in fromLocation.buildings)
            {
                if (building.indoors?.Value == null) continue;
                int index = fromLocation.buildings.IndexOf(building);
                RecurseLoadLocation(realLocation2.buildings[index].indoors.Value, building.indoors.Value, building.buildingType.Value);
            }
            realLocation2.characters.Set(fromLocation.characters);
            realLocation2.furniture.Set(fromLocation.furniture);
            realLocation2.largeTerrainFeatures.Set(fromLocation.largeTerrainFeatures);
            realLocation2.miniJukeboxCount.Value = fromLocation.miniJukeboxCount.Value;
            realLocation2.miniJukeboxTrack.Value = fromLocation.miniJukeboxTrack.Value;
            realLocation2.netObjects.Set(fromLocation.netObjects.Pairs);
            realLocation2.numberOfSpawnedObjectsOnMap = fromLocation.numberOfSpawnedObjectsOnMap;
            realLocation2.piecesOfHay.Value = fromLocation.piecesOfHay.Value;
            realLocation2.resourceClumps.Set(new List<ResourceClump>(fromLocation.resourceClumps));
            realLocation2.terrainFeatures.Set(fromLocation.terrainFeatures.Pairs);
            if (buildingType != null) realLocation2.isAlwaysActive.Set(true);
        }

        public static bool createIndoors_Prefix(Building __instance, GameLocation __result, BuildingData data, string nameOfIndoorsWithoutUnique)
        {
            GameLocation lcl_indoors = null;
            if (data != null && !string.IsNullOrEmpty(data.IndoorMap))
            {
                Type locationType = typeof(GameLocation);
                if (data.IndoorMapType != null)
                {
                    Exception exception = null;
                    try
                    {
                        locationType = Type.GetType(data.IndoorMapType);
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }
                    if ((object)locationType == null || exception != null)
                    {
                        /*
                        IGameLogger log = Game1.log;
                        DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(51, 2);
                        defaultInterpolatedStringHandler.AppendLiteral("Error constructing interior type '");
                        defaultInterpolatedStringHandler.AppendFormatted(data.IndoorMapType);
                        defaultInterpolatedStringHandler.AppendLiteral("' for building '");
                        defaultInterpolatedStringHandler.AppendFormatted(__instance.buildingType.Value);
                        defaultInterpolatedStringHandler.AppendLiteral("'");
                        log.Error(defaultInterpolatedStringHandler.ToStringAndClear() + ((exception != null) ? "." : ": that type doesn't exist."));
                        */
                        locationType = typeof(GameLocation);
                    }
                }
                string mapAssetName = "Maps\\" + data.IndoorMap;
                try
                {
                    lcl_indoors = (GameLocation)Activator.CreateInstance(locationType, mapAssetName, __instance.buildingType.Value);
                }
                catch (Exception)
                {
                    try
                    {
                        lcl_indoors = (GameLocation)Activator.CreateInstance(locationType, mapAssetName);
                    }
                    catch (Exception e)
                    {
                        /*
                        IGameLogger log2 = Game1.log;
                        DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(42, 1);
                        defaultInterpolatedStringHandler.AppendLiteral("Error trying to instantiate indoors for '");
                        defaultInterpolatedStringHandler.AppendFormatted(buildingType);
                        defaultInterpolatedStringHandler.AppendLiteral("'");
                        log2.Error(defaultInterpolatedStringHandler.ToStringAndClear(), e);
                        */
                        lcl_indoors = new GameLocation("Maps\\" + nameOfIndoorsWithoutUnique, __instance.buildingType);
                    }
                }
            }
            if (lcl_indoors != null)
            {
                lcl_indoors.uniqueName.Value = nameOfIndoorsWithoutUnique + (Guid)(Type.GetType("StardewValley.Util.GuidHelper, Stardew Valley").GetMethod("NewGuid", Type.EmptyTypes).Invoke(null, new object[] { }));
                lcl_indoors.IsFarm = true;
                lcl_indoors.isStructure.Value = true;
                __instance.updateInteriorWarps(lcl_indoors);
            }
            __result = lcl_indoors;
            __result?.isAlwaysActive?.Set(true);
            return false;
        }

        /*
        public static bool findStructure_Prefix(GameLocation parentLocation, string name, GameLocation __result)
        {
            List<GameLocation> buildingsListForRecursion = new List<GameLocation>();
            foreach (Building building in parentLocation.buildings)
            {
                staticMonitor.Log($"Checking building of type {building.buildingType.Value} for correct location.", LogLevel.Trace);
                if (building.indoors?.Value != null) buildingsListForRecursion.Add(building.indoors.Value);
                if (building.indoors?.Value?.uniqueName == name)
                {
                    __result = building.GetIndoors();
                    staticMonitor.Log($"Found!", LogLevel.Trace);
                    return false;
                }
            }
            foreach (GameLocation nestCheck in buildingsListForRecursion)
            {
                staticMonitor.Log($"Checking location {nestCheck.uniqueName} for nested buildings.");
                GameLocation maybeFound = Game1.findStructure(nestCheck, name);
                if (maybeFound != null)
                {
                    __result = maybeFound;
                    staticMonitor.Log($"Found matching nested building!", LogLevel.Trace);
                    return false;
                }
            }
            __result = null;
            return false;
        }
        */
    }
}