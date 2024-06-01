using Discord.Interactions;
using Discord.WebSocket;
using Discord;
using System.Reflection;

namespace Zordon_Bot.Handlers;

public class InteractionHandler
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _commands;
    private readonly IServiceProvider _services;
    private readonly IConfiguration _config;

    public InteractionHandler(DiscordSocketClient client,
        InteractionService commands,
        IServiceProvider services,
        IConfiguration config)
    {
        _client = client;
        _commands = commands;
        _services = services;
        _config = config;

    }

    public async Task InitializeAsync()
    {
        // Add the public modules that inherit InteractionModuleBase<T> to the InteractionService
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

        if (IsDebug() && _config["GuildId"] is not null)
        {
            ulong guildId = UInt64.Parse(_config["GuildId"]);
            await _commands.RegisterCommandsToGuildAsync(guildId, true);
        }
        else
        {
            Console.WriteLine("!!!RegisterCommandsGlobally");
            await _commands.RegisterCommandsGloballyAsync(true);
        }

        // Process the InteractionCreated payloads to execute Interactions commands
        _client.InteractionCreated += HandleInteraction;

        _commands.InteractionExecuted += HandleInteractionExecuted;
    }

    private async Task HandleInteraction(SocketInteraction arg)
    {
        try
        {
            // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules
            var ctx = new SocketInteractionContext(_client, arg);
            await _commands.ExecuteCommandAsync(ctx, _services);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);

            // If a Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
            // response, or at least let the user know that something went wrong during the command execution.
            if (arg.Type == InteractionType.ApplicationCommand)
                await arg.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
        }
    }

    private Task HandleInteractionExecuted(ICommandInfo arg1, Discord.IInteractionContext arg2, Discord.Interactions.IResult arg3)
    {
        return Task.CompletedTask;
    }

    private static bool IsDebug()
    {
#if DEBUG
        return true;
#else
        return false;
#endif
    }
}
