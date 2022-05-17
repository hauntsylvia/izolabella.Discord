namespace izolabella.Discord.Internals.Structures.Commands
{
    /// <summary>
    /// A Discord parameter class.
    /// </summary>
    public class CommandParameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandParameter"/> class.
        /// </summary>
        /// <param name="Name">Name of the parameter.</param>
        /// <param name="Description">Description of the parameter.</param>
        /// <param name="Choices">The choices of the parameter.</param>
        /// <param name="ParameterType">The type of parameter.</param>
        /// <param name="IsRequired">Determines whether this parameter is required or optional.</param>
        public CommandParameter(string Name, string Description, string[]? Choices, ApplicationCommandOptionType ParameterType, bool IsRequired)
        {
            this.name = Name;
            this.Description = Description;
            this.choices = Choices;
            this.ParameterType = ParameterType;
            this.IsRequired = ParameterType == ApplicationCommandOptionType.Boolean || IsRequired;
        }

        private readonly string name;
        /// <summary>
        /// The name of the parameter.
        /// </summary>
        public string Name => this.name.ToLower();

        /// <summary>
        /// The description of the parameter.
        /// </summary>
        public string Description { get; }

        private readonly string[]? choices;
        /// <summary>
        /// The choices for this parameter.
        /// </summary>
        public List<ApplicationCommandOptionChoiceProperties> Choices
        {
            get
            {
                List<ApplicationCommandOptionChoiceProperties> List = new();
                if (this.choices != null)
                {
                    foreach (string C in this.choices)
                    {
                        List.Add(new()
                        {
                            Name = C,
                            Value = C
                        });
                    }
                }
                return List;
            }
        }

        /// <summary>
        /// The type of parameter.
        /// </summary>
        public ApplicationCommandOptionType ParameterType { get; }

        /// <summary>
        /// Whether this parameter is required or not.
        /// </summary>
        public bool IsRequired { get; }
    }
}
