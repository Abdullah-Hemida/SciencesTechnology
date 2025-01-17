﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SciencesTechnology.Data.Migrations
{
    /// <inheritdoc />
    public partial class addCreditsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Credits",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Credits",
                table: "AspNetUsers");

           
        }
    }
}
