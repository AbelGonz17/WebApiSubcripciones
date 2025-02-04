using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIAutores.Migrations
{
    /// <inheritdoc />
    public partial class LlavesApis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "llaveAPIs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Llave = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    tipoLlave = table.Column<int>(type: "int", nullable: false),
                    Activa = table.Column<bool>(type: "bit", nullable: false),
                    Usuarioid = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_llaveAPIs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_llaveAPIs_AspNetUsers_Usuarioid",
                        column: x => x.Usuarioid,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_llaveAPIs_Usuarioid",
                table: "llaveAPIs",
                column: "Usuarioid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "llaveAPIs");
        }
    }
}
