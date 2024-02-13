using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WinDexer.Model.Migrations
{
    /// <inheritdoc />
    public partial class v01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RootFolder",
                columns: table => new
                {
                    RootFolderId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Path = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: false),
                    IndexationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    StillFound = table.Column<bool>(type: "INTEGER", nullable: true),
                    Enabled = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RootFolder", x => x.RootFolderId);
                });

            migrationBuilder.CreateTable(
                name: "IndexEntry",
                columns: table => new
                {
                    IndexEntryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Path = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: false),
                    ContainerPath = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: true),
                    RelativePath = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    Extension = table.Column<string>(type: "TEXT", maxLength: 16, nullable: true),
                    IndexationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastSeen = table.Column<DateTime>(type: "TEXT", nullable: true),
                    StillFound = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsFolder = table.Column<bool>(type: "INTEGER", nullable: false),
                    Size = table.Column<long>(type: "INTEGER", nullable: false),
                    FoldersCount = table.Column<int>(type: "INTEGER", nullable: false),
                    FilesCount = table.Column<int>(type: "INTEGER", nullable: false),
                    CreationTimeUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastAccessTimeUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastWriteTimeUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RootFolderId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ParentIndexEntryId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndexEntry", x => x.IndexEntryId);
                    table.ForeignKey(
                        name: "FK_IndexEntry_IndexEntry_ParentIndexEntryId",
                        column: x => x.ParentIndexEntryId,
                        principalTable: "IndexEntry",
                        principalColumn: "IndexEntryId");
                    table.ForeignKey(
                        name: "FK_IndexEntry_RootFolder_RootFolderId",
                        column: x => x.RootFolderId,
                        principalTable: "RootFolder",
                        principalColumn: "RootFolderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IndexEntry_IndexationDate",
                table: "IndexEntry",
                column: "IndexationDate");

            migrationBuilder.CreateIndex(
                name: "IX_IndexEntry_IsFolder",
                table: "IndexEntry",
                column: "IsFolder");

            migrationBuilder.CreateIndex(
                name: "IX_IndexEntry_Name",
                table: "IndexEntry",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_IndexEntry_ParentIndexEntryId",
                table: "IndexEntry",
                column: "ParentIndexEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_IndexEntry_RelativePath",
                table: "IndexEntry",
                column: "RelativePath");

            migrationBuilder.CreateIndex(
                name: "IX_IndexEntry_RootFolderId",
                table: "IndexEntry",
                column: "RootFolderId");

            migrationBuilder.CreateIndex(
                name: "IX_IndexEntry_RootFolderId_RelativePath",
                table: "IndexEntry",
                columns: new[] { "RootFolderId", "RelativePath" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IndexEntry_StillFound",
                table: "IndexEntry",
                column: "StillFound");

            migrationBuilder.CreateIndex(
                name: "IX_RootFolder_Enabled",
                table: "RootFolder",
                column: "Enabled");

            migrationBuilder.CreateIndex(
                name: "IX_RootFolder_IndexationDate",
                table: "RootFolder",
                column: "IndexationDate");

            migrationBuilder.CreateIndex(
                name: "IX_RootFolder_Name",
                table: "RootFolder",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_RootFolder_Path",
                table: "RootFolder",
                column: "Path",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RootFolder_StillFound",
                table: "RootFolder",
                column: "StillFound");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IndexEntry");

            migrationBuilder.DropTable(
                name: "RootFolder");
        }
    }
}
