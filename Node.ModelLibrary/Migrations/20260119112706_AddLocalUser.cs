using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Node.ModelLibrary.Migrations
{
    /// <inheritdoc />
    public partial class AddLocalUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Profiles_AspNetUsers_AppUserId",
                table: "Profiles");

            migrationBuilder.RenameColumn(
                name: "AppUserId",
                table: "Profiles",
                newName: "AppUserLocalId");

            migrationBuilder.RenameIndex(
                name: "IX_Profiles_AppUserId",
                table: "Profiles",
                newName: "IX_Profiles_AppUserLocalId");

            migrationBuilder.AddColumn<Guid>(
                name: "ProfileId",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppUsersLocal",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeleteAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUsersLocal", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ProfileId",
                table: "AspNetUsers",
                column: "ProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Profiles_ProfileId",
                table: "AspNetUsers",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Profiles_AppUsersLocal_AppUserLocalId",
                table: "Profiles",
                column: "AppUserLocalId",
                principalTable: "AppUsersLocal",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Profiles_ProfileId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Profiles_AppUsersLocal_AppUserLocalId",
                table: "Profiles");

            migrationBuilder.DropTable(
                name: "AppUsersLocal");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ProfileId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfileId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "AppUserLocalId",
                table: "Profiles",
                newName: "AppUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Profiles_AppUserLocalId",
                table: "Profiles",
                newName: "IX_Profiles_AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Profiles_AspNetUsers_AppUserId",
                table: "Profiles",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
