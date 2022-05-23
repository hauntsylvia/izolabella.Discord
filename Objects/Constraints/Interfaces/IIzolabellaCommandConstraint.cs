using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using izolabella.Discord.Objects.Enums;

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
