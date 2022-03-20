using izolabella.Discord.Commands.Arguments;
using izolabella.Discord.Commands.Attributes;
using izolabella.Discord.Internals.Structures.Commands;
using izolabella.Discord.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Discord.Internals.Surgical
{
    /// <summary>
    /// A class containing information for ease of information passing.
    /// </summary>
    public class CommandWrapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandWrapper"/> class.
        /// </summary>
        /// <param name="Attribute">The attribute that applies to this command.</param>
        /// <param name="MethodInfo">The method to invoke for this command.</param>
        public CommandWrapper(CommandAttribute Attribute, MethodInfo MethodInfo)
        {
            this.Attribute = Attribute;
            this.MethodInfo = MethodInfo;
        }

        /// <summary>
        /// This command's attribute.
        /// </summary>
        public CommandAttribute Attribute { get; }

        /// <summary>
        /// The slash command compliant name for this command.
        /// </summary>
        public string SlashCommandTag => this.Attribute.Tags.First().ToLower().Replace(' ', '-');
        private MethodInfo MethodInfo { get; }

        /// <summary>
        /// Invokes the wrapped <see cref="MethodInfo"/> for this command.
        /// </summary>
        /// <param name="Context">The message that invoked this command.</param>
        public void InvokeThis(SocketSlashCommand Context)
        {
            List<object?> Params = new() { new CommandArguments(Context) };
            foreach(SocketSlashCommandDataOption Parameter in Context.Data.Options)
            {
                Params.Add(Parameter.Value);
            }
            int ParamCount = this.MethodInfo.GetParameters().Length;
            for(int I = 0; I < ParamCount - Params.Count; I++)
            {
                Params.Add(null);
            }
            this.MethodInfo.Invoke(this.Attribute, Params.ToArray());
        }

        /// <summary>
        /// Gets a list of <see cref="CommandParameter"/> objects for this command's parameter information.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyCollection<CommandParameter> GetCommandParameters()
        {
            List<CommandParameter> Params = new();
            foreach(ParameterInfo Param in this.MethodInfo.GetParameters())
            {
                ApplicationCommandOptionType? ParamType = null;
                Type UnderlyingOrRealType = Nullable.GetUnderlyingType(Param.ParameterType) ?? Param.ParameterType;
                if(UnderlyingOrRealType != null)
                {
                    if (UnderlyingOrRealType == typeof(bool))
                    {
                        ParamType = ApplicationCommandOptionType.Boolean;
                    }
                    else if (UnderlyingOrRealType == typeof(string))
                    {
                        ParamType = ApplicationCommandOptionType.String;
                    }
                    else if (UnderlyingOrRealType == typeof(int))
                    {
                        ParamType = ApplicationCommandOptionType.Integer;
                    }
                    else if (UnderlyingOrRealType == typeof(double))
                    {
                        ParamType = ApplicationCommandOptionType.Number;
                    }
                    else if (typeof(IMentionable).IsAssignableFrom(UnderlyingOrRealType))
                    {
                        ParamType = ApplicationCommandOptionType.Mentionable;
                    }
                    else if (typeof(IUser).IsAssignableFrom(UnderlyingOrRealType))
                    {
                        ParamType = ApplicationCommandOptionType.User;
                    }
                    else if (typeof(IRole).IsAssignableFrom(UnderlyingOrRealType))
                    {
                        ParamType = ApplicationCommandOptionType.Role;
                    }
                    else if (typeof(IGuildChannel).IsAssignableFrom(UnderlyingOrRealType))
                    {
                        ParamType = ApplicationCommandOptionType.Channel;
                    }
                    if (ParamType != null && Param.Name != null)
                    {
                        bool IsOptional = false;
                        if (Nullable.GetUnderlyingType(Param.ParameterType) == null || ParamType.Value == ApplicationCommandOptionType.String)
                        {
                            IsOptional = false;
                        }
                        else if (Nullable.GetUnderlyingType(Param.ParameterType) != null || Param.ParameterType.GetCustomAttributes().Any(Attribute =>
                        {
                            string? Name = Attribute.GetType()?.Name;
                            return Name != null && Name == "NullableAttribute";
                        }))
                        {
                            IsOptional = true;
                        }
                        //bool IsRequired = !(ParameterType.IsValueType || Nullable.GetUnderlyingType(ParameterType) == null);
                        //bool IsRequired = !(Nullable.GetUnderlyingType(ParameterType) != null || !ParameterType.IsValueType);
                        Params.Add(new(Param.Name, Param.Name, ParamType.Value, IsOptional));
                    }
                }
            }
            return Params;
        }
    }
}
