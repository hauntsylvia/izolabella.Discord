using izolabella.Discord.Commands.Arguments;

namespace izolabella.Discord.Commands.Attributes
{
    /// <summary>
    /// An attribute used to specify that a method can be invoked for a command.
    /// </summary>
    /// <remarks>
    /// Methods should include a <see cref="CommandArguments"/> parameter.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandAttribute"/> class.
        /// </summary>
        /// <param name="Tags">Tags to search for within the <see cref="SocketMessage.Content"/> property.</param>
        /// <param name="Description">Description of this command.</param>
        public CommandAttribute(string[] Tags, string? Description)
        {
            this.Tags = Tags;
            this.Description = Description;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandAttribute"/> class.
        /// </summary>
        /// <param name="Tags">Tags to search for within the <see cref="SocketMessage.Content"/> property.</param>
        /// <param name="Blacklist">The ids of Discord users to disallow from invoking this command.</param>
        /// <param name="Whitelist">The exclusive ids of Discord users to allow from invoking this command.</param>
        /// <param name="Description">Description of this command.</param>
        public CommandAttribute(string[] Tags, ulong[]? Whitelist, ulong[] Blacklist, string? Description)
        {
            this.Tags = Tags;
            this.Whitelist = Whitelist;
            this.Blacklist = Blacklist;
            this.Description = Description;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandAttribute"/> class.
        /// </summary>
        /// <param name="Tags">Tags to search for within the <see cref="SocketMessage.Content"/> property.</param>
        /// <param name="Whitelist">The exclusive ids of Discord users to allow from invoking this command.</param>
        /// <param name="Description">Description of this command.</param>
        public CommandAttribute(string[] Tags, ulong[] Whitelist, string? Description)
        {
            this.Tags = Tags;
            this.Whitelist = Whitelist;
            this.Description = Description;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandAttribute"/> class.
        /// </summary>
        /// <param name="Tags">Tags to search for within the <see cref="SocketMessage.Content"/> property.</param>
        /// <param name="Whitelist">The exclusive ids of Discord users to allow from invoking this command.</param>
        /// <param name="Blacklist">The ids of Discord users to disallow from invoking this command.</param>
        /// <param name="Description">Description of this command.</param>
        /// <param name="Defer">If true, this indicates the command needs more than 3 seconds to respond to the interaction.
        /// Also requires the attached method to use FollowUpAsync instead of RespondAsync to interactions.</param>
        /// <param name="LocalOnly">If true, any responses will be sent to only the person who began the interaction.</param>
        public CommandAttribute(string[] Tags, ulong[]? Whitelist, ulong[]? Blacklist, string? Description, bool Defer, bool LocalOnly)
        {
            this.Tags = Tags;
            this.Whitelist = Whitelist;
            this.Blacklist = Blacklist;
            this.Description = Description;
            this.Defer = Defer;
            this.LocalOnly = LocalOnly;
        }

        /// <summary>
        /// Tags to use for this command.
        /// </summary>
        public string[] Tags { get; }

        /// <summary>
        /// An exclusive list of Discord users to allow usage for this command to.
        /// </summary>
        public ulong[]? Whitelist { get; }

        /// <summary>
        /// An exclusive list of Discord users to disallow usage for this command to.
        /// </summary>
        public ulong[]? Blacklist { get; }

        /// <summary>
        /// Description of this command.
        /// </summary>
        public string? Description { get; }

        /// <summary>
        /// If true, this indicates the command needs more than 3 seconds to respond to the interaction.
        /// </summary>
        /// <remarks>This is especially useful if the comand depends on external APIs for data, or if it is just generally a time-intensive command.</remarks>
        public bool Defer { get; set; } = true;

        /// <summary>
        /// If true, any responses will be sent to only the person who began the interaction.
        /// </summary>
        public bool LocalOnly { get; set; } = true;
    }
}
