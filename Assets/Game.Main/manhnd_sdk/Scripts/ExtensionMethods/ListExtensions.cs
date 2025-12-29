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

            int count = System.Math.Min(n, list.Count);
            List<T> result = new List<T>(count);
            
            // indices list to shuffle
            List<int> indices = new List<int>(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                indices.Add(i);
            }
            
            // Fisher-Yates shuffle
            for (int i = 0; i < count; i++)
            {
                int randomIndex = UnityEngine.Random.Range(i, indices.Count);
                result.Add(list[indices[randomIndex]]);
                
                (indices[randomIndex], indices[i]) = (indices[i], indices[randomIndex]);
            }
            
            return result;
        }
        
        /// <summary>
        /// Get random elements from list (up to n elements) while preserving original order
        /// </summary>
        public static List<T> GetRandomElementsPreserveOrder<T>(this List<T> list, int n)
        {
            if (list.Count == 0)
            {
                return new List<T>();
            }

            int count = System.Math.Min(n, list.Count);
            
            // Generate random indices
            List<int> allIndices = new List<int>(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                allIndices.Add(i);
            }
            
            // Shuffle and take first 'count' indices
            List<int> selectedIndices = new List<int>(count);
            for (int i = 0; i < count; i++)
            {
                int randomIndex = UnityEngine.Random.Range(i, allIndices.Count);
                selectedIndices.Add(allIndices[randomIndex]);
                
                (allIndices[randomIndex], allIndices[i]) = (allIndices[i], allIndices[randomIndex]);
            }
            
            // Sort selected indices to preserve original order
            selectedIndices.Sort();
            
            // Build result in order
            List<T> result = new List<T>(count);
            for (int i = 0; i < selectedIndices.Count; i++)
            {
                result.Add(list[selectedIndices[i]]);
            }
            
            return result;
        }
    }
}