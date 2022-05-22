global using Discord;
global using Discord.Net;
global using Discord.WebSocket;
global using izolabella.ConsoleHelper;
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
        /// <param name="Client">The <see cref="DiscordSocketClient"/> to be used. The <see cref="DiscordSocketClient.StartAsync"/> should be called before constructing this class.</param>
        /// <param name="LoggingLevel">The level for which this library should log important things to the console.</param>
        public DiscordWrapper(DiscordSocketClient Client, LoggingLevel LoggingLevel = LoggingLevel.None)
        {
            this.Client = Client;
            DiscordWrapper.LoggingLevel = LoggingLevel;
            this.CommandHandler = new(Client);
        }

        /// <summary>
        /// The wrapped <see cref="DiscordSocketClient"/>.
        /// </summary>
        public DiscordSocketClient Client { get; }
        /// <summary>
        /// The level for which this library should log important things to the console.
        /// </summary>
        public static LoggingLevel LoggingLevel { get => PrettyConsole.MinimumAcceptedLoggingLevel; set => PrettyConsole.MinimumAcceptedLoggingLevel = value; }

        /// <summary>
        /// The command handler to be used for this instance.
        /// </summary>
        public CommandHandler CommandHandler { get; }
    }
}
