using izolabella.Discord.Commands.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
