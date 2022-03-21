using izolabella.Discord.Internals.Surgical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Discord.Internals.Structures.Commands
{
    /// <summary>
    /// A class used to create commands on Discord from the <see cref="CommandWrapper"/> class.
    /// </summary>
    public class CommandCreator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandCreator"/> class.
        /// </summary>
        /// <param name="Instance"></param>
        public CommandCreator(DiscordSocketClient Instance)
        {
            this.Instance = Instance;
        }

        /// <summary>
        /// 
        /// </summary>
        public DiscordSocketClient Instance { get; }

        /// <summary>
        /// Creates commands.
        /// </summary>
        /// <param name="ToCreate">A <see cref="IReadOnlyCollection{CommandWrapper}"/> of <see cref="CommandWrapper"/> objects used to help 
        /// the method create the commands on Discord.</param>
        /// <returns></returns>
        private async Task CreateCommands(IReadOnlyCollection<CommandWrapper> ToCreate)
        {
            try
            {
                foreach (SocketGuild Guild in this.Instance.Guilds)
                {
                    foreach (CommandWrapper Command in ToCreate)
                    {
                        List<SlashCommandOptionBuilder> Options = new();
                        IReadOnlyCollection<CommandParameter> Parameters = Command.GetCommandParameters();
                        foreach (CommandParameter Param in Parameters)
                        {
                            Options.Add(new SlashCommandOptionBuilder()
                            {
                                Name = Param.Name,
                                Description = Param.Description,
                                IsRequired = Param.IsRequired,
                                Type = Param.ParameterType,
                            });
                        }
                        SlashCommandBuilder SocketCommand = new()
                        {
                            Name = Command.SlashCommandTag,
                            Description = Command.Attribute.Description,
                            IsDefaultPermission = true,
                            Options = Options.Count > 0 ? Options : null
                        };
                        await Guild.CreateApplicationCommandAsync(SocketCommand.Build());
                    }
                }
            }
            catch(Exception Ex)
            {
                Console.WriteLine(Ex);
            }
        }

        /// <summary>
        /// Delete or modify existing commands.
        /// </summary>
        /// <returns></returns>
        public async Task UpdateCommands(IReadOnlyCollection<CommandWrapper> CurrentCommands)
        {
            foreach (SocketGuild Guild in this.Instance.Guilds)
            {
                IReadOnlyCollection<SocketApplicationCommand> ExistingCommands = await Guild.GetApplicationCommandsAsync();
                foreach (CommandWrapper Command in CurrentCommands)
                {
                    SocketApplicationCommand? AlreadyExistingCommand = ExistingCommands.FirstOrDefault(ExistingCommand => ExistingCommand.Name == Command.SlashCommandTag);
                    if (AlreadyExistingCommand != null)
                    {
                        //bool NeedsUpdate = AlreadyExistingCommand.Name != Command.SlashCommandTag ||
                        //    AlreadyExistingCommand.Description != Command.Attribute.Description ||
                        //    AlreadyExistingCommand.Options.Count != Command.GetCommandParameters().Count;
                        //if (NeedsUpdate)
                        //{
                        //    await AlreadyExistingCommand.DeleteAsync();
                        //    await this.CreateCommands(new List<CommandWrapper>() { Command });
                        //}
                        await AlreadyExistingCommand.DeleteAsync();
                        await this.CreateCommands(new List<CommandWrapper>() { Command });
                    }
                    else
                    {
                        await this.CreateCommands(new List<CommandWrapper>() { Command });
                    }
                }
                if (ExistingCommands.Any(ExistingCommand => CurrentCommands.Any(CurrentCommand => ExistingCommand.Name != CurrentCommand.SlashCommandTag)))
                {
                    IEnumerable<SocketApplicationCommand> ForDeletion = ExistingCommands.Where(ExistingCommand => !CurrentCommands.Any(CurrentCommand => ExistingCommand.Name == CurrentCommand.SlashCommandTag));
                    foreach (SocketApplicationCommand ToDelete in ForDeletion)
                    {
                        await ToDelete.DeleteAsync();
                    }
                }
            }
        }
    }
}
