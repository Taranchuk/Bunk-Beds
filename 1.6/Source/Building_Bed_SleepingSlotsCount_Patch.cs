using HarmonyLib;
using RimWorld;
using Verse;

namespace BunkBeds
{
    [HarmonyPatch(typeof(Building_Bed), "SleepingSlotsCount", MethodType.Getter)]
    public static class Building_Bed_SleepingSlotsCount_Patch
    {
        public static Building_Bed curBed;
        public static int curCount;
        public static bool Prefix(Building_Bed __instance, ref int __result)
        {
            if (curBed != null && __instance == curBed)
            {
                __result = curCount;
                return false;
            }
            if (__instance.IsBunkBed(out var bunkBed)) 
            {
                curBed = __instance;
                curCount = __result = bunkBed.Props.pawnCount;
                return false;
            }
            return true;
        }
    }
}
