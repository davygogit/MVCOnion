using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init_fin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Etages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Niveau = table.Column<int>(type: "int", nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImgPlanEtagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Etages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Salles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Numero = table.Column<int>(type: "int", nullable: false),
                    ImgSallePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Favori = table.Column<bool>(type: "bit", nullable: true),
                    TypeSalle = table.Column<int>(type: "int", nullable: false),
                    CoordonneeX = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CoordonneeY = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NbTables = table.Column<int>(type: "int", nullable: true),
                    NbPlaces = table.Column<int>(type: "int", nullable: true),
                    EtageId = table.Column<int>(type: "int", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    PriseElectrique = table.Column<bool>(type: "bit", nullable: true),
                    MicroOndes = table.Column<int>(type: "int", nullable: true),
                    Frigo = table.Column<bool>(type: "bit", nullable: true),
                    Evier = table.Column<int>(type: "int", nullable: true),
                    Distributeur = table.Column<bool>(type: "bit", nullable: true),
                    Ecran = table.Column<bool>(type: "bit", nullable: true),
                    Camera = table.Column<bool>(type: "bit", nullable: true),
                    TableauBlanc = table.Column<bool>(type: "bit", nullable: true),
                    SystemeAudio = table.Column<bool>(type: "bit", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Salles_Etages_EtageId",
                        column: x => x.EtageId,
                        principalTable: "Etages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Salles_EtageId",
                table: "Salles",
                column: "EtageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Salles");

            migrationBuilder.DropTable(
                name: "Etages");
        }
    }
}
