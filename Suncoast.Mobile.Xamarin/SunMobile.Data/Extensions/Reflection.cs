using System;
using System.Reflection;

namespace SunBlock.DataTransferObjects.Extensions
{
    public static class Reflection
    {

        public static bool IsCollection(this Type type)
        {
            return ((type.IsClass && type.IsSubclassOf(typeof(System.Collections.CollectionBase)))
                    || (type.GetInterface("IList", true) != null)
                    || (type.IsClass && type.IsGenericType && type.IsGenericCollection()));

        }

        public static bool IsGenericCollection(this Type type)
        {
            return type.GetProperty("Count") != null && type.GetProperty("Item") != null;
        }


        public static int GetCollectionCount(this PropertyInfo prop, object container)
        {
            object collection = prop.GetValue(container, null);
            return collection.GetCollectionCount();
        }

        public static int GetCollectionCount(this object collection)
        {

            PropertyInfo countProperty = collection.GetType().GetProperty("Count", BindingFlags.Public | BindingFlags.Instance);

            if (countProperty != null)
            {
                return (int)countProperty.GetValue(collection, null);
            }
            else
            {
                return -1;
            }
        }

        public static object GetCollectionItem(this PropertyInfo prop, object container, int index)
        {
            object collection = prop.GetValue(container, null);
            return collection.GetCollectionItem(index);

        }

        public static object GetCollectionItem(this object collection, int index)
        {
            PropertyInfo itemProperty = collection.GetType().GetProperty("Item", BindingFlags.Public | BindingFlags.Instance);

            if (itemProperty != null)
                return itemProperty.GetValue(collection, new object[] { index });
            else
                return null;


        }
    }
}
