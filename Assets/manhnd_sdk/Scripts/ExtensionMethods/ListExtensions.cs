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
        
    }
}