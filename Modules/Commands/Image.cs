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
    public class Image : ModuleBase
    {
        //Cat Json Module
        public class RandomCat
        {
            public RandomCat(string link)
            {
                Url = link;
            }

            public string Url { get; private set; }
        }

        //Dog Json Module
        public class RandomDog
        {
            public RandomDog(string link)
            {
                Url = link;
            }

            public string Url { get; private set; }
        }

        //Birb Json Module
        public class RandomBirb
        {
            public RandomBirb(string link)
            {
                Url = link;
            }

            public string Url { get; private set; }
        }

        //Hug Json Module
        public class RandomHug
        {
            public RandomHug(string link)
            {
                Url = link;
            }

            public string Url { get; private set; }
        }

        //Discord Command Service Setup
        private CommandService _service;
        public Image(CommandService service)
        {
            _service = service;
        }

        //Begin Commands
        [Command("cat")]
        [Summary("Meow.")]
        public async Task Cat()
        {
            RandomCat randomCat = null;

            using (WebClient webClient = new WebClient())
            {
                try
                {
                    webClient.UseDefaultCredentials = true;
                    string json = webClient.DownloadString("https://some-random-api.ml/img/cat");
                    randomCat = JsonConvert.DeserializeObject<RandomCat>(json);
                }
                catch (Exception ex)
                {
                    await ReplyAsync($"The API used for cat images has failed and isn't responding. Please try again. If this problem persists, let us know in the support server (<https://discord.gg/kbtu49S>).");
                    Console.WriteLine(ex);
                }
            }

            Random random = new Random();
            int redRnd = random.Next(0, 255);
            Random random2 = new Random();
            int greenRnd = random.Next(0, 255);
            Random random3 = new Random();
            int blueRnd = random.Next(0, 255);

            var foot = new EmbedFooterBuilder()
            {
                Text = $"Powered by https://some-random-api.ml/",
            };

            var embed = new EmbedBuilder()
            {
                Title = "Random Cat Generator",
                ImageUrl = $"{randomCat.Url}",
                Color = new Color(redRnd, greenRnd, blueRnd),
                Footer = foot,
            };

            await ReplyAsync("", false, embed.Build());
        }

        [Command("dog")]
        [Summary("Bork.")]
        public async Task Dog()
        {
            RandomDog randomDog = null;

            using (WebClient webClient = new WebClient())
            {
                try
                {
                    webClient.UseDefaultCredentials = true;
                    string json = webClient.DownloadString("https://some-random-api.ml/img/dog");
                    randomDog = JsonConvert.DeserializeObject<RandomDog>(json);
                }
                catch (Exception ex)
                {
                    await ReplyAsync($"The API used for dog images has failed and isn't responding. Please try again. If this problem persists, let us know in the support server (<https://discord.gg/kbtu49S>).");
                    Console.WriteLine(ex);
                }
            }

            Random random = new Random();
            int redRnd = random.Next(0, 255);
            Random random2 = new Random();
            int greenRnd = random.Next(0, 255);
            Random random3 = new Random();
            int blueRnd = random.Next(0, 255);

            var foot = new EmbedFooterBuilder()
            {
                Text = $"Powered by https://some-random-api.ml/",
            };

            var embed = new EmbedBuilder()
            {
                Title = "Random Dog Generator",
                ImageUrl = $"{randomDog.Url}",
                Color = new Color(redRnd, greenRnd, blueRnd),
                Footer = foot,
            };

            await ReplyAsync("", false, embed.Build());
        }

        [Command("birb")]
        [Alias("bird")]
        [Summary("HONK")]
        public async Task Birb()
        {
            RandomBirb randomBirb = null;

            using (WebClient webClient = new WebClient())
            {
                try
                {
                    webClient.UseDefaultCredentials = true;
                    string json = webClient.DownloadString("https://some-random-api.ml/img/birb");
                    randomBirb = JsonConvert.DeserializeObject<RandomBirb>(json);
                }
                catch (Exception ex)
                {
                    await ReplyAsync($"The API used for birb (bird) images has failed and isn't responding. Please try again. If this problem persists, let us know in the support server (<https://discord.gg/kbtu49S>).");
                    Console.WriteLine(ex);
                }
            }

            Random random = new Random();
            int redRnd = random.Next(0, 255);
            Random random2 = new Random();
            int greenRnd = random.Next(0, 255);
            Random random3 = new Random();
            int blueRnd = random.Next(0, 255);

            var foot = new EmbedFooterBuilder()
            {
                Text = $"Powered by https://some-random-api.ml/",
            };

            var embed = new EmbedBuilder()
            {
                Title = "Random Birb Generator",
                ImageUrl = $"{randomBirb.Url}",
                Color = new Color(redRnd, greenRnd, blueRnd),
                Footer = foot,
            };

            await ReplyAsync("", false, embed.Build());
        }

        [Command("hug")]
        [Summary("Hugs another user.")]
        public async Task Hug(IGuildUser user = null)
        {
            RandomHug randomHug = null;

            if (user == null || user.Username == Context.User.Username)
            {
                await ReplyAsync("You can't hug youself, unless you really want to.");
                return;
            }

            using (WebClient webClient = new WebClient())
            {
                try
                {
                    webClient.UseDefaultCredentials = true;
                    string json = webClient.DownloadString("https://some-random-api.ml/animu/hug");
                    randomHug = JsonConvert.DeserializeObject<RandomHug>(json);
                }
                catch (Exception ex)
                {
                    await ReplyAsync($"The API used for hug GIFs has failed and isn't responding. Please try again. If this problem persists, let us know in the support server (<https://discord.gg/kbtu49S>).");
                    Console.WriteLine(ex);
                }
            }

            Random random = new Random();
            int redRnd = random.Next(0, 255);
            Random random2 = new Random();
            int greenRnd = random.Next(0, 255);
            Random random3 = new Random();
            int blueRnd = random.Next(0, 255);

            var auth = new EmbedAuthorBuilder()
            {
                Name = $"{Context.User.Username} hugged {user.Username}",
            };

            var foot = new EmbedFooterBuilder()
            {
                Text = $"Powered by https://some-random-api.ml/",
            };

            var embed = new EmbedBuilder()
            {
                Author = auth,
                ImageUrl = $"{randomHug.Url}",
                Color = new Color(redRnd, greenRnd, blueRnd),
                Footer = foot,
            };

            await ReplyAsync("", false, embed.Build());
        }
    }
}
