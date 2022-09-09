using Discord.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Discord.Objects.Structures.Discord.Receivers;

/// <summary>
/// Represents a class that will receive a particular update on Discord's side.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class IzolabellaReceiver<T>
{
    /// <summary>
    /// The name of this receiver.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// The predicate that must return true in order for the receiver to run.
    /// </summary>
    public abstract Predicate<T> ValidPredicate { get; }

    /// <summary>
    /// The method that will run when an error occurs.
    /// </summary>
    /// <returns></returns>
    public abstract Task OnErrorAsync(HttpException Exception);
}
