using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GardenCentresApi.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_AspNetUsers_UserId",
                table: "UserProfiles");

            migrationBuilder.DropIndex(
                name: "IX_GardenCentres_Region",
                table: "GardenCentres");

            migrationBuilder.DeleteData(
                table: "GardenCentres",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "GardenCentres",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "Location",
                table: "GardenCentres");

            migrationBuilder.AlterColumn<string>(
                name: "Region",
                table: "GardenCentres",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "GardenCentres",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Region = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GardenCentres_LocationId",
                table: "GardenCentres",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_GardenCentres_Locations_LocationId",
                table: "GardenCentres",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GardenCentres_Locations_LocationId",
                table: "GardenCentres");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropIndex(
                name: "IX_GardenCentres_LocationId",
                table: "GardenCentres");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "GardenCentres");

            migrationBuilder.AlterColumn<string>(
                name: "Region",
                table: "GardenCentres",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "GardenCentres",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "GardenCentres",
                columns: new[] { "Id", "Location", "Name", "Region" },
                values: new object[,]
                {
                    { 1, "NY", "US Garden 1", "US" },
                    { 2, "London", "UK Garden 1", "UK" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_GardenCentres_Region",
                table: "GardenCentres",
                column: "Region");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_AspNetUsers_UserId",
                table: "UserProfiles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
