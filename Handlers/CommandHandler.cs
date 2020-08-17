using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.IO;
using Geometric;
using System;
using Microsoft.Extensions.DependencyInjection;
using Discord.Addons.Interactive;

namespace Geometric
{
    public class CommandHandler : ModuleBase
    {
        private CommandService commands;
        private DiscordSocketClient client;
        private IServiceProvider services;

        string prefixPath;
        string rolePath;
        string guildPath;

        public async Task Install(DiscordSocketClient c)
        {
            client = c;

            services = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton<InteractiveService>()
                .BuildServiceProvider();

            commands = new CommandService();
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);

            client.MessageReceived += HandleCommand;

            client.UserJoined += AnnounceJoinedUser;
            client.UserLeft += AnnounceLeftUser;

            client.JoinedGuild += BotJoinedGuild;
            client.LeftGuild += BotLeftGuild;
        }

        public async Task AnnounceJoinedUser(SocketGuildUser user)
        {
            var devChannel = client.GetChannel(384125484231032832) as SocketTextChannel;
            var guild = user.Guild.Name;
            var users = (client as DiscordSocketClient).Guilds.Sum(g => g.Users.Count);

            Random rand = new Random();
            int rndNum = rand.Next(0, 7);

            if (rndNum == 1)
            {
                await client.SetActivityAsync(new Game($"for g.help | {BotConfig.Version} | Users: {users}", ActivityType.Watching));
            }

            await devChannel.SendMessageAsync($"<:member_join:690401890860662794> {user} in {guild}");

            //Begin AutoRole Module
            var serverId = user.Guild.Id;
            rolePath = @"BotDatabase\AutoRole\" + serverId + ".txt";

            if (File.Exists(rolePath))
            {
                List<string> roleName = File.ReadAllLines(rolePath).ToList();
                int roleCount = File.ReadAllLines(rolePath).Count();

                if (roleCount > 1)
                {
                    for(int i = 0; i < roleCount; i++)
                    {
                        var role = user.Guild.Roles.FirstOrDefault(x => x.Name == roleName[i]);
                        await user.AddRoleAsync(role);
                    }
                }
                else
                {
                    var role = user.Guild.Roles.FirstOrDefault(x => x.Name == roleName[0]);
                    await user.AddRoleAsync(role);
                }
            }

            //Begin Welcome Log Module
            guildPath = @"BotDatabase\WelcomeMessage\" + serverId;

            if (Directory.Exists(guildPath))
            {
                string[] messagePaths = Directory.GetFiles(guildPath);

                if (messagePaths == null) return;

                foreach (string messagePath in messagePaths)
                {
                    List<string> messageTxt = File.ReadAllLines(messagePath).ToList();
                    string channelIdTxt = Path.GetFileNameWithoutExtension(messagePath);
                    var msgChannel = user.Guild.GetTextChannel(Convert.ToUInt64(channelIdTxt));

                    await msgChannel.SendMessageAsync($"{messageTxt[0].Replace("{user}", user.Mention)}");
                }
            }
            else
            {
                return;
            }
        }

        public async Task AnnounceLeftUser(SocketGuildUser user)
        {
            var devChannel = client.GetChannel(384125484231032832) as SocketTextChannel;
            var guild = user.Guild.Name;
            var users = (client as DiscordSocketClient).Guilds.Sum(g => g.Users.Count).ToString();

            Random rand = new Random();
            int rndNum = rand.Next(0, 7);

            if (rndNum == 1)
            {
                await client.SetActivityAsync(new Game($"for g.help | {BotConfig.Version} | Users: {users}", ActivityType.Watching));
            }

            await devChannel.SendMessageAsync($"<:member_leave:743979522990538862> {user} in {guild}");
        }

        public async Task BotJoinedGuild(SocketGuild server)
        {
            var devChannel = client.GetChannel(382991625858842624) as SocketTextChannel;
            var servers = (client as DiscordSocketClient).Guilds.Count;

            Random rand = new Random();
            int rndNum = rand.Next(0, 7);

            if (rndNum == 1)
            {
                await client.SetActivityAsync(new Game($"for g.help | {BotConfig.Version} | Servers: {servers}", ActivityType.Watching));
            }

            await devChannel.SendMessageAsync($"<:member_join:690401890860662794> Geometric joined: `{server}`. <a:partypepe:690374178595143700>");
        }

        public async Task BotLeftGuild(SocketGuild server)
        {
            var devChannel = client.GetChannel(382991625858842624) as SocketTextChannel;
            var servers = (client as DiscordSocketClient).Guilds.Count.ToString();

            Random rand = new Random();
            int rndNum = rand.Next(0, 7);

            if (rndNum == 1)
            {
                await client.SetActivityAsync(new Game($"for g.help | {BotConfig.Version} | Servers: {servers}", ActivityType.Watching));
            }

            await devChannel.SendMessageAsync($"<:member_leave:690401898502684673> Geometric left: `{server}`.");
        }

        public async Task HandleCommand(SocketMessage s)
        {
            
            if (s.Author.IsWebhook) return;
            if (s.Author.IsBot) return;
            if (s.Channel.GetType() == typeof(SocketDMChannel)) return;

            SocketUserMessage msg = s as SocketUserMessage;

            var context = new SocketCommandContext(client, msg);

            //Begin Prefix Module
            var serverId = context.Guild.Id;
            prefixPath = @"BotDatabase\Prefix\" + serverId + ".txt";

            if (!File.Exists(prefixPath))
            {
                int argPos = 0;

                if (msg.HasStringPrefix("g.", ref argPos) || msg.HasMentionPrefix(client.CurrentUser, ref argPos))
                {
                    var result = await commands.ExecuteAsync(context, argPos, services);
                }
            }
            else
            {
                List<string> thePrefix = File.ReadAllLines(prefixPath).ToList();

                int argPos = 0;

                if (msg.HasStringPrefix($"{thePrefix[0]}", ref argPos) || msg.HasMentionPrefix(client.CurrentUser, ref argPos))
                {
                    var result = await commands.ExecuteAsync(context, argPos, services);
                }
            }
        }
    }
}
