using Discord.Net;
using izolabella.Discord.Objects.Arguments;
using izolabella.Discord.Objects.Constraints.Interfaces;
using izolabella.Discord.Objects.Parameters;
using izolabella.Discord.Objects.Structures.Discord.Commands.Enums;

namespace izolabella.Discord.Objects.Structures.Discord.Commands
{
    /// <summary>
    /// An abstract class that defines commands.
    /// As of now, writing a constructor will cause the handler to not construct the implementing type.
    /// Do not write a constructor.
    /// </summary>
    public abstract class IzolabellaCommand
    {
        /// <summary>
        /// The type of command this is.
        /// </summary>
        public CommandType Command { get; protected set; } = CommandType.Main;

        /// <summary>
        /// The subcommands belonging to this command.
        /// </summary>
        public virtual List<IzolabellaSubCommand> SubCommands { get; } = new();

        /// <summary>
        /// The name of the command.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// The description of the command.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// If true, this command is only allowed to be invoked in guilds (servers).
        /// </summary>
        public abstract bool GuildsOnly { get; }

        /// <summary>
        /// A list of the parameters users may invoke this command with.
        /// </summary>
        public abstract List<IzolabellaCommandParameter> Parameters { get; }

        /// <summary>
        /// A list of constraints that must all return true for the handler to invoke this command.
        /// </summary>
        public abstract List<IIzolabellaCommandConstraint> Constraints { get; }

        /// <summary>
        /// The method that will run when the command is invoked.
        /// </summary>
        /// <param name="Context">The context the handler will pass.</param>
        /// <param name="Arguments">The arguments the end user has invoked this command with.</param>
        /// <returns></returns>
        public abstract Task RunAsync(CommandContext Context, IzolabellaCommandArgument[] Arguments);

        /// <summary>
        /// The method that will run when all commands have been initialized. Comes from a static context: fired once.
        /// </summary>
        /// 
        /// <remarks>
        /// This method is particularly good for being able to define parameters and constraints based off other commands.
        /// </remarks>
        /// <returns></returns>
        public virtual Task OnLoadAsync(IzolabellaCommand[] AllCommands)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// The method that will run if the command invocation fails due to set constraints.
        /// </summary>
        /// <param name="Context">The context the handler will pass.</param>
        /// <param name="Arguments">The arguments the end user has invoked this command with.</param>
        /// <param name="ConstraintThatFailed">The constraint that caused this method to get fired by the handler.</param>
        /// <returns></returns>
        public virtual Task OnConstrainmentAsync(CommandContext Context, IzolabellaCommandArgument[] Arguments, IIzolabellaCommandConstraint ConstraintThatFailed)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// The method that will run if the command has failed due to a <see cref="HttpException"/>.
        /// </summary>
        /// <param name="Context">The context the handler may pass.</param>
        /// <param name="Error">The error that caused this method to be invoked.</param>
        /// <returns></returns>
        public virtual Task OnErrorAsync(CommandContext? Context, HttpException Error)
        {
            return Task.CompletedTask;
        }
    }
}
