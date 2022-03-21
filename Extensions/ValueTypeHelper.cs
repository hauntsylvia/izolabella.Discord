using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Discord.Extensions
{
    internal static class ValueTypeHelper
    {
        public static bool IsNullable(this Type Ty)
        {
            return Ty.IsValueType && Activator.CreateInstance(Ty) == null;
        }
    }
}
