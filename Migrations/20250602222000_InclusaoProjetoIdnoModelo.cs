using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiarioDeEspecime.Migrations
{
    /// <inheritdoc />
    public partial class InclusaoProjetoIdnoModelo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Especimes_Projetos_ProjetoId",
                table: "Especimes");

            migrationBuilder.AlterColumn<int>(
                name: "ProjetoId",
                table: "Especimes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Especimes_Projetos_ProjetoId",
                table: "Especimes",
                column: "ProjetoId",
                principalTable: "Projetos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Especimes_Projetos_ProjetoId",
                table: "Especimes");

            migrationBuilder.AlterColumn<int>(
                name: "ProjetoId",
                table: "Especimes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Especimes_Projetos_ProjetoId",
                table: "Especimes",
                column: "ProjetoId",
                principalTable: "Projetos",
                principalColumn: "Id");
        }
    }
}
