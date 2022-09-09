using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Discord.Objects.Structures.Discord.Commands;

/// <summary>
/// An abstract class representing a sub command on Discord.
/// Writing constructors on sub commands is perfectly okay, since they are accessed through main command
/// properties.
/// </summary>
public abstract class IzolabellaSubCommand : IzolabellaCommand
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IzolabellaSubCommand"/> class.
    /// </summary>
    public IzolabellaSubCommand()
    {
        this.Command = Enums.CommandType.Sub;
    }
}
