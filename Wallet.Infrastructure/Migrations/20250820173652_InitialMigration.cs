using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wallet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "ExchangeRates",
                schema: "dbo",
                columns: table => new
                {
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    BaseCurrency = table.Column<string>(type: "nchar(3)", fixedLength: true, maxLength: 3, nullable: false),
                    CounterCurrency = table.Column<string>(type: "nchar(3)", fixedLength: true, maxLength: 3, nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,9)", precision: 18, scale: 9, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeRates", x => new { x.Date, x.BaseCurrency, x.CounterCurrency });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExchangeRates",
                schema: "dbo");
        }
    }
}
