using Discord;
using Discord.Commands;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Modules;

namespace Oppai_Bot
{
    class Oppai
    {
        DiscordClient discord;

        private readonly Random rng = new Random();

        public Oppai()
        {
            discord = new DiscordClient(x =>
            {
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = Log;
            });

            discord.UsingCommands(x =>
            {
                x.PrefixChar = '>';
                x.AllowMentionPrefix = true;
            });

            discord.GetService<CommandService>().CreateGroup("", cgb =>
            {
                cgb.CreateCommand("rule34")
                    .Parameter("tag", ParameterType.Unparsed)
                    .Do(async e =>
                    {
                        var tag = e.GetArg("tag")?.Trim() ?? "";
                        var link = await SearchHelper.GetRule34ImageLink(tag).ConfigureAwait(false);
                        if (string.IsNullOrWhiteSpace(link))
                            await e.Channel.SendMessage("Search yielded no results ;(");
                        else
                            await e.Channel.SendMessage(link).ConfigureAwait(false);
                    });

                cgb.CreateCommand("hentai")
                   .Parameter("tag", ParameterType.Unparsed)
                    .Do(async e =>
                    {
                        var tag = e.GetArg("tag")?.Trim() ?? "";

                        var links = await Task.WhenAll(SearchHelper.GetDanbooruImageLink("rating%3Aexplicit+" + tag), SearchHelper.GetGelbooruImageLink("rating%3Aexplicit+" + tag)).ConfigureAwait(false);

                        if (links.All(l => l == null))
                        {
                            await e.Channel.SendMessage("`No results.`");
                            return;
                        }

                        await e.Channel.SendMessage(String.Join("\n\n", links)).ConfigureAwait(false);
                    });

                cgb.CreateCommand("e621")
                    .Parameter("tag", ParameterType.Unparsed)
                    .Do(async e =>
                    {
                        var tag = e.GetArg("tag")?.Trim() ?? "";
                        await e.Channel.SendMessage(await SearchHelper.GetE621ImageLink(tag).ConfigureAwait(false)).ConfigureAwait(false);
                    });

                cgb.CreateCommand("boobs")
                    .Do(async e =>
                    {
                        try
                        {
                            var obj = JArray.Parse(await SearchHelper.GetResponseStringAsync($"http://api.oboobs.ru/boobs/{rng.Next(0, 9380)}").ConfigureAwait(false))[0];
                            await e.Channel.SendMessage($"http://media.oboobs.ru/{ obj["preview"].ToString() }").ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            await e.Channel.SendMessage($"💢 boobs {ex.Message}").ConfigureAwait(false);
                        }
                    });

                cgb.CreateCommand("butts")
                    .Do(async e =>
                    {
                        try
                        {
                            var obj = JArray.Parse(await SearchHelper.GetResponseStringAsync($"http://api.obutts.ru/butts/{rng.Next(0, 3373)}").ConfigureAwait(false))[0];
                            await e.Channel.SendMessage($"http://media.obutts.ru/{ obj["preview"].ToString() }").ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            await e.Channel.SendMessage($"💢 butts {ex.Message}").ConfigureAwait(false);
                        }
                    });

            });

            discord.ExecuteAndWait(async () =>
            {
                await discord.Connect("MjI1MTY4NDEzMDYyODU2NzA1.CrlH7w.25dqpDyd1j_TlIJg0DeYNdVFfxY", TokenType.Bot);
            });
        }

        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
