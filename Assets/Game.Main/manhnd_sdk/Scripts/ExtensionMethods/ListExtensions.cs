using System.Collections.Generic;

namespace manhnd_sdk.Scripts.ExtensionMethods
{
    public static class ListExtensions
    {
        public static T RemoveFirst<T>(this List<T> list)
        {
            if (list.Count == 0)
            {
                throw new System.InvalidOperationException("The list is empty.");
            }

            T firstItem = list[0];
            list.RemoveAt(0);
            return firstItem;
        }
        
        public static T RemoveLast<T>(this List<T> list)
        {
            if (list.Count == 0)
            {
                throw new System.InvalidOperationException("The list is empty.");
            }

            int lastIndex = list.Count - 1;
            T lastItem = list[lastIndex];
            list.RemoveAt(lastIndex);
            return lastItem;
        }
        
        /// <summary>
        /// Get random elements from list (up to n elements)
        /// </summary>
        public static List<T> GetRandomElements<T>(this List<T> list, int n)
        {
            if (list.Count == 0)
            {
                return new List<T>();
            }

            // Số phần tử thực sự lấy ra không vượt quá số phần tử trong list
            int count = System.Math.Min(n, list.Count);
            List<T> result = new List<T>(count);
            
            // Tạo danh sách index để shuffle
            List<int> indices = new List<int>(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                indices.Add(i);
            }
            
            // Fisher-Yates shuffle để lấy n phần tử ngẫu nhiên
            for (int i = 0; i < count; i++)
            {
                int randomIndex = UnityEngine.Random.Range(i, indices.Count);
                result.Add(list[indices[randomIndex]]);
                
                // Swap
                (indices[randomIndex], indices[i]) = (indices[i], indices[randomIndex]);
            }
            
            return result;
        }
    }
}