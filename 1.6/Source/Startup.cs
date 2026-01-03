using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace BunkBeds
{
    [StaticConstructorOnStartup]
    public static class Utils
    {
        public static ThingWithComps bunkBed;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBunkBed(this ThingWithComps bed)
        {
            if (bunkBed != null && bunkBed == bed)
            {
                return true;
            }
            if (bed != null && CompBunkBed.bunkBeds.Contains(bed))
            {
                bunkBed = bed;
                return true;
            }
            return false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBunkBed(this ThingWithComps bed, out CompBunkBed comp)
        {
            comp = bed?.GetComp<CompBunkBed>();
            return comp != null;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class HotSwappableAttribute : Attribute
    {
    }
}
