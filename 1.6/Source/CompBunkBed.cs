using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace BunkBeds
{
    public enum Axis
    {
        X,
        Y,
        Z
    }

    public class RotationalOffsets
    {
        public List<Vector3> North = new List<Vector3>();
        public List<Vector3> South = new List<Vector3>();
        public List<Vector3> East = new List<Vector3>();
        public List<Vector3> West = new List<Vector3>();
    }

    public class RotationalGraphicSizes
    {
        public Vector2? North = null;
        public Vector2? South = null;
        public Vector2? East = null;
        public Vector2? West = null;
    }

    [HotSwappable]
    public class CompProperties_BunkBed : CompProperties
    {
        public List<GraphicData> bedTopGraphicDatas;
        public int pawnCount;
        public RotationalOffsets topGraphicOffsets;
        public RotationalOffsets pawnOffsets;
        public RotationalOffsets labelOffsets;
        public RotationalGraphicSizes graphicSizes;
        public bool preventSharingThought;

        public CompProperties_BunkBed()
        {
            this.compClass = typeof(CompBunkBed);
            topGraphicOffsets = new RotationalOffsets();
            pawnOffsets = new RotationalOffsets();
            labelOffsets = new RotationalOffsets();
            graphicSizes = new RotationalGraphicSizes();
        }
        
        public override void PostLoadSpecial(ThingDef parentDef)
        {
            base.PostLoadSpecial(parentDef);
            InitializeOffsetLists();
        }
        
        private void InitializeOffsetLists()
        {
            InitializeRotationalOffsets(pawnOffsets, pawnCount);
            int topGraphicsNeeded = System.Math.Max(0, pawnCount - 1);
            InitializeRotationalOffsets(topGraphicOffsets, topGraphicsNeeded);
            InitializeRotationalOffsets(labelOffsets, pawnCount);
        }
        
        private void InitializeRotationalOffsets(RotationalOffsets offsets, int count)
        {
            if (offsets.North == null || offsets.North.Count == 0)
                offsets.North = InitializeListWithZeroOffsets(count);
            if (offsets.South == null || offsets.South.Count == 0)
                offsets.South = InitializeListWithZeroOffsets(count);
            if (offsets.East == null || offsets.East.Count == 0)
                offsets.East = InitializeListWithZeroOffsets(count);
            if (offsets.West == null || offsets.West.Count == 0)
                offsets.West = InitializeListWithZeroOffsets(count);
        }
        
        private List<Vector3> InitializeListWithZeroOffsets(int count)
        {
            var list = new List<Vector3>();
            for (int i = 0; i < count; i++)
            {
                list.Add(Vector3.zero);
            }
            return list;
        }
    }

    [HotSwappable]
    public class CompBunkBed : ThingComp
    {
        public static HashSet<ThingWithComps> bunkBeds = new HashSet<ThingWithComps>();
        public CompProperties_BunkBed Props => props as CompProperties_BunkBed;
        public int BunkBedLevel => Props.pawnCount - 1;
        public List<Graphic> topGraphics;
        private Graphic originalGraphic;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            bunkBeds.Add(this.parent);
        }
        
        public override void PostDraw()
        {
            base.PostDraw();
            bool needsRebuild = false;
            if (topGraphics is null)
            {
                needsRebuild = true;
            }
            else
            {
                for (var i = 0; i < topGraphics.Count; i++)
                {
                    if (topGraphics[i].color != this.parent.DrawColor
                        || topGraphics[i].colorTwo != this.parent.DrawColorTwo)
                    {
                        needsRebuild = true;
                        break;
                    }
                }
            }
            if (needsRebuild)
            {
                UpdateParentGraphic();
                topGraphics = new List<Graphic>();
                foreach (var graphicData in Props.bedTopGraphicDatas)
                {
                    var modifiedGraphicData = graphicData;
                    if (Props.graphicSizes != null)
                    {
                        Vector2? sizeOverride = null;
                        switch (parent.Rotation.AsInt)
                        {
                            case 0: sizeOverride = Props.graphicSizes.North; break;
                            case 2: sizeOverride = Props.graphicSizes.South; break;
                            case 1: sizeOverride = Props.graphicSizes.East; break;
                            case 3: sizeOverride = Props.graphicSizes.West; break;
                        }
                        
                        if (sizeOverride.HasValue)
                        {
                            modifiedGraphicData = new GraphicData();
                            modifiedGraphicData.CopyFrom(graphicData);
                            modifiedGraphicData.drawSize = sizeOverride.Value;
                        }
                    }
                    
                    topGraphics.Add(modifiedGraphicData.GraphicColoredFor(this.parent));
                }
            }
            for (var i = 1; i < BunkBedLevel + 1; i++)
            {
                var drawPos = GetDrawOffsetForBunkBeds(this.parent.Rotation, i, this.parent.DrawPos, Props);
                topGraphics[i - 1].Draw(drawPos, parent.Rotation, parent);
            }
        }
        
        private void UpdateParentGraphic()
        {
            if (Props.graphicSizes != null)
            {
                Vector2? sizeOverride = null;
                switch (parent.Rotation.AsInt)
                {
                    case 0: sizeOverride = Props.graphicSizes.North; break;
                    case 2: sizeOverride = Props.graphicSizes.South; break;
                    case 1: sizeOverride = Props.graphicSizes.East; break;
                    case 3: sizeOverride = Props.graphicSizes.West; break;
                }

                if (originalGraphic == null)
                {
                    originalGraphic = parent.graphicInt;
                }

                if (sizeOverride.HasValue)
                {
                    var modifiedGraphicData = new GraphicData();
                    modifiedGraphicData.CopyFrom(parent.def.graphicData);
                    modifiedGraphicData.drawSize = sizeOverride.Value;
                    var modifiedGraphic = modifiedGraphicData.GraphicColoredFor(parent);
                    parent.graphicInt = modifiedGraphic;
                }
                else
                {
                    parent.graphicInt = originalGraphic;
                }
            }
        }

        public Vector3 GetDrawOffsetForBunkBeds(Rot4 rotation, int bunkLevel, Vector3 drawPos, CompProperties_BunkBed props)
        {
            drawPos.y += 1 + bunkLevel;
            drawPos.y += 0.001f * Mathf.Max(1, parent.Map.Size.z - parent.Position.z);
            List<Vector3> offsets = null;
            switch (rotation.AsInt)
            {
                case 0: offsets = props.topGraphicOffsets.North; break;
                case 1: offsets = props.topGraphicOffsets.East; break;
                case 2: offsets = props.topGraphicOffsets.South; break;
                case 3: offsets = props.topGraphicOffsets.West; break;
            }

            if (bunkLevel > 0 && offsets.Count >= bunkLevel)
            {
                drawPos += offsets[bunkLevel - 1];
            }
            return drawPos;
        }

        public Vector3 GetDrawOffsetForPawns(int bunkLevel, Vector3 drawPos)
        {
            drawPos.y += 0.5f + bunkLevel;
            List<Vector3> offsets = null;
            switch (parent.Rotation.AsInt)
            {
                case 0: offsets = Props.pawnOffsets.North; break;
                case 1: offsets = Props.pawnOffsets.East; break;
                case 2: offsets = Props.pawnOffsets.South; break;
                case 3: offsets = Props.pawnOffsets.West; break;
            }

            if (offsets.Count > bunkLevel)
            {
                drawPos += offsets[bunkLevel];
            }
            return drawPos;
        }

        public Vector3 GetDrawOffsetForLabels(int bunkLevel, Vector3 drawPos)
        {
            List<Vector3> offsets = null;
            switch (parent.Rotation.AsInt)
            {
                case 0: offsets = Props.labelOffsets.North; break;
                case 1: offsets = Props.labelOffsets.East; break;
                case 2: offsets = Props.labelOffsets.South; break;
                case 3: offsets = Props.labelOffsets.West; break;
            }

            if (offsets.Count > bunkLevel)
            {
                drawPos += offsets[bunkLevel];
            }
            return drawPos;
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (DebugSettings.ShowDevGizmos)
            {
                yield return new Command_Action
                {
                    defaultLabel = "Adjust Bunk Bed Offsets",
                    defaultDesc = "Adjust all bunk bed offsets",
                    icon = ContentFinder<Texture2D>.Get("UI/Buttons/DevRoot/OpenInspector", false) ?? BaseContent.BadTex,
                    action = delegate
                    {
                        Find.WindowStack.Add(new Dialog_BunkBedOffsets(this));
                    }
                };
            }
        }

        public override void DrawGUIOverlay()
        {
            base.DrawGUIOverlay();
            var bed = this.parent as Building_Bed;
            if (bed.Medical || Find.CameraDriver.CurrentZoom != 0 || !bed.PlayerCanSeeOwners)
            {
                return;
            }
            Color defaultThingLabelColor = GenMapUI.DefaultThingLabelColor;
            if (!bed.OwnersForReading.Any() && (Building_Bed_DrawGUIOverlay_Patch.guestBedType is null
                || Building_Bed_DrawGUIOverlay_Patch.guestBedType.IsAssignableFrom(this.parent.def.thingClass) is false))
            {
                GenMapUI.DrawThingLabel(bed, "Unowned".Translate(), defaultThingLabelColor);
                return;
            }
            if (bed.OwnersForReading.Count == 1)
            {
                Pawn pawn = bed.OwnersForReading[0];
                if ((!pawn.InBed() || pawn.CurrentBed() != bed) && (!pawn.RaceProps.Animal || Prefs.AnimalNameMode.ShouldDisplayAnimalName(pawn)))
                {
                    GenMapUI.DrawThingLabel(this.parent, pawn.LabelShort, defaultThingLabelColor);
                }
                return;
            }
            for (int i = 0; i < bed.OwnersForReading.Count; i++)
            {
                Pawn pawn2 = bed.OwnersForReading[i];
                GenMapUI.DrawThingLabel(GetMultiOwnersLabelScreenPosFor(i), pawn2.LabelShort, defaultThingLabelColor);
            }
        }
        private Vector3 GetMultiOwnersLabelScreenPosFor(int slotIndex)
        {
            Vector3 drawPos = this.parent.DrawPos;
            var result = this.GetDrawOffsetForLabels(slotIndex, drawPos).MapToUIPosition();
            return result;
        }
    }
}
