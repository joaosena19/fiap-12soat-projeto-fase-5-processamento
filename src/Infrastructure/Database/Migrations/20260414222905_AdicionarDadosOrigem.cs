using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarDadosOrigem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "extensao",
                table: "processamento_diagramas",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "localizacao_url",
                table: "processamento_diagramas",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "nome_fisico",
                table: "processamento_diagramas",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "nome_original",
                table: "processamento_diagramas",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "extensao",
                table: "processamento_diagramas");

            migrationBuilder.DropColumn(
                name: "localizacao_url",
                table: "processamento_diagramas");

            migrationBuilder.DropColumn(
                name: "nome_fisico",
                table: "processamento_diagramas");

            migrationBuilder.DropColumn(
                name: "nome_original",
                table: "processamento_diagramas");
        }
    }
}
