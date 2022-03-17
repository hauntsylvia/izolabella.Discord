﻿using izolabella.Discord.Commands.Arguments;
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
        /// To allow or not to allow bots to interact be able to invoke the <see cref="CommandHandler.CommandInvoked"/> event and commands.
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
        /// Initializes a new instance of the <see cref="CommandHandler"/> class.
        /// </summary>
        /// <param name="Reference">The <see cref="DiscordSocketClient"/> reference this handler should use.</param>
        public CommandHandler(DiscordSocketClient Reference)
        {
            this.AllowBotInteractions = false;
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
            await new CommandValidator(this.Commands, this.Reference).Validate();
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
                            {
                                await Arg.DeferAsync(Command.Attribute.LocalOnly);
                            }

                            Command.InvokeThis(Arg);
                            PrettyConsole.Log("Slash Command Invoked", $"Slash command was invoked.", LoggingLevel.Information);
                            if (this.CommandInvoked != null)
                            {
                                await this.CommandInvoked.Invoke(this, new(Arg, Command));
                            }

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
