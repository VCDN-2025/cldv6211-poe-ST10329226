// Inside your new AddVenueIdToEvents.cs migration file
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventEase.Migrations
{
    /// <inheritdoc />
    public partial class AddVenueIdToEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Add the column as nullable initially
            migrationBuilder.AddColumn<int>(
                name: "VenueID",
                table: "Events",
                type: "int",
                nullable: true); // <--- Make sure this is 'true' here first

            // Step 2: Update existing Events to assign a valid VenueID
            // IMPORTANT: Ensure you have at least one Venue record in your 'Venues' table
            // before running Update-Database, or this SQL will fail.
            migrationBuilder.Sql("UPDATE Events SET VenueID = (SELECT TOP 1 VenueID FROM Venues);");

            // Step 3: Alter the column to be non-nullable
            migrationBuilder.AlterColumn<int>(
                name: "VenueID",
                table: "Events",
                type: "int",
                nullable: false, // <--- Only after update, make it 'false'
                oldNullable: true); // Must specify oldNullable when using AlterColumn


            migrationBuilder.CreateIndex(
                name: "IX_Events_VenueID",
                table: "Events",
                column: "VenueID");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Venues_VenueID",
                table: "Events",
                column: "VenueID",
                principalTable: "Venues",
                principalColumn: "VenueID",
                onDelete: ReferentialAction.Restrict); // Or .Cascade if you prefer
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Venues_VenueID",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_VenueID",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "VenueID",
                table: "Events");
        }
    }
}