using izolabella.Discord.Objects.Constants.Enums;
using izolabella.Discord.Objects.Constraints.Interfaces;

namespace izolabella.Discord.Objects.Constraints.Implementations;

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

    /// <inheritdoc/>
    public ulong? ConstrainToOneGuildOfThisId { get; set; }

    Task<bool> IIzolabellaCommandConstraint.CheckCommandValidityAsync(SocketSlashCommand CommandFired)
    {
        return Task.FromResult(this.UserIds.Any(Id => Id == CommandFired.User.Id));
    }
}
