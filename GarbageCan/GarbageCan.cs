﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Config.Net;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GarbageCan
{
    internal static class GarbageCan
    {
        public static DiscordClient Client;
        public static CommandsNextExtension Commands;
        public static IBotConfig Config;

        public static DiscordEmoji Check;

        private static List<IFeature> _botFeatures;
        private static bool _shutdown;

        private static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .CreateLogger();

                var logFactory = new LoggerFactory().AddSerilog();

                Log.Information("Reading config...");
                BuildConfig();

                Client = new DiscordClient(new DiscordConfiguration
                {
                    Token = Config.token,
                    TokenType = TokenType.Bot,
                    LoggerFactory = logFactory,
                    MinimumLogLevel = LogLevel.Debug
                });

                Log.Information("Initializing features...");
                _botFeatures = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                    .Where(x => typeof(IFeature).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                    .Select(x => (IFeature) Activator.CreateInstance(x))
                    .ToList();

                foreach (var feature in _botFeatures)
                {
                    Log.Information("Feature " + feature.GetType().Name + " found, attempting to initialize...");
                    try
                    {
                        feature.Init(Client);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.ToString());
                        Environment.Exit(1);
                    }
                    Log.Information("Success!");
                }

                Commands = Client.UseCommandsNext(new CommandsNextConfiguration
                {
                    StringPrefixes = new[] {Config.commandPrefix}
                });

                Commands.RegisterCommands(Assembly.GetExecutingAssembly());

                Client.Ready += (_, _) =>
                {
                    Check = DiscordEmoji.FromName(Client, ":white_check_mark:");
                    return Task.CompletedTask;
                };

                await Client.ConnectAsync();

                AppDomain.CurrentDomain.ProcessExit += Shutdown;
                AppDomain.CurrentDomain.DomainUnload += Shutdown;
                Console.CancelKeyPress += Shutdown;
            });

            while (!_shutdown)
            {
            }
        }

        public static void BuildConfig()
        {
            Config = new ConfigurationBuilder<IBotConfig>()
                .UseJsonFile("dev.json")
                .UseEnvironmentVariables()
                .Build();

            if (Config == null) throw new NullReferenceException("Attempted to build config, but got null");
        }

        private static void Shutdown(object? sender, EventArgs eventArgs)
        {
            if (_shutdown || Environment.ExitCode > 0) return;
            Log.Information("Shutting down...");

            foreach (var feature in _botFeatures) feature.Cleanup();
            Client.UpdateStatusAsync(null, UserStatus.Offline).GetAwaiter().GetResult();
            Client.Dispose();

            _shutdown = true;
        }
    }
}