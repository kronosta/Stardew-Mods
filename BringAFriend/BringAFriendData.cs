using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BringAFriend
{
    internal class BringAFriendData
    {
        public List<CreatedNPC> NPCsCreated { get; set; }
        public List<string> UsedKeys { get; set; }

        public BringAFriendData()
        {
            NPCsCreated = new List<CreatedNPC>();
            UsedKeys = new List<string>();
        }

        public BringAFriendData(List<CreatedNPC> npcsCreated, List<string> usedKeys)
        {
            NPCsCreated = npcsCreated;
            UsedKeys = usedKeys;
        }
    }

#nullable enable
    internal class CreatedNPC
    {
        public string Name { get; set; }
        public string Disposition { get; set; }
        public string? GiftTastes { get; set; }
        public Dictionary<string, string>? Dialogue { get; set; }
        public Dictionary<string, string>? MarriageDialogue { get; set; }
        public string SpriteAsset { get; set; }
        public string PortraitAsset { get; set; }
        public Dictionary<string, string>? Schedule { get; set; }
        public List<string>? EngagementDialogue { get; set; }
        public Dictionary<string, string>? PotentialNPCsToAdd { get; set; }

        public CreatedNPC()
        {
            Name = "BAFCustom";
            Disposition = "teen/rude/outgoing/neutral/female/datable/Sebastian/Town/fall 13/Caroline 'mom' Pierre 'dad'/SeedShop 1 9/Abigail";
            SpriteAsset = "Characters/Pam";
            PortraitAsset = "Portraits/Pam";
        }
        public CreatedNPC(string name, string disposition, string spriteAsset, string portraitAsset)
        {
            Name = name;
            Disposition = disposition;
            SpriteAsset = spriteAsset;
            PortraitAsset = portraitAsset;
        }

        public CreatedNPC(
            string name, 
            string disposition, 
            string? giftTastes, 
            Dictionary<string, string>? dialogue,
            Dictionary<string, string>? marriageDialogue,
            string spriteAsset,
            string portraitAsset,
            Dictionary<string, string>? schedule,
            List<string> engagementDialogue)
        {
            Name = name;
            Disposition = disposition;
            GiftTastes = giftTastes;
            Dialogue = dialogue;
            MarriageDialogue = marriageDialogue;
            SpriteAsset = spriteAsset;
            PortraitAsset = portraitAsset;
            Schedule = schedule;
            EngagementDialogue = engagementDialogue;
        }
    }
#nullable disable
}
