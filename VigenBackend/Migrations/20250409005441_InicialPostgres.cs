using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Vigen_Repository.Migrations
{
    public partial class InicialPostgres : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "notify",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    state_id = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    organization_type_id = table.Column<int>(type: "integer", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notify", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "organization",
                columns: table => new
                {
                    nit = table.Column<string>(type: "text", nullable: false),
                    password = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    tel = table.Column<string>(type: "text", nullable: false),
                    phone = table.Column<string>(type: "text", nullable: false),
                    organization_type_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organization", x => x.nit);
                });

            migrationBuilder.CreateTable(
                name: "organization_type",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organization_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "poll",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    genero = table.Column<int>(type: "integer", nullable: false),
                    orientacion_sexual = table.Column<int>(type: "integer", nullable: false),
                    edad = table.Column<int>(type: "integer", nullable: false),
                    municipio = table.Column<int>(type: "integer", nullable: false),
                    sector = table.Column<int>(type: "integer", nullable: false),
                    nivel_educativo = table.Column<int>(type: "integer", nullable: false),
                    estado_civil = table.Column<int>(type: "integer", nullable: false),
                    etnia = table.Column<int>(type: "integer", nullable: false),
                    ingresos = table.Column<int>(type: "integer", nullable: false),
                    ocupacion = table.Column<int>(type: "integer", nullable: false),
                    p1 = table.Column<int>(type: "integer", nullable: false),
                    p2 = table.Column<int>(type: "integer", nullable: false),
                    p3 = table.Column<int>(type: "integer", nullable: false),
                    p4 = table.Column<int>(type: "integer", nullable: false),
                    p5 = table.Column<int>(type: "integer", nullable: false),
                    p6 = table.Column<int>(type: "integer", nullable: false),
                    p7 = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_poll", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "site",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    nit = table.Column<string>(type: "text", nullable: false),
                    ubication = table.Column<string>(type: "text", nullable: false),
                    range = table.Column<int>(type: "integer", nullable: false),
                    country_code = table.Column<string>(type: "text", nullable: false),
                    phone = table.Column<string>(type: "text", nullable: false),
                    tel = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site", x => new { x.id, x.nit });
                });

            migrationBuilder.CreateTable(
                name: "state",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_state", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    identification = table.Column<string>(type: "text", nullable: false),
                    password = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    gender = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: false),
                    birthdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    country_code = table.Column<string>(type: "text", nullable: false),
                    phone = table.Column<string>(type: "text", nullable: false),
                    occupation = table.Column<string>(type: "text", nullable: false),
                    postal_code = table.Column<string>(type: "text", nullable: false),
                    marital_status = table.Column<string>(type: "text", nullable: false),
                    ubication = table.Column<string>(type: "text", nullable: false),
                    code = table.Column<string>(type: "text", nullable: false),
                    verification = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.identification);
                });

            migrationBuilder.CreateTable(
                name: "violence_type",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_violence_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "violence_types_organization",
                columns: table => new
                {
                    organization_type_id = table.Column<int>(type: "integer", nullable: false),
                    id_violence = table.Column<string>(type: "text", nullable: false)
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
