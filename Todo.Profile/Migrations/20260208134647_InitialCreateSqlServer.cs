using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Todo.Profile.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateSqlServer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Ensure Identity tables exist on SQL Server (created if missing)
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[Users] (
  [Id] nvarchar(450) NOT NULL PRIMARY KEY,
  [UserName] nvarchar(256) NULL,
  [NormalizedUserName] nvarchar(256) NULL,
  [Email] nvarchar(256) NULL,
  [NormalizedEmail] nvarchar(256) NULL,
  [EmailConfirmed] bit NOT NULL DEFAULT 0,
  [PasswordHash] nvarchar(max) NULL,
  [SecurityStamp] nvarchar(max) NULL,
  [ConcurrencyStamp] nvarchar(max) NULL,
  [PhoneNumber] nvarchar(max) NULL,
  [PhoneNumberConfirmed] bit NOT NULL DEFAULT 0,
  [TwoFactorEnabled] bit NOT NULL DEFAULT 0,
  [LockoutEnd] datetimeoffset NULL,
  [LockoutEnabled] bit NOT NULL DEFAULT 0,
  [AccessFailedCount] int NOT NULL DEFAULT 0
);
END

IF OBJECT_ID(N'dbo.AspNetRoles', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[AspNetRoles] (
  [Id] nvarchar(450) NOT NULL PRIMARY KEY,
  [Name] nvarchar(256) NULL,
  [NormalizedName] nvarchar(256) NULL,
  [ConcurrencyStamp] nvarchar(max) NULL
);
END

IF OBJECT_ID(N'dbo.AspNetUserClaims', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[AspNetUserClaims] (
  [Id] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
  [UserId] nvarchar(450) NOT NULL,
  [ClaimType] nvarchar(max) NULL,
  [ClaimValue] nvarchar(max) NULL
);
ALTER TABLE [dbo].[AspNetUserClaims] ADD CONSTRAINT FK_AspNetUserClaims_Users_UserId FOREIGN KEY([UserId]) REFERENCES [dbo].[Users]([Id]) ON DELETE CASCADE;
END

IF OBJECT_ID(N'dbo.AspNetUserLogins', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[AspNetUserLogins] (
  [LoginProvider] nvarchar(450) NOT NULL,
  [ProviderKey] nvarchar(450) NOT NULL,
  [ProviderDisplayName] nvarchar(max) NULL,
  [UserId] nvarchar(450) NOT NULL,
  CONSTRAINT PK_AspNetUserLogins PRIMARY KEY (LoginProvider, ProviderKey)
);
ALTER TABLE [dbo].[AspNetUserLogins] ADD CONSTRAINT FK_AspNetUserLogins_Users_UserId FOREIGN KEY([UserId]) REFERENCES [dbo].[Users]([Id]) ON DELETE CASCADE;
END

IF OBJECT_ID(N'dbo.AspNetUserRoles', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[AspNetUserRoles] (
  [UserId] nvarchar(450) NOT NULL,
  [RoleId] nvarchar(450) NOT NULL,
  CONSTRAINT PK_AspNetUserRoles PRIMARY KEY ([UserId], [RoleId])
);
ALTER TABLE [dbo].[AspNetUserRoles] ADD CONSTRAINT FK_AspNetUserRoles_Users_UserId FOREIGN KEY([UserId]) REFERENCES [dbo].[Users]([Id]) ON DELETE CASCADE;
ALTER TABLE [dbo].[AspNetUserRoles] ADD CONSTRAINT FK_AspNetUserRoles_AspNetRoles_RoleId FOREIGN KEY([RoleId]) REFERENCES [dbo].[AspNetRoles]([Id]) ON DELETE CASCADE;
END

IF OBJECT_ID(N'dbo.AspNetUserTokens', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[AspNetUserTokens] (
  [UserId] nvarchar(450) NOT NULL,
  [LoginProvider] nvarchar(450) NOT NULL,
  [Name] nvarchar(450) NOT NULL,
  [Value] nvarchar(max) NULL,
  CONSTRAINT PK_AspNetUserTokens PRIMARY KEY ([UserId], [LoginProvider], [Name])
);
ALTER TABLE [dbo].[AspNetUserTokens] ADD CONSTRAINT FK_AspNetUserTokens_Users_UserId FOREIGN KEY([UserId]) REFERENCES [dbo].[Users]([Id]) ON DELETE CASCADE;
END

IF OBJECT_ID(N'dbo.AspNetRoleClaims', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[AspNetRoleClaims] (
  [Id] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
  [RoleId] nvarchar(450) NOT NULL,
  [ClaimType] nvarchar(max) NULL,
  [ClaimValue] nvarchar(max) NULL
);
ALTER TABLE [dbo].[AspNetRoleClaims] ADD CONSTRAINT FK_AspNetRoleClaims_AspNetRoles_RoleId FOREIGN KEY([RoleId]) REFERENCES [dbo].[AspNetRoles]([Id]) ON DELETE CASCADE;
END

SET QUOTED_IDENTIFIER ON;
CREATE UNIQUE INDEX UserNameIndex ON [Users] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
CREATE UNIQUE INDEX RoleNameIndex ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "Users",
                type: "TEXT",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "TwoFactorEnabled",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "SecurityStamp",
                table: "Users",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "PhoneNumberConfirmed",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedUserName",
                table: "Users",
                type: "TEXT",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedEmail",
                table: "Users",
                type: "TEXT",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LockoutEnd",
                table: "Users",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "LockoutEnabled",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "EmailConfirmed",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "TEXT",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                table: "Users",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AccessFailedCount",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Users",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "AspNetUserTokens",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AspNetUserTokens",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "AspNetUserRoles",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AspNetUserRoles",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AspNetUserLogins",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderDisplayName",
                table: "AspNetUserLogins",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AspNetUserClaims",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ClaimValue",
                table: "AspNetUserClaims",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClaimType",
                table: "AspNetUserClaims",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "AspNetUserClaims",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedName",
                table: "AspNetRoles",
                type: "TEXT",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetRoles",
                type: "TEXT",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                table: "AspNetRoles",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AspNetRoles",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "AspNetRoleClaims",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ClaimValue",
                table: "AspNetRoleClaims",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClaimType",
                table: "AspNetRoleClaims",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "AspNetRoleClaims",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Users",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);
        }
    }
}
