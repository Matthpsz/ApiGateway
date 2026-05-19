using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Gateway.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LogsTelemetria",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Rota = table.Column<string>(type: "text", nullable: false),
                    MetodoHttp = table.Column<string>(type: "text", nullable: false),
                    StatusCode = table.Column<int>(type: "integer", nullable: false),
                    TempoExecucaoMs = table.Column<long>(type: "bigint", nullable: false),
                    DataRequisicao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ChaveApi = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogsTelemetria", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LogsTelemetria_DataRequisicao",
                table: "LogsTelemetria",
                column: "DataRequisicao");

            migrationBuilder.CreateIndex(
                name: "IX_LogsTelemetria_Rota",
                table: "LogsTelemetria",
                column: "Rota");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogsTelemetria");
        }
    }
}
