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
    public class IzolabellaCommandParameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IzolabellaCommandParameter"/> class.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Description"></param>
        /// <param name="OptionType"></param>
        /// <param name="IsRequired"></param>
        public IzolabellaCommandParameter(string Name, string Description, ApplicationCommandOptionType OptionType, bool IsRequired)
        {
            this.Name = Name;
            this.Description = Description;
            this.OptionType = OptionType;
            this.IsRequired = IsRequired;
        }

        /// <summary>
        /// The name of this parameter.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The description of this parameter.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The type of this parameter.
        /// </summary>
        public ApplicationCommandOptionType OptionType { get; }

        /// <summary>
        /// Whether this parameter is required or not.
        /// </summary>
        public bool IsRequired { get; }

        /// <summary>
        /// 
        /// </summary>
        public List<IzolabellaCommandParameterChoices>? Choices { get; set; }
    }
}
