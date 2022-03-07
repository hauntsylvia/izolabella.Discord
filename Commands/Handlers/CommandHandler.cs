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

        public CommandHandler(DiscordSocketClient Reference)
        {
            this.allowBotInteractions = false;
            this.Reference = Reference;
        }

        public Task StartReceiving()
        {
            this.Reference.MessageReceived += this.Reference_MessageReceived;
            return Task.CompletedTask;
        }

        private async Task Reference_MessageReceived(SocketMessage Message)
        {
            if(!Message.Author.IsBot || this.AllowBotInteractions)
            {

            }
        }
    }
}
