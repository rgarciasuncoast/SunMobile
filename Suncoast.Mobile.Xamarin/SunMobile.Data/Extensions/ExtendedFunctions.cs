using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SunBlock.DataTransferObjects.Extensions
{
    public static class ExtendedFunctions
    {
        #region List<string> extensions...

        public static void AddIfNotNullOrWhiteSpace(this List<string> list, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                list.Add(value);
        }

        public static string ToFomattedString(this List<string> list, bool useHtmlBreaks = false)
        {
            var errorBuilder = new StringBuilder();

            list.ForEach(str => errorBuilder.Append(str + (useHtmlBreaks ? "<br />" : Environment.NewLine)));

            return errorBuilder.ToString();

        }

        public static T RetryOnFault<T>(this TaskFactory factory, 
    Func<T> function, int maxTries)
        {
            while (maxTries > 0)
            {
                maxTries--;
                try { return function(); }
                catch { if (maxTries <= 0) throw; }
            }
            return default(T);
        }

        public static T RetryIf<T>(this TaskFactory factory,
    Func<T> function, int maxTries, Func<bool> retryIf )
        {
            while (maxTries > 0 && retryIf())
            {
                maxTries--;
                try { return function(); }
                catch { if (maxTries <= 0) throw; }
            }
            return default(T);
        }

        #endregion
    }
}
