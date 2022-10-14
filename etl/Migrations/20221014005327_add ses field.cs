﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace core.Migrations
{
    public partial class addsesfield : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RawEmail",
                table: "Items",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RawEmail",
                table: "Items");
        }
    }
}
