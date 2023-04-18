using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainReservationSystem.Migrations
{
    /// <inheritdoc />
    public partial class PassengerDetailsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PassengerDetails_UserProfileDetails_UserId",
                table: "PassengerDetails");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "PassengerDetails",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BookingId",
                table: "PassengerDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PassengerDetails_Bookings_UserId",
                table: "PassengerDetails",
                column: "UserId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PassengerDetails_Bookings_UserId",
                table: "PassengerDetails");

            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "PassengerDetails");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "PassengerDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_PassengerDetails_UserProfileDetails_UserId",
                table: "PassengerDetails",
                column: "UserId",
                principalTable: "UserProfileDetails",
                principalColumn: "Id");
        }
    }
}
