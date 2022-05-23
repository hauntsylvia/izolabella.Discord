using izolabella.Discord.Objects.Arguments;
using izolabella.Discord.Objects.Constraints.Interfaces;
using izolabella.Discord.Objects.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Discord.Objects.Interfaces
{
    /// <summary>
    /// An interface that defines commands.
    /// </summary>
    /// <remarks>
    /// As of now, writing a constructor will cause the handler to not construct the implementing type.
    /// Do not write a constructor.
    /// </remarks>
    public interface IIzolabellaCommand
    {
        /// <summary>
        /// The name of the command.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The description of the command.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// A list of the parameters users may invoke this command with.
        /// </summary>
        public List<IzolabellaCommandParameter> Parameters { get; }

        /// <summary>
        /// A list of constraints that must all return true for the handler to invoke this command.
        /// </summary>
        public List<IIzolabellaCommandConstraint> Constraints { get; }

        /// <summary>
        /// The method that will run when the command is invoked.
        /// </summary>
        /// <param name="Context">The context the handler will pass.</param>
        /// <param name="Arguments">The arguments the end user has invoked this command with.</param>
        /// <returns></returns>
        public Task RunAsync(CommandContext Context, IzolabellaCommandArgument[] Arguments);

        /// <summary>
        /// The method that will run when all commands have been initialized. Comes from a static context: fired once.
        /// </summary>
        /// <returns></returns>
        public Task OnLoadAsync(IIzolabellaCommand[] AllCommands);

        /// <summary>
        /// The method that will run if the command invocation fails due to set constraints.
        /// </summary>
        /// <param name="Context">The context the handler will pass.</param>
        /// <param name="Arguments">The arguments the end user has invoked this command with.</param>
        /// <param name="ConstraintThatFailed">The constraint that caused this method to get fired by the handler.</param>
        /// <returns></returns>
        public Task OnConstrainmentAsync(CommandContext Context, IzolabellaCommandArgument[] Arguments, IIzolabellaCommandConstraint ConstraintThatFailed);
    }
}
