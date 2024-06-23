using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCityToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.Sql("UPDATE Users SET City = 'Greenbush' WHERE UserName = 'lisa'; UPDATE Users SET City = 'Celeryville' WHERE UserName = 'karen'; UPDATE Users SET City = 'Rosewood' WHERE UserName = 'margo'; UPDATE Users SET City = 'Orviston' WHERE UserName = 'lois'; UPDATE Users SET City = 'Germanton' WHERE UserName = 'ruthie'; UPDATE Users SET City = 'Cliff' WHERE UserName = 'todd'; UPDATE Users SET City = 'Welda' WHERE UserName = 'porter'; UPDATE Users SET City = 'Clarence' WHERE UserName = 'mayo'; UPDATE Users SET City = 'Herald' WHERE UserName = 'skinner'; UPDATE Users SET City = 'Lupton' WHERE UserName = 'davis';");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Users");
        }
    }
}
