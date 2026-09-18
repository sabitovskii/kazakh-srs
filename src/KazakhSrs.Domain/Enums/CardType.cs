namespace KazakhSrs.Domain.Enums;

/// <summary>Mirrors the Postgres native enum `card_type` (see Infrastructure migrations).</summary>
public enum CardType
{
    Qa,
    TranslationProd,
    ClozeSuffix,
    Shadowing,
}
