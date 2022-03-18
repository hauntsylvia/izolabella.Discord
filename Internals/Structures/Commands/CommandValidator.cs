using izolabella.Discord.Commands.Handlers;
using izolabella.Discord.Internals.Surgical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Discord.Internals.Structures.Commands
{
    /// <summary>
    /// Reasons a command request may be invalid.
    /// </summary>
    public enum InvalidCommandReasons
    {
        /// <summary>
        /// The parameters the command requires are not of the same type.
        /// </summary>
        Required_Parameters_Are_Incorrect,
        /// <summary>
        /// The parameters the command requires were not found in the request.
        /// </summary>
        Required_Parameters_Are_Missing,
        /// <summary>
        /// The user is blacklisted from this command.
        /// </summary>
        User_Is_Blacklisted,
        /// <summary>
        /// The user is not whitelisted for this command.
        /// </summary>
        User_Is_Not_Whitelisted,
        /// <summary>
        /// The reason is unknown.
        /// </summary>
        Unknown
    }

    /// <summary>
    /// A class for processing commands that already exist.
    /// </summary>
    public class CommandValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandValidator"/> class.
        /// </summary>
        /// <param name="Instance"></param>
        public CommandValidator(CommandHandler Instance)
        {
            this.Instance = Instance;
        }

        /// <summary>
        /// The instance this class should use.
        /// </summary>
        public CommandHandler Instance { get; }

        /// <summary>
        /// Validate a request.
        /// </summary>
        /// <param name="DiscordCommand"></param>
        /// <param name="CommandRelevance"></param>
        /// <returns>A reason the request is invalid, or null if the request is valid.</returns>
        public static InvalidCommandReasons? ValidateCommand(SocketSlashCommand DiscordCommand, CommandWrapper CommandRelevance)
        {
            bool IsWhitelisted = (CommandRelevance.Attribute.Whitelist != null && CommandRelevance.Attribute.Whitelist.Any(Id => DiscordCommand.User.Id == Id)) || CommandRelevance.Attribute.Whitelist == null;
            bool IsBlacklisted = (CommandRelevance.Attribute.Blacklist != null && CommandRelevance.Attribute.Blacklist.Any(Id => DiscordCommand.User.Id == Id)) || false;
            if (!IsWhitelisted)
            {
                return InvalidCommandReasons.User_Is_Not_Whitelisted;
            }
            else if (IsBlacklisted)
            {
                return InvalidCommandReasons.User_Is_Blacklisted;
            }
            IReadOnlyCollection<CommandParameter> Parameters = CommandRelevance.GetCommandParameters();
            foreach (SocketSlashCommandDataOption Option in DiscordCommand.Data.Options)
            {
                foreach (CommandParameter Param in Parameters)
                {
                    if (Param.Name == Option.Name && Option.Type != Param.ParameterType)
                    {
                        return InvalidCommandReasons.Required_Parameters_Are_Incorrect;
                    }
                }
            }
            foreach (CommandParameter Param in Parameters)
            {
                if (Param.IsRequired && !DiscordCommand.Data.Options.Any(O => O.Name == Param.Name))
                {
                    return InvalidCommandReasons.Required_Parameters_Are_Missing;
                }
            }
            return null;
        }
    }
}
