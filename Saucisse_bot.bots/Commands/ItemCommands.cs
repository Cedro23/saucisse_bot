using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Saucisse_bot.Bots.Handlers.Dialogue;
using Saucisse_bot.Bots.Handlers.Dialogue.Steps;
using Saucisse_bot.Core.Services.Items;
using Saucisse_bot.DAL.Models.Items;
using System.Threading.Tasks;

namespace Saucisse_bot.Bots.Commands
{
    [Group("item")]
    public class ItemCommands : BaseCommandModule
    {
        private readonly IItemService _itemService;

        public ItemCommands(IItemService itemService)
        {
            _itemService = itemService;
        }

        #region User commands
        [Command("list")]
        [Description("Returns the list of existing items for the current guild")]
        public async Task ItemList(CommandContext ctx)
        {
            var items = await _itemService.GetItemList(ctx.Guild.Id).ConfigureAwait(false);
            var embed = new DiscordEmbedBuilder();

            if (items.Count > 0)
            {
                embed = new DiscordEmbedBuilder()
                {
                    Title = "Item list :",
                    Color = DiscordColor.Yellow
                };

                foreach (var item in items)
                {
                    embed.AddField($"{item.Name} [{item.Price}]", $"{item.Description}");
                }
            }
            else
            {
                embed = new DiscordEmbedBuilder()
                {
                    Title = "List of items :",
                    Description = "There currently are no items on this guild",
                    Color = DiscordColor.Red
                };
            }

            await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
        }

        [Command("info")]
        [Description("Return the informations of an item")]
        public async Task ItemInfo(CommandContext ctx, params string[] itemNameSplit)
        {
            string itemName = string.Join(' ', itemNameSplit);

            var item = await _itemService.GetItemByNameAsync(ctx.Guild.Id, itemName).ConfigureAwait(false);
            var embed = new DiscordEmbedBuilder();

            if (item == null)
            {
                embed = new DiscordEmbedBuilder()
                {
                    Title = "404 NOT FOUND",
                    Color = DiscordColor.Red
                };
                embed.AddField(itemName, "This item could not be found... Are you sure it's the correct name ?");

            }
            else
            {
                DiscordEmbedBuilder.EmbedThumbnail thumbnail = new DiscordEmbedBuilder.EmbedThumbnail();
                thumbnail.Url = item.ImageUrl;
                embed = new DiscordEmbedBuilder()
                {
                    Title = item.Name,
                    Thumbnail = thumbnail,
                    Color = DiscordColor.Green
                };
                embed.AddField("Description", item.Description);
                embed.AddField("Price", $"{item.Price} golds");
            }

            await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
        }

        [Command("buy")]
        [Description("Adds the item to the user's inventory if he has enough golds.")]
        public async Task Buy(CommandContext ctx, params string[] itemNameSplit)
        {
            string itemName = string.Join(' ', itemNameSplit);
            var embed = new DiscordEmbedBuilder();

            var result = await _itemService.PurchaseItemAsync(ctx.Member.Id, ctx.Guild.Id, itemName);

            if (!result.IsOk)
            {
                embed = new DiscordEmbedBuilder()
                {
                    Title = "Failure!",
                    Color = DiscordColor.Red
                };
                embed.AddField(itemName, result.ErrMsg);

            }
            else
            {
                embed = new DiscordEmbedBuilder()
                {
                    Title = "Succes!",
                    Description = $"You correctly purchased {result.Item.Name}",
                    Color = DiscordColor.Green
                };
            }

            await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
        } 
        #endregion

        #region Admin commands
        [Command("create")]
        [Hidden]
        [RequireRoles(RoleCheckMode.Any, "Owner", "Admin")]
        public async Task CreateItem(CommandContext ctx)
        {
            var itemImageUrlStep = new StringStep("Please enter the image URL for this item?", null);
            var itemPriceStep = new IntStep("How much does the item cost?", itemImageUrlStep, 1);
            var itemDescriptionStep = new StringStep("What is the item about?", itemPriceStep);
            var itemNameStep = new StringStep("What will the item be called?", itemDescriptionStep);

            var item = new Item();
            item.GuildId = ctx.Guild.Id;

            itemNameStep.OnValidResult += (result) => item.Name = result;
            itemDescriptionStep.OnValidResult += (result) => item.Description = result;
            itemPriceStep.OnValidResult += (result) => item.Price = result;
            itemImageUrlStep.OnValidResult += (result) => item.ImageUrl = result;

            var inputDialogueHandler = new DialogueHandler(
                ctx.Client,
                ctx.Channel,
                ctx.User,
                itemNameStep
            );

            bool succeeded = await inputDialogueHandler.ProcessDialogue().ConfigureAwait(false);

            if (!succeeded) { return; }

            await _itemService.CreateNewItemAsync(item).ConfigureAwait(false);

            await ctx.Channel.SendMessageAsync($"Item {item.Name} succesfully created with Id: {item.Id}").ConfigureAwait(false);
        }

        [Command("delete")]
        [Hidden]
        [RequireRoles(RoleCheckMode.Any, "Owner", "Admin")]
        public async Task DeleteItem(CommandContext ctx, params string[] itemNameSplit)
        {
            bool isOk = false;
            string itemName = string.Join(' ', itemNameSplit);
            var item = await _itemService.GetItemByNameAsync(ctx.Guild.Id, itemName).ConfigureAwait(false);
            var embed = new DiscordEmbedBuilder();

            if (item != null)
            {
                isOk = await _itemService.DeleteItemAsync(item).ConfigureAwait(false);
            }

            if (isOk)
            {
                embed = new DiscordEmbedBuilder()
                {
                    Title = "Succes!",
                    Description = "The item was correctly deleted.",
                    Color = DiscordColor.Green
                };
            }
            else
            {
                embed = new DiscordEmbedBuilder()
                {
                    Title = "Failure!",
                    Description = "Failed to delete the item. Maybe it doesn't exist.",
                    Color = DiscordColor.Red
                };
            }

            await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false); 
        }
        #endregion
    }
}
