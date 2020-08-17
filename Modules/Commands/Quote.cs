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
    public class Quote : ModuleBase
    {
        //Discord Command Service Setup
        private CommandService _service;
        public Quote(CommandService service)
        {
            _service = service;
        }

        // Begin Commands
        [Command("createquote")] // This command will be removed in a future update.
        [Summary("Allows a user to create a quote.")]
        public async Task CreateQuote([Remainder] SocketGuildUser mentionUser = null)
        {
            var userId = Context.User.Id;
            string userPath = @"BotDatabase\QuoteFiles\" + userId + ".txt";

            if (!File.Exists(userPath))
            {
                File.Create(@"BotDatabase\QuoteFiles\" + userId + ".txt");

                await Context.Channel.SendMessageAsync("devlog: file created <a:partywumpus:690374298661027891>");
            }
            else
            {
                await ReplyAsync($"devlog: you have a quote already <a:partywumpus:690374298661027891>");
            }
        }

        [Command("removequote")]
        [Summary("Deletes the database section a user's account is registered in.")]
        public async Task RemoveQuote([Remainder] SocketGuildUser mentionUser = null)
        {
            var userId = Context.User.Id;
            string userPath = @"BotDatabase\QuoteFiles\" + userId + ".txt";

            if (File.Exists(userPath))
            {
                File.Delete(@"BotDatabase\QuoteFiles\" + userId + ".txt");

                await Context.Channel.SendMessageAsync("devlog: file deleted <a:cowboypepe:690374238544330792>");
            }
            else
            {
                await ReplyAsync($"devlog: you dont have a quote <a:cowboypepe:690374238544330792>");
            }
        }

        [Command("quote")]
        [Summary("Displays a your quote or a user's quote.")]
        public async Task GetQuote([Remainder] SocketGuildUser mentionUser = null)
        {
            if (mentionUser == null)
            {
                var userId = Context.User.Id;
                string userPath = @"BotDatabase\QuoteFiles\" + userId + ".txt";

                List<string> theQuote = File.ReadAllLines(userPath).ToList();

                if (!File.Exists(userPath))
                {
                    await ReplyAsync("devlog: user file doesn't exist <a:partypepe:690374178595143700>");
                }
                else
                {
                    await ReplyAsync($"> {theQuote[0]} \n" +
                        $" - {Context.User.Mention}");
                }
            }
            else
            {
                var userId = mentionUser.Id;
                string userPath = @"BotDatabase\QuoteFiles\" + userId + ".txt";

                List<string> theQuote = File.ReadAllLines(userPath).ToList();

                if (!File.Exists(userPath))
                {
                    await ReplyAsync("devlog: user file doesn't exist <a:partypepe:690374178595143700>");
                }
                else
                {
                    await ReplyAsync($"> {theQuote[0]} \n" +
                        $" - {mentionUser.Mention}");
                }
            }
        }

        [Command("changemyquote")]
        [Alias("cmq")]
        [Summary("Changes your quote.")]
        public async Task ChangeMyQuote([Remainder, Summary("This will be your quote")] string quote = null)
        {
            var userId = Context.User.Id;
            string userPath = @"BotDatabase\QuoteFiles\" + userId + ".txt";

            if (quote == null)
            {
                await ReplyAsync("can't have a blank quote");
                return;
            }

            if (!File.Exists(userPath))
            {
                await ReplyAsync($"devlog: no file exists for user `{userId}` <a:partypepe:690374178595143700>");
            }
            else
            {
                List<string> theQuote = File.ReadAllLines(userPath).ToList();

                theQuote.Clear();
                theQuote.Add($"{quote}");

                File.WriteAllLines(userPath, theQuote);

                await ReplyAsync($"<a:partypepe:690374178595143700> {Context.User.Mention}, You have changed your quote to: '{quote}'");
            }
        }
    }
}
