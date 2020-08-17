using Discord.Addons.Interactive;
using Discord.Commands;
using System.Threading.Tasks;
using System;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using Discord;
using System.Collections.Concurrent;
using Geometric.Handlers;
using System.Globalization;

namespace Geometric.Modules.Commands
{
    public class Interactive : InteractiveBase
    {
        // NextMessageAsync will wait for the next message to come in over the gateway, given certain criteria
        // By default, this will be limited to messages from the source user in the source channel
        // This method will block the gateway, so it should be ran in async mode.

        private static TimeSpan reactDuration = TimeSpan.FromSeconds(5);

        private static ReactionCallbackData GenerateColorRole(ColorRole colorRole)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle($"Color Role")
                .WithColor(new Color(colorRole.r, colorRole.g, colorRole.b))
                .WithDescription(
                    $"Do you like this color? HEX: {colorRole.HexColor.ToUpper()} / RGB: {colorRole.r}, {colorRole.g}, {colorRole.b}" + Environment.NewLine
                    + "React with your choice."
                    );

            var rcbd = new ReactionCallbackData("", embedBuilder.Build(), true, true, reactDuration, async c => await ReactionEndedAsync(c, colorRole).ConfigureAwait(false));
            foreach (IEmote answerEmoji in colorRole.Answers.Select(x => x.AnswerEmoji))
            {
                _ = rcbd.WithCallback(answerEmoji, (c, r) => ColorPicker(r, colorRole));
            }
            return rcbd;
        }

        private static Task ColorPicker(SocketReaction reaction, ColorRole colorRole)
        {
            ColorRoleAnswer answer = colorRole.Answers.SingleOrDefault(e => e.AnswerEmoji.Equals(reaction.Emote));
            if (reaction.UserId == colorRole.UserId)
            {
                var guild = (reaction.Channel as SocketGuildChannel).Guild as SocketGuild;
                var chan = guild.GetChannel(colorRole.ChannelId) as SocketTextChannel;
                var _user = guild.GetUser(colorRole.UserId);
                var botRole = guild.Roles.FirstOrDefault(e => e.Name == "Geometric");

                if (answer.Answer == "Yes")
                {
                    var _role = guild.Roles.FirstOrDefault(x => x.Name == $"USER-{colorRole.UserId}") as IRole;

                    if (_role != null)
                    {
                        var role = _role;

                        role.ModifyAsync(r => r.Color = new Color(colorRole.r, colorRole.g, colorRole.b));
                        role.ModifyAsync(r => r.Position = botRole.Position);

                        _user.AddRoleAsync(role as IRole);

                        chan.SendMessageAsync($"{_user.Mention} has been given role {role.Name}");
                    }
                    else
                    {
                        var role = guild.CreateRoleAsync($"USER-{colorRole.UserId}", null, new Color(colorRole.r, colorRole.g, colorRole.b), false, false, null);

                        role.Result.ModifyAsync(r => r.Position = botRole.Position);

                        _user.AddRoleAsync(role.Result);

                        chan.SendMessageAsync($"{_user.Mention} has been given role {role.Result.Name}");
                    }
                }
                else if (answer.Answer == "No")
                {
                    chan.SendMessageAsync($"{_user.Mention} cancelled");
                }

                if (answer != null && reaction.User.IsSpecified)
                {
                    _ = colorRole.ReactionUsers.AddOrUpdate(
                            answer,
                            new List<IUser> { reaction.User.Value },
                            (_, list) =>
                            {
                                list.Add(reaction.User.Value);
                                return list;
                            }
                    );
                }
            }
            return Task.CompletedTask;
        }

        private static async Task ReactionEndedAsync(SocketCommandContext context, ColorRole colorRole)
        {
            if (colorRole == null)
            {
                return;
            }
            if (colorRole.ReactionUsers.IsEmpty)
            {
                _ = await context.Channel.SendMessageAsync("took too long").ConfigureAwait(false);
            }
        }

        [Command("colorrole")]
        [Summary("Creates a Custom Color Role for a User.")]
        [RequireBotPermission(ChannelPermission.AddReactions | ChannelPermission.ManageMessages | ChannelPermission.ManageRoles)]
        public async Task ColorRoleAsync([Summary("Hex Color")] string hexString = null)
        {
            if (hexString == null)
            {
                _ = await ReplyAsync("Provide a color, ex `ffffff`").ConfigureAwait(false);
                return;
            }

            int rVal, gVal, bVal = 0;

            rVal = int.Parse(hexString.Substring(0, 2), NumberStyles.AllowHexSpecifier);
            gVal = int.Parse(hexString.Substring(2, 2), NumberStyles.AllowHexSpecifier);
            bVal = int.Parse(hexString.Substring(4, 2), NumberStyles.AllowHexSpecifier);

            var emoteBank = Context.Client.GetGuild(690373608228519997);

            var colorRole = new ColorRole
            {
                GuildId = Context.Guild.Id,
                ChannelId = Context.Channel.Id,
                UserId = Context.User.Id,
                MessageId = Context.Message.Id,
                HexColor = hexString,
                r = rVal,
                g = gVal,
                b = bVal,
                ColorObj = new Color(rVal, gVal, bVal),
                Answers = new List<ColorRoleAnswer>
                {
                    new ColorRoleAnswer("Yes", emoteBank.Emotes.First(e => e.Name == "yes")), //742446277916360724
                    new ColorRoleAnswer("No", emoteBank.Emotes.First(e => e.Name == "no")), //742446286245986364
                }
            };
            _ = await InlineReactionReplyAsync(GenerateColorRole(colorRole), false).ConfigureAwait(false);
        }

        [Command("removecolor")]
        [Summary("Removes the user's Custom Color role.")]
        [RequireBotPermission(ChannelPermission.ManageRoles)]
        public async Task RemoveColorAsync()
        {
            IRole _role = Context.Guild.Roles.FirstOrDefault(x => x.Name == $"USER-{Context.User.Id}");

            if (_role != null)
            {
                await ReplyAsync($"Removed your color role. Use `g.colorrole` `{_role.Color.R.ToString("X2") + _role.Color.G.ToString("X2") + _role.Color.B.ToString("X2")}` to get your color back.");

                await _role.DeleteAsync();
            }
            else
            {
                await ReplyAsync($"You don't have a color role. Use `g.colorrole` `ffffff` to get one.");
            }
        }

        [Command("currentcolor")]
        [Summary("Gets the user's Custom Color role color.")]
        [RequireBotPermission(ChannelPermission.ManageRoles)]
        public async Task CurrentColorAsync(IGuildUser user = null)
        {
            if (user == null)
            {
                user = Context.User as IGuildUser;
                IRole _role = Context.Guild.Roles.FirstOrDefault(x => x.Name == $"USER-{user.Id}");

                if (_role != null)
                {
                    await ReplyAsync($"{user.Mention}, Your current color is: HEX: {_role.Color.R.ToString("X2") + _role.Color.G.ToString("X2") + _role.Color.B.ToString("X2")} / RGB: {_role.Color.R}, {_role.Color.G}, {_role.Color.B}");
                }
                else
                {
                    await ReplyAsync($"You don't have a color role. Use `g.colorrole` `ffffff` to get one.");
                }
            }
            else
            {
                IRole _role = Context.Guild.Roles.FirstOrDefault(x => x.Name == $"USER-{user.Id}");

                if (_role != null)
                {
                    await ReplyAsync($"{user.Mention}'s current color is: HEX: {_role.Color.R.ToString("X2") + _role.Color.G.ToString("X2") + _role.Color.B.ToString("X2")} / RGB: {_role.Color.R}, {_role.Color.G}, {_role.Color.B}");
                }
                else
                {
                    await ReplyAsync($"They do not have a color role. Tell them to use `g.colorrole` `ffffff` to get one.");
                }
            }
        }

        [Command("math", RunMode = RunMode.Async)]
        public async Task TryNextMessageAsync()
        {
            await ReplyAsync("What is 2 + 2?");
            var response = await NextMessageAsync();
            if (response != null)
            {
                if (response.ToString() == "4")
                {
                    await ReplyAsync($"You are a genius.");
                }
                else if (response.ToString().ToLower() == "fish")
                {
                    await ReplyAsync($":fish:");
                }
                else
                {
                    await ReplyAsync($"It's simple math, 2 + 2 doesn't equal {response}");
                }
            }
            else
            {
                await ReplyAsync("You can't do math that fast?... (timeout)");
            }
        }

        [Command("paginator")] // This is a placeholder page command for later.
        [RequireOwner]
        public async Task Test_Paginator()
        {
            var pages = new PaginatedMessage();
            var page1 = new PaginatedMessage.Page();
            List<PaginatedMessage.Page> pageList = new List<PaginatedMessage.Page>();

            var reactions = new ReactionList();

            var emoteBank = Context.Client.GetGuild(397846250797662208);

            reactions.Forward = true;

            pageList.Add(page1);

            pages.Title = "test";
            pages.Pages = pageList;
            page1.Description = "1";

            await PagedReplyAsync(pages, reactions);
        }
    }
}
