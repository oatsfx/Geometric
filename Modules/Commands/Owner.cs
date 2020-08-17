using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using Geometric;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Dynamic;

namespace GeometricDiscordBot.Modules.Commands
{
    public class Owner : ModuleBase
    {
        private CommandService _service;
        public Owner(CommandService service)
        {
            _service = service;
        }

        readonly string emoji1 = $"<a:verified:690374136526012506> -> `<a:verified:690374136526012506>`\n" +
            $"<a:success:690374332723101757> -> `<a:success:690374332723101757>`\n" +
            $"<a:ping:690374270441750558> -> `<a:ping:690374270441750558>`\n" +
            $"<a:partywumpus:690374298661027891> -> `<a:partywumpus:690374298661027891>`\n" +
            $"<a:partypepe:690374178595143700> -> `<a:partypepe:690374178595143700>`\n" +
            $"<a:partyparrot:690374106280886282> -> `<a:partyparrot:690374106280886282>`\n" +
            $"<a:cowboypepe:690374238544330792> -> `<a:cowboypepe:690374238544330792>`";

        readonly string emoji2 = $"<a:chaoscontrol:691431189038497922> -> `<a:chaoscontrol:691431189038497922>`\n" +
            $"<a:owowhatsthis:691429502764449883> -> `<a:owowhatsthis:691429502764449883>`\n" +
            $"<a:furretwalk:691429975710105601> -> `<a:furretwalk:691429975710105601>`\n" +
            $"<:member_join:690401890860662794> -> `<:member_join:690401890860662794>`\n" +
            $"<:member_leave:743979522990538862> -> `<:member_leave:743979522990538862>`";

        readonly string emoji3 = $"<:yes:742446247314718801> -> `<:yes:742446247314718801>`\n" +
            $"<:no:742446247515914371> -> `<:no:742446247515914371>`\n" +
            $"<:warn:742493629624483891> -> `<:warn:742493629624483891>`\n" +
            $"<:download:743981057082392667> -> `<:download:743981057082392667>`";

        [Command("setgame")]
        [Summary("Sets the game of the bot.")]
        [RequireOwner]
        public async Task Setgame([Remainder] string game)
        {
            await (Context.Client as DiscordSocketClient).SetGameAsync(game);
            await Context.Channel.SendMessageAsync("**Successfully set the game to `" + game + "`!**");
            Console.WriteLine($"{DateTime.Now}: Game was changed to {game}");
        }

        [Command("userupdate")]
        [Summary("Updates the amount of Users the bot has.")]
        [RequireOwner]
        public async Task Userupdate()
        {
            var GuildUser = await Context.Guild.GetUserAsync(Context.User.Id);
            if (!(GuildUser.Id == 261121732704862208))
            {
                await Context.Channel.SendMessageAsync("permission denied");
            }
            else
            {
                await (Context.Client as DiscordSocketClient).SetActivityAsync(new Game($"for g.help | {BotConfig.Version} | Users: {(Context.Client as DiscordSocketClient).Guilds.Sum(g => g.Users.Count).ToString()}", ActivityType.Watching));

                //await (Context.Client as DiscordSocketClient).SetGameAsync($"g.cmdlist | {version} | Users: {(Context.Client as DiscordSocketClient).Guilds.Sum(g => g.Users.Count).ToString()}");
            }
        }

        [Command("guildupdate")]
        [Summary("Updates the amount of Guilds the bot is in.")]
        [RequireOwner]
        public async Task Guildupdate()
        {
            var GuildUser = await Context.Guild.GetUserAsync(Context.User.Id);
            if (!(GuildUser.Id == 261121732704862208))
            {
                await Context.Channel.SendMessageAsync("permission denied");
            }
            else
            {
                await (Context.Client as DiscordSocketClient).SetActivityAsync(new Game($"for g.help | {BotConfig.Version} | Servers: {(Context.Client as DiscordSocketClient).Guilds.Count.ToString()}", ActivityType.Watching));

                //await (Context.Client as DiscordSocketClient).SetGameAsync($"g.cmdlist | {version} | Servers: {(Context.Client as DiscordSocketClient).Guilds.Count.ToString()}");
            } 
        }

        [Command("devemoji")] // This is getting changed to be time efficient.
        [Summary("Sends a message with emojis and their IDs.")]
        [RequireOwner]
        public async Task Devtest([Remainder]string input = null)
        {
            var embed = new EmbedBuilder()
            {
                Color = new Color(35, 35, 35),
            };

            embed.Description = $"{emoji3}";

            await Context.Message.DeleteAsync();

            await ReplyAsync("", false, embed.Build());
        }

        [Command("editconfig")]
        [Summary("To edit config.json")]
        [RequireOwner]
        public async Task Editconfig(string section = null, [Remainder] string query = null)
        {
            //-----------------This is for using config.json. DO NOT DELETE, IT BREAKS THE CONFIG SETUP :D
            string json = File.ReadAllText(@"BotDatabase\config.json");
            BotConfig config = JsonConvert.DeserializeObject<BotConfig>(json);

            dynamic jsonObj = JsonConvert.DeserializeObject(json);
            jsonObj[$"{char.ToUpper(section[0])}{section.Substring(1)}"] = $"{query}";
            string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            File.WriteAllText(@"BotDatabase\config.json", output);

            //-----------------This reassigns what we just added.
            json = File.ReadAllText(@"BotDatabase\config.json");
            config = JsonConvert.DeserializeObject<BotConfig>(json);

            await ReplyAsync($"changed {char.ToUpper(section[0])}{section.Substring(1)} to {query}");
        }

        [Command("listguilds")]
        [Summary("Lists all guilds the bot is in.")]
        [RequireOwner]
        public async Task ListGuildsAsync()
        {
            Random random = new Random();
            int redRnd = random.Next(0, 255);
            Random random2 = new Random();
            int greenRnd = random.Next(0, 255);
            Random random3 = new Random();
            int blueRnd = random.Next(0, 255);

            IReadOnlyCollection<IGuild> guilds = await Context.Client.GetGuildsAsync().ConfigureAwait(false);
            var builder = new EmbedBuilder()
                .WithAuthor(Context.Client.CurrentUser)
                .WithColor(new Color(redRnd, blueRnd, greenRnd))
                .WithTitle("Guild List");
            foreach (IGuild guild in guilds)
            {
                int channelCount = (await guild.GetChannelsAsync().ConfigureAwait(false)).Count;
                int userCount = (await guild.GetUsersAsync().ConfigureAwait(false)).Count;
                IGuildUser owner = await guild.GetOwnerAsync().ConfigureAwait(false);
                _ = builder.AddField(guild.Name, $"{guild.Id}\n{owner}", true);
            }
            _ = await ReplyAsync(embed: builder.Build()).ConfigureAwait(false);
        }
    }
}
