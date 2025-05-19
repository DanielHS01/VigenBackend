using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vigen_Repository.Migrations
{
    public partial class Inicial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "notify",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    state_id = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    organization_type_id = table.Column<int>(type: "int", nullable: false),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notify", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "organization",
                columns: table => new
                {
                    nit = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    tel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    organization_type_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organization", x => x.nit);
                });

            migrationBuilder.CreateTable(
                name: "organization_type",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organization_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "poll",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    genero = table.Column<int>(type: "int", nullable: false),
                    orientacion_sexual = table.Column<int>(type: "int", nullable: false),
                    edad = table.Column<int>(type: "int", nullable: false),
                    municipio = table.Column<int>(type: "int", nullable: false),
                    sector = table.Column<int>(type: "int", nullable: false),
                    nivel_educativo = table.Column<int>(type: "int", nullable: false),
                    estado_civil = table.Column<int>(type: "int", nullable: false),
                    etnia = table.Column<int>(type: "int", nullable: false),
                    ingresos = table.Column<int>(type: "int", nullable: false),
                    ocupacion = table.Column<int>(type: "int", nullable: false),
                    p1 = table.Column<int>(type: "int", nullable: false),
                    p2 = table.Column<int>(type: "int", nullable: false),
                    p3 = table.Column<int>(type: "int", nullable: false),
                    p4 = table.Column<int>(type: "int", nullable: false),
                    p5 = table.Column<int>(type: "int", nullable: false),
                    p6 = table.Column<int>(type: "int", nullable: false),
                    p7 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_poll", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "site",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    nit = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ubication = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    range = table.Column<int>(type: "int", nullable: false),
                    country_code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    tel = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site", x => new { x.id, x.nit });
                });

            migrationBuilder.CreateTable(
                name: "state",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_state", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    identification = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    birthdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    country_code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    occupation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    postal_code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    marital_status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ubication = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    verification = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.identification);
                });

            migrationBuilder.CreateTable(
                name: "violence_type",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_violence_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "violence_types_organization",
                columns: table => new
                {
                    organization_type_id = table.Column<int>(type: "int", nullable: false),
                    id_violence = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_violence_types_organization", x => new { x.organization_type_id, x.id_violence });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notify");

            migrationBuilder.DropTable(
                name: "organization");

            migrationBuilder.DropTable(
                name: "organization_type");

            migrationBuilder.DropTable(
                name: "poll");

            migrationBuilder.DropTable(
                name: "site");

            migrationBuilder.DropTable(
                name: "state");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "violence_type");

            migrationBuilder.DropTable(
                name: "violence_types_organization");
        }
    }
}
