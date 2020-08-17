using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Geometric;
using Newtonsoft.Json;

namespace GeometricDiscordBot.Modules.Commands
{
    public class Info : ModuleBase
    {
        private CommandService _service;

        public Info(CommandService service)
        {
            _service = service;
        }

        [Command("help")]
        [Summary("Shows a list of all available commands to the user.")]
        public async Task HelpAsync()
        {
            List<string> thePrefix = new List<string> { "g." };

            var dmChannel = await Context.User.GetOrCreateDMChannelAsync(); /* A channel is created so that the commands will be privately sent to the user, and not flood the chat. */

            var serverId = Context.Guild.Id;
            string prefixPath = @"C:\Users\Ryan\Desktop\BotDatabase\Prefix\" + serverId + ".txt";

            if (File.Exists(prefixPath))
            {
                thePrefix = File.ReadAllLines(prefixPath).ToList();
            }

            string prefix = $"{thePrefix[0]}";  /* put your chosen prefix here */

            var builder = new EmbedBuilder()
            {
                Color = new Color(BotConfig.EmbedColor.R, BotConfig.EmbedColor.G, BotConfig.EmbedColor.B),
                Description = "Commands"
            };

            foreach (var module in _service.Modules) /* we are now going to loop through the modules taken from the service we initiated earlier ! */
            {
                string description = null;
                foreach (var cmd in module.Commands) /* and now we loop through all the commands per module aswell, oh my! */
                {
                    var result = await cmd.CheckPreconditionsAsync(Context); /* gotta check if they pass */
                    if (result.IsSuccess)
                        description += $"{prefix}{cmd.Aliases.First()}\n"; /* if they DO pass, we ADD that command's first alias (aka it's actual name) to the description tag of this embed */
                }

                if (!string.IsNullOrWhiteSpace(description)) /* if the module wasn't empty, we go and add a field where we drop all the data into! */
                {
                    builder.AddField(x =>
                    {
                        x.Name = module.Name;
                        x.Value = description;
                        x.IsInline = true;
                    });
                }
            }

            await dmChannel.SendMessageAsync("", false, builder.Build()); /* then we send it to the user. */

            await ReplyAsync($"{Context.User.Mention}, check your DMs. <a:verified:690374136526012506>");
        }

        [Command("commandhelp")]
        [Alias("chelp")]
        [Summary("Provides help with a command with its aliases, parameters, and summary.")]
        public async Task CmdHelpAsync(string command)
        {
            var result = _service.Search(Context, command);

            List<string> thePrefix = new List<string> { "g." };
            var serverId = Context.Guild.Id;
            string prefixPath = @"C:\Users\Ryan\Desktop\BotDatabase\Prefix\" + serverId + ".txt";
            string para = "";

            if (File.Exists(prefixPath))
            {
                thePrefix = File.ReadAllLines(prefixPath).ToList();

            }
            string prefix = thePrefix[0];

            if (!result.IsSuccess)
            {
                await ReplyAsync($"There is no command: {prefix}{command}");
                return;
            }
            
            var builder = new EmbedBuilder()
            {
                Color = new Color(BotConfig.EmbedColor.R, BotConfig.EmbedColor.G, BotConfig.EmbedColor.B),
                Title = $"Command Help: {prefix}{command}"
            };

            foreach (var match in result.Commands)
            {
                var cmd = match.Command;
                var checkPreCons = await cmd.CheckPreconditionsAsync(Context); /* gotta check if they pass */

                if (!checkPreCons.IsSuccess)
                {
                    await ReplyAsync($"There is no command: {prefix}{command}");
                    return;
                }

                if (cmd.Parameters.Count == 0)
                {
                    para = "";
                }
                else
                {
                    para = $"__Parameters:__ {string.Join(", ", cmd.Parameters.Select(p => p.Name))}\n";
                }

                builder.AddField(x =>
                {
                    x.Name = $"Command Information";
                    x.Value = $"__Aliases:__ {prefix}{string.Join($", {prefix}", cmd.Aliases)}\n" + 
                              $"{para}" +
                              $"__Summary:__ {cmd.Summary}";
                    x.IsInline = false;
                });
            }

            await ReplyAsync("", false, builder.Build());
        }

        [Command("uptime")]
        [Summary("Shows the bot's uptime.")]
        public async Task Uptime()
        {

            var auth = new EmbedAuthorBuilder()
            {
                Name = "Uptime",
            };

            var embed = new EmbedBuilder()
            {
                Color = new Color(70, 140, 70),
                Author = auth,
            };

            using (var process = Process.GetCurrentProcess())
            {
                var time = DateTime.Now - process.StartTime;

                var sb = new StringBuilder();
                if (time.Days > 0)
                {
                    sb.Append($"{time.Days}d ");
                }
                if (time.Hours > 0)
                {
                    sb.Append($"{time.Hours}h ");
                }
                if (time.Minutes > 0)
                {
                    sb.Append($"{time.Minutes}m ");
                }
                sb.Append($"{time.Seconds}s ");

                embed.Title =
                    $"{sb}";

                await this.ReplyAsync("", false, embed.Build());
            }
        }

        [Command("botinfo")]
        [Summary("Shows the bot's information.")]
        public async Task BotInfo()
        {
            using (var process = Process.GetCurrentProcess())
            {
                var embed = new EmbedBuilder();
                var application = await Context.Client.GetApplicationInfoAsync();
                embed.ThumbnailUrl = application.IconUrl.ToString();
                embed.WithColor(new Color(BotConfig.EmbedColor.R, BotConfig.EmbedColor.G, BotConfig.EmbedColor.B))
                .AddField(y =>
                {
                    y.Name = "Author";
                    y.Value = application.Owner.Username; application.Owner.Id.ToString();
                    y.IsInline = false;
                })
                .AddField(y =>
                {
                    y.Name = "Uptime";
                    var time = DateTime.Now - process.StartTime;
                    var sb = new StringBuilder();
                    if (time.Days > 0)
                    {
                        sb.Append($"{time.Days}d ");
                    }
                    if (time.Hours > 0)
                    {
                        sb.Append($"{time.Hours}h ");
                    }
                    if (time.Minutes > 0)
                    {
                        sb.Append($"{time.Minutes}m ");
                    }
                    sb.Append($"{time.Seconds}s ");
                    y.Value = sb.ToString();
                    y.IsInline = true;
                })
                .AddField(y =>
                {
                    y.Name = "Discord.Net Version";
                    y.Value = DiscordConfig.Version;
                    y.IsInline = true;
                })
                .AddField(y =>
                {
                    y.Name = "Bot Version";
                    y.Value = BotConfig.Version;
                    y.IsInline = true;
                })
                .AddField(y =>
                {
                    y.Name = "Server Amount";
                    y.Value = (Context.Client as DiscordSocketClient).Guilds.Count.ToString();
                    y.IsInline = true;
                })
                .AddField(y =>
                {
                    y.Name = "Number Of Users";
                    y.Value = (Context.Client as DiscordSocketClient).Guilds.Sum(g => g.Users.Count).ToString();
                    y.IsInline = true;
                })
                .AddField(y =>
                {
                    y.Name = "GitHub Repository";
                    y.Value = $"https://github.com/oatsfx/Geometric";
                    y.IsInline = true;
                });

                await ReplyAsync("", embed: embed.Build());
            }
        }

        [Command("userinfo")]
        [Summary("Shows a user's information.")]
        public async Task UserInfo(IGuildUser user = null)
        {
            if (user == null)
            {
                user = Context.User as IGuildUser;
            }

            var date = $"{user.CreatedAt.DateTime.ToLocalTime()}"; //$"{user.CreatedAt.Month}/{user.CreatedAt.Day}/{user.CreatedAt.Year} {user.CreatedAt.TimeOfDay}";
            var D = user.Username;
            var A = user.Discriminator;
            var T = user.Id;
            var S = date;
            var P = user.Status;
            var CC = user.JoinedAt;

            var auth = new EmbedAuthorBuilder()
            {
                Name = $"{user.Username} ({P})",
                IconUrl = user.GetAvatarUrl(),
            };

            var foot = new EmbedFooterBuilder()
            {
                Text = $"ID: {T}"
            };

            var embed = new EmbedBuilder()
            {
                Author = auth,
                Footer = foot,
            };

            embed.WithThumbnailUrl(user.GetAvatarUrl());
            embed.Description = $"{user.Mention}";
            embed.WithColor(new Color(BotConfig.EmbedColor.R, BotConfig.EmbedColor.G, BotConfig.EmbedColor.B))
            .AddField(y =>
             {
                 y.Name = "Joined";
                 y.Value = CC.ToString().Substring(0, CC.ToString().Length - 7);
                 y.IsInline = true;
             })
             .AddField(y =>
             {
                 y.Name = "Registered";
                 y.Value = S;
                 y.IsInline = true;
             });

            await ReplyAsync("", false, embed.Build());
        }

        [Command("serverinfo")]
        [Alias("guildinfo")]
        [Remarks("Shows a server's information.")]
        public async Task GuildInfo()
        {
            var gld = Context.Guild as SocketGuild;
            var client = Context.Client as DiscordSocketClient;

            var auth = new EmbedAuthorBuilder()
            {
                Name = $"{gld.Name}",
                IconUrl = gld.IconUrl.ToString(),
            };

            var foot = new EmbedFooterBuilder()
            {
                Text = $"ID: {gld.Id}"
            };

            var embed = new EmbedBuilder()
            {
                Author = auth,
                Footer = foot,
            };

            embed.WithThumbnailUrl(gld.IconUrl.ToString());
            embed.WithColor(new Color(BotConfig.EmbedColor.R, BotConfig.EmbedColor.G, BotConfig.EmbedColor.B))
            .AddField(y =>
            {
                y.Name = "Owner";
                y.Value = gld.Owner.Username;
                y.IsInline = true;
            })
            .AddField(y =>
            {
                y.Name = "Region";
                y.Value = gld.VoiceRegionId;
                y.IsInline = true;
            })
            .AddField(y =>
            {
                y.Name = "Text Channels";
                y.Value = gld.TextChannels.Count;
                y.IsInline = true;
            })
            .AddField(y =>
            {
                y.Name = "Voice Channels";
                y.Value = gld.VoiceChannels.Count;
                y.IsInline = true;
            })
            .AddField(y =>
            {
                y.Name = "Members";
                y.Value = gld.MemberCount;
                y.IsInline = true;
            });

            await ReplyAsync("", false, embed.Build());
        }

        [Command("emojis")]
        [Summary("Shows the servers that the bot gets emojis from.")]
        public async Task Emojis()
        {
            var embed = new EmbedBuilder();
            var application = await Context.Client.GetApplicationInfoAsync();
            var auth = new EmbedAuthorBuilder()
            {
                Name = $"Emotes",
                IconUrl = application.IconUrl.ToString(),
            };
            var foot = new EmbedFooterBuilder()
            {
                Text = $"Feel free to join these servers to use the Emote for your bots!",
            };
            embed.Author = auth;
            embed.Footer = foot;
            embed.WithColor(new Color(BotConfig.EmbedColor.R, BotConfig.EmbedColor.G, BotConfig.EmbedColor.B))
            .AddField(y =>
            {
                y.Name = $"Emote Bank I";
                y.Value = $"[Join](https://discord.gg/8z3dEaX)";
                y.IsInline = true;
            });

            await ReplyAsync("", embed: embed.Build());
        }

        [Command("invite")]
        [Summary("Provies links that support this bot.")]
        public async Task Invite()
        {
            var embed = new EmbedBuilder();
            embed.WithColor(new Color(BotConfig.EmbedColor.R, BotConfig.EmbedColor.G, BotConfig.EmbedColor.B));
            var application = await Context.Client.GetApplicationInfoAsync();

            embed.ThumbnailUrl = "https://cdn.discordapp.com/avatars/338072870368575517/436115f4ed372cbd91da796db16a82b1.png?size=1024";

            embed.Title = "Invite Links";
            embed.Description =
                "__**Invite Geometric to your server!**__" +
                "\n[Invite Geometric](https://discord.com/oauth2/authorize?client_id=338072870368575517&scope=bot&permissions=2146954358)" +
                "\n__**Join the official support server if you have any questions!**__" +
                "\n[Join](https://discord.gg/kbtu49S)" +
                "\n__**View us on top.gg! (Pending Verification)**__" +
                "\n[Geometric on Top.GG](https://top.gg/bot/338072870368575517)";

            await ReplyAsync("", false, embed.Build());
        }
    }
}
