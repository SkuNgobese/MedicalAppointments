using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalAppointments.Api.Migrations
{
    /// <inheritdoc />
    public partial class UserHospital : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Patient_IsActive",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Patient_RemoveDate",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "Patient_IDNumber",
                table: "AspNetUsers",
                newName: "MedicalAidNumber");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SuperAdmin_HospitalId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SuperAdmin_HospitalId",
                table: "AspNetUsers",
                column: "SuperAdmin_HospitalId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Hospitals_SuperAdmin_HospitalId",
                table: "AspNetUsers",
                column: "SuperAdmin_HospitalId",
                principalTable: "Hospitals",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Hospitals_SuperAdmin_HospitalId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SuperAdmin_HospitalId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SuperAdmin_HospitalId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "MedicalAidNumber",
                table: "AspNetUsers",
                newName: "Patient_IDNumber");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "AspNetUsers",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<bool>(
                name: "Patient_IsActive",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Patient_RemoveDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }
    }
}
