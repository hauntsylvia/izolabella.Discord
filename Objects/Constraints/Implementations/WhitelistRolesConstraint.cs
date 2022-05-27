using izolabella.Discord.Objects.Constraints.Interfaces;
using izolabella.Discord.Objects.Enums;

namespace izolabella.Discord.Objects.Constraints.Implementations
{
    /// <summary>
    /// A derivation of <see cref="IIzolabellaCommandConstraint"/> regarding a whitelist of users that may invoke the parent command.
    /// </summary>
    public class WhitelistRolesConstraint : IIzolabellaCommandConstraint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WhitelistUsersConstraint"/> class.
        /// </summary>
        /// <param name="ValidOnlyInGuilds">If true, the constraint will not allow the command to be invoked unless it was invoked in a guild.</param>
        /// <param name="RoleIds">An array of ids belonging to discord roles that users must possess to invoke the command.</param>
        public WhitelistRolesConstraint(bool ValidOnlyInGuilds = true, params ulong[] RoleIds)
        {
            this.ValidOnlyInGuilds = ValidOnlyInGuilds;
            this.RoleIds = RoleIds;
        }

        /// <summary>
        /// If true, the constraint will not allow the command to be invoked unless it was invoked in a guild.
        /// </summary>
        public bool ValidOnlyInGuilds { get; }

        /// <summary>
        /// An array of ids belonging to discord roles that users must possess to invoke the command.
        /// </summary>
        public ulong[] RoleIds { get; }

        /// <inheritdoc/>
        public ConstraintTypes Type => ConstraintTypes.WhitelistRoles;

        /// <inheritdoc/>
        public ulong? ConstrainToOneGuildOfThisId { get; set; }

        Task<bool> IIzolabellaCommandConstraint.CheckCommandValidityAsync(SocketSlashCommand CommandFired)
        {
            return CommandFired.User is SocketGuildUser SUser
                ? Task.FromResult(SUser.Roles.Any(Role => this.RoleIds.FirstOrDefault(RId => RId == Role.Id) != default))
                : Task.FromResult(!this.ValidOnlyInGuilds);
        }
    }
}
