using System;
using System.Runtime.CompilerServices;
using Verse;

namespace BunkBeds
{
    [StaticConstructorOnStartup]
    public static class Utils
    {
        public static ThingWithComps bunkBed;
        public static CompBunkBed compBunkBed;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBunkBed(this ThingWithComps bed)
        {
            if (bed is null) return false;
            if (bunkBed == bed)
            {
                return true;
            }
            if (CompBunkBed.bunkBeds.ContainsKey(bed.thingIDNumber))
            {
                bunkBed = bed;
                return true;
            }
            return false;
        }

        public static ThingWithComps bunkBed2;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBunkBed(this ThingWithComps bed, out CompBunkBed comp)
        {
            if (bed != null && bed == bunkBed2)
            {
                comp = compBunkBed;
                return comp != null;
            }
            if (bed != null && CompBunkBed.bunkBeds.TryGetValue(bed.thingIDNumber, out comp))
            {
                bunkBed2 = bed;
                compBunkBed = comp;
                return true;
            }
            comp = null;
            return false;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class HotSwappableAttribute : Attribute
    {
    }
}
