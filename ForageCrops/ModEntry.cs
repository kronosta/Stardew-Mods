using StardewModdingAPI;
using HarmonyLib;
using StardewValley;
using StardewValley.GameData.Crops;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using Microsoft.Xna.Framework;
using System.Data;

namespace Kronosta.ForageCrops
{
    public class ModEntry : Mod
    {
        public static string cfSpawns, cfColorSpawns;
        public static Random random = new Random((int)DateTime.Now.ToBinary());
        public static IMonitor SMonitor;
        public override void Entry(IModHelper helper)
        {
            cfSpawns = $"{this.ModManifest.UniqueID}/Spawns";
            cfColorSpawns = $"{this.ModManifest.UniqueID}/ColorSpawns";
            SMonitor = this.Monitor;

            Harmony harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.Patch(
                original: AccessTools.Method(typeof(Crop), nameof(Crop.newDay)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.Crop_newDay_prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Crop), nameof(Crop.GetData)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.Crop_GetData_postfix))
            );
        }

        public static bool Crop_newDay_prefix(Crop __instance, int state)
        {
            try
            {
                CropData data = __instance.GetData();
                if (data == null) return true;
                Dictionary<string, string> fields = data.CustomFields;
                if (fields == null) return true;
                if (fields.ContainsKey(cfSpawns))
                {
                    if (__instance.currentPhase.Value >= (__instance.phaseDays.Count - 2))
                    {
                        string[] spawns = fields[cfSpawns].Split(" ");
                        GameLocation environment = __instance.currentLocation;
                        Vector2 tileVector = __instance.tilePosition;
                        if (environment.objects.TryGetValue(tileVector, out var obj))
                        {
                            if (obj is IndoorPot pot)
                            {
                                pot.heldObject.Value = ItemRegistry.Create<StardewValley.Object>(spawns[random.Next() % spawns.Length]);
                                pot.hoeDirt.Value.crop = null;
                            }
                            else
                            {
                                environment.objects.Remove(tileVector);
                            }
                        }
                        if (!environment.objects.ContainsKey(tileVector))
                        {
                            StardewValley.Object spawned = ItemRegistry.Create<StardewValley.Object>(spawns[random.Next() % spawns.Length]);
                            spawned.IsSpawnedObject = true;
                            spawned.CanBeGrabbed = true;
                            spawned.SpecialVariable = 724519;
                            environment.objects.Add(tileVector, spawned);
                        }
                        if (environment.terrainFeatures.TryGetValue(tileVector, out var terrainFeature) && terrainFeature is HoeDirt dirt)
                        {
                            dirt.crop = null;
                        }
                        return false;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                SMonitor.Log($"Failed in {nameof(Crop_newDay_prefix)}:\n{e}");
                return true;
            }
        }

        public static void Crop_GetData_postfix(Crop __instance, ref CropData __result)
        {
            try
            {
                if (__result == null)
                {
                    CropData data;
                    string id = __instance.netSeedIndex.Value;
                    if (Crop.TryGetData(id, out data))
                    {
                        __result = data;
                    }
                }
            }
            catch (Exception e)
            {
                SMonitor.Log($"Failed in {nameof(Crop_GetData_postfix)}:\n{e}");
            }
        }
    }
}