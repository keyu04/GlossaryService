using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GlossaryService.Migrations
{
    /// <inheritdoc />
    public partial class initGlossary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GlossaryTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlossaryTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GlossaryKeywords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Term = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Synonyms = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TagId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlossaryKeywords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GlossaryKeywords_GlossaryTags_TagId",
                        column: x => x.TagId,
                        principalTable: "GlossaryTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GlossaryKeywords_DeletedAt",
                table: "GlossaryKeywords",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_GlossaryKeywords_TagId",
                table: "GlossaryKeywords",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_GlossaryTags_DeletedAt",
                table: "GlossaryTags",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_GlossaryTags_Slug",
                table: "GlossaryTags",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GlossaryKeywords");

            migrationBuilder.DropTable(
                name: "GlossaryTags");
        }
    }
}
