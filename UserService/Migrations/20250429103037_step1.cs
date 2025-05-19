using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace UserService.Migrations
{
    /// <inheritdoc />
    public partial class step1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Guid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.UniqueConstraint("AK_Users_Guid", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "UserDescription",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDescription", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDescription_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRemoved",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RemovedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRemoved", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRemoved_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserDescription_UserId",
                table: "UserDescription",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRemoved_UserId",
                table: "UserRemoved",
                column: "UserId");

            //// Seed data for Users tablle
            //migrationBuilder.InsertData(
            //    table: "Users",
            //    columns: new[] { "Id", "Guid" },
            //    values: new object[,]
            //    {
            //      { 1, Guid.NewGuid() },
            //      { 2, Guid.NewGuid() }
            //    });

            //// Seed data for UserDescription table
            //migrationBuilder.InsertData(
            //    table: "UserDescription",
            //    columns: new[] { "Id", "ModifiedDate", "UserId", "FirstName", "LastName", "Email", "PhoneNumber", "UserName", "Password" },
            //    values: new object[,]
            //    {
            //        { 1, DateTime.UtcNow, 1, "John", "Doe", "john.doe@example.com", "1234567890", "johndoe", "password123" },
            //        { 2, DateTime.UtcNow, 2, "Jane", "Smith", "jane.smith@example.com", "0987654321", "janesmith", "password456" }
            //    });
        }

        /// <inheritdoc />
        /// 
        /// To rolleback the migration : dotenet ef database update 0
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserDescription");

            migrationBuilder.DropTable(
                name: "UserRemoved");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
