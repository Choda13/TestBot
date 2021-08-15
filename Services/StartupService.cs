using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace TestBot.Services
{
    public class StartupService
    {
        public static IServiceProvider _provider;
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly IConfigurationRoot _config;

        public StartupService(IServiceProvider provider, DiscordSocketClient discord, CommandService commands, IConfigurationRoot config)
        {
            _provider = provider;
            _discord = discord;
            _commands = commands;
            _config = config;
        }

        public async Task StartAsync()
        {
            string token = _config["tokens:discord"];
            if(string.IsNullOrEmpty(token))
            {
                Console.WriteLine("Nemas token u _config.yml");
                return;
            }
            await _discord.LoginAsync(TokenType.Bot, token);
            await _discord.StartAsync();
            _discord.Log += DiscordLog;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        }
        private Task DiscordLog(LogMessage arg)
        {
            Console.WriteLine(arg.Message);
            return Task.CompletedTask;
        }
    }
}
