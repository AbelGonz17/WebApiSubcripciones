using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIAutores.Migrations
{
    /// <inheritdoc />
    public partial class Restricciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RestriccionesDominio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LlaveId = table.Column<int>(type: "int", nullable: false),
                    Dominio = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestriccionesDominio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestriccionesDominio_llaveAPIs_LlaveId",
                        column: x => x.LlaveId,
                        principalTable: "llaveAPIs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RestriccionesIPs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    llaveId = table.Column<int>(type: "int", nullable: false),
                    IP = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestriccionesIPs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestriccionesIPs_llaveAPIs_llaveId",
                        column: x => x.llaveId,
                        principalTable: "llaveAPIs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RestriccionesDominio_LlaveId",
                table: "RestriccionesDominio",
                column: "LlaveId");

            migrationBuilder.CreateIndex(
                name: "IX_RestriccionesIPs_llaveId",
                table: "RestriccionesIPs",
                column: "llaveId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RestriccionesDominio");

            migrationBuilder.DropTable(
                name: "RestriccionesIPs");
        }
    }
}
