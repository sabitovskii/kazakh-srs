using KazakhSrs.Application.Abstractions;
using KazakhSrs.Application.UseCases;
using KazakhSrs.Bot.Telegram;
using KazakhSrs.Infrastructure.Persistence;
using KazakhSrs.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

// Same fallback as KazakhSrsDbContextFactory (design-time tooling) — kept consistent so
// local `dotnet run` "just works" against the same Docker Postgres without extra config.
// A real deployment (Hetzner) must set KAZAKHSRS_CONNECTION_STRING for real.
var connectionString =
    builder.Configuration["KAZAKHSRS_CONNECTION_STRING"]
    ?? "Host=localhost;Database=kazakhsrs;Username=postgres;Password=postgres";

builder.Services.AddDbContext<KazakhSrsDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITopicRepository, TopicRepository>();
builder.Services.AddScoped<INoteRepository, NoteRepository>();
builder.Services.AddScoped<ICardRepository, CardRepository>();
builder.Services.AddScoped<IReviewLogRepository, ReviewLogRepository>();
builder.Services.AddScoped<ReviewCardUseCase>();

builder.Services.AddScoped<IUpdateDispatcher, UpdateDispatcher>();

var botToken = builder.Configuration["KAZAKHSRS_BOT_TOKEN"]
    ?? throw new InvalidOperationException("KAZAKHSRS_BOT_TOKEN is not set.");
builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(botToken));

builder.Services.AddControllers();

var app = builder.Build();

// If KAZAKHSRS_WEBHOOK_URL is set, (re)register the webhook on every startup — idempotent
// on Telegram's side, and convenient for local dev: change the ngrok URL, restart, done.
var webhookUrl = builder.Configuration["KAZAKHSRS_WEBHOOK_URL"];
if (!string.IsNullOrEmpty(webhookUrl))
{
    var botClient = app.Services.GetRequiredService<ITelegramBotClient>();
    var webhookSecret = builder.Configuration["KAZAKHSRS_WEBHOOK_SECRET"];
    await botClient.SetWebhook(webhookUrl, secretToken: webhookSecret);
}

app.MapControllers();

app.Run();
