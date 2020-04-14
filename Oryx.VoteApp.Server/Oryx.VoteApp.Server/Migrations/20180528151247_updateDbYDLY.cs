using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Oryx.VoteApp.Server.Migrations
{
    public partial class updateDbYDLY : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuestionId",
                table: "JDLYOption",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JdlyId",
                table: "JDLYLog",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "JDLYQuestion",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CloseSeconds = table.Column<int>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    EnableStartTime = table.Column<double>(nullable: false),
                    EnableTime = table.Column<DateTime>(nullable: false),
                    IsEnable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JDLYQuestion", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JDLYOption_QuestionId",
                table: "JDLYOption",
                column: "QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_JDLYOption_JDLYQuestion_QuestionId",
                table: "JDLYOption",
                column: "QuestionId",
                principalTable: "JDLYQuestion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JDLYOption_JDLYQuestion_QuestionId",
                table: "JDLYOption");

            migrationBuilder.DropTable(
                name: "JDLYQuestion");

            migrationBuilder.DropIndex(
                name: "IX_JDLYOption_QuestionId",
                table: "JDLYOption");

            migrationBuilder.DropColumn(
                name: "QuestionId",
                table: "JDLYOption");

            migrationBuilder.DropColumn(
                name: "JdlyId",
                table: "JDLYLog");
        }
    }
}
