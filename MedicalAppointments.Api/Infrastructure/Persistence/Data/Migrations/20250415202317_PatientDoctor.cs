using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalAppointments.Api.Migrations
{
    /// <inheritdoc />
    public partial class PatientDoctor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_PrimaryDoctorId",
                table: "AspNetUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_PrimaryDoctorId",
                table: "AspNetUsers",
                column: "PrimaryDoctorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_PrimaryDoctorId",
                table: "AspNetUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_PrimaryDoctorId",
                table: "AspNetUsers",
                column: "PrimaryDoctorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
