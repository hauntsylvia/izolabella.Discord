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
    public class CommandHandler
    {
        private bool allowBotInteractions;
        public bool AllowBotInteractions { get => this.allowBotInteractions; set => this.allowBotInteractions = value; }
        private DiscordSocketClient Reference { get; }
        private IReadOnlyCollection<CommandWrapper> Commands { get; }

        public delegate Task CommandRejectedHandler(object Sender, CommandRejectedArgs Arg);
        public event CommandRejectedHandler? CommandRejected;
        
        /// <summary>
        /// Fires after the command has run.
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="Arg"></param>
        /// <returns></returns>
        public delegate Task CommandInvokedHandler(object Sender, CommandInvokedArgs Arg);
        public event CommandInvokedHandler? CommandInvoked;

        /// <summary>
        /// Should return true if the message is valid for the command in context.
        /// </summary>
        public Func<SocketMessage, CommandAttribute, bool>? MessageReceived { get; set; }

        public CommandHandler(DiscordSocketClient Reference)
        {
            this.allowBotInteractions = false;
            this.Reference = Reference;
            this.Commands = CommandSurgeon.GetCommandWrappers();
        }

        public Task StartReceiving()
        {
            this.Reference.MessageReceived += this.Reference_MessageReceived;
            return Task.CompletedTask;
        }

        private async Task Reference_MessageReceived(SocketMessage Message)
        {
            if(this.MessageReceived != null)
            {
                if (!Message.Author.IsBot || this.AllowBotInteractions)
                {
                    foreach (CommandWrapper Command in this.Commands)
                    {
                        bool IsWhitelisted = (Command.Attribute.Whitelist != null && Command.Attribute.Whitelist.Any(Id => Message.Author.Id == Id)) || Command.Attribute.Whitelist == null;
                        bool IsBlacklisted = Command.Attribute.Blacklist != null && Command.Attribute.Blacklist.Any(Id => Message.Author.Id == Id);
                        bool ValidMessage = this.MessageReceived.Invoke(Message, Command.Attribute);
                        if (IsWhitelisted && !IsBlacklisted && ValidMessage)
                        {
                            Command.InvokeThis();
                            if(this.CommandInvoked != null)
                                await this.CommandInvoked.Invoke(this, new(Message, Command));
                        }
                        else if (ValidMessage && this.CommandRejected != null)
                            await this.CommandRejected.Invoke(this, new(Message, Command));
                    }
                }
            }
        }
    }
}
