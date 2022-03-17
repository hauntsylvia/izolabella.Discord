using izolabella.Discord.Commands.Handlers;
using izolabella.Discord.Internals.Surgical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Discord.Internals.Structures.Commands
{
    /// <summary>
    /// A class for processing commands that already exist.
    /// </summary>
    public class CommandValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandValidator"/> class.
        /// </summary>
        /// <param name="CurrentCommands"></param>
        /// <param name="Instance"></param>
        public CommandValidator(IReadOnlyCollection<CommandWrapper> CurrentCommands, DiscordSocketClient Instance)
        {
            this.CurrentCommands = CurrentCommands;
            this.Instance = Instance;
        }

        /// <summary>
        /// The current commands used by the caller.
        /// </summary>
        public IReadOnlyCollection<CommandWrapper> CurrentCommands { get; }

        /// <summary>
        /// The instance this class should use.
        /// </summary>
        public DiscordSocketClient Instance { get; }

        /// <summary>
        /// Delete or modify existing commands.
        /// </summary>
        /// <returns></returns>
        public async Task Validate()
        {
            PrettyConsole.Log($"{nameof(CommandHandler)} Recieving.", $"An instance of the class {nameof(CommandHandler)} is ready to process commands.", LoggingLevel.Information);
            foreach (SocketGuild Guild in this.Instance.Guilds)
            {
                PrettyConsole.Log($"Slash Command Updates", $"{nameof(CommandHandler)} is updating slash commands.", LoggingLevel.Information);
                IReadOnlyCollection<SocketApplicationCommand> ExistingCommands = await Guild.GetApplicationCommandsAsync();
                foreach (CommandWrapper Command in this.CurrentCommands)
                {
                    SocketApplicationCommand? AlreadyExistingCommand = ExistingCommands.FirstOrDefault(ExistingCommand => ExistingCommand.Name == Command.SlashCommandTag);
                    if (AlreadyExistingCommand == null)
                    {
                        SlashCommandBuilder SlashCommand = new();
                        SlashCommand.WithName(Command.SlashCommandTag);
                        SlashCommand.WithDescription(Command.Attribute.Description ?? "(no description)");
                        SocketGuildUser CurrentUser = Guild.GetUser(this.Instance.CurrentUser.Id);
                        if (CurrentUser.GuildPermissions.UseApplicationCommands)
                        {
                            SocketApplicationCommand CommandCreated = await Guild.CreateApplicationCommandAsync(SlashCommand.Build());
                        }
                    }
                    else
                    {
                        if (AlreadyExistingCommand.Description != Command.Attribute.Description)
                        {
                            await AlreadyExistingCommand.ModifyAsync(CommandStuff =>
                            {
                                CommandStuff.Name = Command.SlashCommandTag;
                            });
                            PrettyConsole.Log($"Command Already Exists", "Updated command's description.", LoggingLevel.Information);
                        }
                    }
                }
                if (ExistingCommands.Any(ExistingCommand => this.CurrentCommands.Any(CurrentCommand => ExistingCommand.Name != CurrentCommand.SlashCommandTag)))
                {
                    IEnumerable<SocketApplicationCommand> ForDeletion = ExistingCommands.Where(ExistingCommand => !this.CurrentCommands.Any(CurrentCommand => ExistingCommand.Name == CurrentCommand.SlashCommandTag));
                    foreach (SocketApplicationCommand ToDelete in ForDeletion)
                    {
                        await ToDelete.DeleteAsync();
                        PrettyConsole.Log("Command Deletion", @$"The command ""{ToDelete.Name}"" of id [{ToDelete.Id}] has been deleted due to irrelevance.", LoggingLevel.Information);
                    }
                }
            }
        }
    }
}
