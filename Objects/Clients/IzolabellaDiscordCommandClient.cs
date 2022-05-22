global using Discord;
global using Discord.Net;
global using Discord.WebSocket;
global using izolabella.ConsoleHelper;
using izolabella.Discord.Objects.Interfaces;
using izolabella.Discord.Objects.Util;
using System.Reflection;

namespace izolabella.Discord.Objects.Clients
{
    /// <summary>
    /// A class used for wrapping an instance of <see cref="DiscordSocketClient"/>.
    /// </summary>
    public class IzolabellaDiscordCommandClient
    {
        /// <summary>
        /// Initializes a new instance of <see cref="IzolabellaDiscordCommandClient"/>.
        /// </summary>
        /// <param name="Config">The client's configurations.</param>
        public IzolabellaDiscordCommandClient(DiscordSocketConfig Config)
        {
            this.Client = new(Config);
            this.Commands = GetIzolabellaCommands();
        }

        /// <summary>
        /// The wrapped <see cref="DiscordSocketClient"/>.
        /// </summary>
        public DiscordSocketClient Client { get; }

        /// <summary>
        /// The commands found by this instance.
        /// </summary>
        public List<IIzolabellaCommand> Commands { get; }

        /// <summary>
        /// Logs in and connects the <see cref="Client"/>.
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        public async Task StartAsync(string Token)
        {
            this.Client.SlashCommandExecuted += async (PassedCommand) =>
            {
                IIzolabellaCommand? Command = this.Commands.FirstOrDefault(
                    Iz => NameConformer.DiscordCommandConformity(Iz.Name) == NameConformer.DiscordCommandConformity(PassedCommand.CommandName));
                if(Command != null)
                {
                    Type CommandType = Command.GetType();
                    List<object?> SentParameters = new();
                    foreach(SocketSlashCommandDataOption Argument in PassedCommand.Data.Options)
                    {
                        SentParameters.Add(Argument.Value);
                    }
                    object? Instance = Activator.CreateInstance(CommandType, SentParameters.ToArray());
                    if(Instance != null && Instance is IIzolabellaCommand IzCommand)
                    {
                        await IzCommand.RunAsync(new(PassedCommand));
                    }
                }
            };
            await this.Client.LoginAsync(TokenType.Bot, Token, true);
            await this.Client.StartAsync();
        }
   
        internal static List<IIzolabellaCommand> GetIzolabellaCommands()
        {
            List<IIzolabellaCommand> Commands = new();
            foreach(Assembly Ass in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach(Type T in Ass.GetTypes())
                {
                    if(T.IsAssignableFrom(typeof(IIzolabellaCommand)) && !T.IsInterface)
                    {
                        object? Instance = Activator.CreateInstance(T);
                        if(Instance != null && Instance is IIzolabellaCommand I)
                        {
                            Commands.Add(I);
                        }
                    }
                }
            }
            return Commands;
        }
    }
}
