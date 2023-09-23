namespace izolabella.Discord.Objects.Parameters
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <param name="Name"></param>
    /// <param name="Description"></param>
    /// <param name="OptionType"></param>
    /// <param name="IsRequired"></param>
    /// <param name="Value"></param>
    public class IzolabellaCommandArgument(string Name, string Description, ApplicationCommandOptionType OptionType, bool IsRequired, object? Value) : IzolabellaCommandParameter(Name, Description, OptionType, IsRequired)
    {

        /// <summary>
        /// 
        /// </summary>
        public object? Value { get; } = Value;
    }
}