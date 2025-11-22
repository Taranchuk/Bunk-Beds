using HarmonyLib;
using RimWorld;
using Verse;

namespace BunkBeds
{
    [HarmonyPatch(typeof(LovePartnerRelationUtility), "GetMostDislikedNonPartnerBedOwner")]
    public static class LovePartnerRelationUtility_GetMostDislikedNonPartnerBedOwner_Patch
    {
        public static bool Prefix(Pawn p)
        {
            if (BunkBedsSettings.occupantsWillHaveShareBedDebuff is false)
            {
                return false;
            }
            if (p.ownership.OwnedBed.IsBunkBed(out var comp) && comp.Props.preventSharingThought)
            {
                return false;
            }
            return true;
        }
    }

}
