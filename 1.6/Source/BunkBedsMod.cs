using HarmonyLib;
using UnityEngine;
using Verse;

namespace BunkBeds
{
    public class BunkBedsMod : Mod
    {
        public static BunkBedsSettings settings;
        public BunkBedsMod(ModContentPack pack) : base(pack)
        {
            new Harmony("BunkBedsMod").PatchAll();
            settings = GetSettings<BunkBedsSettings>();
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            settings.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            if (ModsConfig.IsActive("Darknote.BunkBeds"))
            {
                return Content.Name;
            }
            return "";
        }
    }

}
