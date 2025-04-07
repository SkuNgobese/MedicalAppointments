using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalAppointments.Api.Infrastructure.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class AppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Hospitals_Doctor_HospitalId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Contacts",
                newName: "ContactNumber");

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

            migrationBuilder.AlterColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(8)",
                oldMaxLength: 8);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Hospitals_Patient_HospitalId",
                table: "AspNetUsers",
                column: "Patient_HospitalId",
                principalTable: "Hospitals",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Hospitals_Patient_HospitalId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "ContactNumber",
                table: "Contacts",
                newName: "PhoneNumber");

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

            migrationBuilder.AlterColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(21)",
                oldMaxLength: 21);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Hospitals_Doctor_HospitalId",
                table: "AspNetUsers",
                column: "Doctor_HospitalId",
                principalTable: "Hospitals",
                principalColumn: "Id");
        }
    }
}
