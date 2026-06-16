using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechMoves_Formative2.Migrations
{
    /// <inheritdoc />
    public partial class AddLocalCostZarToServiceRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "LocalCostZAR",
                table: "ServiceRequests",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocalCostZAR",
                table: "ServiceRequests");
        }
    }
}
