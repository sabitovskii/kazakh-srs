using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KazakhSrs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:card_type", "qa,translation_prod,cloze_suffix,shadowing");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    telegram_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "topics",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_topics", x => x.id);
                    table.ForeignKey(
                        name: "FK_topics_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notes",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    topic_id = table.Column<long>(type: "bigint", nullable: true),
                    title = table.Column<string>(type: "text", nullable: false),
                    body = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notes", x => x.id);
                    table.ForeignKey(
                        name: "FK_notes_topics_topic_id",
                        column: x => x.topic_id,
                        principalTable: "topics",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_notes_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cards",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    topic_id = table.Column<long>(type: "bigint", nullable: true),
                    note_id = table.Column<long>(type: "bigint", nullable: true),
                    type = table.Column<int>(type: "card_type", nullable: false),
                    prompt_text = table.Column<string>(type: "text", nullable: false),
                    canonical_answer = table.Column<string>(type: "text", nullable: false),
                    context_notes = table.Column<string>(type: "text", nullable: true),
                    audio_url = table.Column<string>(type: "text", nullable: true),
                    repetitions = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    interval_days = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ease_factor = table.Column<double>(type: "double precision", nullable: false, defaultValue: 2.5),
                    due_date = table.Column<DateOnly>(type: "date", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cards", x => x.id);
                    table.ForeignKey(
                        name: "FK_cards_notes_note_id",
                        column: x => x.note_id,
                        principalTable: "notes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_cards_topics_topic_id",
                        column: x => x.topic_id,
                        principalTable: "topics",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_cards_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "note_links",
                columns: table => new
                {
                    note_id = table.Column<long>(type: "bigint", nullable: false),
                    linked_note_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_note_links", x => new { x.note_id, x.linked_note_id });
                    table.CheckConstraint("CK_note_links_no_self_link", "note_id <> linked_note_id");
                    table.ForeignKey(
                        name: "FK_note_links_notes_linked_note_id",
                        column: x => x.linked_note_id,
                        principalTable: "notes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_note_links_notes_note_id",
                        column: x => x.note_id,
                        principalTable: "notes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "review_log",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    card_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    reviewed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    grade = table.Column<short>(type: "smallint", nullable: false),
                    user_response_text = table.Column<string>(type: "text", nullable: true),
                    raw_audio_ref = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_review_log", x => x.id);
                    table.CheckConstraint("CK_review_log_grade_range", "grade BETWEEN 0 AND 5");
                    table.ForeignKey(
                        name: "FK_review_log_cards_card_id",
                        column: x => x.card_id,
                        principalTable: "cards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_review_log_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_cards_topic",
                table: "cards",
                column: "topic_id");

            // Hot-path index for "due cards today" (user_id, due_date). Added by hand here rather
            // than via HasIndex in CardConfiguration — due_date lives on the owned SpacedRepetitionState
            // navigation, and EF Core 8's fluent HasIndex couldn't express a composite index spanning
            // an owner property + an owned-type property (see chat history for the two failed attempts).
            // Physically it's just two columns on the same "cards" table, so CreateIndex with raw
            // column names sidesteps the C#-model limitation entirely.
            migrationBuilder.CreateIndex(
                name: "idx_cards_due",
                table: "cards",
                columns: new[] { "user_id", "due_date" });

            migrationBuilder.CreateIndex(
                name: "IX_cards_note_id",
                table: "cards",
                column: "note_id");

            migrationBuilder.CreateIndex(
                name: "IX_cards_user_id",
                table: "cards",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_note_links_linked_note_id",
                table: "note_links",
                column: "linked_note_id");

            migrationBuilder.CreateIndex(
                name: "IX_notes_topic_id",
                table: "notes",
                column: "topic_id");

            migrationBuilder.CreateIndex(
                name: "IX_notes_user_id",
                table: "notes",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "idx_review_log_card",
                table: "review_log",
                columns: new[] { "card_id", "reviewed_at" });

            migrationBuilder.CreateIndex(
                name: "IX_review_log_user_id",
                table: "review_log",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_topics_user_id_name",
                table: "topics",
                columns: new[] { "user_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_telegram_id",
                table: "users",
                column: "telegram_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "note_links");

            migrationBuilder.DropTable(
                name: "review_log");

            migrationBuilder.DropTable(
                name: "cards");

            migrationBuilder.DropTable(
                name: "notes");

            migrationBuilder.DropTable(
                name: "topics");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
