using izolabella.Discord.Commands.Arguments;
using izolabella.Discord.Commands.Attributes;
using izolabella.Discord.Internals.Surgical;
using izolabella.ConsoleHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Discord.Commands.Handlers
{
    /// <summary>
    /// General class for handling commands and providing events for commands.
    /// </summary>
    public class CommandHandler
    {
        private bool allowBotInteractions;
        /// <summary>
        /// To allow or not to allow bots to interact be able to invoke the <see cref="CommandHandler.CommandInvoked"/> event and commands.
        /// </summary>
        public bool AllowBotInteractions { get => this.allowBotInteractions; set => this.allowBotInteractions = value; }


        private DiscordSocketClient Reference { get; }


        private IReadOnlyCollection<CommandWrapper> Commands { get; }


        /// <summary>
        /// Command rejected.
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="Arg">Arguments containing relevant information about the rejection.</param>
        /// <returns></returns>
        public delegate Task CommandRejectedHandler(object Sender, CommandRejectedArgs Arg);
        /// <summary>
        /// Fires when the user in context is not allowed due to <see cref="CommandAttribute.Whitelist"/> or 
        /// <see cref="CommandAttribute.Blacklist"/>, and if <see cref="CommandNeedsValidation"/> returns true.
        /// </summary>
        public event CommandRejectedHandler? CommandRejected;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="Arg">Arguments containing relevant information about the invokation.</param>
        /// <returns></returns>
        public delegate Task CommandInvokedHandler(object Sender, CommandInvokedArgs Arg);
        /// <summary>
        /// Fires after the command has run.
        /// </summary>
        public event CommandInvokedHandler? CommandInvoked;

        /// <summary>
        /// Should return true if the message is valid for the command in context.
        /// </summary>
        public Func<SocketSlashCommand, CommandAttribute, bool>? CommandNeedsValidation { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="CommandHandler"/>.
        /// </summary>
        /// <param name="Reference">The <see cref="DiscordSocketClient"/> reference this handler should use.</param>
        public CommandHandler(DiscordSocketClient Reference)
        {
            this.allowBotInteractions = false;
            this.Reference = Reference;
            this.Commands = CommandSurgeon.GetCommandWrappers();
            PrettyConsole.Log($"{nameof(CommandHandler)} Initialization", $"New {nameof(CommandHandler)} initialized.", LoggingLevel.Information);
        }

        /// <summary>
        /// Makes the instance start receiving and handling messages.
        /// </summary>
        /// <param name="IgnoreExceptions">Set this to true if the bot should continue registering commands even if it doesn't have permissions in all guilds. (Recommended to keep this set to true)</param>
        /// <returns></returns>
        public async Task StartReceiving(bool IgnoreExceptions = true)
        {
            PrettyConsole.Log($"{nameof(CommandHandler)} is receiving.", "", LoggingLevel.Information);
            foreach (SocketGuild Guild in this.Reference.Guilds)
            {
                PrettyConsole.Log($"Slash Command Updates", $"{nameof(CommandHandler)} is updating slash commands.", LoggingLevel.Information);
                IReadOnlyCollection<SocketApplicationCommand> ExistingCommands = await Guild.GetApplicationCommandsAsync();
                foreach (CommandWrapper Command in this.Commands)
                {
                    SocketApplicationCommand? AlreadyExistingCommand = ExistingCommands.FirstOrDefault(ExistingCommand => ExistingCommand.Name == Command.SlashCommandTag);
                    if (AlreadyExistingCommand == null)
                    {
                        SlashCommandBuilder SlashCommand = new();
                        SlashCommand.WithName(Command.SlashCommandTag);
                        SlashCommand.WithDescription(Command.Attribute.Description ?? "(no description)");
                        try
                        {
                            SocketApplicationCommand CommandCreated = await Guild.CreateApplicationCommandAsync(SlashCommand.Build());
                            PrettyConsole.Log($"Command Creation.", $"The command {CommandCreated.Name} [{CommandCreated.Id}] has been created in guild of id {CommandCreated.Guild.Id}.", LoggingLevel.Information);
                        }
                        catch (Exception Exc)
                        {
                            if (!IgnoreExceptions)
                            {
                                PrettyConsole.Log($"Command Creation Failure", Exc, LoggingLevel.Errors);
                                throw;
                            }
                        }
                    }
                    else
                    {
                        PrettyConsole.Log($"Command Already Exists", "Updating command's name if applicable.", LoggingLevel.Information);
                        await AlreadyExistingCommand.ModifyAsync(CommandStuff =>
                        {
                            CommandStuff.Name = Command.SlashCommandTag;
                        });
                    }
                }
                foreach (SocketApplicationCommand ExistingCommand in ExistingCommands)
                {
                    PrettyConsole.Log("Checking command deletion applicability..", $"Finding and deleting commands that are not handled by this application but still registered as a slash command in Discord..", LoggingLevel.Information);
                    if(!this.Commands.Any(Command => Command.SlashCommandTag == ExistingCommand.Name))
                    {
                        await ExistingCommand.DeleteAsync();
                        PrettyConsole.Log("Command Deletion", @$"The command ""{ExistingCommand.Name}"" of id [{ExistingCommand.Id}] has been deleted due to irrelevance.", LoggingLevel.Information);
                    }
                }
            }
            this.Reference.SlashCommandExecuted += this.Reference_SlashCommandExecuted;
        }

        private async Task Reference_SlashCommandExecuted(SocketSlashCommand Arg)
        {
            PrettyConsole.Log("Slash Command Request", $"Processing a new request made by user of id {Arg.Id}..", LoggingLevel.Information);
            if (!Arg.User.IsBot || this.AllowBotInteractions)
            {
                foreach (CommandWrapper Command in this.Commands)
                {
                    if (Command.Attribute.Tags.First().ToLower().Replace(' ', '-') == Arg.Data.Name)
                    {
                        bool IsWhitelisted = (Command.Attribute.Whitelist != null && Command.Attribute.Whitelist.Any(Id => Arg.User.Id == Id)) || Command.Attribute.Whitelist == null;
                        bool IsBlacklisted = (Command.Attribute.Blacklist != null && Command.Attribute.Blacklist.Any(Id => Arg.User.Id == Id)) || false;
                        bool ValidMessage = this.CommandNeedsValidation?.Invoke(Arg, Command.Attribute) ?? true;
                        if (IsWhitelisted && !IsBlacklisted && ValidMessage)
                        {
                            if (Command.Attribute.Defer)
                                await Arg.DeferAsync(Command.Attribute.LocalOnly);
                            Command.InvokeThis(Arg);
                            PrettyConsole.Log("Slash Command Invoked", $"Slash command was invoked.", LoggingLevel.Information);
                            if (this.CommandInvoked != null)
                                await this.CommandInvoked.Invoke(this, new(Arg, Command));
                            break;
                        }
                        else if (ValidMessage && this.CommandRejected != null)
                        {
                            PrettyConsole.Log("Slash Command Rejected", $"Slash command was rejected.", LoggingLevel.Information);
                            await this.CommandRejected.Invoke(this, new(Arg, Command));
                        }
                    }
                }
            }
        }
    }
}
