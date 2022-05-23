global using Discord;
global using Discord.Net;
global using Discord.WebSocket;
global using izolabella.ConsoleHelper;
using izolabella.Discord.Objects.Arguments;
using izolabella.Discord.Objects.Constraints.Interfaces;
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
            this.Client.Ready += () =>
            {
                this.ClientReady = true;
                return Task.CompletedTask;
            };
            this.Commands = GetIzolabellaCommandsAsync().Result;
        }

        /// <summary>
        /// True once the <see cref="DiscordSocketClient.Ready"/> event has fired.
        /// </summary>
        public bool ClientReady { get; private set; } = false;

        /// <summary>
        /// The wrapped <see cref="DiscordSocketClient"/>.
        /// </summary>
        public DiscordSocketClient Client { get; }

        /// <summary>
        /// The commands found by this instance.
        /// </summary>
        public List<IIzolabellaCommand> Commands { get; }

        /// <summary>
        /// The method that will run if the command invocation fails due to set constraints.
        /// </summary>
        /// <param name="Context">The context the handler will pass.</param>
        /// <param name="Arguments">The arguments the end user has invoked this command with.</param>
        /// <param name="ConstraintThatFailed">The constraint that caused this method to get fired by the handler.</param>
        public delegate Task CommandConstrainedHandler(CommandContext Context, IzolabellaCommandArgument[] Arguments, IIzolabellaCommandConstraint ConstraintThatFailed);
        
        /// <summary>
        /// Fired when a command is constrained.
        /// </summary>
        public event CommandConstrainedHandler? OnCommandConstraint;

        /// <summary>
        /// The method that will run after a command is invoked.
        /// </summary>
        /// <param name="Context">The context the handler will pass.</param>
        /// <param name="Arguments">The arguments the end user has invoked this command with.</param>
        /// <param name="CommandInvoked">The command this handler invoked.</param>
        public delegate Task CommandInvokedHandler(CommandContext Context, IzolabellaCommandArgument[] Arguments, IIzolabellaCommand CommandInvoked);
        
        /// <summary>
        /// Fired when a command is constrained.
        /// </summary>
        public event CommandInvokedHandler? CommandInvoked;

        /// <summary>
        /// Logs in and connects the <see cref="Client"/>.
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="RegisterCommandsAtTheSameTime">Set to true if calling this method should also register commands.</param>
        /// <returns></returns>
        public async Task StartAsync(string Token, bool RegisterCommandsAtTheSameTime = true)
        {
            if(RegisterCommandsAtTheSameTime)
            {
                this.Client.Ready += async () =>
                {
                    await this.GetRidOfEmptyCommands();
                    await this.RegisterCommands();
                };
            }
            this.Client.SlashCommandExecuted += async (PassedCommand) =>
            {
                IIzolabellaCommand? Command = this.Commands.FirstOrDefault(
                    Iz => NameConformer.DiscordCommandConformity(Iz.Name) == NameConformer.DiscordCommandConformity(PassedCommand.CommandName));
                if(Command != null)
                {
                    ulong? GuildId = null;
                    if(PassedCommand.User is SocketGuildUser SUser)
                    {
                        GuildId = SUser.Guild.Id;
                    }
                    List<IzolabellaCommandArgument> SentParameters = new();
                    foreach(SocketSlashCommandDataOption Argument in PassedCommand.Data.Options)
                    {
                        SentParameters.Add(new(Argument.Name, "", Argument.Type, true, Argument.Value));
                    }
                    IIzolabellaCommandConstraint? CausesFailure = Command.Constraints.Where(C => C.ConstrainToOneGuildOfThisId == null || GuildId == null || C.ConstrainToOneGuildOfThisId == GuildId).FirstOrDefault(C =>
                    {
                        return !C.CheckCommandValidityAsync(PassedCommand).Result;
                    });
                    if (CausesFailure == null)
                    {
                        await Command.RunAsync(new(PassedCommand), SentParameters.ToArray());
                        await (this.CommandInvoked != null ? this.CommandInvoked.Invoke(new(PassedCommand), SentParameters.ToArray(), Command) : Task.CompletedTask);
                    }
                    else
                    {
                        await (this.OnCommandConstraint != null ? this.OnCommandConstraint.Invoke(new(PassedCommand), SentParameters.ToArray(), CausesFailure) : Task.CompletedTask);
                        await Command.OnConstrainmentAsync(new(PassedCommand), SentParameters.ToArray(), CausesFailure);
                    }
                }
            };
            await this.Client.LoginAsync(TokenType.Bot, Token, true);
            await this.Client.StartAsync();
        }

        /// <summary>
        /// Adds or updates commands.
        /// </summary>
        /// <param name="NewCommands">The commands to add or update.</param>
        /// <returns></returns>
        public async Task UpdateCommandsAsync(params IIzolabellaCommand[] NewCommands)
        {
            if(this.ClientReady)
            {
                foreach(IIzolabellaCommand NewCommand in NewCommands)
                {
                    IIzolabellaCommand? Existing = this.Commands.Find(C => C.Name == NewCommand.Name);
                    if (Existing != null)
                    {
                        this.Commands.Remove(Existing);
                    }
                    this.Commands.Add(NewCommand);
                }
                await this.GetRidOfEmptyCommands();
                await this.RegisterCommands();
            }
            else
            {
                await Task.Delay(50);
                await this.UpdateCommandsAsync(NewCommands);
            }
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
                            Type = Param.OptionType,
                            Choices = Param.Choices?.Select(PC => new ApplicationCommandOptionChoiceProperties()
                            {
                                Name = PC.Name,
                                Value = PC.Value
                            }).ToList()
                        });
                    }
                    SlashCommandBuilder SlashCommandBuilder = new()
                    {
                        Name = NameConformer.DiscordCommandConformity(Command.Name),
                        Description = Command.Description,
                        Options = Options,
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

        /// <summary>
        /// Gets all commands in the app domain.
        /// </summary>
        /// <returns>A list of all commands in the app domain with parameterless constructors.</returns>
        public static async Task<List<IIzolabellaCommand>> GetIzolabellaCommandsAsync()
        {
            List<IIzolabellaCommand> InitializedCommands = new();
            foreach(Assembly Ass in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach(Type T in Ass.GetTypes())
                {
                    if(typeof(IIzolabellaCommand).IsAssignableFrom(T) && !T.IsInterface)
                    {
                        object? Instance = Activator.CreateInstance(T);
                        if(Instance != null && Instance is IIzolabellaCommand I)
                        {
                            InitializedCommands.Add(I);
                        }
                    }
                }
            }
            foreach (IIzolabellaCommand Command in InitializedCommands)
            {
                await Command.OnLoadAsync(InitializedCommands.ToArray());
            }
            return InitializedCommands;
        }
    }
}
