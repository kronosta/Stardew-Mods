using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Objects;
using StardewValley.Menus;
using HarmonyLib;
using System.Security;

namespace Kronosta.TypableBooks
{
    public class ModEntry : Mod
    {
        public static IModHelper SHelper { get; private set; }
        public static IMonitor SMonitor { get; private set; }
        public static string ModID;
        public override void Entry(IModHelper helper)
        {
            SHelper = helper;
            SMonitor = Monitor;
            ModID = this.ModManifest.UniqueID;
            Helper.Events.Content.AssetRequested += this.OnAssetRequested;
            Helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            Harmony harmony = new Harmony(ModID);
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Item), nameof(StardewValley.Item.canStackWith)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.CanStackWith_Prefix))
            );
        }

        public void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
            {
                e.Edit(asset =>
                {
                    var dict = asset.AsDictionary<String, ObjectData>().Data;
                    string id = $"{ModID}_Journal";
                    dict[id] = new ObjectData
                    {
                        Name = id,
                        DisplayName = $"[LocalizedText Strings\\Objects:{id}-Name]",
                        Description = $"[LocalizedText Strings\\Objects:{id}-Desc]",
                        Type = "Basic",
                        Category = -16,
                        Price = 0,
                        Texture = $"Mods/{this.ModManifest.UniqueID}/Objects",
                        SpriteIndex = 0,
                        Edibility = -300,
                        IsDrink = false,
                        Buffs = new List<ObjectBuffData>(),
                        GeodeDropsDefaultItems = false,
                        GeodeDrops = new List<ObjectGeodeDropData>(),
                        ArtifactSpotChances = new Dictionary<string, float>(),
                        CanBeGivenAsGift = false,
                        CanBeTrashed = true,
                        ExcludeFromFishingCollection = true,
                        ExcludeFromRandomSale = true,
                        ExcludeFromShippingCollection = true,
                        ContextTags = new List<string>(),
                        CustomFields = new Dictionary<string, string>()
                    };
                    id += "Signed";
                    dict[id] = new ObjectData
                    {
                        Name = id,
                        DisplayName = $"[LocalizedText Strings\\Objects:{id}-Name]",
                        Description = $"[LocalizedText Strings\\Objects:{id}-Desc]",
                        Type = "Basic",
                        Category = -16,
                        Price = 0,
                        Texture = $"Mods/{this.ModManifest.UniqueID}/Objects",
                        SpriteIndex = 0,
                        Edibility = -300,
                        IsDrink = false,
                        Buffs = new List<ObjectBuffData>(),
                        GeodeDropsDefaultItems = false,
                        GeodeDrops = new List<ObjectGeodeDropData>(),
                        ArtifactSpotChances = new Dictionary<string, float>(),
                        CanBeGivenAsGift = false,
                        CanBeTrashed = true,
                        ExcludeFromFishingCollection = true,
                        ExcludeFromRandomSale = true,
                        ExcludeFromShippingCollection = true,
                        ContextTags = new List<string>(),
                        CustomFields = new Dictionary<string, string>()
                    };
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo($"Mods/{ModID}/Objects"))
            {
                e.LoadFromModFile<Texture2D>("assets/objects.png", AssetLoadPriority.Exclusive);
            }
            else if (e.NameWithoutLocale.IsEquivalentTo($"Strings/Objects"))
            {
                e.Edit(asset =>
                {
                    var dict = asset.AsDictionary<string, string>().Data;
                    dict[$"{ModID}_Journal-Name"] = Helper.Translation.Get("journal-name");
                    dict[$"{ModID}_Journal-Desc"] = Helper.Translation.Get("journal-desc");
                    dict[$"{ModID}_JournalSigned-Name"] = Helper.Translation.Get("journal-signed-name");
                    dict[$"{ModID}_JournalSigned-Desc"] = Helper.Translation.Get("journal-signed-desc");
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo($"Data/Shops"))
            {
                e.Edit(asset =>
                {
                    var dict = asset.AsDictionary<string, StardewValley.GameData.Shops.ShopData>().Data;
                    dict["SeedShop"].Items.Add(new StardewValley.GameData.Shops.ShopItemData
                    {
                        Id = $"{ModID}_Journal",
                        ItemId = $"{ModID}_Journal",
                        Price = 100
                    });
                });
            }
        }

        public void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (e.Button.IsActionButton())
            {
                if ((Game1.player.ActiveItem?.QualifiedItemId ?? "") == $"(O){ModID}_Journal" && Game1.activeClickableMenu == null)
                {
                    Game1.options.actionButton
                        .Select(b => b.ToSButton())
                        .ToList()
                        .ForEach(b => (e.IsDown(b) ? Helper : null)?.Input?.Suppress(b));
                    WriteBook(Game1.player.ActiveItem);
                }
                else if ((Game1.player.ActiveItem?.QualifiedItemId ?? "") == $"(O){ModID}_JournalSigned" && Game1.activeClickableMenu == null)
                {
                    if (Game1.player.ActiveItem.modData.TryGetValue($"{ModID}/BookContents", out string contents))
                        Game1.activeClickableMenu = new LetterViewerMenu((contents ?? "").Replace("[[PB]]", "^^^^^^^^").Replace("[[LB]]", "^"));
                }
            }
        }

        public void WriteBook(Item item)
        {
            Game1.activeClickableMenu = new BookWriteMenu(item);
        }

        public static bool CanStackWith_Prefix(Item __instance, ISalable other, ref bool __result)
        {
            try
            {
                if (__instance.QualifiedItemId == $"(O){ModID}_Journal" || __instance.QualifiedItemId == $"(O){ModID}_JournalSigned")
                {
                    __result = false;
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                SMonitor.Log($"Failed in {nameof(CanStackWith_Prefix)}:\n{ex}");
                return true;
            }
        }
    }
}
