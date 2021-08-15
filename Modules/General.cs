using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
namespace TestBot.Modules
{
    public class General : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task PingAsync()
        {
            await ReplyAsync(Context.User.Mention + " Pong!");
        }
        [Command("echo")]
        [Alias("ponovi", "reci", "napisi", "napiši")]
        public async Task EchoAsync([Remainder] string toEcho)
        {
            await ReplyAsync(toEcho);
        }
        [Command("info")]
        public async Task InfoEmbedAsync(SocketGuildUser arg = null)
        {
            var user = Context.User;
            var guild = Context.Guild;
            var guild_user = user as IGuildUser;
            if (arg != null)
            {
                user = arg as SocketUser;
                guild_user = arg;
            }
            string activity = "Ništa";

            if (user.Activity != null)
            {
                switch (user.Activity.Type)
                {
                    case ActivityType.CustomStatus: activity = ""; break;
                    case ActivityType.Listening: activity = "Sluša "; break;
                    case ActivityType.Playing: activity = "Igra "; break;
                    case ActivityType.Streaming: activity = "Strimuje "; break;
                    case ActivityType.Watching: activity = "Gleda "; break;
                }
                activity += user.Activity.Name;
            }
            var builder = new EmbedBuilder()
            .WithThumbnailUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
            .WithTitle("Info o ciganu sa imenom: " + user.Username + "#" + user.Discriminator)
            .WithColor(Color.DarkBlue)
            .AddField("Nadimak:", guild_user.Nickname ?? "nema")
            .AddField("Trenutno radi:", activity)
            .AddField("Uloge:", string.Join(", ", (user as SocketGuildUser).Roles.Select(x => x.Mention)))
            .AddField("Cigan od:", guild_user.JoinedAt.Value.ToString("dd-MM-yyyy"))
            .WithFooter(footer => footer.Text = "Cvija")
            .WithCurrentTimestamp();

            if (user.Username == "Quev") builder.AddField("Biografija", "Dejana");
            else if (user.Username == "bonnie🤡") builder.AddField("Biografija", "Partner in crime");
            else if (user.Username == "Зурле") builder.AddField("Biografija", "Gizelle");
            else if (user.Username == "Momir") builder.AddField("Biografija", "U bekstvu od vepra");
            else if (user.Username == "BoobBot™") builder.AddField("Biografija", "STD");
            else if (user.Username == "Dejana") builder.AddField("Biografija", "Vepar");
            else if (user.Username == "Sn1pY") builder.AddField("Biografija", "Kornjača");
            else builder.AddField("Biografija", "Virgin");

            await Context.Channel.SendMessageAsync(embed: builder.Build());
        }
        [Command("help")]
        public async Task HelpAsync()
        {
            var builder = new EmbedBuilder();
            builder.WithTitle("Ubij se debilu <:bakugan4:827140983522328606>")
                .AddField("Pod jedan!", "Prestani da govoris anglicizme ko debil neki nego lepo ukucaj <prefix>pomoc, mentolu zaostali," +
                " evo ti malo da se [edukujes](https://www.vokabular.org/)", true)
                .AddField("Pod dva!!", "Retarde, nema ti pomoci znaci pomoc == 0, pomoc == null, bar ne za tebe, bolje da se ubijes", true)
                .AddField("Pod tri!!!", "Ako si stvarno debil da si bar malo razmotrio savet br.2" +
                " evo ti [link](https://www.teenink.com/fiction/realistic_fiction/article/957487/How-To-Kill-Yourself-for-beginners/) " +
                "imbecilu malo da se opametis.", true)
                .WithColor(Color.DarkRed)
                .WithAuthor(Context.Client.CurrentUser)
                .WithFooter("Ko duva zmaj ga cuva", Context.Client.CurrentUser.GetAvatarUrl())
                .WithCurrentTimestamp();
            await Context.Channel.SendMessageAsync(embed: builder.Build());
        }
    }
}
