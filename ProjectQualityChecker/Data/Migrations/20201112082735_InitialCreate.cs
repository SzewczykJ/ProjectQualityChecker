using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ProjectQualityChecker.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Branch",
                table => new
                {
                    BranchId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Branch", x => x.BranchId); });

            migrationBuilder.CreateTable(
                "Developers",
                table => new
                {
                    DeveloperId = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Developers", x => x.DeveloperId); });

            migrationBuilder.CreateTable(
                "Languages",
                table => new
                {
                    LanguageId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Languages", x => x.LanguageId); });

            migrationBuilder.CreateTable(
                "Repositories",
                table => new
                {
                    RepositoryId = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: false),
                    FullName = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    Private = table.Column<bool>(nullable: false),
                    Key = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Repositories", x => x.RepositoryId); });

            migrationBuilder.CreateTable(
                "FileDetails",
                table => new
                {
                    FileDetailId = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    Extension = table.Column<string>(nullable: true),
                    FullPath = table.Column<string>(nullable: true),
                    LanguageId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileDetails", x => x.FileDetailId);
                    table.ForeignKey(
                        "FK_FileDetails_Languages_LanguageId",
                        x => x.LanguageId,
                        "Languages",
                        "LanguageId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "Commits",
                table => new
                {
                    CommitId = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
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
                        "FK_Commits_Branch_BranchId",
                        x => x.BranchId,
                        "Branch",
                        "BranchId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_Commits_Developers_DeveloperId",
                        x => x.DeveloperId,
                        "Developers",
                        "DeveloperId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_Commits_Repositories_RepositoryId",
                        x => x.RepositoryId,
                        "Repositories",
                        "RepositoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "Files",
                table => new
                {
                    FileId = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SHA = table.Column<string>(nullable: true),
                    FileDetailId = table.Column<long>(nullable: true),
                    CommitId = table.Column<long>(nullable: false),
                    Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.FileId);
                    table.ForeignKey(
                        "FK_Files_Commits_CommitId",
                        x => x.CommitId,
                        "Commits",
                        "CommitId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_Files_FileDetails_FileDetailId",
                        x => x.FileDetailId,
                        "FileDetails",
                        "FileDetailId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "Metrics",
                table => new
                {
                    MetricId = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
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
                        "FK_Metrics_Files_FileId",
                        x => x.FileId,
                        "Files",
                        "FileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "IX_Commits_BranchId",
                "Commits",
                "BranchId");

            migrationBuilder.CreateIndex(
                "IX_Commits_DeveloperId",
                "Commits",
                "DeveloperId");

            migrationBuilder.CreateIndex(
                "IX_Commits_RepositoryId",
                "Commits",
                "RepositoryId");

            migrationBuilder.CreateIndex(
                "IX_FileDetails_LanguageId",
                "FileDetails",
                "LanguageId");

            migrationBuilder.CreateIndex(
                "IX_Files_CommitId",
                "Files",
                "CommitId");

            migrationBuilder.CreateIndex(
                "IX_Files_FileDetailId",
                "Files",
                "FileDetailId");

            migrationBuilder.CreateIndex(
                "IX_Metrics_FileId",
                "Metrics",
                "FileId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Metrics");

            migrationBuilder.DropTable(
                "Files");

            migrationBuilder.DropTable(
                "Commits");

            migrationBuilder.DropTable(
                "FileDetails");

            migrationBuilder.DropTable(
                "Branch");

            migrationBuilder.DropTable(
                "Developers");

            migrationBuilder.DropTable(
                "Repositories");

            migrationBuilder.DropTable(
                "Languages");
        }
    }
}