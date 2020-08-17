using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Interactive;
using Discord.WebSocket;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using Discord.Commands;
using System.Reflection;

namespace Geometric
{
    class Program
    {
        public static void Main(string[] args) =>
            new Program().Start().GetAwaiter().GetResult();

        private DiscordSocketClient client;
        private CommandHandler commands;

        public async Task Start()
        {
            //-----------------This is for using config.json. DO NOT DELETE, IT BREAKS THE CONFIG SETUP :D
            string json = File.ReadAllText(@"BotDatabase\config.json");
            BotConfig config = JsonConvert.DeserializeObject<BotConfig>(json);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Current Bot Version: {BotConfig.Version}");

            client = new DiscordSocketClient();
            client.Log += log =>
            {
                Console.WriteLine(log.ToString());
                return Task.CompletedTask;
            };

            await client.LoginAsync(TokenType.Bot, BotConfig.Token);
            await client.StartAsync();
            await client.SetActivityAsync(new Game($"for g.help | {BotConfig.Version}", ActivityType.Watching));

            commands = new CommandHandler();
            await commands.Install(client);

            await Task.Delay(-1);
        }
    }
}
