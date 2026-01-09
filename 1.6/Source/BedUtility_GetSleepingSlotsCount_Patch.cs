using System.Collections.Concurrent;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using System.Threading;

namespace BunkBeds
{
    [HarmonyPatch(typeof(BedUtility), "GetSleepingSlotsCount")]
    public static class BedUtility_GetSleepingSlotsCount_Patch
    {
        public static ConcurrentDictionary<int, CompBunkBed> dictBunkBedComps = new();

        public static bool Prefix(ref int __result)
        {
            if (dictBunkBedComps.GetValueOrDefault(Thread.CurrentThread.ManagedThreadId, null) == null)
                return true;
            __result = dictBunkBedComps[Thread.CurrentThread.ManagedThreadId].Props.pawnCount;
            return false;
        }
    }
}
