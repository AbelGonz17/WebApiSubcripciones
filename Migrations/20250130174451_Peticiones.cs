﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIAutores.Migrations
{
    /// <inheritdoc />
    public partial class Peticiones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Peticiones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LlaveId = table.Column<int>(type: "int", nullable: false),
                    FechaPeticion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Peticiones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Peticiones_llaveAPIs_LlaveId",
                        column: x => x.LlaveId,
                        principalTable: "llaveAPIs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Peticiones_LlaveId",
                table: "Peticiones",
                column: "LlaveId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Peticiones");
        }
    }
}
