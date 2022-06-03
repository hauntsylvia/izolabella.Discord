global using Discord;
global using Discord.WebSocket;
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
        /// <param name="GlobalCommands">If true, all commands will be created globally. This may take up to an hour to show up on Discord's side.</param>
        public IzolabellaDiscordCommandClient(DiscordSocketConfig Config, bool GlobalCommands)
        {
            this.GlobalCommands = GlobalCommands;
            this.Client = new(Config);
            this.Client.Ready += () =>
            {
                this.ClientReady = true;
                return Task.CompletedTask;
            };
            this.Commands = GetIzolabellaCommandsAsync().Result;
            this.Client.JoinedGuild += this.ClientJoinedGuildAsync;
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
        /// A bool representing whether the client should make commands global or not.
        /// </summary>
        public bool GlobalCommands { get; }

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
        /// Fired after a command is invoked.
        /// </summary>
        public event CommandInvokedHandler? CommandInvoked;

        /// <summary>
        /// The method that will run after the client joins a new guild.
        /// </summary>
        /// <param name="Arg">The guild.</param>
        public delegate Task AfterGuildJoinHandler(SocketGuild Arg);

        /// <summary>
        /// Fired after a the client has joined a guild.
        /// </summary>
        public event AfterGuildJoinHandler? AfterJoinedGuild;

        /// <summary>
        /// The method that will run after the client joins a new guild, but before this client processes it.
        /// </summary>
        /// <param name="Arg">The guild.</param>
        public delegate Task BeforeGuildJoinHandler(SocketGuild Arg);

        /// <summary>
        /// Fired after a the client has joined a guild, but before this client processes it.
        /// </summary>
        public event BeforeGuildJoinHandler? JoinedGuild;

        /// <summary>
        /// Updates commands.
        /// </summary>
        /// <param name="Arg">The socket guild argument in this method.</param>
        /// <returns></returns>
        private async Task ClientJoinedGuildAsync(SocketGuild Arg)
        {
            if (this.ClientReady)
            {
                this.JoinedGuild?.Invoke(Arg);
                await this.Client.Rest.GetGuildAsync(Arg.Id);
                if (!this.GlobalCommands)
                {
                    await this.DeleteIrrelevantCommands();
                    await this.RegisterCommandsAsync().ConfigureAwait(false);
                }
                this.AfterJoinedGuild?.Invoke(Arg);
            }
        }

        /// <summary>
        /// Logs in and connects the <see cref="Client"/>.
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="RegisterCommandsAtTheSameTime">Set to true if calling this method should also register commands.</param>
        /// <returns></returns>
        public async Task StartAsync(string Token, bool RegisterCommandsAtTheSameTime = true)
        {
            if (RegisterCommandsAtTheSameTime)
            {
                this.Client.Ready += async () =>
                {
                    await this.DeleteIrrelevantCommands();
                    await this.RegisterCommandsAsync();
                };
            }
            this.Client.SlashCommandExecuted += async (PassedCommand) =>
            {
                IIzolabellaCommand? Command = this.Commands.FirstOrDefault(
                    Iz => NameConformer.DiscordCommandConformity(Iz.Name) == NameConformer.DiscordCommandConformity(PassedCommand.CommandName));
                if (Command != null)
                {
                    ulong? GuildId = null;
                    if (PassedCommand.User is SocketGuildUser SUser)
                    {
                        GuildId = SUser.Guild.Id;
                    }
                    List<IzolabellaCommandArgument> SentParameters = new();
                    foreach (SocketSlashCommandDataOption Argument in PassedCommand.Data.Options)
                    {
                        SentParameters.Add(new(Argument.Name, "", Argument.Type, true, Argument.Value));
                    }
                    IIzolabellaCommandConstraint? CausesFailure = Command.Constraints.Where(C => C.ConstrainToOneGuildOfThisId == null || GuildId == null || C.ConstrainToOneGuildOfThisId == GuildId).FirstOrDefault(C =>
                    {
                        return !C.CheckCommandValidityAsync(PassedCommand).Result;
                    });
                    CommandContext Context = new(PassedCommand, this);
                    if (CausesFailure == null)
                    {
                        await Command.RunAsync(Context, SentParameters.ToArray());
                        await (this.CommandInvoked != null ? this.CommandInvoked.Invoke(Context, SentParameters.ToArray(), Command) : Task.CompletedTask);
                    }
                    else
                    {
                        await (this.OnCommandConstraint != null ? this.OnCommandConstraint.Invoke(Context, SentParameters.ToArray(), CausesFailure) : Task.CompletedTask);
                        await Command.OnConstrainmentAsync(Context, SentParameters.ToArray(), CausesFailure);
                    }
                }
            };
            await this.Client.LoginAsync(TokenType.Bot, Token, true);
            await this.Client.StartAsync();
        }

        /// <summary>
        /// Adds or updates commands. Useful for adding new constraints.
        /// </summary>
        /// <param name="NewCommands">The commands to add or update.</param>
        /// <returns></returns>
        public async Task UpdateCommandsAsync(params IIzolabellaCommand[] NewCommands)
        {
            if (this.ClientReady)
            {
                foreach (IIzolabellaCommand NewCommand in NewCommands)
                {
                    IIzolabellaCommand? Existing = this.Commands.Find(C => C.Name == NewCommand.Name);
                    if (Existing != null)
                    {
                        this.Commands.Remove(Existing);
                    }
                    this.Commands.Add(NewCommand);
                }
                await this.DeleteIrrelevantCommands();
                await this.RegisterCommandsAsync();
            }
            else
            {
                await Task.Delay(50);
                await this.UpdateCommandsAsync(NewCommands);
            }
        }

        /// <summary>
        /// Gets all commands in the app domain.
        /// </summary>
        /// <returns>A list of all commands in the app domain with parameterless constructors.</returns>
        public static async Task<List<IIzolabellaCommand>> GetIzolabellaCommandsAsync()
        {
            List<IIzolabellaCommand> InitializedCommands = new();
            foreach (Assembly Ass in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type T in Ass.GetTypes())
                {
                    if (typeof(IIzolabellaCommand).IsAssignableFrom(T) && !T.IsInterface)
                    {
                        object? Instance = Activator.CreateInstance(T);
                        if (Instance != null && Instance is IIzolabellaCommand I)
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

        internal Task<List<SlashCommandBuilder>> GetCommandBuildersAsync()
        {
            List<SlashCommandBuilder> CommandBuilders = new();
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
                        MaxValue = Param.MaxValue,
                        MinValue = Param.MinimumValue,
                        Choices = Param.Choices?.Select(PC => new ApplicationCommandOptionChoiceProperties()
                        {
                            Name = PC.Name,
                            Value = PC.Value,
                        }).ToList()
                    });
                }
                SlashCommandBuilder SlashCommandBuilder = new()
                {
                    Name = NameConformer.DiscordCommandConformity(Command.Name),
                    Description = Command.Description,
                    Options = Options,
                    IsDMEnabled = !Command.GuildsOnly
                };
                CommandBuilders.Add(SlashCommandBuilder);
            }
            return Task.FromResult(CommandBuilders);
        }

        internal async Task<IReadOnlyCollection<SocketApplicationCommand>> GetRelevantCommandsAsync()
        {
            List<SocketApplicationCommand> Commands = new();
            Commands = !this.GlobalCommands
                ? this.Client.Guilds.SelectMany((G) => G.GetApplicationCommandsAsync().Result).Where(C => C.ApplicationId == this.Client.CurrentUser.Id).ToList()
                : (await this.Client.GetGlobalApplicationCommandsAsync()).ToList();
            return Commands;
        }

        /// <summary>
        /// Returns all guild commands if the client is meant to be using global and vice versa.
        /// </summary>
        /// <returns></returns>
        internal async Task<IReadOnlyCollection<SocketApplicationCommand>> GetIrrelevantCommandsAsync()
        {
            IReadOnlyCollection<SocketApplicationCommand> CurrentCommands = this.GlobalCommands ? this.Client.Guilds.SelectMany((G) =>
            {
                try
                {
                    return G.GetApplicationCommandsAsync().Result;
                }
                catch
                {
                    return new List<SocketApplicationCommand>();
                }
            }
            ).Where(C => C.ApplicationId == this.Client.CurrentUser.Id).ToList() : await this.Client.GetGlobalApplicationCommandsAsync();
            return CurrentCommands;
        }

        internal async Task RegisterCommandsAsync()
        {
            List<SlashCommandBuilder> Commands = await this.GetCommandBuildersAsync();
            foreach (SlashCommandBuilder Command in Commands)
            {
                if (this.GlobalCommands)
                {
                    await this.Client.CreateGlobalApplicationCommandAsync(Command.Build());
                }
                else
                {
                    this.Client.Guilds.ToList()
                                      .ForEach(async G => await G.CreateApplicationCommandAsync(Command.Build()));
                }
            }
        }

        internal async Task DeleteIrrelevantCommands()
        {
            foreach (SocketApplicationCommand Command in await this.GetRelevantCommandsAsync())
            {
                if (Command.ApplicationId == this.Client.CurrentUser.Id)
                {
                    if (!this.Commands.Any(C => NameConformer.DiscordCommandConformity(C.Name) == Command.Name))
                    {
                        await Command.DeleteAsync();
                    }
                }
            }
            (await this.GetIrrelevantCommandsAsync()).ToList().ForEach(async SAC =>
            {
                await SAC.DeleteAsync();
            });
        }
    }
}
