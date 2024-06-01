using Discord.Interactions;
using Discord.WebSocket;
using Discord;
using Zordon_Bot.Handlers;

namespace Zordon_Bot;

public class BotWorker : BackgroundService
{
    private readonly ILogger<BotWorker> _logger;
    private readonly IConfiguration _configuration;
    private readonly DiscordSocketClient _client;
    private readonly InteractionHandler _handler;
    private readonly InteractionService _commands;

    public BotWorker(ILogger<BotWorker> logger,
        IConfiguration configuration,
        InteractionService commands,
        DiscordSocketClient client,
        InteractionHandler handler)
    {
        _logger = logger;
        _configuration = configuration;
        _commands = commands;
        _client = client;
        _handler = handler;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                string discordToken = _configuration["DiscordToken"] ?? throw new Exception("Missing Discord token");

                _logger.LogInformation($"Starting up with token {discordToken}");

                // Subscribe the logging handler to both the client and the CommandService.
                _client.Log += Log;
                _commands.Log += Log; // TODO: ?? Why is the logger not working just for InteractionService??

                // Process when the client is ready, so we can register our commands.
                _client.Ready += _handler.InitializeAsync;

                await _client.LoginAsync(TokenType.Bot, discordToken);
                await _client.StartAsync();

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing contract feed.");
                await StopAsync();
            }
        }

        await StopAsync();
    }

    private async Task StopAsync()
    {
        _logger.LogInformation("Shutting down");

        if (_client != null)
        {
            await _client.LogoutAsync();
            await _client.StopAsync();
        }
    }

    private static Task Log(LogMessage message)
    {
        switch (message.Severity)
        {
            case LogSeverity.Critical:
            case LogSeverity.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            case LogSeverity.Warning:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case LogSeverity.Info:
                Console.ForegroundColor = ConsoleColor.White;
                break;
            case LogSeverity.Verbose:
            case LogSeverity.Debug:
                Console.ForegroundColor = ConsoleColor.DarkGray;
                break;
        }
        Console.WriteLine($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message} {message.Exception}");
        Console.ResetColor();

        return Task.CompletedTask;
    }
}
