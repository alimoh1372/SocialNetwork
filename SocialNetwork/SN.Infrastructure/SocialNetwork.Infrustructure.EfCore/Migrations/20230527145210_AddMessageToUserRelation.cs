using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialNetwork.Infrastructure.EfCore.Migrations
{
    public partial class AddMessageToUserRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RelationRequestMessage",
                table: "UserRelations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RelationRequestMessage",
                table: "UserRelations");
        }
    }
}
