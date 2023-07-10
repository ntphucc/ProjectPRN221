using Microsoft.EntityFrameworkCore.Migrations;
using System.Security;

#nullable disable

namespace ProjectRoomChat.Migrations
{
    /// <inheritdoc />
    public partial class seedUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            for (int i = 1; i <= 100; i++)
            {
                migrationBuilder.InsertData(
                    "AspNetUsers",
                    columns: new[]
                    {
                        "Id",
                        "FullName",
                        "Avatar",
                        "UserName",
                        "Email",
                        "SecurityStamp",
                        "EmailConfirmed",
                        "PhoneNumberConfirmed",
                        "TwoFactorEnabled",
                        "LockoutEnabled",
                        "AccessFailedCount"

                    },
                    values: new object[]
                    {
                        Guid.NewGuid().ToString(),
                        "FullName-" + i.ToString("D3"),
                        "AVT",
                        "UserName-" + i.ToString("D3"),
                        $"email-{i.ToString("D3")}@example.com",
                        Guid.NewGuid().ToString(),
                        true,
                        false,
                        false,
                        false,
                        0
                    }
                );
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}


