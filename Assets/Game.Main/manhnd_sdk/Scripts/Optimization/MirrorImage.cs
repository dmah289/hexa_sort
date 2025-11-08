using System;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
#endif
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

namespace Zego
{
    public enum MirrorMode
    {
        Horizontal,
        Vertical,
        Both
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(MirrorImage)), CanEditMultipleObjects]
    public class MirrorImageEditor : ImageEditor
    {
        SerializedProperty m_PixelsPerUnitMultiplier1;
        private SerializedProperty m_mirrorMode;
        private SerializedProperty m_fillAmount;
        private SerializedProperty m_tileCenter;

        protected override void OnEnable()
        {
            base.OnEnable();
            this.m_mirrorMode = base.serializedObject.FindProperty("mirrorMode");
            this.m_fillAmount = base.serializedObject.FindProperty("m_FillAmount");
            this.m_tileCenter = base.serializedObject.FindProperty("tileCenter");
            m_PixelsPerUnitMultiplier1 = serializedObject.FindProperty("m_PixelsPerUnitMultiplier");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Image image = target as Image;
            if (image.type != Image.Type.Sliced)
                image.type = Image.Type.Sliced;
            if (image.fillMethod != Image.FillMethod.Horizontal)
                image.fillMethod = Image.FillMethod.Horizontal;
            RectTransform rect = image.GetComponent<RectTransform>();
            //  m_bIsDriven = (rect.drivenByObject as Slider)?.fillRect == rect;

            SpriteGUI();
            AppearanceControlsGUI();
            RaycastControlsGUI();
            MaskableControlsGUI();

            EditorGUILayout.PropertyField(this.m_mirrorMode, new GUILayoutOption[0]);

            if (image.type == Image.Type.Sliced)
            {
                EditorGUILayout.PropertyField(this.m_fillAmount, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_tileCenter, new GUILayoutOption[0]);
            }

            EditorGUILayout.PropertyField(this.m_PixelsPerUnitMultiplier1, new GUILayoutOption[0]);
            base.serializedObject.ApplyModifiedProperties();
        }

        [MenuItem("CONTEXT/Image/Swap Mirror Image")]
        public static void MakeBetter(MenuCommand command)
        {
            if (command.context is MirrorImage)
            {
                MirrorImage oldImage = command.context as MirrorImage;

                if (oldImage == null)
                {
                    Debug.LogError("Không tìm thấy Image để thay thế!");
                    return;
                }

                var goOld = oldImage.gameObject;
                var go = new GameObject();
                var mirror = go.AddComponent<Image>();
                SerializedObject serializedObject = new SerializedObject(oldImage);
                serializedObject.FindProperty("m_Script").objectReferenceValue =
                    MonoScript.FromMonoBehaviour(mirror as MonoBehaviour);

                serializedObject.ApplyModifiedProperties();
                WaitSetVerticesDirty(goOld);
                DestroyImmediate(go);
            }
            else
            {
                Image oldImage = command.context as Image;


                if (oldImage == null)
                {
                    Debug.LogError("Không tìm thấy Image để thay thế!");
                    return;
                }

                var goOld = oldImage.gameObject;
                var go = new GameObject();
                var mirror = go.AddComponent<MirrorImage>();
                SerializedObject serializedObject = new SerializedObject(oldImage);
                serializedObject.FindProperty("m_Script").objectReferenceValue =
                    MonoScript.FromMonoBehaviour(mirror as MonoBehaviour);
                serializedObject.ApplyModifiedProperties();
                WaitSetVerticesDirty(goOld);
                DestroyImmediate(go);
            }
        }

        static async void WaitSetVerticesDirty(GameObject go)
        {
            await Task.Delay(100);
            go.GetComponent<Image>().SetVerticesDirty();
        }
    }
#endif

    [RequireComponent(typeof(RectTransform))]
    public class MirrorImage : Image
    {
        public MirrorMode mirrorMode;
        public bool tileCenter;
        static readonly Vector2[] s_VertScratch = new Vector2[4];
        static readonly Vector2[] s_UVScratch = new Vector2[4];
        static readonly Vector2[] s_DefaultUVScratch = new Vector2[4];
        static readonly Vector2[] s_TempVertScratch = new Vector2[4];
        static readonly Vector2[] s_fillAmount = new Vector2[3];

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if (sprite == null)
            {
                base.OnPopulateMesh(toFill);
                return;
            }

            if (!hasBorder)
            {
                GenerateSimpleSprite(toFill, false);
                return;
            }

            Vector4 outer, inner, padding, border;

            if (overrideSprite != null)
            {
                outer = DataUtility.GetOuterUV(overrideSprite);
                inner = DataUtility.GetInnerUV(overrideSprite);
                padding = DataUtility.GetPadding(overrideSprite);
                border = overrideSprite.border;
            }
            else
            {
                outer = Vector4.zero;
                inner = Vector4.zero;
                padding = Vector4.zero;
                border = Vector4.zero;
            }

            if (border.x == 0 && border.y == 0 && border.z == 0 && border.w == 0)
            {
                base.OnPopulateMesh(toFill);
                return;
            }


            Vector2 widthHeight = new Vector2(GetPixelAdjustedRect().width, GetPixelAdjustedRect().height);
            Rect rect = GetPixelAdjustedRect();
            if (border.x != 0 && border.z == 0 &&
                (mirrorMode == MirrorMode.Horizontal || mirrorMode == MirrorMode.Both))
            {
                rect.width = widthHeight.x / 2;
                border.x = border.x * 2;
            }
            else if (border.x == 0 && border.z != 0 &&
                     (mirrorMode == MirrorMode.Horizontal || mirrorMode == MirrorMode.Both))
            {
                rect.width = widthHeight.x / 2;
                border.z = border.z * 2;
            }


            if (border.y != 0 && border.w == 0 && (mirrorMode == MirrorMode.Vertical || mirrorMode == MirrorMode.Both))
            {
                rect.height = widthHeight.y / 2;
                border.y = border.y * 2;
            }

            if (border.y == 0 && border.w != 0 && (mirrorMode == MirrorMode.Vertical || mirrorMode == MirrorMode.Both))
            {
                rect.height = widthHeight.y / 2;
                border.w = border.w * 2;
            }

            Vector4 adjustedBorders = GetAdjustedBorders(border / multipliedPixelsPerUnit, rect);
            padding = padding / multipliedPixelsPerUnit;

            s_VertScratch[0] = new Vector2(padding.x, padding.y);
            s_VertScratch[3] = new Vector2(rect.width - padding.z, rect.height - padding.w);

            s_VertScratch[1].x = adjustedBorders.x;
            s_VertScratch[1].y = adjustedBorders.y;

            s_VertScratch[2].x = rect.width - adjustedBorders.z;
            s_VertScratch[2].y = rect.height - adjustedBorders.w;

            if (border.x != 0 && border.z == 0 &&
                (mirrorMode == MirrorMode.Horizontal || mirrorMode == MirrorMode.Both))
            {
                s_VertScratch[1].x = adjustedBorders.x;
                s_VertScratch[3].x = rect.width * 2 - padding.z;
                s_VertScratch[2].x = s_VertScratch[3].x - s_VertScratch[1].x;
            }
            else if (border.x == 0 && border.z != 0 &&
                     (mirrorMode == MirrorMode.Horizontal || mirrorMode == MirrorMode.Both))
            {
                s_VertScratch[1].x = adjustedBorders.z;
                s_VertScratch[3].x = rect.width * 2 - padding.z;
                s_VertScratch[2].x = s_VertScratch[3].x - s_VertScratch[1].x;
            }

            if (border.y != 0 && border.w == 0 && (mirrorMode == MirrorMode.Vertical || mirrorMode == MirrorMode.Both))
            {
                s_VertScratch[1].y = adjustedBorders.y;
                s_VertScratch[3].y = rect.height * 2 - padding.w;
                s_VertScratch[2].y = s_VertScratch[3].y - s_VertScratch[1].y;
            }
            else if (border.y == 0 && border.w != 0 &&
                     (mirrorMode == MirrorMode.Vertical || mirrorMode == MirrorMode.Both))
            {
                s_VertScratch[1].y = adjustedBorders.w;
                s_VertScratch[3].y = rect.height * 2 - padding.w;
                s_VertScratch[2].y = s_VertScratch[3].y - s_VertScratch[1].y;
            }


            for (int i = 0; i < 4; ++i)
            {
                s_VertScratch[i].x += rect.x;
                s_VertScratch[i].y += rect.y;
            }

            s_UVScratch[0] = new Vector2(outer.x, outer.y);
            s_UVScratch[1] = new Vector2(inner.x, inner.y);
            s_UVScratch[2] = new Vector2(inner.z, inner.w);
            s_UVScratch[3] = new Vector2(outer.z, outer.w);

            for (int i = 0; i < s_DefaultUVScratch.Length; ++i)
            {
                s_DefaultUVScratch[i] = s_UVScratch[i];
            }

            if (border.x != 0 && border.z == 0 &&
                (mirrorMode == MirrorMode.Horizontal || mirrorMode == MirrorMode.Both))
            {
                s_UVScratch[3].x = s_UVScratch[0].x;
                s_UVScratch[2].x = s_UVScratch[1].x;
            }
            else if (border.x == 0 && border.z != 0 &&
                     (mirrorMode == MirrorMode.Horizontal || mirrorMode == MirrorMode.Both))
            {
                s_UVScratch[0].x = s_UVScratch[3].x;
                s_UVScratch[1].x = s_UVScratch[2].x;
            }

            if (border.y != 0 && border.w == 0 && (mirrorMode == MirrorMode.Vertical || mirrorMode == MirrorMode.Both))
            {
                s_UVScratch[3].y = s_UVScratch[0].y;
                s_UVScratch[2].y = s_UVScratch[1].y;
            }
            else if (border.y == 0 && border.w != 0 &&
                     (mirrorMode == MirrorMode.Vertical || mirrorMode == MirrorMode.Both))
            {
                s_UVScratch[0].y = s_UVScratch[3].y;
                s_UVScratch[1].y = s_UVScratch[2].y;
            }


            toFill.Clear();

            for (int i = 0; i < 3; ++i)
            {
                s_fillAmount[i] = Vector2.one;
            }

            //     if (type == Type.Filled)
            {
                for (int i = 0; i < 4; ++i)
                {
                    s_TempVertScratch[i] = s_VertScratch[i];
                }

                for (int x = 0; x < 3; ++x)
                {
                    s_fillAmount[x] = GetFillAmount(x, 0, widthHeight);
                }

                if (fillOrigin == 0)
                {
                    for (int x = 0; x < 3; ++x)
                    {
                        for (int j = x + 1; j < 4; ++j)
                        {
                            var size = s_VertScratch[j].x - s_VertScratch[j - 1].x;
                            s_TempVertScratch[j].x = s_TempVertScratch[j - 1].x + size * s_fillAmount[x].x;
                        }
                    }
                }
                else
                {
                    for (int x = 3; x > 0; --x)
                    {
                        for (int j = x - 1; j >= 0; --j)
                        {
                            var size = s_VertScratch[x].x - s_VertScratch[j].x;
                            s_TempVertScratch[j].x = s_TempVertScratch[x].x - size * s_fillAmount[j].x;
                        }
                    }
                }


                for (int i = 0; i < 4; ++i)
                {
                    s_VertScratch[i] = s_TempVertScratch[i];
                }
            }
            Vector2 fullBorder = new Vector2(border.x + border.z, border.y + border.w);

            for (int x = 0; x < 3; ++x)
            {
                int x2 = x + 1;


                for (int y = 0; y < 3; ++y)
                {
                    if (!fillCenter && x == 1 && y == 1)
                        continue;

                    int y2 = y + 1;

                    // Check for zero or negative dimensions to prevent invalid quads (UUM-71372)
                    if ((s_VertScratch[x2].x - s_VertScratch[x].x <= 0) ||
                        (s_VertScratch[y2].y - s_VertScratch[y].y <= 0))
                        continue;
                    Vector2 fillAmountQuad = s_fillAmount[x];

                    if (x == 1 && tileCenter)
                    {
                        AddQuad(toFill,
                            new Vector2(s_VertScratch[x].x, s_VertScratch[y].y),
                            new Vector2(s_VertScratch[x2].x, s_VertScratch[y2].y),
                            color,
                            new Vector2(s_DefaultUVScratch[x].x, s_DefaultUVScratch[y].y),
                            new Vector2(s_DefaultUVScratch[x2].x, s_DefaultUVScratch[y2].y), new Vector2Int(x, y),
                            overrideSprite.rect.size - new Vector2(overrideSprite.border.x + overrideSprite.border.z,
                                overrideSprite.border.y + overrideSprite.border.w), tileCenter, fillAmountQuad);
                    }
                    else
                    {
                        AddQuad(toFill,
                            new Vector2(s_VertScratch[x].x, s_VertScratch[y].y),
                            new Vector2(s_VertScratch[x2].x, s_VertScratch[y2].y),
                            color,
                            new Vector2(s_UVScratch[x].x, s_UVScratch[y].y),
                            new Vector2(s_UVScratch[x2].x, s_UVScratch[y2].y), new Vector2Int(x, y),
                            overrideSprite.rect.size - new Vector2(overrideSprite.border.x + overrideSprite.border.z,
                                overrideSprite.border.y + overrideSprite.border.w), tileCenter, fillAmountQuad);
                    }
                }
                //Debug.LogError((s_VertScratch[x2].x +1000));
            }
        }

        Vector2 GetFillAmount(int x, int y, Vector2 widthHeight)
        {
            var maxX = widthHeight.x;
            var index = x == 0 ? 1 : (x == 1 ? 0 : 2);
            Vector2 fillAmountQuad = Vector2.one;
            var decreaseTotalX = fillMethod == FillMethod.Horizontal ? maxX * (1 - fillAmount) : 0;
            int lastIndex = 0;
            var totalRound = s_VertScratch[1].x - s_VertScratch[0].x + s_VertScratch[3].x - s_VertScratch[2].x;
            var totalMid = s_VertScratch[2].x - s_VertScratch[1].x;
            if (x == 1)
            {
                if (decreaseTotalX > totalMid)
                {
                    fillAmountQuad.x = 0;
                }
                else
                {
                    if (totalMid > 0)
                        fillAmountQuad.x = (totalMid - decreaseTotalX) / totalMid;
                }
            }
            else
            {
                decreaseTotalX -= totalMid;
                if (decreaseTotalX <= 0)
                {
                    fillAmountQuad.x = 1;
                }
                else
                {
                    if (totalRound > 0)
                        fillAmountQuad.x = (totalRound - decreaseTotalX) / (totalRound);
                }
            }

            return fillAmountQuad;
        }


        void GenerateSimpleSprite(VertexHelper vh, bool lPreserveAspect)
        {
            Vector4 v = GetDrawingDimensions(lPreserveAspect);
            var uv = (overrideSprite != null) ? DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;

            var color32 = color;
            vh.Clear();
            vh.AddVert(new Vector3(v.x, v.y), color32, new Vector2(uv.x, uv.y));
            vh.AddVert(new Vector3(v.x, v.w), color32, new Vector2(uv.x, uv.w));
            vh.AddVert(new Vector3(v.z, v.w), color32, new Vector2(uv.z, uv.w));
            vh.AddVert(new Vector3(v.z, v.y), color32, new Vector2(uv.z, uv.y));

            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(2, 3, 0);
        }

        private Vector4 GetDrawingDimensions(bool shouldPreserveAspect)
        {
            var padding = overrideSprite == null ? Vector4.zero : DataUtility.GetPadding(overrideSprite);
            var size = overrideSprite == null
                ? Vector2.zero
                : new Vector2(overrideSprite.rect.width, overrideSprite.rect.height);

            Rect r = GetPixelAdjustedRect();
            // Debug.Log(string.Format("r:{2}, size:{0}, padding:{1}", size, padding, r));

            int spriteW = Mathf.RoundToInt(size.x);
            int spriteH = Mathf.RoundToInt(size.y);

            var v = new Vector4(
                padding.x / spriteW,
                padding.y / spriteH,
                (spriteW - padding.z) / spriteW,
                (spriteH - padding.w) / spriteH);

            if (shouldPreserveAspect && size.sqrMagnitude > 0.0f)
            {
                PreserveSpriteAspectRatio(ref r, size);
            }

            v = new Vector4(
                r.x + r.width * v.x,
                r.y + r.height * v.y,
                r.x + r.width * v.z,
                r.y + r.height * v.w
            );

            return v;
        }

        private void PreserveSpriteAspectRatio(ref Rect rect, Vector2 spriteSize)
        {
            var spriteRatio = spriteSize.x / spriteSize.y;
            var rectRatio = rect.width / rect.height;

            if (spriteRatio > rectRatio)
            {
                var oldHeight = rect.height;
                rect.height = rect.width * (1.0f / spriteRatio);
                rect.y += (oldHeight - rect.height) * rectTransform.pivot.y;
            }
            else
            {
                var oldWidth = rect.width;
                rect.width = rect.height * spriteRatio;
                rect.x += (oldWidth - rect.width) * rectTransform.pivot.x;
            }
        }

        private Vector4 GetAdjustedBorders(Vector4 border, Rect adjustedRect)
        {
            Rect originalRect = rectTransform.rect;

            for (int axis = 0; axis <= 1; axis++)
            {
                float borderScaleRatio;

                // The adjusted rect (adjusted for pixel correctness)
                // may be slightly larger than the original rect.
                // Adjust the border to match the adjustedRect to avoid
                // small gaps between borders (case 833201).
                if (originalRect.size[axis] != 0)
                {
                    borderScaleRatio = adjustedRect.size[axis] / originalRect.size[axis];
                    border[axis] *= borderScaleRatio;
                    border[axis + 2] *= borderScaleRatio;
                }

                // If the rect is smaller than the combined borders, then there's not room for the borders at their normal size.
                // In order to avoid artefacts with overlapping borders, we scale the borders down to fit.
                float combinedBorders = border[axis] + border[axis + 2];
                if (adjustedRect.size[axis] < combinedBorders && combinedBorders != 0)
                {
                    borderScaleRatio = adjustedRect.size[axis] / combinedBorders;
                    border[axis] *= borderScaleRatio;
                    border[axis + 2] *= borderScaleRatio;
                }
            }

            return border;
        }

        static void AddQuad(VertexHelper vertexHelper, Vector3[] quadPositions, Color32 color, Vector3[] quadUVs)
        {
            int startIndex = vertexHelper.currentVertCount;

            for (int i = 0; i < 4; ++i)
                vertexHelper.AddVert(quadPositions[i], color, quadUVs[i]);

            vertexHelper.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
            vertexHelper.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
        }

        static void AddQuad(VertexHelper vertexHelper, Vector2 posMin, Vector2 posMax, Color32 color, Vector2 uvMin,
            Vector2 uvMax, Vector2Int pos, Vector2 centerDefault, bool tileCenter, Vector2 fillAmount)
        {
            int startIndex = vertexHelper.currentVertCount;
            var maxX = posMax.x;
            var minUV = uvMin;
            var maxUV = uvMax;
            if (pos.x == 2)
            {
                minUV.x = uvMax.x - (uvMax.x - uvMin.x) * fillAmount.x;
            }

            if (pos.x == 0)
            {
                maxUV.x = uvMin.x + (uvMax.x - uvMin.x) * fillAmount.x;
            }

            if (pos.x == 1)
            {
                if (tileCenter)
                {
                    var countTile = (int)Mathf.Ceil((maxX - posMin.x) / centerDefault.x);
                    if (countTile <= 0)
                    {
                        countTile = 1;
                    }

                    for (int i = 0; i < countTile; ++i)
                    {
                        vertexHelper.AddVert(new Vector3(posMin.x + i * centerDefault.x, posMin.y, 0), color,
                            new Vector2(minUV.x, minUV.y));
                        vertexHelper.AddVert(new Vector3(posMin.x + i * centerDefault.x, posMax.y, 0), color,
                            new Vector2(minUV.x, maxUV.y));
                        if (i == countTile - 1)
                        {
                            vertexHelper.AddVert(new Vector3(posMax.x, posMax.y, 0), color,
                                new Vector2(maxUV.x, maxUV.y));
                            vertexHelper.AddVert(new Vector3(posMax.x, posMin.y, 0), color,
                                new Vector2(maxUV.x, minUV.y));
                        }
                        else
                        {
                            vertexHelper.AddVert(new Vector3(posMin.x + (i + 1) * centerDefault.x, posMax.y, 0), color,
                                new Vector2(maxUV.x, maxUV.y));
                            vertexHelper.AddVert(new Vector3(posMin.x + (i + 1) * centerDefault.x, posMin.y, 0), color,
                                new Vector2(maxUV.x, minUV.y));
                        }


                        vertexHelper.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
                        vertexHelper.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
                        startIndex += 4;
                    }
                }
                else
                {
                    vertexHelper.AddVert(new Vector3(posMin.x, posMin.y, 0), color, new Vector2(minUV.x, minUV.y));
                    vertexHelper.AddVert(new Vector3(posMin.x, posMax.y, 0), color, new Vector2(minUV.x, maxUV.y));
                    vertexHelper.AddVert(new Vector3(posMax.x, posMax.y, 0), color, new Vector2(maxUV.x, maxUV.y));
                    vertexHelper.AddVert(new Vector3(posMax.x, posMin.y, 0), color, new Vector2(maxUV.x, minUV.y));

                    vertexHelper.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
                    vertexHelper.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
                }
            }
            else
            {
                vertexHelper.AddVert(new Vector3(posMin.x, posMin.y, 0), color, new Vector2(minUV.x, minUV.y));
                vertexHelper.AddVert(new Vector3(posMin.x, posMax.y, 0), color, new Vector2(minUV.x, maxUV.y));
                vertexHelper.AddVert(new Vector3(maxX, posMax.y, 0), color, new Vector2(maxUV.x, maxUV.y));
                vertexHelper.AddVert(new Vector3(maxX, posMin.y, 0), color, new Vector2(maxUV.x, minUV.y));

                vertexHelper.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
                vertexHelper.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
            }
        }
    }
}