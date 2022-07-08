global using Discord;
global using Discord.WebSocket;
using Discord.Net;
using izolabella.Discord.Objects.Arguments;
using izolabella.Discord.Objects.Constraints.Interfaces;
using izolabella.Discord.Objects.Parameters;
using izolabella.Discord.Objects.Structures.Discord.Commands;
using izolabella.Discord.Objects.Structures.Discord.Receivers;
using izolabella.Discord.Objects.Util;
using izolabella.Util;
using System.Reflection;

namespace izolabella.Discord.Objects.Clients
{
    /// <summary>
    /// A class used for wrapping an instance of <see cref="DiscordSocketClient"/>.
    /// </summary>
    public class IzolabellaDiscordClient : DiscordSocketClient
    {
        /// <summary>
        /// Initializes a new instance of <see cref="IzolabellaDiscordClient"/>.
        /// </summary>
        /// <param name="Config">The client's configurations.</param>
        /// <param name="GlobalCommands">If true, all commands will be created globally. This may take up to an hour to show up on Discord's side.</param>
        public IzolabellaDiscordClient(DiscordSocketConfig Config, bool GlobalCommands) : base(Config)
        {
            this.SelfAss = Assembly.GetCallingAssembly();
            this.GlobalCommands = GlobalCommands;
            this.Ready += () =>
            {
                this.ClientReady = true;
                return Task.CompletedTask;
            };
            this.Commands = GetIzolabellaCommandsAsync(this.SelfAss).Result;
            this.MessageReceivers = this.GetMessageReceivers();
            this.ReactionReceivers = this.GetReactionReceivers();
            this.JoinedGuild += this.ClientJoinedGuildAsync;
        }

        #region properties

        private Assembly SelfAss { get; }

        /// <summary>
        /// True once the <see cref="DiscordSocketClient.Ready"/> event has fired.
        /// </summary>
        public bool ClientReady { get; private set; } = false;

        /// <summary>
        /// The <see cref="IzolabellaCommand"/>s found by this instance.
        /// </summary>
        public List<IzolabellaCommand> Commands { get; }

        /// <summary>
        /// The <see cref="IzolabellaMessageReceiver"/>s found by this instance.
        /// </summary>
        public IEnumerable<IzolabellaMessageReceiver> MessageReceivers { get; }

        /// <summary>
        /// The <see cref="IzolabellaReactionReceiver"/>s found by this instance.
        /// </summary>
        public IEnumerable<IzolabellaReactionReceiver> ReactionReceivers { get; }

        /// <summary>
        /// A bool representing whether the client should make commands global or not.
        /// </summary>
        public bool GlobalCommands { get; }

        #endregion

        #region events

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
        /// The method that will run if a command fails due to a <see cref="HttpException"/>.
        /// </summary>
        public delegate Task CommandExceptionHandler(IzolabellaCommand? Command, CommandContext? Context, HttpException Exception);

        /// <summary>
        /// Fired when an <see cref="IzolabellaCommand"/> errors.
        /// </summary>
        public event CommandExceptionHandler? OnCommandError;

        /// <summary>
        /// The method that will run if a <see cref="IzolabellaMessageReceiver"/> fails due to a <see cref="HttpException"/>.
        /// </summary>
        public delegate Task MessageReceiverExceptionHandler(IzolabellaMessageReceiver Receiver, HttpException Exception);

        /// <summary>
        /// Fired when an <see cref="IzolabellaMessageReceiver"/> errors.
        /// </summary>
        public event MessageReceiverExceptionHandler? OnMessageReceiverError;

        /// <summary>
        /// The method that will run if a <see cref="IzolabellaReactionReceiver"/> fails due to a <see cref="HttpException"/>.
        /// </summary>
        public delegate Task ReactionReceiverExceptionHandler(IzolabellaReactionReceiver Receiver, HttpException Exception);

        /// <summary>
        /// Fired when a command errors.
        /// </summary>
        public event ReactionReceiverExceptionHandler? OnReactionReceiverError;

        /// <summary>
        /// The method that will run after a command is invoked.
        /// </summary>
        /// <param name="Context">The context the handler will pass.</param>
        /// <param name="Arguments">The arguments the end user has invoked this command with.</param>
        /// <param name="CommandInvoked">The command this handler invoked.</param>
        public delegate Task CommandInvokedHandler(CommandContext Context, IzolabellaCommandArgument[] Arguments, IzolabellaCommand CommandInvoked);

        /// <summary>
        /// Fired after a command is invoked.
        /// </summary>
        public event CommandInvokedHandler? CommandInvoked;

        /// <summary>
        /// Run before a command is fired to check command validity.
        /// </summary>
        public Func<IzolabellaCommand, CommandContext, Task<bool>>? PreCommandInvokeCheck { get; set; }

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
        public new event BeforeGuildJoinHandler? JoinedGuild;

        #endregion

        #region methods

        private IEnumerable<IzolabellaMessageReceiver> GetMessageReceivers()
        {
            return BaseImplementationUtil.GetItems<IzolabellaMessageReceiver>(this.SelfAss);
        }

        private IEnumerable<IzolabellaReactionReceiver> GetReactionReceivers()
        {
            return BaseImplementationUtil.GetItems<IzolabellaReactionReceiver>(this.SelfAss);
        }

        /// <summary>
        /// Updates commands.
        /// </summary>
        /// <param name="Arg">The socket guild argument in this method.</param>
        /// <returns></returns>
        private async Task ClientJoinedGuildAsync(SocketGuild Arg)
        {
            if (this.ClientReady)
            {
                _ = (this.JoinedGuild?.Invoke(Arg));
                _ = await this.Rest.GetGuildAsync(Arg.Id);
                if (!this.GlobalCommands)
                {
                    await this.DeleteIrrelevantCommands();
                    await this.RegisterCommandsAsync().ConfigureAwait(false);
                }
                _ = (this.AfterJoinedGuild?.Invoke(Arg));
            }
        }

        /// <summary>
        /// Logs in and connects the <see cref="IzolabellaDiscordClient"/>.
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="RegisterCommandsAtTheSameTime">Set to true if calling this method should also register commands.</param>
        /// <returns></returns>
        public async Task StartAsync(string Token, bool RegisterCommandsAtTheSameTime = true)
        {
            if (RegisterCommandsAtTheSameTime)
            {
                this.Ready += async () =>
                {
                    await this.DeleteIrrelevantCommands();
                    await this.RegisterCommandsAsync();
                };
            }
            this.SlashCommandExecuted += async (PassedCommand) =>
            {
                IzolabellaCommand? Command = this.Commands.FirstOrDefault(
                    Iz => NameConformer.DiscordCommandConformity(Iz.Name) == NameConformer.DiscordCommandConformity(PassedCommand.CommandName));
                if (Command != null)
                {
                    ulong? GuildId = null;
                    if (PassedCommand.User is SocketGuildUser SUser)
                    {
                        GuildId = SUser.Guild.Id;
                    }
                    SocketSlashCommandDataOption? SubArg = PassedCommand.Data.Options.FirstOrDefault(Opt => Opt.Type == ApplicationCommandOptionType.SubCommand);
                    IEnumerable<IzolabellaCommandArgument> SentParameters = PassedCommand.Data.Options.Select<SocketSlashCommandDataOption, IzolabellaCommandArgument>(Data => new(Data.Name, "", Data.Type, true, Data.Value));
                    if (SubArg != null && SubArg.Name is string SubArgVal)
                    {
                        SentParameters = SubArg.Options.Select<SocketSlashCommandDataOption, IzolabellaCommandArgument>(Data => new(Data.Name, "", Data.Type, true, Data.Value));
                        Command = Command.SubCommands.First(C => NameConformer.DiscordCommandConformity(C.Name) == NameConformer.DiscordCommandConformity(SubArgVal));
                    }
                    IIzolabellaCommandConstraint? CausesFailure = Command.Constraints.Where(C => C.ConstrainToOneGuildOfThisId == null || GuildId == null || C.ConstrainToOneGuildOfThisId == GuildId).FirstOrDefault(C => !C.CheckCommandValidityAsync(PassedCommand).Result);
                    CommandContext Context = new(PassedCommand, this);
                    bool RawCheck = await (this.PreCommandInvokeCheck?.Invoke(Command, Context) ?? Task.FromResult(true));
                    bool Check = this.PreCommandInvokeCheck == null || RawCheck;
                    if (Check)
                    {
                        if (CausesFailure == null)
                        {
                            try
                            {
                                await Command.RunAsync(Context, SentParameters.ToArray());
                            }
                            catch(HttpException Ex)
                            {
                                this.OnCommandError?.Invoke(Command, Context, Ex);
                                await Command.OnErrorAsync(Context, Ex);
                            }
                            await (this.CommandInvoked != null ? this.CommandInvoked.Invoke(Context, SentParameters.ToArray(), Command) : Task.CompletedTask);
                        }
                        else
                        {
                            await (this.OnCommandConstraint != null ? this.OnCommandConstraint.Invoke(Context, SentParameters.ToArray(), CausesFailure) : Task.CompletedTask);
                            await Command.OnConstrainmentAsync(Context, SentParameters.ToArray(), CausesFailure);
                        }
                    }
                }
            };
            this.MessageReceived += async (Message) =>
            {
                foreach (IzolabellaMessageReceiver ValidReceiver in this.MessageReceivers.Where(MR => MR.ValidPredicate.Invoke(Message)))
                {
                    try
                    {
                        await ValidReceiver.OnMessageAsync(this, Message);
                    }
                    catch (HttpException Ex)
                    {
                        this.OnMessageReceiverError?.Invoke(ValidReceiver, Ex);
                        await ValidReceiver.OnErrorAsync(Ex);
                    }
                }
            };
            this.ReactionAdded += async (Arg1, Arg2, Arg3) => await this.ClientReactionUpdatedAsync(Arg3, false);
            this.ReactionRemoved += async (Arg1, Arg2, Arg3) => await this.ClientReactionUpdatedAsync(Arg3, true);
            await this.LoginAsync(TokenType.Bot, Token, true);
            await this.StartAsync();
        }

        private async Task ClientReactionUpdatedAsync(SocketReaction Reaction, bool IsRemoval)
        {
            foreach(IzolabellaReactionReceiver ValidReceiver in this.ReactionReceivers.Where(RR => RR.ValidPredicate.Invoke(Reaction)))
            {
                try
                {
                    await ValidReceiver.OnReactionAsync(this, Reaction, IsRemoval);
                }
                catch(HttpException Ex)
                {
                    this.OnReactionReceiverError?.Invoke(ValidReceiver, Ex);
                    await ValidReceiver.OnErrorAsync(Ex);
                }
            }
        }

        /// <summary>
        /// Logs the client out before disposing its resources.
        /// </summary>
        /// <returns></returns>
        public async Task StopAndLogoutAsync()
        {
            await this.LogoutAsync();
            await this.StopAsync();
            await this.DisposeAsync();
        }

        /// <summary>
        /// Adds or updates commands. Useful for adding new constraints.
        /// </summary>
        /// <param name="NewCommands">The commands to add or update.</param>
        /// <returns></returns>
        public async Task UpdateCommandsAsync(params IzolabellaCommand[] NewCommands)
        {
            if (this.ClientReady)
            {
                foreach (IzolabellaCommand NewCommand in NewCommands)
                {
                    IzolabellaCommand? Existing = this.Commands.Find(C => C.Name == NewCommand.Name);
                    if (Existing != null)
                    {
                        _ = this.Commands.Remove(Existing);
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
        public static async Task<List<IzolabellaCommand>> GetIzolabellaCommandsAsync(Assembly LoadsFrom)
        {
            List<IzolabellaCommand> InitializedCommands = BaseImplementationUtil.GetItems<IzolabellaCommand>(LoadsFrom);

            foreach (IzolabellaCommand Command in InitializedCommands)
            {
                await Command.OnLoadAsync(InitializedCommands.ToArray());
            }
            return InitializedCommands;
        }

        internal static List<SlashCommandOptionBuilder> GetCommandParams(IzolabellaCommand Command)
        {
            List<SlashCommandOptionBuilder> Options = new();
            foreach (IzolabellaCommandParameter Param in Command.Parameters)
            {
                if(Command.SubCommands.Count == 0)
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
                        }).ToList(),
                        ChannelTypes = Param.ChannelTypes
                    });
                }
            }
            return Options;
        }

        internal Task<List<SlashCommandBuilder>> GetCommandBuildersAsync()
        {
            List<SlashCommandBuilder> CommandBuilders = new();
            foreach (IzolabellaCommand Command in this.Commands.Where(C => C.Command == Structures.Discord.Commands.Enums.CommandType.Main))
            {
                List<SlashCommandOptionBuilder> Parameters = GetCommandParams(Command);
                foreach(IzolabellaSubCommand SubCommand in Command.SubCommands)
                {
                    Parameters.Add(new()
                    {
                        Type = ApplicationCommandOptionType.SubCommand,
                        Name = NameConformer.DiscordCommandConformity(SubCommand.Name),
                        Description = SubCommand.Description,
                        Options = GetCommandParams(SubCommand),
                    });
                }
                SlashCommandBuilder SlashCommandBuilder = new()
                {
                    Name = NameConformer.DiscordCommandConformity(Command.Name),
                    Description = Command.Description,
                    Options = Parameters,
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
                ? this.Guilds.SelectMany((G) => G.GetApplicationCommandsAsync().Result).Where(C => C.ApplicationId == this.CurrentUser.Id).ToList()
                : (await this.GetGlobalApplicationCommandsAsync()).ToList();
            return Commands;
        }

        /// <summary>
        /// Returns all guild commands if the client is meant to be using global and vice versa.
        /// </summary>
        /// <returns></returns>
        internal async Task<IReadOnlyCollection<SocketApplicationCommand>> GetIrrelevantCommandsAsync()
        {
            IReadOnlyCollection<SocketApplicationCommand> CurrentCommands = this.GlobalCommands ? this.Guilds.SelectMany((G) =>
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
            ).Where(C => C.ApplicationId == this.CurrentUser.Id).ToList() : await this.GetGlobalApplicationCommandsAsync();
            return CurrentCommands;
        }

        internal async Task RegisterCommandsAsync()
        {
            List<SlashCommandBuilder> Commands = await this.GetCommandBuildersAsync();
            foreach (SlashCommandBuilder Command in Commands)
            {
                if (this.GlobalCommands)
                {
                    _ = await this.CreateGlobalApplicationCommandAsync(Command.Build());
                }
                else
                {
                    this.Guilds.ToList().ForEach(async G => await G.CreateApplicationCommandAsync(Command.Build()));
                }
            }
        }

        internal async Task DeleteIrrelevantCommands()
        {
            foreach (SocketApplicationCommand Command in await this.GetRelevantCommandsAsync())
            {
                if (Command.ApplicationId == this.CurrentUser.Id)
                {
                    if (!this.Commands.Any(C => NameConformer.DiscordCommandConformity(C.Name) == Command.Name))
                    {
                        await Command.DeleteAsync();
                    }
                }
            }
            (await this.GetIrrelevantCommandsAsync()).ToList().ForEach(async SAC => await SAC.DeleteAsync());
        }
        #endregion
    }
}
