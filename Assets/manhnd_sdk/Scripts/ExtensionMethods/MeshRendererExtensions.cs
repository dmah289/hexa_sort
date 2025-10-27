using UnityEngine;

namespace manhnd_sdk.Scripts.ExtensionMethods
{
    public static class MeshRendererExtensions
    {
        private static MaterialPropertyBlock mpb = new();
        
        // Property ID
        private static readonly int VertexLitColor = Shader.PropertyToID("_Color");
        
        public static void SetVertexLitColor(this MeshRenderer meshRenderer, Color newColor)
        {
            meshRenderer.GetPropertyBlock(mpb);
            mpb.SetColor(VertexLitColor, newColor);
            meshRenderer.SetPropertyBlock(mpb);
        }
    }
}