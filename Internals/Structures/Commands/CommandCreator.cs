using izolabella.Discord.Internals.Surgical;

namespace izolabella.Discord.Internals.Structures.Commands
{
    /// <summary>
    /// A class used to create commands on Discord from the <see cref="CommandWrapper"/> class.
    /// </summary>
    public class CommandCreator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandCreator"/> class.
        /// </summary>
        /// <param name="Instance"></param>
        public CommandCreator(DiscordSocketClient Instance)
        {
            this.Instance = Instance;
        }

        /// <summary>
        /// 
        /// </summary>
        public DiscordSocketClient Instance { get; }

        private IReadOnlyDictionary<SocketGuild, List<SlashCommandBuilder>> CreateCommands(IReadOnlyCollection<CommandWrapper> ToCreate)
        {
            Dictionary<SocketGuild, List<SlashCommandBuilder>> GuildAndCommands = new();
            foreach (SocketGuild Guild in this.Instance.Guilds)
            {
                GuildAndCommands.Add(Guild, new());
                foreach (CommandWrapper Command in ToCreate)
                {
                    List<SlashCommandOptionBuilder> Options = new();
                    IReadOnlyCollection<CommandParameter> Parameters = Command.GetCommandParameters();
                    foreach (CommandParameter Param in Parameters)
                    {
                        SlashCommandOptionBuilder Opt = new()
                        {
                            Name = Param.Name,
                            Description = Param.Description,
                            IsRequired = Param.IsRequired,
                            Type = Param.ParameterType,
                            Choices = Param.Choices
                        };
                        Options.Add(Opt);
                    }
                    SlashCommandBuilder SocketCommand = new()
                    {
                        Name = Command.SlashCommandTag,
                        Description = Command.Attribute.Description,
                        IsDefaultPermission = true,
                        Options = Options.Count > 0 ? Options : null,
                    };
                    GuildAndCommands[Guild].Add(SocketCommand);
                }
            }
            return GuildAndCommands;
        }

        /// <summary>
        /// </summary>
        /// <returns>A collection commands that need to be created per guild.</returns>
        public async Task<IReadOnlyDictionary<SocketGuild, List<SlashCommandBuilder>>> FilterCommands(IReadOnlyCollection<CommandWrapper> Commands)
        {
            Dictionary<SocketGuild, List<SlashCommandBuilder>> GuildAndCommands = new();
            IReadOnlyDictionary<SocketGuild, List<SlashCommandBuilder>> CurrentCommands = this.CreateCommands(Commands);
            foreach (KeyValuePair<SocketGuild, List<SlashCommandBuilder>> KeyValuePair in CurrentCommands)
            {
                GuildAndCommands.Add(KeyValuePair.Key, new List<SlashCommandBuilder>());
                IReadOnlyCollection<SocketApplicationCommand> DiscordCommands = await KeyValuePair.Key.GetApplicationCommandsAsync();
                foreach (SocketApplicationCommand DiscordCommandToCheck in DiscordCommands)
                {
                    SlashCommandBuilder? PreDiscordCommandToCheck = null;
                    foreach (SlashCommandBuilder PreDiscordCommand in KeyValuePair.Value)
                    {
                        if (PreDiscordCommand.Name == DiscordCommandToCheck.Name)
                        {
                            PreDiscordCommandToCheck = PreDiscordCommand;
                        }
                    }
                    if (PreDiscordCommandToCheck != null && SlashCommandComparer.NeedsUpdate(PreDiscordCommandToCheck, DiscordCommandToCheck))
                    {
                        await DiscordCommandToCheck.DeleteAsync();
                        GuildAndCommands[KeyValuePair.Key].Add(PreDiscordCommandToCheck);
                    }

                    if (KeyValuePair.Value.All(CommandHere => CommandHere.Name != DiscordCommandToCheck.Name))
                    {
                        await DiscordCommandToCheck.DeleteAsync();
                    }
                }
                foreach (SlashCommandBuilder PreDiscordCommand in KeyValuePair.Value)
                {
                    SocketApplicationCommand? DiscordCommandExisting = null;
                    foreach (SocketApplicationCommand DiscordCommandToCheck in DiscordCommands)
                    {
                        if (PreDiscordCommand.Name == DiscordCommandToCheck.Name && PreDiscordCommand.Description == DiscordCommandToCheck.Description)
                        {
                            DiscordCommandExisting = DiscordCommandToCheck;
                        }
                    }
                    if (DiscordCommandExisting == null)
                    {
                        GuildAndCommands[KeyValuePair.Key].Add(PreDiscordCommand);
                    }
                }
            }
            return GuildAndCommands;
        }

        /// <summary>
        /// Finalize all commands and push them to all guilds in Discord.
        /// </summary>
        /// <param name="CommandsToFinalize"></param>
        /// <returns></returns>
        public static async Task FinalizeCommands(IReadOnlyDictionary<SocketGuild, List<SlashCommandBuilder>> CommandsToFinalize)
        {
            foreach (KeyValuePair<SocketGuild, List<SlashCommandBuilder>> KeyValuePair in CommandsToFinalize)
            {
                foreach (SlashCommandBuilder? Command in KeyValuePair.Value)
                {
                    try
                    {
                        await KeyValuePair.Key.CreateApplicationCommandAsync(Command.Build());
                    }
                    catch (HttpException Ex)
                    {
                        Console.WriteLine(Ex);
                    }
                }
            }
        }
    }
}
