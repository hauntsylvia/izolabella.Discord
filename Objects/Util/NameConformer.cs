using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Discord.Objects.Util
{
    internal class NameConformer
    {
        internal static string DiscordCommandConformity(string A)
        {
            return A.Trim().Replace(' ', '-').ToLower();
        }
    }
}
