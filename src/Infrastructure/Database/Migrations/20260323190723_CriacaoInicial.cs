using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class CriacaoInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "processamento_diagramas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    analise_diagrama_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status_processamento = table.Column<string>(type: "text", nullable: false),
                    tentativas_processamento = table.Column<int>(type: "integer", nullable: false),
                    descricao_analise = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    componentes_identificados = table.Column<string>(type: "text", nullable: true),
                    riscos_arquiteturais = table.Column<string>(type: "text", nullable: true),
                    recomendacoes_basicas = table.Column<string>(type: "text", nullable: true),
                    data_criacao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    data_inicio_processamento = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    data_conclusao_processamento = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_processamento_diagramas", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_processamento_diagramas_analise_diagrama_id",
                table: "processamento_diagramas",
                column: "analise_diagrama_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "processamento_diagramas");
        }
    }
}
