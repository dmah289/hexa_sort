using UnityEngine;
using UnityEngine.UI;

namespace Framework.Extensions
{
    public static class TextExtensions
    {
        public static void SetAlpha(this Text image, float alpha)
        {
            Color newColor = image.color;
            newColor.a = alpha;
            image.color = newColor;
        }
    }
}