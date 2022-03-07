using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Discord.Commands.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        public CommandAttribute(string[] Tags)
        {
            this.Tags = Tags;
        }


        public CommandAttribute(string[] Tags, ulong[]? Whitelist, ulong[] Blacklist)
        {
            this.Tags = Tags;
            this.Whitelist = Whitelist;
            this.Blacklist = Blacklist;
        }
        

        public CommandAttribute(string[] Tags, ulong[] Whitelist)
        {
            this.Tags = Tags;
            this.Whitelist = Whitelist;
        }


        public string[] Tags { get; }
        public ulong[]? Whitelist { get; }
        public ulong[]? Blacklist { get; }
    }
}
