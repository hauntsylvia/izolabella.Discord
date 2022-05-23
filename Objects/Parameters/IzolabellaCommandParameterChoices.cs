using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Discord.Objects.Parameters
{
    /// <summary>
    /// A class for presenting choices of a parameter.
    /// </summary>
    public class IzolabellaCommandParameterChoices
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IzolabellaCommandParameterChoices"/> class.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        /// <remarks>
        /// <paramref name="Value"/> must be of type 
        /// <see cref="string"/>, 
        /// <see cref="double"/>, 
        /// <see cref="float"/>, 
        /// <see cref="int"/>, or 
        /// <see cref="bool"/>.
        /// </remarks>
        public IzolabellaCommandParameterChoices(string Name, object Value)
        {
            this.Name = Name;
            this.Value = Value is string || Value is double || Value is float || Value is int || Value is bool
                ? Value
                : throw new ArgumentException("", paramName: nameof(Value));
        }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Value
        /// </summary>
        public object Value { get; }
    }
}
