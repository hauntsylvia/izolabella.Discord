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