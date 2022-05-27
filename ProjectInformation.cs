using izolabella.Discord.Objects.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Discord
{
    /// <summary>
    /// A static class containing relevant information regarding this project.
    /// </summary>
    public static class ProjectInformation
    {
        /// <summary>
        /// The current version of this project, or null if not found.
        /// </summary>
        public static Version? Version => Assembly.GetAssembly(typeof(IIzolabellaCommand))?.GetName().Version;

        /// <summary>
        /// A string relevant to my preferences of being credited for usage on UI of any sort.
        /// </summary>
        public static string AuthorCreditDisplay => $"⊹⊱-☿ Mercury-Izolabella ☿-⊰⊹";

        /// <summary>
        /// A string relevant to my preferences of being credited for this project's usage in another project.
        /// </summary>
        public static string ProjectCreditDisplay => $"izolabella.Discord{(Version != null ? $" {Version.Major}.{Version.Minor}" : string.Empty)}";
    }
}
