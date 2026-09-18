using KazakhSrs.Application.Abstractions;
using KazakhSrs.Application.UseCases;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using User = KazakhSrs.Domain.Entities.User;

namespace KazakhSrs.Bot.Telegram;

/// <summary>
/// Handles the three things the skeleton bot understands: /start, /review, and the
/// grade button presses that /review produces. Card-creation flow (Whisper/Claude/
/// ElevenLabs) is a separate, later piece — not here.
/// </summary>
public class UpdateDispatcher : IUpdateDispatcher
{
    private readonly ITelegramBotClient _bot;
    private readonly IUserRepository _users;
    private readonly ICardRepository _cards;
    private readonly ReviewCardUseCase _reviewCard;

    public UpdateDispatcher(
        ITelegramBotClient bot,
        IUserRepository users,
        ICardRepository cards,
        ReviewCardUseCase reviewCard)
    {
        _bot = bot;
        _users = users;
        _cards = cards;
        _reviewCard = reviewCard;
    }

    public async Task DispatchAsync(Update update, CancellationToken ct)
    {
        if (update.Message is { Text: not null } message)
        {
            if (message.Text.StartsWith("/start", StringComparison.OrdinalIgnoreCase))
                await HandleStartAsync(message, ct);
            else if (message.Text.StartsWith("/review", StringComparison.OrdinalIgnoreCase))
                await HandleReviewAsync(message, ct);

            return;
        }

        if (update.CallbackQuery is not null)
        {
            await HandleReviewCallbackAsync(update.CallbackQuery, ct);
        }
    }

    private async Task HandleStartAsync(Message message, CancellationToken ct)
    {
        var telegramId = message.From!.Id;
        var user = await _users.GetByTelegramIdAsync(telegramId, ct);

        if (user is null)
        {
            user = new User(telegramId, DateTimeOffset.UtcNow);
            await _users.AddAsync(user, ct);
        }

        await _bot.SendMessage(
            message.Chat.Id,
            "Сәлем! Бот готов. Набери /review, чтобы начать повторение.",
            cancellationToken: ct);
    }

    private async Task HandleReviewAsync(Message message, CancellationToken ct)
    {
        var telegramId = message.From!.Id;
        var user = await _users.GetByTelegramIdAsync(telegramId, ct);

        if (user is null)
        {
            await _bot.SendMessage(message.Chat.Id, "Сначала набери /start.", cancellationToken: ct);
            return;
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var dueCards = await _cards.GetDueCardsAsync(user.Id, today, ct);

        if (dueCards.Count == 0)
        {
            await _bot.SendMessage(message.Chat.Id, "На сегодня карточек нет.", cancellationToken: ct);
            return;
        }

        var card = dueCards[0];
        var keyboard = new InlineKeyboardMarkup(new[]
        {
            Enumerable.Range(0, 6)
                .Select(grade => InlineKeyboardButton.WithCallbackData(grade.ToString(), $"review:{card.Id}:{grade}"))
                .ToArray()
        });

        await _bot.SendMessage(message.Chat.Id, card.PromptText, replyMarkup: keyboard, cancellationToken: ct);
    }

    private async Task HandleReviewCallbackAsync(CallbackQuery callback, CancellationToken ct)
    {
        var data = callback.Data;
        if (data is null || !data.StartsWith("review:", StringComparison.Ordinal))
            return;

        var parts = data.Split(':');
        if (parts.Length != 3 || !long.TryParse(parts[1], out var cardId) || !int.TryParse(parts[2], out var grade))
            return;

        var card = await _cards.GetByIdAsync(cardId, ct);
        if (card is null)
            return;

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        await _reviewCard.ExecuteAsync(cardId, grade, today, ct: ct);
        await _bot.AnswerCallbackQuery(callback.Id, cancellationToken: ct);

        if (callback.Message is not null)
        {
            await _bot.EditMessageText(
                callback.Message.Chat.Id,
                callback.Message.MessageId,
                $"{card.PromptText}\n\nОтвет: {card.CanonicalAnswer}\nОценка: {grade}",
                cancellationToken: ct);
        }
    }
}
