using izolabella.Discord.Commands.Attributes;
using System.Reflection;

namespace izolabella.Discord.Internals.Surgical
{
    internal static class CommandSurgeon
    {
        private static IReadOnlyCollection<MethodInfo> GetSupportedMethods()
        {
            List<MethodInfo> Methods = new();
            Assembly? Assembly = Assembly.GetEntryAssembly();
            if (Assembly != null)
            {
                foreach (Type Type in Assembly.GetTypes())
                {
                    MethodInfo[] AllMethodsInType = Type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                    foreach (MethodInfo Method in AllMethodsInType)
                    {
                        Methods.Add(Method);
                    }
                }
            }
            return Methods;
        }

        internal static IReadOnlyCollection<CommandWrapper> GetCommandWrappers()
        {
            List<CommandWrapper> Commands = new();
            IReadOnlyCollection<MethodInfo> Methods = GetSupportedMethods();
            foreach (MethodInfo Method in Methods)
            {
                CommandAttribute? CommandAttribute = Method.GetCustomAttribute<CommandAttribute>();
                if (CommandAttribute != null)
                {
                    Commands.Add(new(CommandAttribute, Method));
                }
            }
            return Commands;
        }
    }
}