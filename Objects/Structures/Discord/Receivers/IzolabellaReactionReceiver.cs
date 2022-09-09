using izolabella.Discord.Objects.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Discord.Objects.Structures.Discord.Receivers;

/// <summary>
/// Represents a class used for receiving <see cref="SocketReaction"/> events.
/// </summary>
public abstract class IzolabellaReactionReceiver : IzolabellaReceiver<SocketReaction>
{
    /// <summary>
    /// The method that runs when a reaction is either added or removed.
    /// </summary>
    /// <param name="Reference">The client running this reaction.</param>
    /// <param name="Reaction">The reaction sent.</param>
    /// <param name="ReactionRemoved">Whether the reaction event is a removal or an addition.</param>
    /// <returns></returns>
    public abstract Task OnReactionAsync(IzolabellaDiscordClient Reference, SocketReaction Reaction, bool ReactionRemoved);
}
