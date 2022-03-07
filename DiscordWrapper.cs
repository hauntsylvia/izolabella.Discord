global using Discord.API;
global using Discord.Net;
global using Discord.WebSocket;
global using Discord;
using izolabella.Discord.Commands.Handlers;

namespace izolabella.Discord
{
    public class DiscordWrapper
    {
        public DiscordWrapper(DiscordSocketClient Client)
        {
            this.Client = Client;
            this.CommandHandler = new(Client);
        }

        public DiscordSocketClient Client { get; }

        public CommandHandler CommandHandler { get; }
    }
}
