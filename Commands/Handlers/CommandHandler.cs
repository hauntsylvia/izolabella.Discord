using izolabella.Discord.Commands.Arguments;
using izolabella.Discord.Commands.Attributes;
using izolabella.Discord.Internals.Surgical;
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
        public Func<SocketMessage, CommandAttribute, bool>? CommandNeedsValidation { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="CommandHandler"/>.
        /// </summary>
        /// <param name="Reference">The <see cref="DiscordSocketClient"/> reference this handler should use.</param>
        public CommandHandler(DiscordSocketClient Reference)
        {
            this.allowBotInteractions = false;
            this.Reference = Reference;
            this.Commands = CommandSurgeon.GetCommandWrappers();
        }

        /// <summary>
        /// Makes the instance start receiving and handling messages.
        /// </summary>
        /// <returns></returns>
        public Task StartReceiving()
        {
            this.Reference.MessageReceived += this.Reference_MessageReceived;
            return Task.CompletedTask;
        }

        private async Task Reference_MessageReceived(SocketMessage Message)
        {
            if(this.CommandNeedsValidation != null)
            {
                if (!Message.Author.IsBot || this.AllowBotInteractions)
                {
                    foreach (CommandWrapper Command in this.Commands)
                    {
                        bool IsWhitelisted = (Command.Attribute.Whitelist != null && Command.Attribute.Whitelist.Any(Id => Message.Author.Id == Id)) || Command.Attribute.Whitelist == null;
                        bool IsBlacklisted = (Command.Attribute.Blacklist != null && Command.Attribute.Blacklist.Any(Id => Message.Author.Id == Id)) || false;
                        bool ValidMessage = this.CommandNeedsValidation.Invoke(Message, Command.Attribute);
                        if (IsWhitelisted && !IsBlacklisted && ValidMessage)
                        {
                            Command.InvokeThis(Message);
                            if(this.CommandInvoked != null)
                                await this.CommandInvoked.Invoke(this, new(Message, Command));
                            break;
                        }
                        else if (ValidMessage && this.CommandRejected != null)
                            await this.CommandRejected.Invoke(this, new(Message, Command));
                    }
                }
            }
        }
    }
}
