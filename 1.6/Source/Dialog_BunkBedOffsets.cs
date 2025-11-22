using RimWorld;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace BunkBeds
{
    [HotSwappable]
    public class Dialog_BunkBedOffsets : Window
    {
        private CompBunkBed bunkBed;
        private Vector2 scrollPosition = Vector2.zero;
        private float scrollViewHeight = 0f;
        private string windowTitle = "Bunk Bed Offsets";

        public Dialog_BunkBedOffsets(CompBunkBed bunkBed)
        {
            this.bunkBed = bunkBed;
            this.doCloseX = true;
            this.doCloseButton = false;
            this.closeOnClickedOutside = false;
            this.absorbInputAroundWindow = true;
            this.draggable = true;
            this.preventCameraMotion = false;
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(0, 0, inRect.width, 30f), windowTitle);
            Text.Font = GameFont.Small;

            Rect scrollViewRect = new Rect(0, 40f, inRect.width, inRect.height - 80f);
            Rect viewRect = new Rect(0, 0, scrollViewRect.width - 20f, scrollViewHeight);

            Widgets.BeginScrollView(scrollViewRect, ref scrollPosition, viewRect);

            float currentY = 0f;
            float sliderHeight = 40f;
            Widgets.Label(new Rect(0, currentY, viewRect.width, 25f), "Pawn Offsets");
            currentY += 30f;

            for (int i = 0; i < bunkBed.Props.pawnCount; i++)
            {
                Widgets.Label(new Rect(0, currentY, viewRect.width, 25f), $"Level {i}");
                currentY += 25f;
                currentY = DrawPawnOffsetSlidersForRotation(viewRect, currentY, i, Rot4.North, sliderHeight, "North");
                currentY = DrawPawnOffsetSlidersForRotation(viewRect, currentY, i, Rot4.South, sliderHeight, "South");
                currentY = DrawPawnOffsetSlidersForRotation(viewRect, currentY, i, Rot4.East, sliderHeight, "East");
                currentY = DrawPawnOffsetSlidersForRotation(viewRect, currentY, i, Rot4.West, sliderHeight, "West");

                Rect resetButtonRect = new Rect(viewRect.width - 210f, currentY, 100f, 30f);
                if (Widgets.ButtonText(resetButtonRect, "Reset"))
                {
                    ResetPawnOffsetsForLevel(i);
                }
                currentY += 40f;
            }
            currentY += 10f;
            Widgets.Label(new Rect(0, currentY, viewRect.width, 25f), "Top Graphic Offsets");
            currentY += 30f;

            int topGraphicsCount = bunkBed.Props.bedTopGraphicDatas.Count;
            for (int i = 0; i < topGraphicsCount; i++)
            {
                Widgets.Label(new Rect(0, currentY, viewRect.width, 25f), $"Level {i}");
                currentY += 25f;
                currentY = DrawTopGraphicOffsetSlidersForRotation(viewRect, currentY, i, Rot4.North, sliderHeight, "North");
                currentY = DrawTopGraphicOffsetSlidersForRotation(viewRect, currentY, i, Rot4.South, sliderHeight, "South");
                currentY = DrawTopGraphicOffsetSlidersForRotation(viewRect, currentY, i, Rot4.East, sliderHeight, "East");
                currentY = DrawTopGraphicOffsetSlidersForRotation(viewRect, currentY, i, Rot4.West, sliderHeight, "West");
                Rect resetButtonRect = new Rect(viewRect.width - 210f, currentY, 100f, 30f);
                if (Widgets.ButtonText(resetButtonRect, "Reset"))
                {
                    ResetTopGraphicOffsetsForLevel(i);
                }
                currentY += 40f;
            }
            currentY += 10f;
            Widgets.Label(new Rect(0, currentY, viewRect.width, 25f), "Label Offsets");
            currentY += 30f;

            for (int i = 0; i < bunkBed.Props.pawnCount; i++)
            {
                Widgets.Label(new Rect(0, currentY, viewRect.width, 25f), $"Level {i}");
                currentY += 25f;
                currentY = DrawLabelOffsetSlidersForRotation(viewRect, currentY, i, Rot4.North, sliderHeight, "North");
                currentY = DrawLabelOffsetSlidersForRotation(viewRect, currentY, i, Rot4.South, sliderHeight, "South");
                currentY = DrawLabelOffsetSlidersForRotation(viewRect, currentY, i, Rot4.East, sliderHeight, "East");
                currentY = DrawLabelOffsetSlidersForRotation(viewRect, currentY, i, Rot4.West, sliderHeight, "West");

                Rect resetButtonRect = new Rect(viewRect.width - 210f, currentY, 100f, 30f);
                if (Widgets.ButtonText(resetButtonRect, "Reset"))
                {
                    ResetLabelOffsetsForLevel(i);
                }
                currentY += 40f;
            }

            scrollViewHeight = currentY + 50f;

            Widgets.EndScrollView();
            Rect buttonRect = new Rect((inRect.width - 100f) / 2f, inRect.height - 40f, 100f, 30f);
            if (Widgets.ButtonText(buttonRect, "Copy XML"))
            {
                string xmlString = GenerateXmlString(bunkBed);
                GUIUtility.systemCopyBuffer = xmlString;
                Messages.Message("XML copied to clipboard", MessageTypeDefOf.SilentInput, false);
            }
        }

        private float DrawXYZSliders(Rect viewRect, float currentY, float sliderHeight, ref Vector3 offset)
        {
            float sliderWidth = (viewRect.width) / 3f;
            sliderWidth -= 10f;
            currentY = DrawSingleSlider(viewRect, currentY, sliderHeight, sliderWidth, 0, "X", ref offset.x);
            currentY = DrawSingleSlider(viewRect, currentY, sliderHeight, sliderWidth, 1, "Y", ref offset.y);
            currentY = DrawSingleSlider(viewRect, currentY, sliderHeight, sliderWidth, 2, "Z", ref offset.z);
            
            currentY += sliderHeight;
            return currentY;
        }
        
        private float DrawSingleSlider(Rect viewRect, float currentY, float sliderHeight, float sliderWidth, int axisIndex, string label, ref float value)
        {
            Vector2 labelSize = Text.CalcSize(label);
            float sliderStartOffset = sliderWidth / 7f;
            float sliderPadding = sliderWidth / 3.75f;
            
            float xOffset = axisIndex * sliderWidth;
            float labelX = xOffset;
            float sliderX = xOffset + sliderStartOffset;
            float sliderWidthAdjusted = sliderWidth - sliderPadding;
            float valueLabelX = sliderX + sliderWidthAdjusted - sliderStartOffset;
            float resetButtonX = sliderX + sliderWidthAdjusted;
            Rect labelRect = new Rect(labelX, currentY, labelSize.x, sliderHeight);
            Widgets.Label(labelRect, label);
            float verticalPadding = sliderHeight / 6f;
            float verticalPaddingDouble = sliderHeight / 3f;
            Rect sliderRect = new Rect(sliderX, currentY + verticalPadding, sliderWidthAdjusted, sliderHeight - verticalPaddingDouble);
            Widgets.HorizontalSlider(sliderRect, ref value, new FloatRange(-2f, 2f), value.ToString("F2"), 0f);
            Rect resetButtonRect = new Rect(resetButtonX, currentY, 24, sliderHeight);
            if (Widgets.ButtonText(resetButtonRect, "R"))
            {
                value = 0f;
            }
            
            return currentY;
        }

        private float DrawPawnOffsetSlidersForRotation(Rect viewRect, float currentY, int level, Rot4 rotation, float sliderHeight, string rotationName)
        {
            Widgets.Label(new Rect(0, currentY, viewRect.width, 25f), $"{rotationName} Rotation");
            currentY += 25f;

            switch (rotation.AsInt)
            {
                case 0:
                    if (bunkBed.Props.pawnOffsets.North.Count > level)
                    {
                        Vector3 offsetNorth = bunkBed.Props.pawnOffsets.North[level];
                        currentY = DrawXYZSliders(viewRect, currentY, sliderHeight, ref offsetNorth);
                        bunkBed.Props.pawnOffsets.North[level] = offsetNorth;
                    }
                    break;
                case 1:
                    if (bunkBed.Props.pawnOffsets.East.Count > level)
                    {
                        Vector3 offsetEast = bunkBed.Props.pawnOffsets.East[level];
                        currentY = DrawXYZSliders(viewRect, currentY, sliderHeight, ref offsetEast);
                        bunkBed.Props.pawnOffsets.East[level] = offsetEast;
                    }
                    break;
                case 2:
                    if (bunkBed.Props.pawnOffsets.South.Count > level)
                    {
                        Vector3 offsetSouth = bunkBed.Props.pawnOffsets.South[level];
                        currentY = DrawXYZSliders(viewRect, currentY, sliderHeight, ref offsetSouth);
                        bunkBed.Props.pawnOffsets.South[level] = offsetSouth;
                    }
                    break;
                case 3:
                    if (bunkBed.Props.pawnOffsets.West.Count > level)
                    {
                        Vector3 offsetWest = bunkBed.Props.pawnOffsets.West[level];
                        currentY = DrawXYZSliders(viewRect, currentY, sliderHeight, ref offsetWest);
                        bunkBed.Props.pawnOffsets.West[level] = offsetWest;
                    }
                    break;
            }

            return currentY;
        }

        private float DrawTopGraphicOffsetSlidersForRotation(Rect viewRect, float currentY, int level, Rot4 rotation, float sliderHeight, string rotationName)
        {
            Widgets.Label(new Rect(0, currentY, viewRect.width, 25f), $"{rotationName} Rotation");
            currentY += 25f;

            switch (rotation.AsInt)
            {
                case 0:
                    if (bunkBed.Props.topGraphicOffsets.North.Count > level)
                    {
                        Vector3 offsetNorth = bunkBed.Props.topGraphicOffsets.North[level];
                        currentY = DrawXYZSliders(viewRect, currentY, sliderHeight, ref offsetNorth);
                        bunkBed.Props.topGraphicOffsets.North[level] = offsetNorth;
                    }
                    break;
                case 1:
                    if (bunkBed.Props.topGraphicOffsets.East.Count > level)
                    {
                        Vector3 offsetEast = bunkBed.Props.topGraphicOffsets.East[level];
                        currentY = DrawXYZSliders(viewRect, currentY, sliderHeight, ref offsetEast);
                        bunkBed.Props.topGraphicOffsets.East[level] = offsetEast;
                    }
                    break;
                case 2:
                    if (bunkBed.Props.topGraphicOffsets.South.Count > level)
                    {
                        Vector3 offsetSouth = bunkBed.Props.topGraphicOffsets.South[level];
                        currentY = DrawXYZSliders(viewRect, currentY, sliderHeight, ref offsetSouth);
                        bunkBed.Props.topGraphicOffsets.South[level] = offsetSouth;
                    }
                    break;
                case 3:
                    if (bunkBed.Props.topGraphicOffsets.West.Count > level)
                    {
                        Vector3 offsetWest = bunkBed.Props.topGraphicOffsets.West[level];
                        currentY = DrawXYZSliders(viewRect, currentY, sliderHeight, ref offsetWest);
                        bunkBed.Props.topGraphicOffsets.West[level] = offsetWest;
                    }
                    break;
            }
           return currentY;
       }

       private float DrawLabelOffsetSlidersForRotation(Rect viewRect, float currentY, int level, Rot4 rotation, float sliderHeight, string rotationName)
        {
            Widgets.Label(new Rect(0, currentY, viewRect.width, 25f), $"{rotationName} Rotation");
            currentY += 25f;

            switch (rotation.AsInt)
            {
                case 0:
                    if (bunkBed.Props.labelOffsets.North.Count > level)
                    {
                        Vector3 offsetNorth = bunkBed.Props.labelOffsets.North[level];
                        currentY = DrawXYZSliders(viewRect, currentY, sliderHeight, ref offsetNorth);
                        bunkBed.Props.labelOffsets.North[level] = offsetNorth;
                    }
                    break;
                case 1:
                    if (bunkBed.Props.labelOffsets.East.Count > level)
                    {
                        Vector3 offsetEast = bunkBed.Props.labelOffsets.East[level];
                        currentY = DrawXYZSliders(viewRect, currentY, sliderHeight, ref offsetEast);
                        bunkBed.Props.labelOffsets.East[level] = offsetEast;
                    }
                    break;
                case 2:
                    if (bunkBed.Props.labelOffsets.South.Count > level)
                    {
                        Vector3 offsetSouth = bunkBed.Props.labelOffsets.South[level];
                        currentY = DrawXYZSliders(viewRect, currentY, sliderHeight, ref offsetSouth);
                        bunkBed.Props.labelOffsets.South[level] = offsetSouth;
                    }
                    break;
                case 3:
                    if (bunkBed.Props.labelOffsets.West.Count > level)
                    {
                        Vector3 offsetWest = bunkBed.Props.labelOffsets.West[level];
                        currentY = DrawXYZSliders(viewRect, currentY, sliderHeight, ref offsetWest);
                        bunkBed.Props.labelOffsets.West[level] = offsetWest;
                    }
                    break;
            }

            return currentY;
        }

        private void ResetPawnOffsetsForLevel(int level)
        {
            if (level < bunkBed.Props.pawnOffsets.North.Count)
                bunkBed.Props.pawnOffsets.North[level] = Vector3.zero;
            if (level < bunkBed.Props.pawnOffsets.South.Count)
                bunkBed.Props.pawnOffsets.South[level] = Vector3.zero;
            if (level < bunkBed.Props.pawnOffsets.East.Count)
                bunkBed.Props.pawnOffsets.East[level] = Vector3.zero;
            if (level < bunkBed.Props.pawnOffsets.West.Count)
                bunkBed.Props.pawnOffsets.West[level] = Vector3.zero;
        }

        private void ResetTopGraphicOffsetsForLevel(int level)
        {
            if (level < bunkBed.Props.topGraphicOffsets.North.Count)
                bunkBed.Props.topGraphicOffsets.North[level] = Vector3.zero;
            if (level < bunkBed.Props.topGraphicOffsets.South.Count)
                bunkBed.Props.topGraphicOffsets.South[level] = Vector3.zero;
            if (level < bunkBed.Props.topGraphicOffsets.East.Count)
                bunkBed.Props.topGraphicOffsets.East[level] = Vector3.zero;
            if (level < bunkBed.Props.topGraphicOffsets.West.Count)
                bunkBed.Props.topGraphicOffsets.West[level] = Vector3.zero;
        }

        private void ResetLabelOffsetsForLevel(int level)
        {
            if (level < bunkBed.Props.labelOffsets.North.Count)
                bunkBed.Props.labelOffsets.North[level] = Vector3.zero;
            if (level < bunkBed.Props.labelOffsets.South.Count)
                bunkBed.Props.labelOffsets.South[level] = Vector3.zero;
            if (level < bunkBed.Props.labelOffsets.East.Count)
                bunkBed.Props.labelOffsets.East[level] = Vector3.zero;
            if (level < bunkBed.Props.labelOffsets.West.Count)
                bunkBed.Props.labelOffsets.West[level] = Vector3.zero;
        }

        private void ResetLabelOffsetsVertical()
        {
            for (int i = 0; i < bunkBed.Props.labelOffsets.North.Count; i++)
            {
                bunkBed.Props.labelOffsets.North[i] = Vector3.zero;
            }
            for (int i = 0; i < bunkBed.Props.labelOffsets.South.Count; i++)
            {
                bunkBed.Props.labelOffsets.South[i] = Vector3.zero;
            }
        }

        private void ResetLabelOffsetsHorizontal()
        {
            for (int i = 0; i < bunkBed.Props.labelOffsets.East.Count; i++)
            {
                bunkBed.Props.labelOffsets.East[i] = Vector3.zero;
            }
            for (int i = 0; i < bunkBed.Props.labelOffsets.West.Count; i++)
            {
                bunkBed.Props.labelOffsets.West[i] = Vector3.zero;
            }
        }

        public static string GenerateXmlString(CompBunkBed bunkBed)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("                <pawnOffsets>");
            sb.AppendLine("                    <North>");
            foreach (Vector3 offset in bunkBed.Props.pawnOffsets.North)
            {
                sb.AppendLine($"                        <li>({offset.x:F3},{offset.y:F3},{offset.z:F3})</li>");
            }
            sb.AppendLine("                    </North>");
            sb.AppendLine("                    <South>");
            foreach (Vector3 offset in bunkBed.Props.pawnOffsets.South)
            {
                sb.AppendLine($"                        <li>({offset.x:F3},{offset.y:F3},{offset.z:F3})</li>");
            }
            sb.AppendLine("                    </South>");
            sb.AppendLine("                    <East>");
            foreach (Vector3 offset in bunkBed.Props.pawnOffsets.East)
            {
                sb.AppendLine($"                        <li>({offset.x:F3},{offset.y:F3},{offset.z:F3})</li>");
            }
            sb.AppendLine("                    </East>");
            sb.AppendLine("                    <West>");
            foreach (Vector3 offset in bunkBed.Props.pawnOffsets.West)
            {
                sb.AppendLine($"                        <li>({offset.x:F3},{offset.y:F3},{offset.z:F3})</li>");
            }
            sb.AppendLine("                    </West>");

            sb.AppendLine("                </pawnOffsets>");
            sb.AppendLine("                <topGraphicOffsets>");
            sb.AppendLine("                    <North>");
            foreach (Vector3 offset in bunkBed.Props.topGraphicOffsets.North)
            {
                sb.AppendLine($"                        <li>({offset.x:F3},{offset.y:F3},{offset.z:F3})</li>");
            }
            sb.AppendLine("                    </North>");
            sb.AppendLine("                    <South>");
            foreach (Vector3 offset in bunkBed.Props.topGraphicOffsets.South)
            {
                sb.AppendLine($"                        <li>({offset.x:F3},{offset.y:F3},{offset.z:F3})</li>");
            }
            sb.AppendLine("                    </South>");
            sb.AppendLine("                    <East>");
            foreach (Vector3 offset in bunkBed.Props.topGraphicOffsets.East)
            {
                sb.AppendLine($"                        <li>({offset.x:F3},{offset.y:F3},{offset.z:F3})</li>");
            }
            sb.AppendLine("                    </East>");
            sb.AppendLine("                    <West>");
            foreach (Vector3 offset in bunkBed.Props.topGraphicOffsets.West)
            {
                sb.AppendLine($"                        <li>({offset.x:F3},{offset.y:F3},{offset.z:F3})</li>");
            }
            sb.AppendLine("                    </West>");

            sb.AppendLine("                </topGraphicOffsets>");
            sb.AppendLine("                <labelOffsets>");
            sb.AppendLine("                    <North>");
            foreach (Vector3 offset in bunkBed.Props.labelOffsets.North)
            {
                sb.AppendLine($"                        <li>({offset.x:F3},{offset.y:F3},{offset.z:F3})</li>");
            }
            sb.AppendLine("                    </North>");
            sb.AppendLine("                    <South>");
            foreach (Vector3 offset in bunkBed.Props.labelOffsets.South)
            {
                sb.AppendLine($"                        <li>({offset.x:F3},{offset.y:F3},{offset.z:F3})</li>");
            }
            sb.AppendLine("                    </South>");
            sb.AppendLine("                    <East>");
            foreach (Vector3 offset in bunkBed.Props.labelOffsets.East)
            {
                sb.AppendLine($"                        <li>({offset.x:F3},{offset.y:F3},{offset.z:F3})</li>");
            }
            sb.AppendLine("                    </East>");
            sb.AppendLine("                    <West>");
            foreach (Vector3 offset in bunkBed.Props.labelOffsets.West)
            {
                sb.AppendLine($"                        <li>({offset.x:F3},{offset.y:F3},{offset.z:F3})</li>");
            }
            sb.AppendLine("                    </West>");

            sb.AppendLine("                </labelOffsets>");

            return sb.ToString();
        }
    }
}
