using izolabella.Discord.Objects.Constants.Enums;

namespace izolabella.Discord.Objects.Constraints.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IIzolabellaCommandConstraint
    {
        /// <summary>
        /// The <see cref="IIzolabellaCommandConstraint"/>'s constraint type.
        /// </summary>
        public ConstraintTypes Type { get; }

        /// <summary>
        /// If not null, this constraint will only apply itself to a particular guild.
        /// </summary>
        public ulong? ConstrainToOneGuildOfThisId { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CommandFired"></param>
        /// <returns></returns>
        public Task<bool> CheckCommandValidityAsync(SocketSlashCommand CommandFired);
    }
}
