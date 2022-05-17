using System.Reflection;

namespace izolabella.Discord.Internals.Structures.Commands
{
    internal static class SlashCommandComparer
    {
        internal static bool NeedsUpdate(SlashCommandBuilder SlashCommandBuilder, SocketApplicationCommand AppCommand)
        {
            if (AppCommand.Type == ApplicationCommandType.Slash)
            {
                int SlashCommandOptionsCount = SlashCommandBuilder.Options?.Count ?? 0;
                int AppCommandOptionsCount = AppCommand.Options?.Count ?? 0;
                if (SlashCommandOptionsCount != AppCommandOptionsCount)
                {
                    return true;
                }
                else if (SlashCommandBuilder.Options != null && AppCommand.Options != null)
                {
                    for (int Index = 0; Index < SlashCommandBuilder.Options.Count; Index++)
                    {
                        SlashCommandOptionBuilder RecentOption = SlashCommandBuilder.Options[Index];
                        if (AppCommand.Options.Count > Index)
                        {
                            SocketApplicationCommandOption ExistingOption = AppCommand.Options.ElementAt(Index);
                            if (ParameterNeedsUpdate(RecentOption, ExistingOption))
                            {
                                return true;
                            }
                        }
                    }
                }

                PropertyInfo[] PropertyInfoOfSlashCommandBuilder = SlashCommandBuilder.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                for (int Index = 0; Index < PropertyInfoOfSlashCommandBuilder.Length; Index++)
                {
                    PropertyInfo RecentCommand = PropertyInfoOfSlashCommandBuilder[Index];
                    PropertyInfo[] PropertyInfoOfDiscordCommand = AppCommand.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    if (PropertyInfoOfDiscordCommand.Length > Index)
                    {
                        PropertyInfo ExistingCommand = PropertyInfoOfDiscordCommand[Index];
                        if (RecentCommand.Name == ExistingCommand.Name)
                        {
                            object? RecentProp = RecentCommand.GetValue(SlashCommandBuilder);
                            object? ExistingProp = ExistingCommand.GetValue(AppCommand);
                            if (RecentProp != null && ExistingProp != null && RecentProp.GetType() == ExistingProp.GetType() && !RecentProp.Equals(ExistingProp))
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
            else
            {
                return true;
            }
        }

        internal static bool ParameterNeedsUpdate(SlashCommandOptionBuilder RecentOption, SocketApplicationCommandOption DiscordOption)
        {
            return RecentOption.IsRequired != DiscordOption.IsRequired ||
                RecentOption.Name != DiscordOption.Name ||
                RecentOption.IsAutocomplete != DiscordOption.IsAutocomplete ||
                RecentOption.Description != DiscordOption.Description ||
                (RecentOption.IsDefault ?? false) != (DiscordOption.IsDefault ?? false);
            //foreach (PropertyInfo PropertyOfRecentOption in RecentOption.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            //{
            //    foreach (PropertyInfo PropertyOfExistingOption in DiscordOption.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            //    {
            //        if (PropertyOfExistingOption.Name == PropertyOfRecentOption.Name)
            //        {
            //            object? RecentOptionProp = PropertyOfRecentOption.GetValue(RecentOption);
            //            object? ExistingOptionProp = PropertyOfExistingOption.GetValue(DiscordOption);
            //            bool ObjectsAreConsideredEqual = RecentOptionProp != null && ExistingOptionProp != null ? ExistingOptionProp.Equals(RecentOptionProp) : RecentOptionProp == null && ExistingOptionProp == null;
            //            bool AreOfSameType = RecentOptionProp != null && ExistingOptionProp != null ? RecentOptionProp.GetType() == ExistingOptionProp.GetType() : RecentOptionProp == null && ExistingOptionProp == null;
            //            if (AreOfSameType && !ObjectsAreConsideredEqual)
            //            {
            //                return true;
            //            }
            //        }
            //    }
            //}
            //return false;
        }
    }
}
