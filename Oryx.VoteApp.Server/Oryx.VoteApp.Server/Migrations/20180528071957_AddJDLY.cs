using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Oryx.VoteApp.Server.Migrations
{
    public partial class AddJDLY : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JDLYLog",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Avarta = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    NickName = table.Column<string>(nullable: true),
                    OptionJson = table.Column<string>(nullable: true),
                    RightNumber = table.Column<int>(nullable: false),
                    RightOption = table.Column<string>(nullable: true),
                    UserCode = table.Column<string>(nullable: true),
                    WrongOption = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JDLYLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JDLYOption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Answer = table.Column<string>(nullable: true),
                    ImgUrl = table.Column<string>(nullable: true),
                    Option1 = table.Column<string>(nullable: true),
                    Option2 = table.Column<string>(nullable: true),
                    Option3 = table.Column<string>(nullable: true),
                    Option4 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JDLYOption", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JDLYLog");

            migrationBuilder.DropTable(
                name: "JDLYOption");
        }
    }
}
