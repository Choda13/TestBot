using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;

namespace TestBot.Services
{
    public class AudioClient
    {
        public IAudioClient audio_client;
        public List<string> song_queue;
        public CancellationTokenSource tokenSource;
        public AudioOutStream AudioOutStream;
        public AudioClient() 
        {
            AudioOutStream = null;
            audio_client = null;
            song_queue = new List<string>();
            tokenSource = null;
        }
        public AudioClient(IAudioClient client)
        {
            audio_client = client;
            song_queue = new List<string>();
            tokenSource = null;
        }
        public void SetOnDisconnectedHandler()
        {
            audio_client.Disconnected += OnDisconnect;
        }
        private Task OnDisconnect(Exception arg)
        {
            song_queue.Clear();
            audio_client.Dispose();
            return Task.CompletedTask;
        }
    }
	public class AudioService
	{
		IAudioClient audio_client;
        List<AudioClient> audio_clients = new List<AudioClient>();
        public AudioService() { }
		public async Task PlaySong(SocketCommandContext context, string path)
        {
            if (context.Guild.AudioClient == null || context.Guild.AudioClient.ConnectionState == ConnectionState.Disconnected) {
                try 
                {
                    audio_client = await (context.User as IGuildUser).VoiceChannel.ConnectAsync();
                    audio_clients.Add(new AudioClient(audio_client));
                }
                catch(Exception e) 
                { 
                    Console.WriteLine(e.Message);
                }
                audio_clients.Find(x => x.audio_client == audio_client).SetOnDisconnectedHandler();
                Task.Run(() => StartPlayingQueue(audio_clients.Find(x => x.audio_client == context.Guild.AudioClient)));
            }
            audio_clients.Find(x => x.audio_client == context.Guild.AudioClient).song_queue.Add(path);
            return;
        }
        private Task StartPlayingQueue(AudioClient client)
        {
            while (client.audio_client.ConnectionState == ConnectionState.Connected)
            {
                while(client.song_queue.Count > 0)
                {
                    try 
                    {
                        PlayAsync(client,client.song_queue.First()).Wait();
                        if(client.song_queue.Count > 0)
                            client.song_queue.RemoveAt(0);
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    
                }
            }
            Console.WriteLine("Ja gotov");
            return Task.CompletedTask;
        }
        private async Task PlayAsync(AudioClient client, string it)
        {
            client.tokenSource = new CancellationTokenSource();
            if (client.AudioOutStream == null) client.AudioOutStream = client.audio_client.CreatePCMStream(AudioApplication.Mixed);
            var audio_stream = client.AudioOutStream;
            var process = CreateStream(it);
            var song_stream = process.StandardOutput.BaseStream;
            try { await song_stream.CopyToAsync(audio_stream, client.tokenSource.Token); }
            catch (Exception e){ Console.WriteLine(e.Message); }
            finally { await audio_stream.FlushAsync();}
        }
        public Process CreateStream(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });

        }
        public Task Skip(IAudioClient client)
        {
            if (client == null) return Task.CompletedTask;
            AudioClient audio = audio_clients.Find(x => x.audio_client == client);
            if (audio == null || audio.audio_client.ConnectionState != ConnectionState.Connected || audio.tokenSource == null) return Task.CompletedTask;
            audio.tokenSource.Cancel();
            audio.tokenSource.Dispose();
            return Task.CompletedTask;
        }
        public Task ClearQueue(IAudioClient client)
        {
            if (client == null) return Task.CompletedTask;
            var audio = audio_clients.Find(x => x.audio_client == client);
            if(audio.audio_client.ConnectionState == ConnectionState.Disconnected || audio == null) return Task.CompletedTask;
            audio.song_queue.Clear();
            return Task.CompletedTask;
        }
        public List<string> ShowQueue(IAudioClient client)
        {
            var audio = audio_clients.Find(x => x.audio_client == client);
            if (audio.audio_client.ConnectionState == ConnectionState.Disconnected || audio == null) return null;
            return audio.song_queue;
        }
        public async Task DisconnectAsync(IAudioClient client)
        {
            await client.StopAsync();
        }
    }
}