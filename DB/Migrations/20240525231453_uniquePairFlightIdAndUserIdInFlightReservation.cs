using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class uniquePairFlightIdAndUserIdInFlightReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FlightReservation_FlightId",
                table: "FlightReservation");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "FlightReservation",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FlightId",
                table: "FlightReservation",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FlightReservation_FlightId_UserId",
                table: "FlightReservation",
                columns: new[] { "FlightId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FlightReservation_FlightId_UserId",
                table: "FlightReservation");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "FlightReservation",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "FlightId",
                table: "FlightReservation",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_FlightReservation_FlightId",
                table: "FlightReservation",
                column: "FlightId");
        }
    }
}
