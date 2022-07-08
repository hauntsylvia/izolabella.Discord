# 💞 izolabella.Discord

This project is an alternative to the command handler provided by Discord.NET for providing more detail to commands in a lightweight manner.

# 🌸 Quality Status
[![CodeFactor](https://www.codefactor.io/repository/github/izolabella/izolabella.discord/badge)](https://www.codefactor.io/repository/github/izolabella/izolabella.discord)

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

The current version of this library uses classes for commands. To create a command, create a class that inherits the abstract class `IzolabellaCommand`. **These classes must have parameterless constructors.**
```cs
public class MyCommand : IzolabellaCommand
{
    public override string Name => "Command";

    public override string Description => "My command's description.'";

    public override bool GuildsOnly => true;

    public override List<IIzolabellaCommandConstraint> Constraints { get; } = new()
    {
        new WhitelistPermissionsConstraint(false, GuildPermission.Administrator)
    };

    public override List<IzolabellaCommandParameter> Parameters { get; } = new()
    {
        new IzolabellaCommandParameter("Param", "This is my parameter!", ApplicationCommandOptionType.Channel, true)
    };

    public override Task RunAsync(CommandContext Context, IzolabellaCommandArgument[] Arguments)
    {
        // command runs here!
    }

    public override Task OnLoadAsync(IIzolabellaCommand[] AllCommands)
    {
        // runs when all commands have been initialized - fired once.
    }

    public override Task OnConstrainmentAsync(CommandContext Context, IzolabellaCommandArgument[] Arguments, IIzolabellaCommandConstraint ConstraintThatFailed)
    {
        // when one of the constrainments don't pass the validity check by the handler, this method gets called.
    }
        
    public override Task OnErrorAsync(HttpException Error)
    {
        // when an error happens, this method will run.
    }
}
```

To create sub-commands, a class that inherits from `IzolabellaSubCommand` must be created, initialized, and placed in a list of a normal `IzolabellaCommand`. For example:
```cs
public class ExampleSub : IzolabellaSubCommand
{
    public override string Name => "Example Name";

    public override string Description => "Example!";

    public override bool GuildsOnly => true;

    public override List<IzolabellaCommandParameter> Parameters => new()
    {
        new("Channel", "Pick a channel!", ApplicationCommandOptionType.Channel, new() { ChannelType.Text }, false)
    };

    public override List<IIzolabellaCommandConstraint> Constraints => new();

    public override async Task RunAsync(CommandContext Context, IzolabellaCommandArgument[] Arguments)
    {
        await Context.UserContext.RespondAsync(text: "abc!!");
    }
}
```

Then, in an `IzolabellaCommand`, you can place it in the `SubCommands` property. As a shorter example (borrowing from the previous `IzolabellaCommand` example):
```cs
public class MyCommand : IzolabellaCommand
{
    public override List<IzolabellaSubCommand> SubCommands => new() { new ExampleSub() };
}
```



To get things going, call the following method on the `IzolabellaDiscordCommandClient` instance you have created:
```cs
await Client.StartAsync();
```

Further, the client itself offers numerous events to hook on to. Browse intellisense context menus to see all the options.

***Arguments will be in kebab case when passed back to the RunAsync methods!!! When comparing arguments by name, please keep this in mind. For example, if I have a parameter named "Channel Id", I should check in the arguments for an argument named "channel-id".***

__I want to make this as useable as possible for you. I have no in-depth documentation since things are currently changing rapidly. For questions and how-tos, please send me a request on Discord at `izolabella.bin#0216`.__
