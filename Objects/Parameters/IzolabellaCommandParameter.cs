namespace izolabella.Discord.Objects.Parameters;

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
    /// <param name="ChannelTypes"></param>
    /// <param name="IsRequired"></param>
    public IzolabellaCommandParameter(string Name, string Description, ApplicationCommandOptionType OptionType, List<ChannelType> ChannelTypes, bool IsRequired)
    {
        this.Name = Name;
        this.Description = Description;
        this.OptionType = OptionType;
        this.ChannelTypes = ChannelTypes;
        this.IsRequired = IsRequired;
    }

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
        this.ChannelTypes = new();
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
    /// The channel types this parameter is allowed to select, when <see cref="OptionType"/> is <see cref="ApplicationCommandOptionType.Channel"/>
    /// </summary>
    public List<ChannelType> ChannelTypes { get; }

    /// <summary>
    /// Whether this parameter is required or not.
    /// </summary>
    public bool IsRequired { get; }

    /// <summary>
    /// All choices available for this parameter.
    /// </summary>
    public List<IzolabellaCommandParameterChoices>? Choices { get; set; }

    /// <summary>
    /// Maximum value.
    /// </summary>
    public double? MaxValue { get; set; }

    /// <summary>
    /// Minimum value.
    /// </summary>
    public double? MinimumValue { get; set; }
}
