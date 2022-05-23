using izolabella.Discord.Objects.Constraints.Interfaces;
using izolabella.Discord.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Discord.Objects.Constraints.Implementations
{
    /// <summary>
    /// A derivation of <see cref="IIzolabellaCommandConstraint"/> regarding a whitelist of users that may invoke the parent command.
    /// </summary>
    public class WhitelistUsersConstraint : IIzolabellaCommandConstraint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WhitelistUsersConstraint"/> class.
        /// </summary>
        /// <param name="UserIds">An array of ids belonging to discord users that may invoke the parent command.</param>
        public WhitelistUsersConstraint(params ulong[] UserIds)
        {
            this.UserIds = UserIds;
        }

        /// <summary>
        /// An array of ids belonging to discord users that may invoke the parent command.
        /// </summary>
        public ulong[] UserIds { get; }

        /// <inheritdoc/>
        public ConstraintTypes Type => ConstraintTypes.WhitelistUsers;

        Task<bool> IIzolabellaCommandConstraint.CheckCommandValidityAsync(SocketSlashCommand CommandFired)
        {
            return Task.FromResult(this.UserIds.Any(Id => Id == CommandFired.User.Id));
        }
    }
}
