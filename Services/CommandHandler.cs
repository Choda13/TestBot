using System;
using System.Text;
using System.Collections.Generic;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace TestBot.Services
{
    public class CommandHandler
    {
        public static IServiceProvider _provider;
        public static DiscordSocketClient _discord;
        public static CommandService _commands;
        public static IConfigurationRoot _config;
        public CommandHandler(DiscordSocketClient discord, CommandService commands, IConfigurationRoot config, IServiceProvider provider)
        {
            _provider = provider;
            _discord = discord;
            _commands = commands;
            _config = config;

            _discord.Ready += OnReady;
            _discord.MessageReceived += OnMessageRecived;
            _commands.Log += CommandLog;
        }

        private Task CommandLog(LogMessage arg)
        {
            Console.WriteLine(arg.Message);
            return Task.CompletedTask;
        }
        private Task OnReady()
        {
            Console.WriteLine("Connected as " + _discord.CurrentUser.Username + "#" + _discord.CurrentUser.Discriminator);
            return Task.CompletedTask;
        }
        private async Task OnMessageRecived(SocketMessage arg)
        {
            var msg = arg as SocketUserMessage;

            if (msg.Author.IsBot)return;
            var context = new SocketCommandContext(_discord, msg);

            int argPos = 0;
            if (!msg.HasStringPrefix(_config["prefix"], ref argPos) && !msg.HasMentionPrefix(_discord.CurrentUser, ref argPos))return;

            var result = await _commands.ExecuteAsync(context, argPos, _provider);

            if(!result.IsSuccess)
            {
                await context.Channel.SendMessageAsync(result.ErrorReason);
            }
        }
    }
}
