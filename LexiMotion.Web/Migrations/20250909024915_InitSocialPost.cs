using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LexiMotion.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitSocialPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SocialPost",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InputText = table.Column<string>(type: "text", nullable: false),
                    PredictedEmotion = table.Column<string>(type: "text", nullable: false),
                    ConfidenceScores = table.Column<string>(type: "jsonb", nullable: false),
                    NegationDetected = table.Column<bool>(type: "boolean", nullable: false),
                    SarcasmDetected = table.Column<bool>(type: "boolean", nullable: false),
                    SarcasmProbability = table.Column<float>(type: "real", nullable: false),
                    FinalEmotion = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialPost", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SocialPost");
        }
    }
}
