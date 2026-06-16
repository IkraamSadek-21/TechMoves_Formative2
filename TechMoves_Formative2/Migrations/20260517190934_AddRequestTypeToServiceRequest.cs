using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechMoves_Formative2.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestTypeToServiceRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequestType",
                table: "ServiceRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestType",
                table: "ServiceRequests");
        }
    }
}
