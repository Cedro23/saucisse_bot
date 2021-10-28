using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saucisse_bot.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class RequireCategoriesAttribute : CheckBaseAttribute
    {
        public IReadOnlyList<string> CategoryNames { get; }
        public ChannelCheckMode CheckMode { get; }

        public RequireCategoriesAttribute(ChannelCheckMode _checkMode, params string[] _channelNames)
        {
            CheckMode = _checkMode;
            CategoryNames = new ReadOnlyCollection<string>(_channelNames);
        }

        public override Task<bool> ExecuteCheckAsync(CommandContext _ctx, bool _help)
        {
            if (_ctx.Guild == null || _ctx.Member == null)
            {
                return Task.FromResult(false);
            }

            bool contains = CategoryNames.Contains(_ctx.Channel.Parent.Name, StringComparer.OrdinalIgnoreCase);

            return CheckMode switch
            {
                ChannelCheckMode.Any => Task.FromResult(contains),

                ChannelCheckMode.None => Task.FromResult(!contains),

                _ => Task.FromResult(false),
            };
        }

    }
}
