using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Reflection;
using System.Reflection.Emit;

namespace SpeciesOrbs
{
    internal sealed class ModEntry : Mod {

        public string currentSpeciesID = null;
        public Dictionary<string, Tuple<Delegate, DynamicMethod>> customEventHandlers = new Dictionary<string, Tuple<Delegate, DynamicMethod>>();
#if NET5_0
        public int orbID = 1;
#else
        public string orbID = "Kronosta.SpeciesOrbs_Orb";
#endif
        public Dictionary<string, object> customHandlerState = new Dictionary<string, object>();
        public static IModHelper publicHelper;
        public static IMonitor publicMonitor;
        List<Tuple<EventInfo, string>> eventList = new List<Tuple<Type, string>>
            {
                new Tuple<Type, string>(typeof(IContentEvents), "AssetRequested"),
                new Tuple<Type, string>(typeof(IContentEvents), "AssetsInvalidated"),
                new Tuple<Type, string>(typeof(IContentEvents), "AssetReady"),
                new Tuple<Type, string>(typeof(IContentEvents), "LocaleChanged"),
                new Tuple<Type, string>(typeof(IContentEvents), "AssetRequested"),
                new Tuple<Type, string>(typeof(IDisplayEvents), "MenuChanged"),
                new Tuple<Type, string>(typeof(IDisplayEvents), "Rendering"),
                new Tuple<Type, string>(typeof(IDisplayEvents), "Rendered"),
                new Tuple<Type, string>(typeof(IDisplayEvents), "RenderingWorld"),
                new Tuple<Type, string>(typeof(IDisplayEvents), "RenderedWorld"),
                new Tuple<Type, string>(typeof(IDisplayEvents), "RenderingActiveMenu"),
                new Tuple<Type, string>(typeof(IDisplayEvents), "RenderedActiveMenu"),
                new Tuple<Type, string>(typeof(IDisplayEvents), "RenderingHud"),
                new Tuple<Type, string>(typeof(IDisplayEvents), "RenderedHud"),
                new Tuple<Type, string>(typeof(IDisplayEvents), "WindowResized"),
                new Tuple<Type, string>(typeof(IGameLoopEvents), "UpdateTicking"),
                new Tuple<Type, string>(typeof(IGameLoopEvents), "UpdateTicked"),
                new Tuple<Type, string>(typeof(IGameLoopEvents), "OneSecondUpdateTicking"),
                new Tuple<Type, string>(typeof(IGameLoopEvents), "OneSecondUpdateTicked"),
                new Tuple<Type, string>(typeof(IGameLoopEvents), "Saving"),
                new Tuple<Type, string>(typeof(IGameLoopEvents), "Saved"),
                new Tuple<Type, string>(typeof(IGameLoopEvents), "DayStarted"),
                new Tuple<Type, string>(typeof(IGameLoopEvents), "DayEnding"),
                new Tuple<Type, string>(typeof(IGameLoopEvents), "TimeChanged"),
                new Tuple<Type, string>(typeof(IInputEvents), "ButtonsChanged"),
                new Tuple<Type, string>(typeof(IInputEvents), "ButtonPressed"),
                new Tuple<Type, string>(typeof(IInputEvents), "ButtonReleased"),
                new Tuple<Type, string>(typeof(IInputEvents), "CursorMoved"),
                new Tuple<Type, string>(typeof(IInputEvents), "MouseWheelScrolled"),
                new Tuple<Type, string>(typeof(IPlayerEvents), "InventoryChanged"),
                new Tuple<Type, string>(typeof(IPlayerEvents), "LevelChanged"),
                new Tuple<Type, string>(typeof(IPlayerEvents), "Warped"),
                new Tuple<Type, string>(typeof(IWorldEvents), "LocationListChanged"),
                new Tuple<Type, string>(typeof(IWorldEvents), "BuildingListChanged"),
                new Tuple<Type, string>(typeof(IWorldEvents), "ChestInventoryChanged"),
                new Tuple<Type, string>(typeof(IWorldEvents), "DebrisListChanged"),
                new Tuple<Type, string>(typeof(IWorldEvents), "FurnitureListChanged"),
                new Tuple<Type, string>(typeof(IWorldEvents), "LargeTerrainFeatureListChanged"),
                new Tuple<Type, string>(typeof(IWorldEvents), "NpcListChanged"),
                new Tuple<Type, string>(typeof(IWorldEvents), "ObjectListChanged"),
                new Tuple<Type, string>(typeof(IWorldEvents), "TerrainFeatureListChanged"),
            }.Select(x => new Tuple<EventInfo, string>(x.Item1.GetEvent(x.Item2), x.Item2)).ToList();
        Dictionary<string, Action<ILGenerator, IL_Instruction, Dictionary<string, Label>>> ilFuncs =
                new Dictionary<string, Action<ILGenerator, IL_Instruction, Dictionary<string, Label>>>
                {
                    ["Add"] =
                        (il, inst, labs) => il.Emit(OpCodes.Add),
                    ["Add_Ovf"] =
                        (il, inst, labs) => il.Emit(OpCodes.Add_Ovf),
                    ["Add_Ovf_Un"] =
                        (il, inst, labs) => il.Emit(OpCodes.Add_Ovf_Un),
                    ["And"] =
                        (il, inst, labs) => il.Emit(OpCodes.And),
                    ["Beq"] =
                        (il, inst, labs) => il.Emit(OpCodes.Beq, labs[inst.LabelNames[0]]),
                    ["Bge"] =
                        (il, inst, labs) => il.Emit(OpCodes.Bge, labs[inst.LabelNames[0]]),
                    ["Bge_Un"] =
                        (il, inst, labs) => il.Emit(OpCodes.Bge_Un, labs[inst.LabelNames[0]]),
                    ["Bgt"] =
                        (il, inst, labs) => il.Emit(OpCodes.Bgt, labs[inst.LabelNames[0]]),
                    ["Bgt_Un"] =
                        (il, inst, labs) => il.Emit(OpCodes.Bgt_Un, labs[inst.LabelNames[0]]),
                    ["Ble"] =
                        (il, inst, labs) => il.Emit(OpCodes.Ble, labs[inst.LabelNames[0]]),
                    ["Ble_Un"] =
                        (il, inst, labs) => il.Emit(OpCodes.Ble_Un, labs[inst.LabelNames[0]]),
                    ["Blt"] =
                        (il, inst, labs) => il.Emit(OpCodes.Blt, labs[inst.LabelNames[0]]),
                    ["Blt_Un"] =
                        (il, inst, labs) => il.Emit(OpCodes.Blt_Un, labs[inst.LabelNames[0]]),
                    ["Bne_Un"] =
                        (il, inst, labs) => il.Emit(OpCodes.Bne_Un, labs[inst.LabelNames[0]]),
                    ["Box"] =
                        (il, inst, labs) => il.Emit(OpCodes.Box, Type.GetType(inst.Type)),
                    ["Br"] =
                        (il, inst, labs) => il.Emit(OpCodes.Br, labs[inst.LabelNames[0]]),
                    ["Brfalse"] =
                        (il, inst, labs) => il.Emit(OpCodes.Brfalse, labs[inst.LabelNames[0]]),
                    ["Brtrue"] =
                        (il, inst, labs) => il.Emit(OpCodes.Brtrue, labs[inst.LabelNames[0]]),
                    ["Call"] =
                        (il, inst, labs) => il.Emit(OpCodes.Call, Type.GetType(inst.Type).GetMethod(inst.MethodName, inst.MethodParams.Select(x => Type.GetType(x)).ToArray())),
                    ["Callvirt"] =
                        (il, inst, labs) => il.Emit(OpCodes.Callvirt, Type.GetType(inst.Type).GetMethod(inst.MethodName, inst.MethodParams.Select(x => Type.GetType(x)).ToArray())),
                    ["Castclass"] =
                        (il, inst, labs) => il.Emit(OpCodes.Castclass, Type.GetType(inst.Type)),
                    ["Ceq"] =
                        (il, inst, labs) => il.Emit(OpCodes.Ceq),
                    ["Cgt"] =
                        (il, inst, labs) => il.Emit(OpCodes.Cgt),
                    ["Cgt_Un"] =
                        (il, inst, labs) => il.Emit(OpCodes.Cgt_Un),
                    ["Ckfinite"] =
                        (il, inst, labs) => il.Emit(OpCodes.Ckfinite),
                    ["Clt"] =
                        (il, inst, labs) => il.Emit(OpCodes.Clt),
                    ["Clt_Un"] =
                        (il, inst, labs) => il.Emit(OpCodes.Clt_Un),
                    ["Constrained"] =
                        (il, inst, labs) => il.Emit(OpCodes.Constrained, Type.GetType(inst.Type)),
                    ["Conv_I1"] =
                        (il, inst, labs) => il.Emit(OpCodes.Conv_I1),
                    ["Conv_I2"] =
                        (il, inst, labs) => il.Emit(OpCodes.Conv_I2),
                    ["Conv_I4"] =
                        (il, inst, labs) => il.Emit(OpCodes.Conv_I4),
                    ["Conv_I8"] =
                        (il, inst, labs) => il.Emit(OpCodes.Conv_I8),
                    ["Conv_Ovf_I1"] =
                        (il, inst, labs) => il.Emit(OpCodes.Conv_Ovf_I1),
                    ["Conv_Ovf_I2"] =
                        (il, inst, labs) => il.Emit(OpCodes.Conv_Ovf_I2),
                    ["Conv_Ovf_I4"] =
                        (il, inst, labs) => il.Emit(OpCodes.Conv_Ovf_I4),
                    ["Conv_Ovf_I8"] =
                        (il, inst, labs) => il.Emit(OpCodes.Conv_Ovf_I8),
                    ["Conv_Ovf_I1_Un"] =
                        (il, inst, labs) => il.Emit(OpCodes.Conv_Ovf_I1_Un),
                    ["Conv_Ovf_I2_Un"] =
                        (il, inst, labs) => il.Emit(OpCodes.Conv_Ovf_I2_Un),
                    ["Conv_Ovf_I4_Un"] =
                        (il, inst, labs) => il.Emit(OpCodes.Conv_Ovf_I4_Un),
                    ["Conv_Ovf_I8_Un"] =
                        (il, inst, labs) => il.Emit(OpCodes.Conv_Ovf_I8_Un),
                    ["Conv_Ovf_U1"] =
                        (il, inst, labs) => il.Emit(OpCodes.Conv_Ovf_U1),
                    ["Conv_Ovf_U2"] =
                        (il, inst, labs) => il.Emit(OpCodes.Conv_Ovf_U2),
                    ["Conv_Ovf_U4"] =
                        (il, inst, labs) => il.Emit(OpCodes.Conv_Ovf_U4),
                    ["Conv_Ovf_U8"] =
                        (il, inst, labs) => il.Emit(OpCodes.Conv_Ovf_U8),
                    ["Conv_Ovf_U1_Un"] =
                        (il, inst, labs) => il.Emit(OpCodes.Conv_Ovf_U1_Un),
                    ["Conv_Ovf_U2_Un"] =
                        (il, inst, labs) => il.Emit(OpCodes.Conv_Ovf_U2_Un),
                    ["Conv_Ovf_U4_Un"] =
                        (il, inst, labs) => il.Emit(OpCodes.Conv_Ovf_U4_Un),
                    ["Conv_Ovf_U8_Un"] =
                        (il, inst, labs) => il.Emit(OpCodes.Conv_Ovf_U8_Un),
                    ["Conv_R_Un"] =
                        (il, inst, labs) => il.Emit(OpCodes.Conv_R_Un),
                    ["Conv_R4"] =
                        (il, inst, labs) => il.Emit(OpCodes.Conv_R4),
                    ["Conv_R8"] =
                        (il, inst, labs) => il.Emit(OpCodes.Conv_R8),
                    ["Conv_U1"] =
                        (il, inst, labs) => il.Emit(OpCodes.Conv_U1),
                    ["Conv_U2"] =
                        (il, inst, labs) => il.Emit(OpCodes.Conv_U2),
                    ["Conv_U4"] =
                        (il, inst, labs) => il.Emit(OpCodes.Conv_U4),
                    ["Conv_U8"] =
                        (il, inst, labs) => il.Emit(OpCodes.Conv_U8),
                    ["Cpblk"] =
                        (il, inst, labs) => il.Emit(OpCodes.Cpblk),
                    ["Cpobj"] =
                        (il, inst, labs) => il.Emit(OpCodes.Cpobj, Type.GetType(inst.Type)),
                    ["Div"] =
                        (il, inst, labs) => il.Emit(OpCodes.Div),
                    ["Div_Un"] =
                        (il, inst, labs) => il.Emit(OpCodes.Div_Un),
                    ["Dup"] =
                        (il, inst, labs) => il.Emit(OpCodes.Dup),
                    ["Endfilter"] =
                        (il, inst, labs) => il.Emit(OpCodes.Endfilter),
                    ["Endfinally"] =
                        (il, inst, labs) => il.Emit(OpCodes.Endfinally),
                    ["Initblk"] =
                        (il, inst, labs) => il.Emit(OpCodes.Initblk),
                    ["Initobj"] =
                        (il, inst, labs) => il.Emit(OpCodes.Initobj, Type.GetType(inst.Type)),
                    ["Isinst"] =
                        (il, inst, labs) => il.Emit(OpCodes.Isinst, Type.GetType(inst.Type)),
                    ["Jmp"] =
                        (il, inst, labs) => il.Emit(OpCodes.Jmp, labs[inst.LabelNames[0]]),
                    ["Ldarg_0"] =
                        (il, inst, labs) => il.Emit(OpCodes.Ldarg_0),
                    ["Ldarg_1"] =
                        (il, inst, labs) => il.Emit(OpCodes.Ldarg_1),
                    ["Ldarga"] =
                        (il, inst, labs) => il.Emit(OpCodes.Ldarga, inst.Int16 ?? 0),
                    ["Ldc_I4"] =
                        (il, inst, labs) => il.Emit(OpCodes.Ldc_I4, inst.Int32 ?? 0),
                    ["Ldc_I8"] =
                        (il, inst, labs) => il.Emit(OpCodes.Ldc_I8, inst.Int64 ?? 0),
                    ["Ldc_R4"] =
                        (il, inst, labs) => il.Emit(OpCodes.Ldc_R4, inst.Single ?? 0),
                    ["Ldc_R8"] =
                        (il, inst, labs) => il.Emit(OpCodes.Ldc_R8, inst.Double ?? 0),
                    ["Ldelem"] =
                        (il, inst, labs) => il.Emit(OpCodes.Ldelem, Type.GetType(inst.Type)),
                    ["Ldelema"] =
                        (il, inst, labs) => il.Emit(OpCodes.Ldelema, Type.GetType(inst.Type)),
                    ["Ldfld"] =
                        (il, inst, labs) => il.Emit(OpCodes.Ldfld, Type.GetType(inst.Type).GetField(inst.Field)),
                    ["Ldflda"] =
                        (il, inst, labs) => il.Emit(OpCodes.Ldflda, Type.GetType(inst.Type).GetField(inst.Field)),
                    ["Ldftn"] =
                        (il, inst, labs) => il.Emit(OpCodes.Ldftn, Type.GetType(inst.Type).GetMethod(inst.MethodName, inst.MethodParams.Select(x => Type.GetType(x)).ToArray())),
                    ["Ldind_I1"] =
                        (il, inst, labs) => il.Emit(OpCodes.Ldind_I1),
                    ["Ldind_I2"] =
                        (il, inst, labs) => il.Emit(OpCodes.Ldind_I2),
                    ["Ldind_I4"] =
                        (il, inst, labs) => il.Emit(OpCodes.Ldind_I4),
                    ["Ldind_I8"] =
                        (il, inst, labs) => il.Emit(OpCodes.Ldind_I8),
                    ["Ldind_R4"] =
                        (il, inst, labs) => il.Emit(OpCodes.Ldind_R4),
                    ["Ldind_R8"] =
                        (il, inst, labs) => il.Emit(OpCodes.Ldind_R8),
                    ["Ldind_Ref"] =
                        (il, inst, labs) => il.Emit(OpCodes.Ldind_Ref),
                    ["Ldind_U1"] =
                        (il, inst, labs) => il.Emit(OpCodes.Ldind_U1),
                    ["Ldind_U2"] =
                        (il, inst, labs) => il.Emit(OpCodes.Ldind_U2),
                    ["Ldind_U4"] =
                        (il, inst, labs) => il.Emit(OpCodes.Ldind_U4),
                    ["Ldlen"] =
                        (il, inst, labs) => il.Emit(OpCodes.Ldlen),
                    ["Ldnull"] =
                        (il, inst, labs) => il.Emit(OpCodes.Ldnull),
                    ["Ldobj"] =
                        (il, inst, labs) => il.Emit(OpCodes.Ldobj, Type.GetType(inst.Type)),
                    ["Ldsfld"] =
                        (il, inst, labs) => il.Emit(OpCodes.Ldsfld, Type.GetType(inst.Type).GetField(inst.Field)),
                    ["Ldsflda"] =
                        (il, inst, labs) => il.Emit(OpCodes.Ldsflda, Type.GetType(inst.Type).GetField(inst.Field)),
                    ["Ldstr"] =
                        (il, inst, labs) => il.Emit(OpCodes.Ldstr, inst.String),
                    ["Leave"] =
                        (il, inst, labs) => il.Emit(OpCodes.Leave, labs[inst.LabelNames[0]]),
                    ["Localloc"] =
                        (il, inst, labs) => il.Emit(OpCodes.Localloc),
                    ["Mkrefany"] =
                        (il, inst, labs) => il.Emit(OpCodes.Mkrefany, Type.GetType(inst.Type)),
                    ["Mul"] =
                        (il, inst, labs) => il.Emit(OpCodes.Mul),
                    ["Mul_Ovf"] =
                        (il, inst, labs) => il.Emit(OpCodes.Mul_Ovf),
                    ["Mul_Ovf_Un"] =
                        (il, inst, labs) => il.Emit(OpCodes.Mul_Ovf_Un),
                    ["Neg"] =
                        (il, inst, labs) => il.Emit(OpCodes.Neg),
                    ["Newarr"] =
                        (il, inst, labs) => il.Emit(OpCodes.Newarr, Type.GetType(inst.Type)),
                    ["Newobj"] =
                        (il, inst, labs) => il.Emit(OpCodes.Newobj, Type.GetType(inst.Type).GetConstructor(inst.ConstructorParams.Select(x => Type.GetType(x)).ToArray())),
                    ["Nop"] =
                         (il, inst, labs) => il.Emit(OpCodes.Nop),
                    ["Not"] =
                         (il, inst, labs) => il.Emit(OpCodes.Not),
                    ["Or"] =
                         (il, inst, labs) => il.Emit(OpCodes.Or),
                    ["Pop"] =
                         (il, inst, labs) => il.Emit(OpCodes.Pop),
                    ["Readonly"] =
                         (il, inst, labs) => il.Emit(OpCodes.Readonly),
                    ["Refanytype"] =
                         (il, inst, labs) => il.Emit(OpCodes.Refanytype),
                    ["Refanyval"] =
                         (il, inst, labs) => il.Emit(OpCodes.Refanyval, Type.GetType(inst.Type)),
                    ["Rem"] =
                         (il, inst, labs) => il.Emit(OpCodes.Rem),
                    ["Rem_Un"] =
                         (il, inst, labs) => il.Emit(OpCodes.Rem_Un),
                    ["Ret"] =
                         (il, inst, labs) => il.Emit(OpCodes.Ret),
                    ["Rethrow"] =
                         (il, inst, labs) => il.Emit(OpCodes.Rethrow),
                    ["Shl"] =
                         (il, inst, labs) => il.Emit(OpCodes.Shl),
                    ["Shr"] =
                         (il, inst, labs) => il.Emit(OpCodes.Shr),
                    ["Shr_Un"] =
                         (il, inst, labs) => il.Emit(OpCodes.Shr_Un),
                    ["Sizeof"] =
                         (il, inst, labs) => il.Emit(OpCodes.Sizeof, Type.GetType(inst.Type)),
                    ["Starg"] =
                         (il, inst, labs) => il.Emit(OpCodes.Starg, inst.Int16 ?? 0),
                    ["Stelem"] =
                         (il, inst, labs) => il.Emit(OpCodes.Stelem, Type.GetType(inst.Type)),
                    ["Stfld"] =
                         (il, inst, labs) => il.Emit(OpCodes.Stfld, Type.GetType(inst.Type).GetField(inst.Field)),
                    ["Stind_I1"] =
                        (il, inst, labs) => il.Emit(OpCodes.Stind_I1),
                    ["Stind_I2"] =
                        (il, inst, labs) => il.Emit(OpCodes.Stind_I2),
                    ["Stind_I4"] =
                        (il, inst, labs) => il.Emit(OpCodes.Stind_I4),
                    ["Stind_I8"] =
                        (il, inst, labs) => il.Emit(OpCodes.Stind_I8),
                    ["Stind_R4"] =
                        (il, inst, labs) => il.Emit(OpCodes.Stind_R4),
                    ["Stind_R8"] =
                        (il, inst, labs) => il.Emit(OpCodes.Stind_R8),
                    ["Stind_Ref"] =
                        (il, inst, labs) => il.Emit(OpCodes.Stind_Ref),
                    ["Stobj"] =
                        (il, inst, labs) => il.Emit(OpCodes.Stobj, Type.GetType(inst.Type)),
                    ["Stsfld"] =
                         (il, inst, labs) => il.Emit(OpCodes.Stsfld, Type.GetType(inst.Type).GetField(inst.Field)),
                    ["Sub"] =
                        (il, inst, labs) => il.Emit(OpCodes.Sub),
                    ["Sub_Ovf"] =
                        (il, inst, labs) => il.Emit(OpCodes.Sub_Ovf),
                    ["Sub_Ovf_Un"] =
                        (il, inst, labs) => il.Emit(OpCodes.Sub_Ovf_Un),
                    ["Sub"] =
                        (il, inst, labs) => il.Emit(OpCodes.Sub),
                    ["Switch"] =
                        (il, inst, labs) => il.Emit(OpCodes.Switch, inst.LabelNames.Select(x => labs[x]).ToArray()),
                    ["Tailcall"] =
                        (il, inst, labs) => il.Emit(OpCodes.Tailcall),
                    ["Throw"] =
                        (il, inst, labs) => il.Emit(OpCodes.Throw),
                    ["Unaligned"] =
                        (il, inst, labs) => il.Emit(OpCodes.Unaligned, inst.SByte ?? 0),
                    ["Unbox"] =
                        (il, inst, labs) => il.Emit(OpCodes.Unbox, Type.GetType(inst.Type)),
                    ["Unbox_Any"] =
                        (il, inst, labs) => il.Emit(OpCodes.Unbox_Any, Type.GetType(inst.Type)),
                    ["Volatile"] =
                        (il, inst, labs) => il.Emit(OpCodes.Volatile),
                    ["Xor"] =
                        (il, inst, labs) => il.Emit(OpCodes.Xor),
                    ["#DefineLabel"] =
                        (il, inst, labs) => inst.LabelNames.ForEach(x => labs.Add(x, il.DefineLabel())),
                    ["#MarkLabel"] =
                        (il, inst, labs) => il.MarkLabel(labs[inst.LabelNames[0]]),
                    ["#Try"] =
                        (il, inst, labs) => labs.Add(inst.LabelNames[0], il.BeginExceptionBlock()),
                    ["#Catch"] =
                        (il, inst, labs) => il.BeginCatchBlock(Type.GetType(inst.Type)),
                    ["#Finally"] =
                        (il, inst, labs) => il.BeginFinallyBlock(),
                };
        public List<string> farmerSpriteAssets = new List<string>
        {
            "accessories",
            "farmer_base",
            "farmer_base_bald",
            "farmer_girl_base",
            "farmer_girl_base_bald",
            "hairstyles",
            "hairstyles2",
            "hats",
            "pants",
            "shirts",
            "shoeColors",
            "skinColors",
        }
        .Select(x => "Characters/Farmer/" + x)
        .ToList();
        public override void Entry(IModHelper helper)
        {
            publicHelper = helper;
            publicMonitor = Monitor;
            publicHelper.Events.Content.AssetRequested += OnAssetRequested;
            publicHelper.Events.Input.ButtonPressed += OnButtonPressed;
            publicHelper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/ObjectInformation"))
            {
                e.Edit(asset =>
                {
#if NET5_0
                    var dict = asset.AsDictionary<int, string>().Data;
#else
                    var dict = asset.AsDictionary<string, string().Data;
#endif
                    dict[orbID] =
                    "Species Orb/" +
                    "1000/" +
                    "-300/" +
                    "Basic -2/" +
                    "Species Orb/" + "" +
                    "Lets you transform into another species./" +
                    "///"
#if NET6_0
                    + "0/" + "Mods\\Kronosta.SpeciesOrbs\\Objects";
#endif
                    ;
                });
            }
            else if (e.NameWithoutLocale.StartsWith("Characters/Farmer"))
            {
                e.LoadFrom(() =>
                {
                    try
                    {
                        return Helper.GameContent.Load<Texture2D>($"Mods/Kronosta.SpeciesOrbs/{e.NameWithoutLocale.Name.Substring(e.NameWithoutLocale.Name.LastIndexOf("/"))}");
                    }
                    catch
                    {
                        return Helper.GameContent.Load<Texture2D>(e.Name);
                    }
                }, AssetLoadPriority.High);
            }
            /*else if (e.NameWithoutLocale.IsEquivalentTo("Maps/springobjects") && (System.Environment.Version.Major == 5))
            {
                e.Edit(asset =>
                {
                    var editor = asset.AsImage();
                    
                });
            }*/
            else if (e.NameWithoutLocale.IsEquivalentTo($"Mods/{this.ModManifest.UniqueID}/Species"))
            {
                e.LoadFrom(() => new Dictionary<string,Species>(), AssetLoadPriority.Exclusive);
            }
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            string species; 
            if (Game1.player.modData.TryGetValue($"{this.ModManifest.UniqueID}/current-species", out species))
            {
                var allSpecies = Helper.GameContent.Load<Dictionary<string, Species>>(
                    $"Mods/{this.ModManifest.UniqueID}/Species"
                );
                var speciesData = allSpecies[species];
                LoadEventHandlers(speciesData);
                farmerSpriteAssets.ForEach(x => publicHelper.GameContent.InvalidateCache(x));
            }
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if ((e.Button == SButton.MouseRight) && 
                (typeof(Item).GetProperty(System.Environment.Version.Major == 5 ? "ParentSheetIndex" : "ItemId").GetValue(Game1.player.CurrentItem).ToString() == orbID.ToString()))
            {
                var allSpecies = Helper.GameContent.Load<Dictionary<string, Species>>(
                    $"Mods/Kronosta.SpeciesOrbs/Species"
                );
                UnloadEventHandlers();
                string orbSpecies = Game1.player.favoriteThing.Value;
                Game1.player.modData[$"{this.ModManifest.UniqueID}/current-species"] = orbSpecies;
                var speciesData = allSpecies[orbSpecies];
                LoadEventHandlers(speciesData);
                farmerSpriteAssets.ForEach(x => publicHelper.GameContent.InvalidateCache(x));
            }
        }

        private void UnloadEventHandlers()
        {
            eventList.ForEach(x => { if (customEventHandlers.TryGetValue(x.Item2, out Tuple<Delegate, DynamicMethod> d)) x.Item1.RemoveEventHandler(new Dictionary<Type, object>
            {
                [typeof(StardewModdingAPI.Events.IContentEvents)] = publicHelper.Events.Content,
                [typeof(StardewModdingAPI.Events.IGameLoopEvents)] = publicHelper.Events.GameLoop,
                [typeof(StardewModdingAPI.Events.IInputEvents)] = publicHelper.Events.Input,
                [typeof(StardewModdingAPI.Events.IPlayerEvents)] = publicHelper.Events.Player,
                [typeof(StardewModdingAPI.Events.IWorldEvents)] = publicHelper.Events.World,
            }[x.Item1.DeclaringType], d.Item1); });
            customEventHandlers.Clear();
        }

        private void LoadEventHandlers(Species speciesData)
        {
            eventList.ForEach(x => { if (typeof(Species).GetProperty(x.Item2).GetValue(speciesData) != null) LoadEventHandler(speciesData, x.Item1, x.Item2); });
        }

        private void LoadEventHandler(Species speciesData, EventInfo ev, string name)
        {
            Type evType = ev.EventHandlerType;
            var handlerMethod = new DynamicMethod("", typeof(void), GetDelegateParameterTypes(evType));
            ILGenerator il = handlerMethod.GetILGenerator();
            var labs = new Dictionary<string, Label>();
            foreach (IL_Instruction inst in (List<IL_Instruction>)typeof(Species).GetProperty(name).GetValue(speciesData))
            {
                ilFuncs[inst.Opcode](il, inst, labs);
            }
            Delegate handler = handlerMethod.CreateDelegate(evType);
            ev.AddEventHandler(new Dictionary<Type, object> { 
                [typeof(StardewModdingAPI.Events.IContentEvents)] = publicHelper.Events.Content,
                [typeof(StardewModdingAPI.Events.IGameLoopEvents)] = publicHelper.Events.GameLoop,
                [typeof(StardewModdingAPI.Events.IInputEvents)] = publicHelper.Events.Input,
                [typeof(StardewModdingAPI.Events.IPlayerEvents)] = publicHelper.Events.Player,
                [typeof(StardewModdingAPI.Events.IWorldEvents)] = publicHelper.Events.World,
            }[ev.DeclaringType], handler);
            customEventHandlers.Add(name, new Tuple<Delegate, DynamicMethod>(handler, handlerMethod));
        }

        //Taken from https://learn.microsoft.com/en-us/dotnet/framework/reflection-and-codedom/how-to-hook-up-a-delegate-using-reflection
        private Type[] GetDelegateParameterTypes(Type d)
        {
            if (d.BaseType != typeof(MulticastDelegate))
                throw new ArgumentException("Not a delegate.", nameof(d));

            MethodInfo invoke = d.GetMethod("Invoke");
            if (invoke == null)
                throw new ArgumentException("Not a delegate.", nameof(d));

            ParameterInfo[] parameters = invoke.GetParameters();
            Type[] typeParameters = new Type[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                typeParameters[i] = parameters[i].ParameterType;
            }
            return typeParameters;
        }

        private Type GetDelegateReturnType(Type d)
        {
            if (d.BaseType != typeof(MulticastDelegate))
                throw new ArgumentException("Not a delegate.", nameof(d));

            MethodInfo invoke = d.GetMethod("Invoke");
            if (invoke == null)
                throw new ArgumentException("Not a delegate.", nameof(d));

            return invoke.ReturnType;
        }
    }
}
