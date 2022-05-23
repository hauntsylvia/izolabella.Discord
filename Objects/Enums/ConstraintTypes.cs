using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Discord.Objects.Enums
{
    /// <summary>
    /// Defines types of constraints.
    /// </summary>
    public enum ConstraintTypes
    {
        /// <summary>
        /// Indicates the parent type is a constraint that operates using user ids.
        /// </summary>
        WhitelistUsers,

        /// <summary>
        /// Indicates the parent type is a constraint that operates using role ids.
        /// </summary>
        WhitelistRoles,

        /// <summary>
        /// Indicates the parent type is a constraint that operates using guild level permissions.
        /// </summary>
        WhitelistPermissions
    }
}
