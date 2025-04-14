using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalAppointments.Shared.Migrations
{
    /// <inheritdoc />
    public partial class Admins : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Hospitals_SysAdmin_HospitalId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "SysAdmin_HospitalId",
                table: "AspNetUsers",
                newName: "Admin_HospitalId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_SysAdmin_HospitalId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_Admin_HospitalId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Hospitals_Admin_HospitalId",
                table: "AspNetUsers",
                column: "Admin_HospitalId",
                principalTable: "Hospitals",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Hospitals_Admin_HospitalId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "Admin_HospitalId",
                table: "AspNetUsers",
                newName: "SysAdmin_HospitalId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_Admin_HospitalId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_SysAdmin_HospitalId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Hospitals_SysAdmin_HospitalId",
                table: "AspNetUsers",
                column: "SysAdmin_HospitalId",
                principalTable: "Hospitals",
                principalColumn: "Id");
        }
    }
}
