using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Discord.Objects.Parameters
{
    /// <summary>
    /// 
    /// </summary>
    public class IzolabellaCommandArgument : IzolabellaCommandParameter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Description"></param>
        /// <param name="OptionType"></param>
        /// <param name="IsRequired"></param>
        /// <param name="Value"></param>
        public IzolabellaCommandArgument(string Name, string Description, ApplicationCommandOptionType OptionType, bool IsRequired, object? Value) : base(Name, Description, OptionType, IsRequired)
        {
            this.Value = Value;
        }

        /// <summary>
        /// 
        /// </summary>
        public object? Value { get; }
    }
}
