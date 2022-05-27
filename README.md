# 💞 izolabella.Discord

This project was created as an alternative command handling library to the provided Discord.NET command handler, mostly for custom behavior not already provided.
It is also considerably lighter than other handlers, and thus much easier to get started with.

# 👩‍💻 Getting Started
This project is available through [NuGet.org](https://www.nuget.org/packages/izolabella.Discord/);
- __Package Manager (Visual Studio)__
```
Install-Package izolabella.Discord
```
- __.NET CLI__
```
dotnet add package izolabella.Discord
```
This project uses [Discord.NET](https://www.nuget.org/packages/Discord.Net/) to function!*

# ⌨️ Code
A new instance of the `IzolabellaDiscordCommandClient` class must be initialized. The class takes a `DiscordSocketConfig` argument for the client, and a `bool` for whether or not the commands should be updated per guild or globally.
```cs
IzolabellaDiscordCommandClient Client = new(new DiscordSocketConfig(), false);
```

The current version of this library uses classes for commands. To create a command, create a class that inherits the interface `IIzolabellaCommand`.
```cs
namespace MyDiscordBot.Commands
{
    public class MyCommand : IIzolabellaCommand
    {
        public string Name => "Command";

        public string Description => "My command's description.'";

        public bool GuildsOnly => true;

        public List<IIzolabellaCommandConstraint> Constraints { get; } = new();

        public IzolabellaCommandParameter[] Parameters => new[]
        {
            new IzolabellaCommandParameter("Param", "This is my parameter!", ApplicationCommandOptionType.Channel, true)
        };

        public Task RunAsync(CommandContext Context, IzolabellaCommandArgument[] Arguments)
        {
            // command runs here!
        }

        public Task OnLoadAsync(IIzolabellaCommand[] AllCommands)
        {
            // runs when all commands have been initialized - fired once.
        }

        public Task OnConstrainmentAsync(CommandContext Context, IzolabellaCommandArgument[] Arguments, IIzolabellaCommandConstraint ConstraintThatFailed)
        {
            // when one of the constrainments don't pass the validity check by the handler, this method gets called.
        }
    }
}
```

To get things going, call the following method on the `IzolabellaDiscordCommandClient` instance you have created:
```cs
await Client.StartAsync();
```

__I want to make this as useable as possible for you. I have no in-depth documentation since things are currently changing rapidly. For questions and how-tos, please send me a request on Discord at `izolabella.bin#0216`.__
