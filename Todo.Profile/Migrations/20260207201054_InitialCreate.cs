using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Todo.Profile.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // No-op: this migration was scaffolded for SQLite and is not applied on SQL Server.
            // The SQL Server specific migration (InitialCreateSqlServer) will create the schema.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No-op: this migration is not applied on SQL Server.
        }
    }
}
