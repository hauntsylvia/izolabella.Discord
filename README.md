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

To mark a method as a command, the `CommandAttribute` attribute should be attached to a method with a `CommandArguments` parameter.
```cs
namespace MyDiscordBot.Commands
{
    public class Example
    {
        [CommandAttribute(new string[] { "Tag1", "Tag2" })]
        public static void ExampleCommand(CommandArguments Args)
        {
            Console.Out.WriteLine(Args.Message.Author.Username + " has fired this command!");
        }
    }
}

```

## This is no longer valid as of the 2.0.0 version.
~~Messages must be validated manually for now, using the following code (or any code you want/need for command validation*).~~
```cs
DiscordWrapper.CommandHandler.CommandNeedsValidation += (SocketMessage Message, CommandAttribute Attr) =>
            {
                return Message.MentionedUsers.Any(User => User.Id == DiscordClient.CurrentUser.Id) && Attr.Tags.Any(Tag => Message.Content.ToLower().Contains(Tag.ToLower()));
            };
```
**Once the above is accomplished, call the method `DiscordWrapper.StartReceiving()` to allow the handler to begin processing messages.**
