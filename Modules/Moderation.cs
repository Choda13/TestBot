using System.Linq;
using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace TestBot.Modules
{
    public class Moderation : ModuleBase<SocketCommandContext>
    {
        [Command("cisti")]
        [Alias("purge","suladipe", "šuladipe")]
        public async Task PurgeAsync(int brojPoruka = 100)
        {
            var msgs = await Context.Channel.GetMessagesAsync().FlattenAsync();
            msgs = msgs.Take(Math.Min(brojPoruka, msgs.Count()));
            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(msgs);
            await (Context.Channel as SocketTextChannel).DeleteMessageAsync(Context.Message);
        }
        [Command("cisti")]
        [Alias("purge", "suladipe", "šuladipe")]
        public async Task PurgeAsync(IRole role=null, int brojPoruka = 100)
        {
            var msgs = await Context.Channel.GetMessagesAsync().FlattenAsync();
            msgs = msgs.Where(x => (x.Author as IGuildUser).RoleIds.Any(y => y == role.Id)).Take(brojPoruka+1);    
            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(msgs);
            await (Context.Channel as SocketTextChannel).DeleteMessageAsync(Context.Message);
        }
        [Command("cisti")]
        [Alias("purge", "suladipe", "šuladipe")]
        public async Task PurgeAsync(SocketGuildUser user = null, int brojPoruka = 100, int ok=1)
        {
            var msgs = await Context.Channel.GetMessagesAsync().FlattenAsync();
            msgs = msgs.Where(x => (x.Author as SocketGuildUser) == user).Take(brojPoruka+1);
            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(msgs);
            await (Context.Channel as SocketTextChannel).DeleteMessageAsync(Context.Message);
        }
    }
}
