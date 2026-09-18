using KazakhSrs.Bot.Telegram;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace KazakhSrs.Bot.Controllers;

[ApiController]
[Route("bot")]
public class WebhookController : ControllerBase
{
    private readonly IUpdateDispatcher _dispatcher;
    private readonly IConfiguration _configuration;

    public WebhookController(IUpdateDispatcher dispatcher, IConfiguration configuration)
    {
        _dispatcher = dispatcher;
        _configuration = configuration;
    }

    /// <summary>
    /// Telegram webhook entry point. Acks fast (200 OK) after handing the update to the
    /// dispatcher — current commands are quick DB calls, so handling stays inline for now.
    /// When the slow AI-based card-creation flow lands, this is where a background
    /// queue would go instead of awaiting the dispatcher directly.
    /// </summary>
    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook(
        [FromBody] Update update,
        [FromHeader(Name = "X-Telegram-Bot-Api-Secret-Token")] string? secretToken,
        CancellationToken ct)
    {
        var expectedSecret = _configuration["KAZAKHSRS_WEBHOOK_SECRET"];
        if (string.IsNullOrEmpty(expectedSecret) || secretToken != expectedSecret)
            return Unauthorized();

        await _dispatcher.DispatchAsync(update, ct);
        return Ok();
    }
}
