using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System;

namespace ProjectQualityChecker.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Branch",
                columns: table => new
                {
                    BranchId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branch", x => x.BranchId);
                });

            migrationBuilder.CreateTable(
                name: "Developers",
                columns: table => new
                {
                    DeveloperId = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Developers", x => x.DeveloperId);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    LanguageId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.LanguageId);
                });

            migrationBuilder.CreateTable(
                name: "Repositories",
                columns: table => new
                {
                    RepositoryId = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: false),
                    FullName = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    Private = table.Column<bool>(nullable: false),
                    Key = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repositories", x => x.RepositoryId);
                });

            migrationBuilder.CreateTable(
                name: "FileDetails",
                columns: table => new
                {
                    FileDetailId = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    Extension = table.Column<string>(nullable: true),
                    FullPath = table.Column<string>(nullable: true),
                    LanguageId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileDetails", x => x.FileDetailId);
                    table.ForeignKey(
                        name: "FK_FileDetails_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "LanguageId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Commits",
                columns: table => new
                {
                    CommitId = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Sha = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    DeveloperId = table.Column<long>(nullable: false),
                    RepositoryId = table.Column<long>(nullable: false),
                    BranchId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commits", x => x.CommitId);
                    table.ForeignKey(
                        name: "FK_Commits_Branch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branch",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Commits_Developers_DeveloperId",
                        column: x => x.DeveloperId,
                        principalTable: "Developers",
                        principalColumn: "DeveloperId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Commits_Repositories_RepositoryId",
                        column: x => x.RepositoryId,
                        principalTable: "Repositories",
                        principalColumn: "RepositoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    FileId = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SHA = table.Column<string>(nullable: true),
                    FileDetailId = table.Column<long>(nullable: true),
                    CommitId = table.Column<long>(nullable: false),
                    Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.FileId);
                    table.ForeignKey(
                        name: "FK_Files_Commits_CommitId",
                        column: x => x.CommitId,
                        principalTable: "Commits",
                        principalColumn: "CommitId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Files_FileDetails_FileDetailId",
                        column: x => x.FileDetailId,
                        principalTable: "FileDetails",
                        principalColumn: "FileDetailId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Metrics",
                columns: table => new
                {
                    MetricId = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Complexity = table.Column<int>(nullable: true),
                    CognitiveComplexity = table.Column<int>(nullable: true),
                    DuplicatedLines = table.Column<int>(nullable: true),
                    CodeSmells = table.Column<int>(nullable: true),
                    NewCodeSmells = table.Column<int>(nullable: true),
                    CommentLines = table.Column<int>(nullable: true),
                    CommentLinesDensity = table.Column<double>(nullable: true),
                    Ncloc = table.Column<int>(nullable: true),
                    Statements = table.Column<int>(nullable: true),
                    BranchCoverage = table.Column<double>(nullable: true),
                    LineCoverage = table.Column<double>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    FileId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Metrics", x => x.MetricId);
                    table.ForeignKey(
                        name: "FK_Metrics_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "FileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Commits_BranchId",
                table: "Commits",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Commits_DeveloperId",
                table: "Commits",
                column: "DeveloperId");

            migrationBuilder.CreateIndex(
                name: "IX_Commits_RepositoryId",
                table: "Commits",
                column: "RepositoryId");

            migrationBuilder.CreateIndex(
                name: "IX_FileDetails_LanguageId",
                table: "FileDetails",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_CommitId",
                table: "Files",
                column: "CommitId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_FileDetailId",
                table: "Files",
                column: "FileDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_Metrics_FileId",
                table: "Metrics",
                column: "FileId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Metrics");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "Commits");

            migrationBuilder.DropTable(
                name: "FileDetails");

            migrationBuilder.DropTable(
                name: "Branch");

            migrationBuilder.DropTable(
                name: "Developers");

            migrationBuilder.DropTable(
                name: "Repositories");

            migrationBuilder.DropTable(
                name: "Languages");
        }
    }
}