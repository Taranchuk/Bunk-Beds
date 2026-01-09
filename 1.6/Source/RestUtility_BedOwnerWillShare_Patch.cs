using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace BunkBeds
{
    [HarmonyPatch(typeof(RestUtility), nameof(RestUtility.BedOwnerWillShare))]
    public static partial class RestUtility_BedOwnerWillShare_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            var isAnyOwnerLovePartnerOf = AccessTools.Method(typeof(RestUtility), nameof(RestUtility.IsAnyOwnerLovePartnerOf));
            var codes = codeInstructions.ToList();
            for (var i = 0; i < codes.Count; i++)
            {
                var code = codes[i];
                yield return code;
                if (code.opcode == OpCodes.Brtrue_S && codes[i - 1].Calls(isAnyOwnerLovePartnerOf))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RestUtility_BedOwnerWillShare_Patch), nameof(IsNotBunkBed)));
                    yield return new CodeInstruction(OpCodes.Brfalse_S, code.operand);
                }
            }
        }

        public static bool IsNotBunkBed(Building_Bed bed)
        {
            if (bed.IsBunkBed())
            {
                return false;
            }
            return true;
        }
    }
}
