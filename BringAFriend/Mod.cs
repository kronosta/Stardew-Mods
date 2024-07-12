using System;
using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Monsters;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Reflection.Metadata;

namespace BringAFriend
{
    internal sealed class ModEntry : Mod
    {
        BringAFriendData bafData = new BringAFriendData();
        public override void Entry(IModHelper helper)
        {
            helper.Events.Content.AssetRequested += this.OnAssetRequested;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.GameLoop.Saving += this.OnSaving;
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Kronosta/BringAFriend/Dialogue"))
            {
                e.LoadFrom(
                    () => new Dictionary<string, string>(),
                    AssetLoadPriority.High
                );
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/NPCDispositions"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, string>().Data;
                    foreach (CreatedNPC npc in bafData.NPCsCreated)
                    {
                        data[npc.Name] = npc.Disposition;
                        Monitor.Log($"Added NPC {npc.Name} to Data/NPCDispositions.");
                    }
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/NPCGiftTastes"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, string>().Data;
                    foreach (CreatedNPC npc in bafData.NPCsCreated)
                    {
                        data[npc.Name] = npc.GiftTastes;
                        Monitor.Log($"Added NPC {npc.Name} to Data/NPCGiftTastes.");
                    }
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/EngagementDialogue"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, string>().Data;
                    foreach (CreatedNPC npc in bafData.NPCsCreated)
                    {
                        if (npc.EngagementDialogue != null)
                        {
                            for (int i = 0; i < npc.EngagementDialogue.Count; i++)
                            {
                                data[npc.Name + i] = npc.EngagementDialogue[i];
                            }
                            Monitor.Log($"Added NPC {npc.Name} to Data/EngagementDialogue.");
                        }
                    }
                });
            }
            else if (e.NameWithoutLocale.Name.StartsWith("Characters/Dialogue/"))
            {
                foreach (CreatedNPC npc in bafData.NPCsCreated)
                {
                    if (e.Name.IsEquivalentTo($"Characters/Dialogue/{npc.Name}"))
                    {
                        e.LoadFrom(
                            () =>
                            {
                                return npc.Dialogue ?? new Dictionary<string, string>();
                            },
                            AssetLoadPriority.Medium
                        );
                    }
                }
            }
            else if (e.NameWithoutLocale.Name.StartsWith("Characters/Dialogue/MarriageDialogue"))
            {
                foreach (CreatedNPC npc in bafData.NPCsCreated)
                {
                    if (e.Name.IsEquivalentTo($"Characters/Dialogue/MarriageDialogue{npc.Name}"))
                    {
                        e.LoadFrom(
                            () =>
                            {
                                return npc.MarriageDialogue ?? new Dictionary<string, string>();
                            },
                            AssetLoadPriority.Medium
                        );
                    }
                }
            }
            else if (e.NameWithoutLocale.Name.ToLower().StartsWith("characters/schedules/"))
            {
                foreach (CreatedNPC npc in bafData.NPCsCreated)
                {
                    if (e.Name.IsEquivalentTo($"Characters/schedules/{npc.Name}"))
                    {
                        e.LoadFrom(
                            () =>
                            {
                                return npc.Schedule ?? new Dictionary<string, string>() ;
                            },
                            AssetLoadPriority.Medium
                        );
                    }
                }
            }
            else if (e.NameWithoutLocale.Name.StartsWith("Characters/"))
            {
                foreach (CreatedNPC npc in bafData.NPCsCreated)
                {
                    if (e.Name.IsEquivalentTo($"Characters/{npc.Name}"))
                    {
                        e.LoadFrom(
                            () =>
                            {
                                if (npc.SpriteAsset.StartsWith("#"))
                                {
                                    return Helper.GameContent.Load<Texture2D>(npc.SpriteAsset.Substring(1));
                                }
                                else
                                {
                                    return Helper.ModContent.Load<Texture2D>(npc.SpriteAsset);
                                }
                            },
                            AssetLoadPriority.Medium
                        );
                    }
                }
            }
            else if (e.NameWithoutLocale.Name.StartsWith("Portraits/"))
            {
                foreach (CreatedNPC npc in bafData.NPCsCreated)
                {
                    if (e.Name.IsEquivalentTo($"Portraits/{npc.Name}"))
                    {
                        e.LoadFrom(
                            () =>
                            {
                                if (npc.PortraitAsset.StartsWith("#"))
                                {
                                    return Helper.GameContent.Load<Texture2D>(npc.PortraitAsset.Substring(1));
                                }
                                else
                                {
                                    return Helper.ModContent.Load<Texture2D>(npc.PortraitAsset);
                                }
                            },
                            AssetLoadPriority.Medium
                        );
                    }
                }
            }
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            _ = Helper.GameContent.Load<Dictionary<string,string>>("Data/Kronosta/BringAFriend/Dialogue");
            bafData = Helper.Data.ReadJsonFile<BringAFriendData>($"data/{Constants.SaveFolderName}.json") ?? new BringAFriendData();
            Helper.GameContent.InvalidateCache("Data/NPCDispositions");
        }

        private void OnSaving(object sender, SavingEventArgs e)
        {
            var npcCreationData = Helper.GameContent.Load<Dictionary<string,string>>("Data/Kronosta/BringAFriend/Dialogue");
            foreach (var entry in npcCreationData)
            {
                if (bafData.UsedKeys.Contains(entry.Key))
                {
                    continue;
                }
                if (entry.Key.StartsWith("Answer:"))
                {
                    int responseKey = 0;
                    try
                    {
                        Int32.Parse(entry.Key.Substring(7).Trim());
                    }
                    catch
                    {
                        Monitor.Log($"Invalid key {entry.Key} in Data/Kronosta/BringAFriend/Dialogue");
                        return;
                    }
                    if (Game1.player.DialogueQuestionsAnswered.Contains(responseKey))
                    {
                        bafData.NPCsCreated.AddRange(JsonSerializer.Deserialize<List<CreatedNPC>>(entry.Value));
                        bafData.UsedKeys.Add(entry.Key);
                        Monitor.Log($"Created some NPCs from dialogue answer {responseKey}.");
                    }
                }
                else
                {
                    if (Game1.player.mailReceived.Contains(entry.Key))
                    {
                        bafData.NPCsCreated.AddRange(JsonSerializer.Deserialize<List<CreatedNPC>>(entry.Value));
                        bafData.UsedKeys.Add(entry.Key);
                        Monitor.Log($"Created some NPCs from mail flag {entry.Key}");
                    }
                }
            }
            Helper.Data.WriteJsonFile<BringAFriendData>($"data/{Constants.SaveFolderName}.json", bafData);
        }
    }
}
