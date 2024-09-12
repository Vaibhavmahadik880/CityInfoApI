using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CityInfo.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialDataSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Cities",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "It is known for its Mughal architecture and cuisine.", "Lucknow" },
                    { 2, "The financial capital of India, famous for Bollywood.", "Mumbai" },
                    { 3, "Known for its cultural heritage and colonial architecture.", "Kolkata" },
                    { 4, "Famous for its palaces and the pink buildings in the old city.", "Jaipur" },
                    { 5, "Known for its beaches and Dravidian-style temples.", "Chennai" }
                });

            migrationBuilder.InsertData(
                table: "PointOfInterests",
                columns: new[] { "Id", "CityId", "Description", "Name" },
                values: new object[,]
                {
                    { 1, 1, "The most visited urban park.", "Central Park" },
                    { 2, 1, "A 103-story skyscraper located in midtown Manhattan.", "Empire State Building" },
                    { 3, 2, "A historic fort and UNESCO World Heritage site.", "Red Fort" },
                    { 4, 2, "A war memorial and popular tourist spot.", "India Gate" },
                    { 5, 3, "A famous botanical garden with a glass house.", "Lalbagh Botanical Garden" },
                    { 6, 3, "A palace known for its Tudor-style architecture.", "Bangalore Palace" },
                    { 7, 4, "A large marble building and museum.", "Victoria Memorial" },
                    { 8, 4, "A cantilever bridge over the Hooghly River.", "Howrah Bridge" },
                    { 9, 5, "The longest natural urban beach in India.", "Marina Beach" },
                    { 10, 5, "A famous Hindu temple dedicated to Lord Shiva.", "Kapaleeshwarar Temple" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PointOfInterests",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "PointOfInterests",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "PointOfInterests",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "PointOfInterests",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "PointOfInterests",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "PointOfInterests",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "PointOfInterests",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "PointOfInterests",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "PointOfInterests",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "PointOfInterests",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Cities");
        }
    }
}
