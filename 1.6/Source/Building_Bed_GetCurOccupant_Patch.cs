using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace BunkBeds
{
    [HotSwappable]
    [HarmonyPatch(typeof(Building_Bed), "GetCurOccupant")]
    public static class Building_Bed_GetCurOccupant_Patch
    {
        public static bool Prefix(Building_Bed __instance, ref Pawn __result, int slotIndex)
        {
            if (__instance.IsBunkBed(out var comp))
            {
                __result = GetCurOccupant(__instance, comp, slotIndex);
                return false;
            }
            return true;
        }

        public static Pawn GetCurOccupant(Building_Bed __instance, CompBunkBed bunkBedComp, int slotIndex)
        {
            if (!__instance.Spawned)
                return null;

            var comp = bunkBedComp.cachedCompAssignableToPawn;
            // Incorrect slotIndex
            if (slotIndex < 0 || slotIndex >= bunkBedComp.Props.pawnCount)
                return null;

            // Special handling for medical beds.
            // Since no pawns are assigned slots in medical beds, we need to do a workaround here.
            // We could use this workaround for normal sleep as well, but the other approach is much faster.
            // This approach would also cause pawn name labels to not match their sleeping slots.
            // Only use if the other approach has bugs with it.
            if (__instance.Medical)
            {
                var sleepingSlotPos = __instance.Position;
                var list = __instance.Map.thingGrid.ThingsListAt(sleepingSlotPos);

                var pawnIndex = 0;
                for (var i = 0; i < list.Count; i++)
                {
                    if (list[i] is Pawn pawn && pawn.CurJob != null && pawn.GetPosture().InBed())
                    {
                        // If the current pawn we found matches the index, return them
                        if (pawnIndex == slotIndex)
                            return pawn;

                        // If the current pawn doesn't match the index, increment the index and keep looking for a pawn
                        pawnIndex++;
                    }
                }
            }
            // Check if the list is actually filled up to the slot we're trying to check
            else if (slotIndex < comp.AssignedPawnsForReading.Count)
            {
                // No pawn is assigned or pawn not in bed posture
                var sleepingSlotPos = __instance.Position;
                var pawn = comp.AssignedPawnsForReading[slotIndex];
                // If the pawn is not null, has a job, currently has the same map and position as the sleeping spot,
                // and has in bed posture, then the pawn is in bed. We could check ThingList at sleeping spot
                // position like vanilla, but since we know the pawn who's using this slot - we can skip it.
                if (pawn?.CurJob != null && pawn.Map == __instance.Map && pawn.Position == sleepingSlotPos && pawn.GetPosture().InBed())
                    return pawn;
            }

            return null;
        }
    }

}
