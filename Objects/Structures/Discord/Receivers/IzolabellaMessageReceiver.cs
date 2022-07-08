using izolabella.Discord.Objects.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Discord.Objects.Structures.Discord.Receivers
{
    /// <summary>
    /// Represents a class used for receiving <see cref="SocketMessage"/> events.
    /// </summary>
    public abstract class IzolabellaMessageReceiver : IzolabellaReceiver<SocketMessage>
    {
        /// <summary>
        /// The method that will run when a message is received.
        /// </summary>
        /// <param name="Reference">The client running this reaction.</param>
        /// <param name="Message">The message sent.</param>
        /// <returns></returns>
        public abstract Task OnMessageAsync(IzolabellaDiscordClient Reference, SocketMessage Message);
    }
}
