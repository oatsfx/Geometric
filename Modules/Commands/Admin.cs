using Discord;
using Discord.Commands;
using Discord.Audio;
using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.WebSocket;
using System.Threading;
using System.IO;
using Geometric;

namespace GeometricDiscordBot.Modules.Commands
{
    public class Admin : ModuleBase
    {
        private CommandService _service;

        public Admin(CommandService service)
        {
            _service = service;
        }

        [Command("purge")]
        [Summary("Deletes an specific amount of messages.")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task Purge([Remainder] int messagesToBeDeleted = 0)
        {
            if (messagesToBeDeleted <= 100)
            {
                var channel = Context.Channel as ITextChannel;
                IEnumerable<IMessage> messagesToDelete = await Context.Channel.GetMessagesAsync(messagesToBeDeleted + 1).FlattenAsync();

                var m = await Context.Channel.SendMessageAsync("<a:chaoscontrol:691431189038497922>");

                await Task.Delay(2200);

                await channel.DeleteMessagesAsync(messagesToDelete);
                await m.DeleteAsync();
            }
            else
            {
                await ReplyAsync("**Max number of messages to delete is 100.**");
            }
        }

        [Command("kick")]
        [Summary("Kicks a user in a guild.")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task KickAsync(SocketGuildUser user = null, [Remainder] string reason = null)
        {
            if (user == null)
            {
                await ReplyAsync("**You forgot to mention a user to kick!**");
            }
            else if (string.IsNullOrWhiteSpace(reason))
            {
                await ReplyAsync("**You forgot to give a reason for the member to be kicked.**");
            }
            else
            {
                var gld = Context.Guild as SocketGuild;
                var embed = new EmbedBuilder();
                embed.WithColor(new Color(BotConfig.EmbedColor.R, BotConfig.EmbedColor.G, BotConfig.EmbedColor.B));
                embed.Title = $"**{user.Username} has been kicked from {user.Guild}!!**";
                embed.Description = $"**Username: **{user.Username}\n**Guild Name: **{user.Guild.Name}\n**Kicked by: **{Context.User.Mention}!\n**Reason: **{reason}";

                await user.KickAsync();
                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
        }

        [Command("ban")]
        [Summary("Bans a user in a guild.")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task BanAsync(SocketGuildUser user = null, [Remainder] string reason = null)
        {
            if (user == null)
            {
                await ReplyAsync("**You forgot to mention a user to ban!**");
            }
            else if (string.IsNullOrWhiteSpace(reason))
            {
                await ReplyAsync("**You forgot to give a reason for the member to be banned.**");
            }
            else
            {
                var gld = Context.Guild as SocketGuild;
                var embed = new EmbedBuilder();
                embed.WithColor(new Color(BotConfig.EmbedColor.R, BotConfig.EmbedColor.G, BotConfig.EmbedColor.B));
                embed.Title = $"**{user.Username} has been banned from {user.Guild}!!**";
                embed.Description = $"**Username: **{user.Username}\n**Guild Name: **{user.Guild.Name}\n**Banned by: **{Context.User.Mention}!\n**Reason: **{reason}";

                await gld.AddBanAsync(user);
                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
        }

        [Command("prefix")]
        [Summary("Gives or changes the current guild's prefix.")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task Prefix([Remainder] string prefix = null)
        {
            var serverId = Context.Guild.Id;
            string prefixPath = @"BotDatabase\Prefix\" + serverId + ".txt";
            try
            {
                if (prefix == null)
                {
                    if (!File.Exists(prefixPath))
                    {
                        await ReplyAsync($"prefix in this server is `g.`");
                    }
                    else
                    {
                        List<string> thePrefix = File.ReadAllLines(prefixPath).ToList();

                        await ReplyAsync($"prefix in this server is `{thePrefix[0]}`; *reset using `{thePrefix[0]}prefix reset`*");
                    }
                }
                else if (prefix == "g.")
                {
                    
                    if (!File.Exists(prefixPath))
                    {
                        await ReplyAsync($"the default prefix is `g.`; use `g.prefix reset`");
                    }
                    else
                    {
                        List<string> thePrefix = File.ReadAllLines(prefixPath).ToList();

                        await ReplyAsync($"the default prefix is `g.`; use `{thePrefix[0]}prefix reset`");
                    }
                }
                else if (prefix.ToLower() == "reset")
                {
                    if (!File.Exists(prefixPath))
                    {
                        await ReplyAsync($"no custom prefix");
                    }
                    else
                    {
                        File.Delete(prefixPath);

                        await ReplyAsync($"prefix in this server was reset to `g.`");
                    }
                }
                else
                {
                    if (!File.Exists(prefixPath))
                    {
                        var prefixFile = File.Create(prefixPath);
                        prefixFile.Close();

                        List<string> thePrefix = File.ReadAllLines(prefixPath).ToList();

                        thePrefix.Clear();
                        thePrefix.Add($"{prefix}");

                        File.WriteAllLines(prefixPath, thePrefix);

                        await ReplyAsync($"prefix in this server is `{prefix}`; *reset using `{prefix}prefix reset`*");
                    }
                    else
                    {
                        List<string> thePrefix = File.ReadAllLines(prefixPath).ToList();

                        thePrefix.Clear();
                        thePrefix.Add($"{prefix}");

                        File.WriteAllLines(prefixPath, thePrefix);

                        await ReplyAsync($"prefix in this server is `{prefix}`; *reset using `{prefix}prefix reset`*");
                    }
                }
            }
            catch(Exception x)
            {
                Console.WriteLine(x);
            }
        }

        [Command("autorole")]
        [Summary("Make the bot assign a role when a member joins your Discord server.")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task Autorole(string action = null, [Remainder] string roleName = null)
        {
            var serverId = Context.Guild.Id;
            string rolePath = @"BotDatabase\AutoRole\" + serverId + ".txt";
            string roleList = "";
            string actionLower = action.ToLower();

            if (actionLower == "remove")
            {
                if (File.Exists(rolePath))
                {
                    var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == roleName);
                    List<string> roleNameTxt = File.ReadAllLines(rolePath).ToList();
                    int roleCount = File.ReadAllLines(rolePath).Count();

                    for (int i = 0; i < roleCount; i++)
                    {
                        if (roleNameTxt[i] == role.Name)
                        {
                            roleNameTxt.RemoveAt(i);
                            File.WriteAllLines(rolePath, roleNameTxt);
                            await ReplyAsync($"{role.Name} has been removed.");
                            return;
                        }
                    }

                    await ReplyAsync($"{role.Name} does not exist");
                }
                else
                {
                    await ReplyAsync($"No roles to remove.");
                }
            }
            else if (actionLower == "removeall")
            {
                if (File.Exists(rolePath))
                {
                    File.Delete(rolePath);

                    await ReplyAsync($"Deleted all auto roles.");
                }
                else
                {
                    await ReplyAsync($"No roles to remove.");
                }
            }
            else if (actionLower == "add")
            {
                if (File.Exists(rolePath))
                {
                    var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == roleName);
                    List<string> roleNameTxt = File.ReadAllLines(rolePath).ToList();
                    int roleCount = File.ReadAllLines(rolePath).Count();

                    for (int i = 0; i < roleCount; i++)
                    {
                        if (roleNameTxt[i] == role.Name)
                        {
                            await ReplyAsync($"{role.Name} already exists");
                            return;
                        }
                    }

                    roleNameTxt.Add($"{role.Name}");
                    File.WriteAllLines(rolePath, roleNameTxt);

                    await ReplyAsync($"Added {role.Name} to Auto Role list.");
                }
                else
                {
                    File.Create(rolePath);

                    var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == roleName);
                    List<string> roleNameTxt = File.ReadAllLines(rolePath).ToList();
                    int roleCount = File.ReadAllLines(rolePath).Count();

                    for (int i = 0; i < roleCount; i++)
                    {
                        if (roleNameTxt[i] == role.Name)
                        {
                            await ReplyAsync($"{role.Name} already exists");
                            return;
                        }
                    }

                    roleNameTxt.Add($"{role.Name}");
                    File.WriteAllLines(rolePath, roleNameTxt);

                    await ReplyAsync($"Added {role.Name} to Auto Role list.");
                }
            }
            else if (actionLower == "list")
            {
                if (File.Exists(rolePath))
                {
                    List<string> roleNameTxt = File.ReadAllLines(rolePath).ToList();
                    int roleCount = File.ReadAllLines(rolePath).Count();

                    if (roleCount > 1)
                    {
                        for (int i = 0; i < roleCount; i++)
                        {
                            if (i == roleCount - 1)
                            {
                                roleList += $"{roleNameTxt[i]}";
                            }
                            else
                            {
                                roleList += $"{roleNameTxt[i]}, ";
                            }
                        }
                    }
                    else
                    {
                        roleList = $"{roleNameTxt[0]}";
                    }
                    await ReplyAsync($"Current Auto Roles in {Context.Guild} are: {roleList}");
                }
                else
                {
                    await ReplyAsync($"Auto Role is not setup. Use `g.autorole add [role name]` to get started.");
                }
            }
        }

        [Command("welcomemessage")]
        [Alias("welcomemsg")]
        [Summary("Make the bot provide a welcome message when a new user joins your Discord server.")]
        [RequireUserPermission(GuildPermission.ManageChannels)]
        public async Task WelcomeMessage(string action = null, IGuildChannel postChannel = null, [Remainder] string message = null)
        {
            var serverId = Context.Guild.Id;
            var channelId = postChannel.Id;
            string directPath = @"BotDatabase\WelcomeMessage\" + serverId;
            string messagePath = directPath + @"\" + channelId + ".txt";
            string actionLower = action.ToLower();

            if (!Directory.Exists(directPath))
            {
                Directory.CreateDirectory(directPath);
                Directory.GetAccessControl(directPath);
            }

            try
            {
                if (actionLower == "add")
                {
                    if (File.Exists(messagePath))
                    {
                        File.GetAccessControl(messagePath);
                        List<string> messageTxt = File.ReadAllLines(messagePath).ToList();

                        messageTxt.Clear();
                        messageTxt.Add($"{message}");
                        File.WriteAllLines(messagePath, messageTxt);

                        await ReplyAsync($"Added <#{channelId}> with message: `{message}`");
                    }
                    else
                    {
                        var messageFile = File.Create(messagePath);
                        messageFile.Close();

                        List<string> messageTxt = File.ReadAllLines(messagePath).ToList();

                        messageTxt.Add($"{message}");
                        File.WriteAllLines(messagePath, messageTxt);

                        await ReplyAsync($"Added <#{channelId}> with message: `{message}`");
                    }
                }
                else if (actionLower == "remove")
                {
                    if (File.Exists(messagePath))
                    {
                        List<string> messageTxt = File.ReadAllLines(messagePath).ToList();
                        string channelIdTxt = Path.GetFileNameWithoutExtension(messagePath);

                        if (channelIdTxt == postChannel.Id.ToString())
                        {
                            await ReplyAsync($"**Welcome log deleted.**" +
                            $"\n__Message:__ {messageTxt[0]}" +
                            $"\n__Channel:__ <#{channelIdTxt}>");

                            File.Delete(messagePath);
                        }
                        else
                        {
                            await ReplyAsync($"The channel <#{channelIdTxt}> has no welcome log to remove.");
                        }
                    }
                    else
                    {
                        await ReplyAsync($"No welcome log to remove.");
                    }
                }
                else if (actionLower == "list")
                {
                    if (File.Exists(messagePath))
                    {
                        List<string> messageTxt = File.ReadAllLines(messagePath).ToList();
                        string channelIdTxt = Path.GetFileNameWithoutExtension(messagePath);

                        if (channelIdTxt == postChannel.Id.ToString())
                        {
                            await ReplyAsync($"**Current Welcome Message.**" +
                            $"\n__Message:__ {messageTxt[0]}" +
                            $"\n__Channel:__ <#{channelIdTxt}>");
                        }
                        else
                        {
                            await ReplyAsync($"The channel <#{channelIdTxt}> has no welcome log.");
                        }
                    }
                    else
                    {
                        await ReplyAsync($"This server does not have welcome log set up.");
                    }
                }
            }
            catch(Exception x)
            {
                Console.WriteLine(x);
            }
        }
    }
}

