using HexaSort.Scripts.Core.Entities.Piece;
using UnityEngine;

namespace manhnd_sdk.Scripts.ExtensionMethods
{
    public static class MeshRendererExtensions
    {
        private static MaterialPropertyBlock mpb = new();
        
        // Property ID
        private static readonly int VertexLitColor = Shader.PropertyToID("_Color");
        private static readonly int VertexLitMainTexSt = Shader.PropertyToID("_MainTex_ST");
        private static readonly int MaskTexSt = Shader.PropertyToID("_MaskTex_ST");
        
        // Config
        private const float OffsetUnit = 0.25f;
        private const float PiecesPerRow = 4f;
        
        public static void SetVertexLitColor(this MeshRenderer meshRenderer, Color newColor)
        {
            meshRenderer.GetPropertyBlock(mpb);
            mpb.SetColor(VertexLitColor, newColor);
            meshRenderer.SetPropertyBlock(mpb);
        }

        public static void SetOffsetTexture(this MeshRenderer meshRenderer, ColorType colorType)
        {
            meshRenderer.GetPropertyBlock(mpb);
            
            float offsetX = (byte)colorType % PiecesPerRow * OffsetUnit;
            float offsetY = -(byte)colorType / PiecesPerRow * OffsetUnit;
            mpb.SetVector(VertexLitMainTexSt, new Vector4(1, 1, offsetX, offsetY));
            
            meshRenderer.SetPropertyBlock(mpb);
        }
    }
}