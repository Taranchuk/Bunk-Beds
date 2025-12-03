using HarmonyLib;
using RimWorld;
using Verse;
using System.Reflection;

namespace BunkBeds
{
    [HarmonyPatch]
    public static class BedUtility_OtherOwnerScore_Patch
    {
        public static bool Prepare() => ModsConfig.IsActive("Orion.Hospitality") && TargetMethod() != null;

        static MethodBase TargetMethod()
        {
            var bedUtilityType = AccessTools.TypeByName("Hospitality.Utilities.BedUtility");
            if (bedUtilityType != null)
            {
                var methodToPatch = AccessTools.Method(bedUtilityType, "OtherOwnerScore", new[] { typeof(Building_Bed), typeof(Pawn) });
                if (methodToPatch != null)
                {
                    return methodToPatch;
                }
            }

            return null;
        }

        public static bool Prefix(Building_Bed bed, Pawn guest, ref int __result)
        {
            if (bed.IsBunkBed())
            {
                __result = 0;
                return false;
            }
            return true;
        }
    }

}
