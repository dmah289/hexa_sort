using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Extensions
{
    public static class ImageExtensions
    {
        public static async UniTask FadeAlphaToTarget(this Image image, float fadeDuration, float targetAlpha = 0)
        {
            float timer = 0;
            float curAlpha = image.color.a;
            
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                
                curAlpha = Mathf.Lerp(curAlpha, targetAlpha, timer / fadeDuration);
                Color newColor = image.color;
                newColor.a = curAlpha;
                image.color = newColor;
                await UniTask.Yield();
            }

            Color finalColor = image.color;
            finalColor.a = targetAlpha;
            image.color = finalColor;
        }

        public static void SetAlpha(this Image image, float alpha)
        {
            Color newColor = image.color;
            newColor.a = alpha;
            image.color = newColor;
        }
    }
}