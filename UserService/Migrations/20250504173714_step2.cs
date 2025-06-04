using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService.Migrations
{
    /// <inheritdoc />
    public partial class step2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RoleCsv",
                table: "UserDescription",
                type: "text",
                nullable: false,
                defaultValue: "");

            // Seed data for Users tablle
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Guid" },
                values: new object[,]
                {
                  { 1, Guid.NewGuid() },
                  { 2, Guid.NewGuid() }
                });

            // Seed data for UserDescription table
            // Password: "Password123#"
            migrationBuilder.InsertData(
                table: "UserDescription",
                columns: new[] { "Id", "ModifiedDate", "UserId", "FirstName", "LastName",
                 "Email", "PhoneNumber", "UserName", "Password", "RoleCsv" },
                values: new object[,]
                {
                    { 1, DateTime.UtcNow, 1, "John", "Doe", "john.doe@example.com", "1234567890",
                     "johndoe", "$2a$13$srpMBw0KivtkRgeSRiSNl.ug.vwkJYSDKvno3QjFnDjjGdWmWr7xa", "User" },
                    { 2, DateTime.UtcNow, 2, "Jane", "Smith", "jane.smith@example.com", "0987654321",
                     "janesmith", "$2a$13$srpMBw0KivtkRgeSRiSNl.ug.vwkJYSDKvno3QjFnDjjGdWmWr7xa", "User" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoleCsv",
                table: "UserDescription");
        }
    }
}
