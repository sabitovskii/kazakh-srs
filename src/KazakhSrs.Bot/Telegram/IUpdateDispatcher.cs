using Telegram.Bot.Types;

namespace KazakhSrs.Bot.Telegram;

/// <summary>Routes an incoming Telegram Update to the right command/callback handler.</summary>
public interface IUpdateDispatcher
{
    Task DispatchAsync(Update update, CancellationToken ct);
}
