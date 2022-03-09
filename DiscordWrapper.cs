global using Discord.API;
global using Discord.Net;
global using Discord.WebSocket;
global using Discord;


using izolabella.Discord.Commands.Handlers;


namespace izolabella.Discord
{
    /// <summary>
    /// A class used for wrapping an instance of <see cref="CommandHandler"/> and an instance of <see cref="DiscordSocketClient"/>.
    /// </summary>
    public class DiscordWrapper
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DiscordWrapper"/>.
        /// </summary>
        /// <param name="Client">The <see cref="DiscordSocketClient"/> to be used.</param>
        public DiscordWrapper(DiscordSocketClient Client)
        {
            this.Client = Client;
            this.CommandHandler = new(Client);
        }

        /// <summary>
        /// The wrapped <see cref="DiscordSocketClient"/>.
        /// </summary>
        public DiscordSocketClient Client { get; }

        /// <summary>
        /// The command handler to be used for this instance.
        /// </summary>
        public CommandHandler CommandHandler { get; }
    }
}
