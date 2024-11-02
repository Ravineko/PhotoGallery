using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotoGallery.Migrations;

/// <inheritdoc />
public partial class AuthCodes : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "PasswordHash",
            table: "Users",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "TwoFactorEnabled",
            table: "Users",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.CreateTable(
            name: "PromotionCodes",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                IsUsed = table.Column<bool>(type: "bit", nullable: false),
                UserId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PromotionCodes", x => x.Id);
                table.ForeignKey(
                    name: "FK_PromotionCodes_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "TwoFactorCodes",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                IsUsed = table.Column<bool>(type: "bit", nullable: false),
                UserId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TwoFactorCodes", x => x.Id);
                table.ForeignKey(
                    name: "FK_TwoFactorCodes_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_PromotionCodes_UserId",
            table: "PromotionCodes",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_TwoFactorCodes_UserId",
            table: "TwoFactorCodes",
            column: "UserId");

        migrationBuilder.InsertData(
           table: "Roles",
           columns: new[] { "Id", "Name" },
           values: new object[,]
           {
                { 1, "Admin" },
                { 2, "User" },
                { 3, "SuperAdmin" }
           });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "PromotionCodes");

        migrationBuilder.DropTable(
            name: "TwoFactorCodes");

        migrationBuilder.DropColumn(
            name: "PasswordHash",
            table: "Users");

        migrationBuilder.DropColumn(
            name: "TwoFactorEnabled",
            table: "Users");

        migrationBuilder.DeleteData(
           table: "Roles",
           keyColumn: "Id",
           keyValues: new object[] { 1, 2, 3 });
    }
}
