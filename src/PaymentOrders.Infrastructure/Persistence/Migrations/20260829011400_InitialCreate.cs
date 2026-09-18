using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentOrders.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "idempotency_records",
                columns: table => new
                {
                    Key = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    PaymentOrderId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_idempotency_records", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "payment_orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    SourceAccount = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    TargetAccount = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    SwiftCode = table.Column<string>(type: "TEXT", maxLength: 11, nullable: true),
                    ScheduledFor = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Commission = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true),
                    FailureReason = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "payment_order_events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PaymentOrderId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_order_events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_payment_order_events_payment_orders_PaymentOrderId",
                        column: x => x.PaymentOrderId,
                        principalTable: "payment_orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_idempotency_records_PaymentOrderId",
                table: "idempotency_records",
                column: "PaymentOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_payment_order_events_PaymentOrderId_OccurredAt",
                table: "payment_order_events",
                columns: new[] { "PaymentOrderId", "OccurredAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "idempotency_records");

            migrationBuilder.DropTable(
                name: "payment_order_events");

            migrationBuilder.DropTable(
                name: "payment_orders");
        }
    }
}
