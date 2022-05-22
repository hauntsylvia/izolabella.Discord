global using Discord;
global using Discord.Net;
global using Discord.WebSocket;
global using izolabella.ConsoleHelper;
using izolabella.Discord.Objects.Interfaces;
using izolabella.Discord.Objects.Parameters;
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
            this.Client.Ready += async () =>
            {
                await this.GetRidOfEmptyCommands();
                await this.RegisterCommands();
            };
            this.Client.SlashCommandExecuted += async (PassedCommand) =>
            {
                IIzolabellaCommand? Command = this.Commands.FirstOrDefault(
                    Iz => NameConformer.DiscordCommandConformity(Iz.Name) == NameConformer.DiscordCommandConformity(PassedCommand.CommandName));
                if(Command != null)
                {
                    List<IzolabellaCommandArgument> SentParameters = new();
                    foreach(SocketSlashCommandDataOption Argument in PassedCommand.Data.Options)
                    {
                        SentParameters.Add(new(Argument.Name, "", Argument.Type, true, Argument.Value));
                    }
                    await Command.RunAsync(new(PassedCommand), SentParameters.ToArray());
                }
            };
            await this.Client.LoginAsync(TokenType.Bot, Token, true);
            await this.Client.StartAsync();
        }

        internal async Task RegisterCommands()
        {
            foreach (SocketGuild Guild in this.Client.Guilds)
            {
                foreach (IIzolabellaCommand Command in this.Commands)
                {
                    List<SlashCommandOptionBuilder> Options = new();
                    foreach (IzolabellaCommandParameter Param in Command.Parameters)
                    {
                        Options.Add(new()
                        {
                            Name = NameConformer.DiscordCommandConformity(Param.Name),
                            Description = Param.Description,
                            IsRequired = Param.IsRequired,
                            Type = Param.OptionType
                        });
                    }
                    SlashCommandBuilder SlashCommandBuilder = new()
                    {
                        Name = NameConformer.DiscordCommandConformity(Command.Name),
                        Description = Command.Description,
                        Options = Options
                    };
                    await Guild.CreateApplicationCommandAsync(SlashCommandBuilder.Build());
                }
            }
        }

        internal async Task GetRidOfEmptyCommands()
        {
            foreach(SocketGuild G in this.Client.Guilds)
            {
                foreach (SocketApplicationCommand Command in await G.GetApplicationCommandsAsync())
                { 
                    if(Command.ApplicationId == this.Client.CurrentUser.Id)
                    {
                        if(!this.Commands.Any(C => NameConformer.DiscordCommandConformity(C.Name) == Command.Name))
                        {
                            await Command.DeleteAsync();
                        }
                    }
                }
            }
        }

        internal static List<IIzolabellaCommand> GetIzolabellaCommands()
        {
            List<IIzolabellaCommand> Commands = new();
            foreach(Assembly Ass in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach(Type T in Ass.GetTypes())
                {
                    if(typeof(IIzolabellaCommand).IsAssignableFrom(T) && !T.IsInterface)
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
