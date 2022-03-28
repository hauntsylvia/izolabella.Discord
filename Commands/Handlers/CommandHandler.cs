using izolabella.Discord.Commands.Arguments;
using izolabella.Discord.Commands.Attributes;
using izolabella.Discord.Internals.Surgical;
using izolabella.ConsoleHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using izolabella.Discord.Internals.Structures.Commands;

namespace izolabella.Discord.Commands.Handlers
{
    /// <summary>
    /// General class for handling commands and providing events for commands.
    /// </summary>
    public class CommandHandler
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandler"/> class.
        /// </summary>
        /// <param name="Reference">The <see cref="DiscordSocketClient"/> reference this handler should use.</param>
        public CommandHandler(DiscordSocketClient Reference)
        {
            this.AllowBotInteractions = false;
            this.Reference = Reference;
            this.Commands = CommandSurgeon.GetCommandWrappers();
            this.CommandValidator = new(this);
        }

        /// <summary>
        /// To allow or not to allow bots to interact be able to invoke the <see cref="CommandInvoked"/> event and commands.
        /// </summary>
        public bool AllowBotInteractions { get; set; }

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
        /// Initializes a new instance of the <see cref="CommandInvokedHandler"/> class.
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
        /// Command validator.
        /// </summary>
        public CommandValidator CommandValidator { get; }

        /// <summary>
        /// Makes the instance start receiving and handling messages.
        /// </summary>
        /// <param name="IgnoreExceptions">Set this to true if the bot should continue registering commands even if it doesn't have permissions in all guilds.</param>
        /// <returns></returns>
        [Obsolete($"Use without the IgnoreExceptions parameter.")]
        public async Task StartReceiving(bool IgnoreExceptions = true)
        {
            await this.StartReceiving();
        }

        /// <summary>
        /// Makes the instance start receiving and handling messages.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        ///     This will invoke the <see cref="CommandCreator.FinalizeCommands(IReadOnlyDictionary{SocketGuild, List{SlashCommandBuilder}})"/>
        ///     method.
        /// </remarks>
        public async Task StartReceiving()
        {
            CommandCreator CommandCreator = new(this.Reference);
            IReadOnlyDictionary<SocketGuild, List<SlashCommandBuilder>> CommandsToCreate = await CommandCreator.FilterCommands(this.Commands);
            await CommandCreator.FinalizeCommands(CommandsToCreate);
            this.Reference.SlashCommandExecuted += this.Reference_SlashCommandExecuted;
        }

        private async Task Reference_SlashCommandExecuted(SocketSlashCommand Arg)
        {
            if (!Arg.User.IsBot || this.AllowBotInteractions)
            {
                foreach (CommandWrapper Command in this.Commands)
                {
                    if (Command.SlashCommandTag == Arg.Data.Name)
                    {
                        InvalidCommandReasons? InvalidWhy = CommandValidator.ValidateCommand(Arg, Command);
                        bool ValidCommand = (this.CommandNeedsValidation?.Invoke(Arg, Command.Attribute) ?? true) && InvalidWhy == null;
                        if (Command.Attribute.Defer)
                        {
                            await Arg.DeferAsync(Command.Attribute.LocalOnly);
                        }
                        if (ValidCommand)
                        {
                            Command.InvokeThis(Arg);
                            if (this.CommandInvoked != null)
                            {
                                await this.CommandInvoked.Invoke(this, new(Arg, Command));
                            }
                        }
                        else if (this.CommandRejected != null)
                        {
                            await this.CommandRejected.Invoke(this, new(Arg, Command));
                        }

                        break;
                    }
                }
            }
        }
    }
}
