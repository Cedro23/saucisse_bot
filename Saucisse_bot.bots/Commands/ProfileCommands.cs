using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Saucisse_bot.Core.Services.Profiles;
using Saucisse_bot.DAL.Models.Profiles;
using System.Linq;
using System.Threading.Tasks;

namespace Saucisse_bot.Bots.Commands
{
    [Group("profile")]
    [Description("These commands revolve around users profiles")]
    public class ProfileCommands : BaseCommandModule
    {
        private IProfileService _profileService;

        public ProfileCommands(IProfileService profileService)
        {
            _profileService = profileService;
        }

        #region User commands
        [Command("create")]
        [Cooldown(1, 30, CooldownBucketType.User)]
        [Description("Creates a profile for the user who used the command")]
        public async Task CreateProfile(CommandContext ctx)
        {
            Profile profile = await _profileService.GetProfileAsync(ctx.Guild.Id, ctx.Member.Id).ConfigureAwait(false);
            DiscordEmbedBuilder profileEmbed;

            if (profile != null)
            {
                profileEmbed = new DiscordEmbedBuilder
                {
                    Title = "Your profile already exists",
                    Description = $"{ctx.Member.Mention}, your profile already exists, please use \"!profile show\" to display it",
                    Color = DiscordColor.Yellow
                };
            }
            else
            {
                bool isCreated = await _profileService.CreateProfileAsync(ctx.Guild.Id, ctx.Member.Id);
                if (isCreated)
                {
                    profileEmbed = new DiscordEmbedBuilder
                    {
                        Title = "Your profile has been created",
                        Description = $"{ctx.Member.Mention}, your profile has correctly been created",
                        Color = DiscordColor.Green
                    };
                }
                else
                {
                    profileEmbed = new DiscordEmbedBuilder
                    {
                        Title = "There was a problem creating your profile",
                        Description = $"{ctx.Member.Mention}, your profile was not created, please try using the command again, or call for an admin",
                        Color = DiscordColor.Red
                    };
                }
            }
            await ctx.Channel.SendMessageAsync(embed: profileEmbed).ConfigureAwait(false);
        }

        #region Show profile
        [Command("show")]
        [Cooldown(1, 30, CooldownBucketType.User)]
        [Description("Returns the server based profile of the user who used the command")]
        public async Task Profile(CommandContext ctx)
        {
            await GetProfileToDisplayAsync(ctx, ctx.Member.Id);
        }

        [Command("show")]
        [Cooldown(1, 30, CooldownBucketType.User)]
        [Description("Returns the server based profile of the mentionned user")]
        public async Task Profile(CommandContext ctx, DiscordMember member)
        {
            await GetProfileToDisplayAsync(ctx, member.Id);
        }

        private async Task GetProfileToDisplayAsync(CommandContext ctx, ulong memberId)
        {
            Profile profile = await _profileService.GetProfileAsync(ctx.Guild.Id, memberId).ConfigureAwait(false);
            DiscordEmbedBuilder profileEmbed;

            if (profile != null)
            {
                DiscordMember member = await ctx.Guild.GetMemberAsync(profile.DiscordId);

                DiscordEmbedBuilder.EmbedThumbnail thumbnail = new DiscordEmbedBuilder.EmbedThumbnail();
                thumbnail.Url = member.AvatarUrl;

                profileEmbed = new DiscordEmbedBuilder
                {
                    Title = $"{member.DisplayName}'s Profile",
                    Thumbnail = thumbnail
                };

                profileEmbed.AddField("Level", profile.Level.ToString());
                profileEmbed.AddField("Xp", profile.Xp.ToString());
                profileEmbed.AddField("Gold", profile.Gold.ToString());
                if (profile.Items.Count > 0)
                {
                    profileEmbed.AddField("Items", string.Join(", ", profile.Items.Select(x => x.Item.Name)));
                }
            }
            else
            {
                profileEmbed = new DiscordEmbedBuilder
                {
                    Title = $"404 PROFILE NOT FOUND",
                    Description = "If you want to create your profile, please use the command \"!profile create\"",
                    Color = DiscordColor.Red
                };
            }

            await ctx.Channel.SendMessageAsync(embed: profileEmbed).ConfigureAwait(false);
        }
        #endregion

        [Command("list")]
        [Cooldown(1, 30, CooldownBucketType.User)]
        [Description("Returns a list of all profiles existing on this server")]
        public async Task ProfileList(CommandContext ctx)
        {
            var profiles = await _profileService.GetProfileListAsync(ctx.Guild.Id).ConfigureAwait(false);
            var embed = new DiscordEmbedBuilder()
            {
                Title = "List of profiles :",
                Color = DiscordColor.Orange
            };

            foreach (var profile in profiles)
            {
                var member = await ctx.Guild.GetMemberAsync(profile.DiscordId).ConfigureAwait(false);
                embed.AddField($"{member.DisplayName}", $"{profile.Gold}golds");
            }

            await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
        }
        #endregion

        #region Admin commands
        [Command("create")]
        [Hidden]
        [Description("Creates a profile for the specified user")]
        [RequireOwner]
        public async Task CreateProfile(CommandContext ctx, DiscordMember member)
        {
            Profile profile = await _profileService.GetProfileAsync(ctx.Guild.Id, member.Id).ConfigureAwait(false);
            DiscordEmbedBuilder profileEmbed;

            if (profile != null)
            {
                profileEmbed = new DiscordEmbedBuilder
                {
                    Title = "Your profile already exists",
                    Description = $"{member.Mention}'s profile already exists, please use \"!profile show {member.Mention}\" to display it",
                    Color = DiscordColor.Yellow
                };
            }
            else
            {
                bool isCreated = await _profileService.CreateProfileAsync(ctx.Guild.Id, member.Id);
                if (isCreated)
                {
                    profileEmbed = new DiscordEmbedBuilder
                    {
                        Title = "Your profile has been created",
                        Description = $"{member.Mention}'s profile has correctly been created",
                        Color = DiscordColor.Green
                    };
                }
                else
                {
                    profileEmbed = new DiscordEmbedBuilder
                    {
                        Title = "There was a problem creating your profile",
                        Description = $"{member.Mention}'s profile was not created",
                        Color = DiscordColor.Red
                    };
                }
            }
            await ctx.Channel.SendMessageAsync(embed: profileEmbed).ConfigureAwait(false);
        }

        [Command("reset")]
        [Hidden]
        [Description("Resets a profile to 0 XP and 100 golds")]
        [RequireOwner]
        public async Task ResetProfile(CommandContext ctx, DiscordMember member)
        {
            var result = await _profileService.ResetProfileAsync(ctx.Guild.Id, member.Id).ConfigureAwait(false);
            var embed = new DiscordEmbedBuilder()
            {
                Title = "Profile reset"
            };

            if (result.IsOk)
            {
                embed.Description = $"The reset of {member.Mention}'s profile was successful!";
                embed.Color = DiscordColor.Green;
            }
            else
            {
                embed.Description = $"The reset of {member.Mention}'s profile failed!";
                embed.Color = DiscordColor.Red;
                embed.AddField("Error", result.ErrMsg);
            }

            await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
        }

        [Command("resetall")]
        [Hidden]
        [Description("Resets a profile to 0 XP and 100 golds")]
        [RequireOwner]
        public async Task ResetAllProfiles(CommandContext ctx)
        {
            var result = await _profileService.ResetAllProfilesAsync(ctx.Guild.Id).ConfigureAwait(false);
            var embed = new DiscordEmbedBuilder()
            {
                Title = "Global profile reset"
            };

            if (result.IsOk)
            {
                embed.Description = $"The reset of all profiles was successful!";
                embed.Color = DiscordColor.Green;
            }
            else
            {
                embed.Description = $"The reset of all profiles failed!";
                embed.Color = DiscordColor.Red;
                embed.AddField("Error", result.ErrMsg);
            }

            await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
        }

        [Command("delete")]
        [Hidden]
        [Description("Deletes a profile")]
        [RequireOwner]
        public async Task DeleteProfile(CommandContext ctx, DiscordMember member)
        {
            var result = await _profileService.DeleteProfileAsync(ctx.Guild.Id, member.Id).ConfigureAwait(false);
            var embed = new DiscordEmbedBuilder()
            {
                Title = "Profile deletion"
            };

            if (result.IsOk)
            {
                embed.Description = $"{member.Mention}'s profile was successfuly deleted!";
                embed.Color = DiscordColor.Green;
            }
            else
            {
                embed.Description = $"{member.Mention}'s profile deletion failed!";
                embed.Color = DiscordColor.Red;
                embed.AddField("Error", result.ErrMsg);
            }

            await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
        }

        [Command("deleteall")]
        [Hidden]
        [Description("Deletes a profile")]
        [RequireOwner]
        public async Task DeleteAllProfiles(CommandContext ctx)
        {
            var result = await _profileService.DeleteAllProfilesAsync(ctx.Guild.Id).ConfigureAwait(false);
            var embed = new DiscordEmbedBuilder()
            {
                Title = "Global profile deletion"
            };

            if (result.IsOk)
            {
                embed.Description = $"All profiles were successfuly deleted!";
                embed.Color = DiscordColor.Green;
            }
            else
            {
                embed.Description = $"The deletion of all profiles failed!";
                embed.Color = DiscordColor.Red;
                embed.AddField("Error", result.ErrMsg);
            }

            await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
        }

        [Command("avatar")]
        [Hidden]
        [Description("Displays the avatar of the given user")]
        [RequireOwner]
        public async Task DisplayAvatar(CommandContext ctx, DiscordMember member)
        {
            var user = await ctx.Guild.GetMemberAsync(member.Id).ConfigureAwait(false);

            if (user != null)
                await ctx.Channel.SendMessageAsync(user.AvatarUrl).ConfigureAwait(false);
            else
                await ctx.Channel.SendMessageAsync("This user could not be found").ConfigureAwait(false);
        }
        #endregion
    }
}
