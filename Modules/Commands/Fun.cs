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

namespace GeometricDiscordBot.Modules.Commands
{
    public class Fun : ModuleBase
    {
        //Animal Fact Json Module
        public class RandomFact
        {
            public RandomFact(string fact)
            {
                factText = fact;
            }

            public string factText { get; private set; }
        }

        //Pokemon Json Module
        public partial class GetPokemon
        {
            public string Name { get; set; }
            public long Id { get; set; }
            public string[] Type { get; set; }
            public string[] Species { get; set; }
            public string[] Abilities { get; set; }
            public string Height { get; set; }
            public string Weight { get; set; }
            //public long BaseExperience { get; set; }
            //public string[] Gender { get; set; }
            //public string[] EggGroups { get; set; }
            //public Stats Stats { get; set; }
            //public Family Family { get; set; }
            public Sprites Sprites { get; set; }
            //public string Description { get; set; }
            //public long Generation { get; set; }
        }
        /*
        public partial class Family
        {
            public long EvolutionStage { get; set; }
            public object[] EvolutionLine { get; set; }
        }*/

        public partial class Sprites
        {
            public Uri Normal { get; set; }
            public Uri Animated { get; set; }
        }
        /*
        public partial class Stats
        {
            public long Hp { get; set; }
            public long Attack { get; set; }
            public long Defense { get; set; }
            public long SpAtk { get; set; }
            public long SpDef { get; set; }
            public long Speed { get; set; }
            public long Total { get; set; }
        }*/

        //Arrays
        string[] responses;
        string[] jokes;
        string[] wyr;
        string[] puns;

        //Discord Command Service Setup
        private CommandService _service;
        public Fun(CommandService service)
        {
            _service = service;
        }

        //Begin Commands
        [Command("randomcolor")]
        [Alias("rc")]
        [Summary("Gives user a random color.")]
        public async Task RandomColor()
        {
            using (var process = Process.GetCurrentProcess())
            {
                Random random = new Random();
                int redRnd = random.Next(0, 255);
                Random random2 = new Random();
                int greenRnd = random.Next(0, 255);
                Random random3 = new Random();
                int blueRnd = random.Next(0, 255);
                /*this is required for up time*/
                var embed = new EmbedBuilder();
                var application = await Context.Client.GetApplicationInfoAsync(); /*for lib version*/
                embed.WithColor(new Color(redRnd, greenRnd, blueRnd)) /*Hexacode colours*/

                .AddField(y =>
                {
                    /*new embed field*/
                    y.Name = $"**Random Color for: {Context.User.Username}!**";  /*Field name here*/
                    y.Value = "The color's RGB values are (Color on Left): **" + redRnd + ", " + greenRnd + ", " + blueRnd + "**."; /*Code here. If INT convert to string*/
                    y.IsInline = false;
                });
                await this.ReplyAsync("", embed: embed.Build());
            }
        }

        [Command("8ball")]
        [Summary("Receive a Yes or No answer from the bot.")]
        public async Task Eightball([Remainder] string input)
        {
            var m = await Context.Channel.SendMessageAsync("<a:owowhatsthis:691429502764449883>");

            await Task.Delay(1000);

            await m.DeleteAsync();

            responses = new string[]
            {
                "**Yes. Undoubtingly. :ok_hand:**",
                "**It will never happen EVER! Not even in a parallel universe. :no_good:**",
                "**I wasn't playing attention, ask again. :joy:**",
                "**A little blurry, but it looked like a yes to me. :white_check_mark:**",
                "**Heck. No. :x:**"
            };

            var thumbnailUrl = "http://www.clker.com/cliparts/a/4/4/9/12370998112143206469nicubunu_Eight_ball.svg.med.png";

            var auth = new EmbedAuthorBuilder()
            {
                Name = "Magic 8 Ball!",
                IconUrl = thumbnailUrl,
            };

            var embed = new EmbedBuilder()
            {
                Color = new Color(35, 35, 35),
                Author = auth,
            };

            Random rnd = new Random();

            int response = rnd.Next(responses.Length);

            string responsePost = responses[response];

            embed.Title =
                $"Question: {input}";

            embed.Description =
                $"\nThe Magic 8 Ball says: {responsePost}";

            await ReplyAsync("", false, embed.Build());
        }

        [Command("joke")]
        [Summary("The bot will send you a joke.")]
        public async Task Joke()
        {
            Random random = new Random();
            int redRnd = random.Next(0, 255);
            Random random2 = new Random();
            int greenRnd = random.Next(0, 255);
            Random random3 = new Random();
            int blueRnd = random.Next(0, 255);

            jokes = new string[]
            {
                "**Q:** What do you call a fat psychic?" +
                "\n**A:** A four chin teller.",
                "I am a nobody, nobody is perfect, therefore I am perfect.",
                "**Q:** What's the difference between snowmen and snowladies?" +
                "\n**A:** Snowballs.",
                "**Q:** What do you call two fat people having a chat?" +
                "\n**A:** A heavy discussion.",
                "If con is the opposite of pro, it must mean Congress is the opposite of progress?",
                "A doctor tells a woman she can no longer touch anything alcoholic. So she gets a divorce.",
                "Politicians and diapers have one thing in common. They should both be changed regularly, and for the same reason.",
                "**Q:** What do you call a blonde with a brain?" +
                "\n**A:** A golden retriever.",
                "**Q:** What do you do if an idiot throws a grenade at you?" +
                "\n**A:** Pull the pin and throw it back at him!",
                "Three blondes walk into a building. You'd think one of them would've seen it...",
                "The sole purpose of a child's middle name is so they can tell when they're really in trouble.",
                "**God gave us the brain to work out problems. However, we use it to create more problems.**",
                "When wearing a bikini, women reveal 90% of their body... men are so polite they only look at the covered parts.",
                "Do not argue with an idiot. He will drag you down to his level and beat you with experience.",
                "Children: You spend the first 2 years of their life teaching them to walk and talk. Then you spend the next 16 years telling them to sit down and shut-up.",
                "Whenever I fill out an application, in the part that says ''If an emergency, notify:'' I put ''DOCTOR''. What's my mother going to do?",
            };

            Random rnd = new Random();

            int response = rnd.Next(jokes.Length);

            string responsePost = jokes[response];

            var auth = new EmbedAuthorBuilder()
            {
                Name = "Joke!",
            };

            var embed = new EmbedBuilder()
            {
                Color = new Color(redRnd, greenRnd, blueRnd),
                Author = auth,
            };

            embed.Description = $"{responsePost}";

            await ReplyAsync("", false, embed.Build());
        }

        [Command("wyr")]
        [Summary("The bot will ask you a would you rather quesiton.")]
        public async Task Wyr()
        {
            Random random = new Random();
            int redRnd = random.Next(0, 255);
            Random random2 = new Random();
            int greenRnd = random.Next(0, 255);
            Random random3 = new Random();
            int blueRnd = random.Next(0, 255);

            wyr = new string[]
            {
                "Would you rather, poop through your mouth, or have taste buds on your butthole?",
                "Would you rather, never speak again, or always speak your mind?",
                "Would you rather, eat a bowl of vomit, or lick a hobo's foot?",
                "Would you rather, watch your parents do it for the rest of your life, or join in once to never have to watch again?",
                "Would you rather, smell like eggs when you burp, or have a green cloud appear when you fart?",
                "Would you rather, have the hiccups for life, or feel like you have to sneeze and never sneeze for life?",
                "Would you rather, live like a king all alone, or be homeless with your friends and family?",
                "Would you rather, eat a potato and feel its pain, or be a potato?",
                "Would you rather, know when you're going to die, or how you're going to die?",
                "Would you rather, have a working lightsaber, or feed all the starving children in Africa?",
                "Would you rather, swim through human feeces, or dead bodies?",
                "Would you rather, eat poop that tastes like chocolate, or eat chocolate that tastes like poop?",
                "Would you rather, the aliens that make first contact be robotic, or organic?",
                "Would you rather, have one real get out of jail free card, or a key that opens any door?",
                "Would you rather, be married to a 10 with a bad personality, or a 6 with an amazing personality?",
                "Would you rather, have all traffic lights you approach be green, or never have to stand in line again?",
                "Would you rather, be able to see 10 minutes into your own future, or 10 minutes into the future of anyone but yourself?",
            };

            Random rnd = new Random();

            int response = rnd.Next(wyr.Length);

            string responsePost = wyr[response];

            var auth = new EmbedAuthorBuilder()
            {
                Name = "Would you Rather?",
            };

            var embed = new EmbedBuilder()
            {
                Color = new Color(redRnd, greenRnd, blueRnd),
                Author = auth,
            };

            embed.Description = $"{responsePost}";

            var reactMsg = await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("beer")]
        [Summary("Hold my beer...")]
        public async Task Beer()
        {
            await Context.Channel.SendMessageAsync($"Hold my beer, {Context.User.Mention}. :beer:");
        }

        [Command("clap")]
        [Summary("Replaces the spaces in a message with :clap: emojis.")]
        public async Task Clap([Remainder] string input = null)
        {
            if (input != null)
            {
                string replace = input.Replace(" ", " :clap: ");

                await ReplyAsync($"{replace}");
            }
            else
            {
                await ReplyAsync($"I :clap: can :clap: not :clap: clap :clap: something :clap: that :clap: isn't :clap: there!");
            }
        }

        [Command("pun")]
        [Summary("The bot will send you a pun.")]
        public async Task Thonk()
        {
            Random random = new Random();
            int redRnd = random.Next(0, 255);
            Random random2 = new Random();
            int greenRnd = random.Next(0, 255);
            Random random3 = new Random();
            int blueRnd = random.Next(0, 255);

            var client = Context.Client as DiscordSocketClient;

            puns = new string[]
            {
                "The machine at the coin factory just suddenly stopped working, with no explanation. It doesn't make any cents!",
                "Did you hear about the guy whose whole left side was cut off? He's all right now.",
                "I wasn't originally going to get a brain transplant, but then I changed my mind.",
                "Why don't some couples go to the gym? Because some relationships don't work out.",
                "Have you ever tried to eat a clock? It's very time consuming.",
                "Did you hear about the guy who got hit in the head with a can of soda? He was lucky it was a soft drink.",
                "I sent my baby off to the army. They put him in the infantry.",
                "I used to be a banker, but I lost interest.",
                "My mother never saw the irony in calling me a son-of-a-bitch.",
                "I'm reading a book about anti-gravity. It's impossible to put down.",
                "Claustrophobic people are more productive thinking outside the box.",
                "When William joined the army he disliked the phrase 'fire at will'.",
                "It's not that the man did not know how to juggle, he just didn't have the balls to do it.",
                "Police were called to a daycare where a three-year-old was resisting a rest.",
                "Something about subtraction just doesn't add up.",
                "I'm glad I know sign language, it's pretty handy.",
                "The first time I used an elevator it was really uplifting.",
                "I don't know if I just got hit by freezing rain, but it hurt like hail.",
                "A bicycle can't stand on its own because it is two-tired.",
                "I used to have a fear of hurdles, but I got over it.",
                "I lift weights only on Saturday and Sunday because Monday to Friday are weak days.",
                "I really wanted a camouflage shirt, but I couldn't find one.",
                "To write with a broken pencil is pointless.",
                "I knew a guy who collected candy canes, they were all in mint condition.",
                "I saw a beaver movie last night, it was the best dam movie I've ever seen.",
                "A new type of broom came out, it is sweeping the nation.",
                "I'm really good at being lazy. In fact, my doctor even said that if I continue being this lazy I should expect atrophy.",
            };

            var auth = new EmbedAuthorBuilder()
            {
                Name = "Pun!",
            };

            var embed = new EmbedBuilder()
            {
                Color = new Color(redRnd, greenRnd, blueRnd),
                Author = auth,
            };

            Random rnd = new Random();

            int response = rnd.Next(puns.Length);

            string punPost = puns[response];

            embed.Description = $"{punPost}";

            var msg = await ReplyAsync("", false, embed.Build());

            var emoteBank = client.GetGuild(397846250797662208);

            IEmote emote = emoteBank.Emotes.First(e => e.Name == "blobfacepalm");

            await msg.AddReactionAsync(emote);
        }

        [Command("cool")]
        [Summary("Too cool for school.")]
        public async Task Cool()
        {
            var msg = await ReplyAsync("( ͡° ͜ʖ ͡°)>⌐■-■");
            await Task.Delay(1000);
            await msg.ModifyAsync(x => { x.Content = "( ͡⌐■ ͜ʖ ͡-■)"; });
        }

        [Command("pokemon")]
        [Summary("Gets the information on a Pokémon.")]
        public async Task Pokemon([Remainder] string pokemonName = null)
        {
            string speciesConvert = "";
            string abilitiesConvert = "";
            string typeConvert = "";

            if (pokemonName == null)
            {
                await ReplyAsync("provide a pokemon name");
                return;
            }

            pokemonName = pokemonName.Replace(" ", "%20");

            GetPokemon getPokemon = null;

            using (WebClient webClient = new WebClient())
            {
                try
                {
                    webClient.UseDefaultCredentials = true;
                    string json = webClient.DownloadString("https://some-random-api.ml/pokedex?pokemon=" + pokemonName);
                    getPokemon = JsonConvert.DeserializeObject<GetPokemon>(json.Substring(1, json.Length - 2));
                }
                catch (Exception ex)
                {
                    await ReplyAsync($"that wasnt a pokemon");
                    Console.WriteLine(ex);
                }
            }

            for (int i = 0; i < getPokemon.Species.Length; i++) //Makes Species String
            {
                speciesConvert += $"{getPokemon.Species[i]} ";
            }

            for (int i = 0; i < getPokemon.Type.Length; i++) //Makes Types String
            {
                if (i == getPokemon.Type.Length - 1)
                {
                    typeConvert += $"{getPokemon.Type[i]}";
                }
                else
                {
                    typeConvert += $"{getPokemon.Type[i]} / ";
                }
            }

            for (int i = 0; i < getPokemon.Abilities.Length; i++) //Makes Abilities String
            {
                if (getPokemon.Abilities.Length == 1) //For Pokémon with 1 ability
                {
                    abilitiesConvert += $"{getPokemon.Abilities[i]}";
                }
                else //For Pokémon with >1 abilities
                {
                    if (i == getPokemon.Abilities.Length - 1)
                    {
                        abilitiesConvert += $"HA: {getPokemon.Abilities[i]}";
                    }
                    else
                    {
                        abilitiesConvert += $"{getPokemon.Abilities[i]} / ";
                    }
                }
            }

            string[] pokemonInfo = { $"{getPokemon.Name}",
                $"{getPokemon.Id}",
                $"{getPokemon.Height}",
                $"{getPokemon.Weight}",
                $"{speciesConvert}",
                $"{abilitiesConvert}",
                $"{typeConvert}",
                $"{getPokemon.Sprites.Normal}",};

            for (int i = 0; i < pokemonInfo.Length; i++)
            {
                var bytes = Encoding.Default.GetBytes(pokemonInfo[i]); //Fixes â€™ (formatting) in the original string from the API (kind of annoying)
                pokemonInfo[i] = Encoding.UTF8.GetString(bytes);
            }

            var foot = new EmbedFooterBuilder()
            {
                Text = $"Powered by https://some-random-api.ml/",
            };

            var embed = new EmbedBuilder();
            embed.Title = char.ToUpper(pokemonInfo[0][0]) + pokemonInfo[0].Substring(1);
            embed.Footer = foot;
            embed.ThumbnailUrl = pokemonInfo[7];
            embed.WithColor(new Color(249, 51, 24))
            .AddField(y =>
            {
                y.Name = "ID";
                y.Value = pokemonInfo[1];
                y.IsInline = true;
            })
            .AddField(y =>
            {
                y.Name = "Height";
                y.Value = pokemonInfo[2];
                y.IsInline = true;
            })
            .AddField(y =>
            {
                y.Name = "Weight";
                y.Value = pokemonInfo[3];
                y.IsInline = true;
            })
            .AddField(y =>
            {
                y.Name = "Species";
                y.Value = pokemonInfo[4];
                y.IsInline = true;
            })
            .AddField(y =>
            {
                y.Name = "Abilities";
                y.Value = pokemonInfo[5];
                y.IsInline = true;
            })
            .AddField(y =>
            {
                y.Name = "Typing";
                y.Value = pokemonInfo[6];
                y.IsInline = true;
            });

            await ReplyAsync($"", false, embed.Build());
        }

        /*
        [Command("youtubecomment")]
        [Alias("ytc")]
        [Summary("Creates a fake YouTube comment.")]
        public async Task YoutubeComment([Remainder] string comment = null)
        {
            if (comment == null)
            {
                await ReplyAsync("can't make a blank comment, try adding a message");
                return;
            }

            comment = comment.Replace(" ", "%20");

            string imageUrl = "https://some-random-api.ml/canvas/youtube-comment?avatar=" + Context.User.GetAvatarUrl() + $"&username={Context.User.Username}&comment={comment}";

            FakeYoutubeComment fakeYtComment = null;

            using (WebClient webClient = new WebClient())
            {
                try
                {
                    webClient.UseDefaultCredentials = true;
                }
                catch (Exception ex)
                {
                    await ReplyAsync($"doesnt work");
                    Console.WriteLine(ex);
                }
            }

            var foot = new EmbedFooterBuilder()
            {
                Text = $"Powered by https://some-random-api.ml/",
            };

            var embed = new EmbedBuilder()
            {
                Title = "Fake YouTube Comment",
                ImageUrl = $"{imageUrl}",
                Color = new Color(229, 45, 39),
                Footer = foot,
            };

            await ReplyAsync(imageUrl, false, embed.Build());
        }*/

        [Command("animalfact")]
        [Alias("afact")]
        [Summary("Provides a random fact about an animal.")]
        public async Task Fact([Remainder] string animal = null)
        {
            if (animal == null)
            {
                await ReplyAsync($"Possible animal facts are: **dog**, **cat**, **panda**, **fox**, **bird**, **koala**, **racoon**, **kangaroo**, **elephant**, **giraffe**, or **whale**.");
                return;
            }
            else
            {
                RandomFact randomFact = null;

                //Removes Slashes to prevent Link issues
                animal = animal.Replace("/", "");
                animal = animal.Replace(@"\", "");

                using (WebClient webClient = new WebClient())
                {
                    try
                    {
                        webClient.UseDefaultCredentials = true;
                        string json = webClient.DownloadString("https://some-random-api.ml/facts/" + animal);
                        randomFact = JsonConvert.DeserializeObject<RandomFact>(json);
                    }
                    catch (Exception ex)
                    {
                        await ReplyAsync($"That animal does not exist in the API. Please use: **dog**, **cat**, **panda**, **fox**, **bird**, **koala**, **racoon**, **kangaroo**, **elephant**, **giraffe**, or **whale**. If this problem persists, the API doesn't work.");
                        Console.WriteLine(ex);
                    }
                }

                var bytes = Encoding.Default.GetBytes(randomFact.factText); //Fixes â€™ in the original string from the API (kind of annoying)
                var factEncoded = Encoding.UTF8.GetString(bytes);

                Random random = new Random();
                int redRnd = random.Next(0, 255);
                Random random2 = new Random();
                int greenRnd = random.Next(0, 255);
                Random random3 = new Random();
                int blueRnd = random.Next(0, 255);

                var auth = new EmbedAuthorBuilder()
                {
                    Name = $"Random {char.ToUpper(animal[0])}{animal.Substring(1)} Fact",
                };

                var embed = new EmbedBuilder()
                {
                    Color = new Color(redRnd, greenRnd, blueRnd),
                    Author = auth,
                    Description = factEncoded,
                };

                await ReplyAsync("", false, embed.Build());
            }
        }
    }
}

