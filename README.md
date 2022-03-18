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
A new instance of the `DiscordWrapper` class must be initialized.
```cs
DiscordSocketClient DiscordNETClient = new DiscordSocketClient();
DiscordWrapper DiscordWrapper = new DiscordWrapper(DiscordNETClient);
```

The `DiscordWrapper` class has a property (`CommandHandler`) that allows for other optional configurations for message pre-validation, such as whether to allow bots
to interact with the `CommandHandler` or not.
```cs
DiscordWrapper.CommandHandler.AllowBotInteractions = false;
```

To mark a method as a command, the `CommandAttribute` attribute should be attached to a method with a `CommandArguments` parameter. Other parameters may be provided as `Option`s in Discord. [Read more](https://discord.com/developers/docs/interactions/application-commands) about slash command options.
```cs
namespace MyDiscordBot.Commands
{
    public class Example
    {
        [CommandAttribute(new string[] { "slash-command", }, "Description of my slash command.)]
        public static void ExampleCommand(CommandArguments Args, double FirstParameter, string? SecondOptionalParameter)
        {
            Console.Out.WriteLine(Args.User.UserName + $" has fired this command with the following parameters:\n{FirstParameter}, {(SecondOptionalParameter ?? "[no second param]")}");
        }
    }
}
```
**Once the above is accomplished, call the `DiscordWrapper.StartReceiving()` method to allow the handler to begin processing your commands.**
