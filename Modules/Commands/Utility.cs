using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.WebSocket;
using System.Threading;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace Geometric.Modules.Commands
{
    public class Utility : ModuleBase
    {
        //Team Randomizer
        public class Randomizer
        {
            public static void Randomize<T>(T[] items)
            {
                Random rand = new Random();

                // For each spot in the array, pick a random item to swap into that spot.
                for (int i = 0; i < items.Length - 1; i++)
                {
                    int j = rand.Next(i, items.Length);
                    T temp = items[i];
                    items[i] = items[j];
                    items[j] = temp;
                }
            }
        }

        //Discord Command Service Setup
        private CommandService _service;
        public Utility(CommandService service)
        {
            _service = service;
        }

        // Begin Commands
        [Command("ping")]
        [Summary("Provides the latency of the bot.")]
        public async Task Ping()
        {
            var auth = new EmbedAuthorBuilder()
            {
                Name = "pong",
            };

            var embed = new EmbedBuilder()
            {
                Color = new Color(70, 140, 70),
                Author = auth,
            };

            embed.Description =
                $"{(Context.Client as DiscordSocketClient).Latency} ms";

            await ReplyAsync("", false, embed.Build());
        }

        [Command("discriminator")]
        [Alias("discrim")]
        [Summary("Do other users have the same discriminator as you? Let's find out!")]
        public async Task Discriminator(ushort discriminatorValue = 0000 /*Discord only allows 0001-9999 as Discriminator values*/)
        {
            var currentUser = Context.User;

            if (discriminatorValue == 0000) // Checks to see if parameter was blank (Discord only allows 0001-9999 as Discriminator values)
            {
                discriminatorValue = currentUser.DiscriminatorValue; // If it was blank (0000), this sets the discrimVal to the command user's
            }

            List<string> matches = new List<string>();
            string matchesPost = "";

            IReadOnlyCollection<IGuild> guilds = await Context.Client.GetGuildsAsync().ConfigureAwait(false); // Gets every guild the bot client is in

            foreach (IGuild guild in guilds)
            {
                IReadOnlyCollection<IUser> users = await guild.GetUsersAsync().ConfigureAwait(false); // Gets every user in every guild

                foreach (IUser user in users)
                {
                    if (discriminatorValue == user.DiscriminatorValue && currentUser.Username != user.Username /*prevents command user from matching*/ ) // Checks for other users with command user's Discriminator
                    {
                        if (!user.IsBot) // Prevents bots from matching
                        {
                            matches.Add($"{user}"); // Adds them to a list of Discriminator matches
                        }
                    }
                }
            }

            List<string> distinctMatches = matches.Distinct().ToList(); // Removes Duplicate Users (if user appears more than once in a guild)

            for (int i = 0; i < distinctMatches.Count && i < 9 /*Only allows 9 matches*/ ; i++)
            {
                matchesPost += $"{distinctMatches[i]}\n"; // Converts to a string to post to discord
            }

            if (matchesPost.Length == 0) // Checks for results
            {
                await ReplyAsync($"There are no users that I can see with Discriminator: **{discriminatorValue.ToString().PadLeft(4, '0')}**" +
                    $"\nI am currently in {(Context.Client as DiscordSocketClient).Guilds.Count.ToString()} servers. That isn't enough to have that Discriminator match with other users."); // Posts to Discord the results
            }
            else
            {
                await ReplyAsync($"Users with Discriminator **{discriminatorValue.ToString().PadLeft(4, '0')}**:\n{matchesPost}"); // Posts to Discord the results
            }
        }

        [Command("team")]
        [Summary("Randomizes a team when given a list of names.")]
        public async Task Random(params String[] teamMembers)
        {
            Random random = new Random();
            int redRnd = random.Next(0, 255);
            Random random2 = new Random();
            int greenRnd = random.Next(0, 255);
            Random random3 = new Random();
            int blueRnd = random.Next(0, 255);

            //Randomize.
            Randomizer.Randomize(teamMembers);

            StringBuilder sb1 = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();

            for (int i = 0; i < (teamMembers.Length) / 2; i++)
            {
                sb1.AppendLine($"{teamMembers[i].First().ToString().ToUpper() + teamMembers[i].ToString().Substring(1)}");
            }

            for (int i = (teamMembers.Length) / 2; i < teamMembers.Length; i++)
            {
                sb2.AppendLine($"{teamMembers[i].First().ToString().ToUpper() + teamMembers[i].ToString().Substring(1)}");
            }

            var embed = new EmbedBuilder();
            var auth = new EmbedAuthorBuilder()
            {
                Name = $"Team Generator",
            };
            var foot = new EmbedFooterBuilder()
            {
                Text = $"Generated by {Context.User.Username}",
            };
            embed.Footer = foot;
            embed.WithColor(new Color(redRnd, blueRnd, greenRnd))
            .AddField(y =>
            {
                y.Name = $"Team 1";
                y.Value = $"{sb1.ToString().First().ToString().ToUpper() + sb1.ToString().Substring(1)}";
                y.IsInline = true;
            })
            .AddField(y =>
            {
                y.Name = $"Team 2";
                y.Value = $"{sb2.ToString().First().ToString().ToUpper() + sb2.ToString().Substring(1)}";
                y.IsInline = true;
            });

            await ReplyAsync("", false, embed.Build());
        }
    }
}
