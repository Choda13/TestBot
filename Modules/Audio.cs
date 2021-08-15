using System;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using TestBot.Services;
namespace TestBot.Modules
{
    public class Audio : ModuleBase<SocketCommandContext>
    {
        public AudioService AudioService { get; set; }
        public static string AudioPath = "C:\\Users\\JOHN_SNOW\\source\\repos\\TestBot\\audio\\";
        public static string ImagePath = "C:\\Users\\JOHN_SNOW\\source\\repos\\TestBot\\img\\";
        [Command("cvijo")]
        public async Task CvijaAsync()
        {
            await AudioService.ClearQueue(Context.Guild.AudioClient);
            await AudioService.Skip(Context.Guild.AudioClient);
            await Context.Message.Channel.SendFileAsync(ImagePath + "cvija.gif");
            await Task.Delay(1000);
            await AudioService.PlaySong(Context, AudioPath + "audio.mp3");
        }
        [Command("play")]
        public async Task PlaySong()
        {
            var VoiceChannel = (Context.User as SocketGuildUser).VoiceChannel;
            if (VoiceChannel == null)
            {
                await ReplyAsync("Budi u kanalu moronu");
                return;
            }
            await AudioService.PlaySong(Context, AudioPath+"audio.mp3");
        }
        [Command("alah")]
        public async Task AlahaAsync()
        {
            var VoiceChannel = (Context.User as SocketGuildUser).VoiceChannel;
            if (VoiceChannel == null){await ReplyAsync("Budi u kanalu moronu");return;}
            await AudioService.PlaySong(Context, AudioPath + "bashar.mp3");
        }
        [Command("skip")]
        public async Task Skip()
        {
            if(Context.Guild.AudioClient == null || Context.Guild.AudioClient.ConnectionState == ConnectionState.Disconnected) { await ReplyAsync("Nisam u kanalu majmuneee"); return; }
            await AudioService.Skip(Context.Guild.AudioClient);
        }
        [Command("disconnect")]
        [Alias("odjebi","odjebi cvijo","bezi")]
        public async Task DisconnectAsync()
        {
            await AudioService.DisconnectAsync(Context.Guild.AudioClient);
        }
        [Command("clear")]
        public async Task ClearQueue()
        {
            if(Context.Guild.AudioClient == null || Context.Guild.AudioClient.ConnectionState == ConnectionState.Disconnected)
            {
                await ReplyAsync("Nisam u kanalu majmuneee");
                return;
            }
            await AudioService.ClearQueue(Context.Guild.AudioClient);
        }
        [Command("queue")]
        public Task ShowQueue()
        {
            if (Context.Guild.AudioClient == null || Context.Guild.AudioClient.ConnectionState == ConnectionState.Disconnected) return Task.CompletedTask;
            var queue = AudioService.ShowQueue(Context.Guild.AudioClient);
            
            return Task.CompletedTask;
        } 
    }
    //
    //STOJAN MODULE
    //
    [Group("stojan")]
    [Summary("Sve komande koje su vezane za stojana")]
    public class Stojan : ModuleBase<SocketCommandContext>
    {
        string AudioPath = Audio.AudioPath;
        public AudioService AudioService { get; set; }

        [Command("rdjo")]
        [Alias("rđo")]
        [Summary("Stojan lično ulazi u kanal i kaže rđooo jednaa")]
        public async Task RđoAsync()
        {
            var channel = (Context.User as IGuildUser).VoiceChannel;
            if (channel == null) { await ReplyAsync("Nisi u kanalu rđo jedna"); return; }
            await AudioService.PlaySong(Context, AudioPath + "rdjo.mp3");
        }
        [Command("to lutko")]
        public async Task LutkoAsync()
        {
            var channel = (Context.User as IGuildUser).VoiceChannel;
            if (channel == null) { await ReplyAsync("Nisi u kanalu rđo jedna"); return; }
            await AudioService.PlaySong(Context, AudioPath + "tolutko.mp3");
        }
        [Command("sapunjas")]
        [Alias("sapunjaš")]
        public async Task SapunjasAsync()
        {
            var channel = (Context.User as IGuildUser).VoiceChannel;
            if (channel == null) { await ReplyAsync("Nisi u kanalu rđo jedna"); return; }
            await AudioService.PlaySong(Context, AudioPath + "sapunjas_macora.mp3");
        }
        [Command("sveti petar")]
        public async Task SvetiPetarAsync()
        {
            var channel = (Context.User as IGuildUser).VoiceChannel;
            if (channel == null) { await ReplyAsync("Nisi u kanalu rđo jedna"); return; }
            await AudioService.PlaySong(Context, AudioPath + "sveti_petar.mp3");
        }
    }
}
