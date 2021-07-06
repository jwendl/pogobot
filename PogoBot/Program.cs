using Discord;
using Discord.Commands;
using Discord.WebSocket;
using EasyDatabase.Core;
using EasyDatabase.Core.Interfaces;
using EasyDatabase.Core.Services;
using EasyDatabase.FileRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PogoBot.Configuration;
using PogoBot.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PogoBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true)
                .AddEnvironmentVariables()
                .Build();

            var discordSettings = new DiscordSettings(); ;
            configuration.Bind(nameof(DiscordSettings), discordSettings);

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<DiscordSocketClient>();
            serviceCollection.AddSingleton<CommandService>();
            serviceCollection.AddSingleton<CommandHandlingService>();
            serviceCollection.AddScoped<IRepository, FileRepository>();
            serviceCollection.AddScoped<Service>();
            serviceCollection.AddScoped<Storage>();

            using var serviceProvider = serviceCollection.BuildServiceProvider();
            var commandService = serviceProvider.GetRequiredService<CommandService>();
            commandService.Log += (logMessage) =>
            {
                Console.WriteLine(logMessage.ToString());

                return Task.CompletedTask;
            };

            var discordSocketClient = serviceProvider.GetRequiredService<DiscordSocketClient>();
            await discordSocketClient.LoginAsync(TokenType.Bot, discordSettings.Token);
            await discordSocketClient.StartAsync();

            var commandHandlingService = serviceProvider.GetRequiredService<CommandHandlingService>();
            await commandHandlingService.InitializeAsync();

            await Task.Delay(Timeout.Infinite);
        }
    }
}
