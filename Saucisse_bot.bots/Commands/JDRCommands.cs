using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace Saucisse_bot.Bots.Commands
{
    [Group("jdr")]
    [Description("These commands revolve around 'JDR' commands")]
    public class JDRCommands : BaseCommandModule
    {

        [Command("deafen")]
        [Aliases("dfn")]
        [Description("Deafens the users given as parameters. \r\nUse the commands with no parameters to invert the voice status of everyone except the member using the command.")]
        [RequireRoles(RoleCheckMode.Any, "Owner", "Admin", "Aventuriers intrépides (+ le MJ)")]
        public async Task DeafenMember(CommandContext ctx, params DiscordMember[] members)
        {
            var channel = ctx.Member.VoiceState.Channel;
            if (channel != null)
            {
                if (members.Length > 0)
                {
                    foreach (var m in members)
                    {
                        if (m.VoiceState.Channel == channel)
                        {
                            await m.SetDeafAsync(!m.VoiceState.IsServerDeafened);
                            await m.SetMuteAsync(!m.VoiceState.IsServerMuted);
                        }
                    }
                }
                else
                {
                    var membersInChan = channel.Users;
                    foreach (var m in membersInChan)
                    {
                        if (m.VoiceState.Channel == channel && m.Id != ctx.Member.Id)
                        {
                            await m.SetDeafAsync(!m.VoiceState.IsServerDeafened);
                            await m.SetMuteAsync(!m.VoiceState.IsServerMuted);
                        }
                    }
                } 
            }
        }
    }
}
