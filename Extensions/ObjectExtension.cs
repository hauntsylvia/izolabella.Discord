using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Discord.Extensions
{
    internal static partial class ObjectExtension
    {
#pragma warning disable IDE0060 // Remove unused parameter
        public static bool IsNullable<T>(this T Self)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            return false;
        }
    }
}
