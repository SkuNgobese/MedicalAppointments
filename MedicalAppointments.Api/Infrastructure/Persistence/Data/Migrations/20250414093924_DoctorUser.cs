using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalAppointments.Shared.Migrations
{
    /// <inheritdoc />
    public partial class DoctorUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Hospitals_Patient_HospitalId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "Patient_RemoveDate",
                table: "AspNetUsers",
                newName: "Doctor_RemoveDate");

            migrationBuilder.RenameColumn(
                name: "Patient_IsActive",
                table: "AspNetUsers",
                newName: "Doctor_IsActive");

            migrationBuilder.RenameColumn(
                name: "Patient_IDNumber",
                table: "AspNetUsers",
                newName: "Doctor_IDNumber");

            migrationBuilder.RenameColumn(
                name: "Patient_HospitalId",
                table: "AspNetUsers",
                newName: "Doctor_HospitalId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_Patient_HospitalId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_Doctor_HospitalId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Hospitals_Doctor_HospitalId",
                table: "AspNetUsers",
                column: "Doctor_HospitalId",
                principalTable: "Hospitals",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Hospitals_Doctor_HospitalId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "Doctor_RemoveDate",
                table: "AspNetUsers",
                newName: "Patient_RemoveDate");

            migrationBuilder.RenameColumn(
                name: "Doctor_IsActive",
                table: "AspNetUsers",
                newName: "Patient_IsActive");

            migrationBuilder.RenameColumn(
                name: "Doctor_IDNumber",
                table: "AspNetUsers",
                newName: "Patient_IDNumber");

            migrationBuilder.RenameColumn(
                name: "Doctor_HospitalId",
                table: "AspNetUsers",
                newName: "Patient_HospitalId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_Doctor_HospitalId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_Patient_HospitalId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Hospitals_Patient_HospitalId",
                table: "AspNetUsers",
                column: "Patient_HospitalId",
                principalTable: "Hospitals",
                principalColumn: "Id");
        }
    }
}
