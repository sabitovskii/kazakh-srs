# KazakhSrs — фаза 1 (Clean Architecture + EF Core)

Ядро SM-2, доменная модель и слой персистентности из плана реализации. Бот, веб-панель и интеграции (Whisper/Claude/ElevenLabs) — следующие фазы.

## Структура

- `src/KazakhSrs.Domain` — сущности (`User`, `Topic`, `Note`, `Card`, `ReviewLog`), `CardType`, а также `Sm2Engine`/`SpacedRepetitionState`. Ноль ссылок на EF Core или Postgres — чистая доменная логика.
- `src/KazakhSrs.Application` — интерфейсы репозиториев (`ICardRepository` и т.п.) и юзкейсы (`ReviewCardUseCase`). Знает только про Domain. Отдельного `IUnitOfWork` нет: каждый `AddAsync` сам коммитит (`SaveChangesAsync`), а поскольку репозитории в рамках одного юзкейса шарят один scoped `DbContext`, последний `SaveChanges` в цепочке заодно фиксирует и более ранние изменения (например, `Card`, изменённый через `card.Review(...)`, коммитится вместе с новым `ReviewLog`).
- `src/KazakhSrs.Infrastructure` — `KazakhSrsDbContext`, конфигурации `IEntityTypeConfiguration<T>` (по одной на сущность, в `Persistence/Configurations`), реализации репозиториев, `KazakhSrsDbContextFactory` для design-time (`dotnet ef`).
- `tests/KazakhSrs.Domain.Tests` — xUnit-тесты: `Sm2EngineTests` (9 сценариев на чистый движок) + `CardTests` (проверка, что `Card.Review` корректно делегирует в `Sm2Engine`).

## Запуск

```bash
dotnet restore
dotnet build
dotnet test
```

## Миграции (EF Core Code-First)

Схема больше не живёт в ручном SQL — она описана через entity-классы и `IEntityTypeConfiguration<T>` в `KazakhSrs.Infrastructure`. `KazakhSrsDbContextFactory` реализует `IDesignTimeDbContextFactory`, так что `dotnet ef` работает прямо на классовой библиотеке, без отдельного host/API-проекта:

```bash
dotnet tool install --global dotnet-ef   # если ещё не установлен
dotnet ef migrations add InitialCreate --project src/KazakhSrs.Infrastructure --startup-project src/KazakhSrs.Infrastructure
dotnet ef database update --project src/KazakhSrs.Infrastructure --startup-project src/KazakhSrs.Infrastructure
```

Файл миграции получит автоматическое имя вида `20260919193045_InitialCreate.cs` (timestamp + имя из команды).

Строка подключения берётся из переменной окружения `KAZAKHSRS_CONNECTION_STRING`, иначе — локальный дефолт `Host=localhost;Database=kazakhsrs;Username=postgres;Password=postgres` (см. `KazakhSrsDbContextFactory`).

## Известные места, которые стоит перепроверить при первом restore/build

Этот код написан и проверен вручную (структура, типы, LINQ), но не прогонялся через реальный компилятор — в моём окружении `nuget.org` недоступен (сетевая политика), поэтому `dotnet restore` тут не выполнялся. Скорее всего всё соберётся, но два места стоит проверить в первую очередь:

- `CardConfiguration.cs` — составной индекс `idx_cards_due` по `(UserId, State.DueDate)`, где `DueDate` — часть owned-типа `SpacedRepetitionState`. Синтаксис `HasIndex(nameof(Card.UserId), "State.DueDate")` должен работать в EF Core 8, но это единственное место, которое я не мог перепроверить компиляцией.
- `NoteConfiguration.cs` — self-referencing many-to-many через `UsingEntity<Dictionary<string,object>>` для `note_links`, с явными именами колонок (`note_id`, `linked_note_id`) и check-constraint на самоссылку.

## Дальше (фаза 2 по плану)

Bot skeleton + флоу создания карточки: голос → Whisper → коррекция → варианты перевода → выбор → сборка → подтверждение → ElevenLabs → сохранение как `Card` + `ReviewLog` (через `ReviewCardUseCase` и репозитории).
