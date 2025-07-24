using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneticProgramming.Playground.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionToConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Experiments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    DatasetName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    StartTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    BestFitness = table.Column<double>(type: "REAL", nullable: false),
                    FinalFitness = table.Column<double>(type: "REAL", nullable: false),
                    InitialFitness = table.Column<double>(type: "REAL", nullable: false),
                    TestAccuracy = table.Column<double>(type: "REAL", nullable: false),
                    GenerationsCompleted = table.Column<int>(type: "INTEGER", nullable: false),
                    TreeComplexity = table.Column<int>(type: "INTEGER", nullable: false),
                    BestIndividualString = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Experiments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExperimentConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ExperimentId = table.Column<int>(type: "INTEGER", nullable: false),
                    PopulationSize = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxGenerations = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxTreeDepth = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxTreeLength = table.Column<int>(type: "INTEGER", nullable: false),
                    RandomSeed = table.Column<int>(type: "INTEGER", nullable: false),
                    CrossoverProbability = table.Column<double>(type: "REAL", nullable: false),
                    MutationProbability = table.Column<double>(type: "REAL", nullable: false),
                    UseBasicMath = table.Column<bool>(type: "INTEGER", nullable: false),
                    UseAdvancedMath = table.Column<bool>(type: "INTEGER", nullable: false),
                    UseStatistics = table.Column<bool>(type: "INTEGER", nullable: false),
                    AllowConstants = table.Column<bool>(type: "INTEGER", nullable: false),
                    ProblemType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ClassificationFitnessType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    RegressionFitnessType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ParsimonyPressure = table.Column<double>(type: "REAL", nullable: false),
                    TreeCreationMethod = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    MutationType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CrossoverType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExperimentConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExperimentConfigurations_Experiments_ExperimentId",
                        column: x => x.ExperimentId,
                        principalTable: "Experiments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenerationMetrics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ExperimentId = table.Column<int>(type: "INTEGER", nullable: false),
                    Generation = table.Column<int>(type: "INTEGER", nullable: false),
                    BestFitness = table.Column<double>(type: "REAL", nullable: false),
                    AverageFitness = table.Column<double>(type: "REAL", nullable: false),
                    WorstFitness = table.Column<double>(type: "REAL", nullable: false),
                    Diversity = table.Column<double>(type: "REAL", nullable: false),
                    BestTreeSize = table.Column<int>(type: "INTEGER", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenerationMetrics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenerationMetrics_Experiments_ExperimentId",
                        column: x => x.ExperimentId,
                        principalTable: "Experiments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExperimentConfigurations_ExperimentId",
                table: "ExperimentConfigurations",
                column: "ExperimentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Experiments_StartTime",
                table: "Experiments",
                column: "StartTime");

            migrationBuilder.CreateIndex(
                name: "IX_GenerationMetrics_ExperimentId_Generation",
                table: "GenerationMetrics",
                columns: new[] { "ExperimentId", "Generation" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExperimentConfigurations");

            migrationBuilder.DropTable(
                name: "GenerationMetrics");

            migrationBuilder.DropTable(
                name: "Experiments");
        }
    }
}
