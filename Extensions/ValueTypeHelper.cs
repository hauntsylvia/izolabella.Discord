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
