using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EICInventorySystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBOQAndCustodyEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BOQId",
                table: "Requisitions",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Source",
                table: "Requisitions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProjectBOQId",
                table: "InventoryTransactions",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProjectBOQs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BOQNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ProjectId = table.Column<int>(type: "INTEGER", nullable: false),
                    FactoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    WarehouseId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RequiredDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IssuedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CompletedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalQuantity = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    IssuedQuantity = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    TotalItems = table.Column<int>(type: "INTEGER", nullable: false),
                    RequiresCommanderReserve = table.Column<bool>(type: "INTEGER", nullable: false),
                    CommanderReserveQuantity = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    CommanderApprovalId = table.Column<int>(type: "INTEGER", nullable: true),
                    CommanderApprovalDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CommanderApprovalNotes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    CommanderApprovalNotesArabic = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    OriginalBOQId = table.Column<int>(type: "INTEGER", nullable: true),
                    IsRemainingBOQ = table.Column<bool>(type: "INTEGER", nullable: false),
                    PartialIssueReason = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    PartialIssueReasonArabic = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    ApprovedById = table.Column<int>(type: "INTEGER", nullable: true),
                    ApprovalNotes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    ApprovalNotesArabic = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    NotesArabic = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectBOQs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectBOQs_Factories_FactoryId",
                        column: x => x.FactoryId,
                        principalTable: "Factories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectBOQs_ProjectBOQs_OriginalBOQId",
                        column: x => x.OriginalBOQId,
                        principalTable: "ProjectBOQs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectBOQs_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectBOQs_Users_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectBOQs_Users_CommanderApprovalId",
                        column: x => x.CommanderApprovalId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectBOQs_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Workers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WorkerCode = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    NameArabic = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    MilitaryRank = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    MilitaryRankArabic = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    NationalId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    FactoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    DepartmentId = table.Column<int>(type: "INTEGER", nullable: false),
                    JoinDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workers_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Workers_Factories_FactoryId",
                        column: x => x.FactoryId,
                        principalTable: "Factories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectBOQItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BOQId = table.Column<int>(type: "INTEGER", nullable: false),
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    RequestedQuantity = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    IssuedQuantity = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    IsFromCommanderReserve = table.Column<bool>(type: "INTEGER", nullable: false),
                    CommanderReserveQuantity = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    AvailableStock = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    PartialIssueReason = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    PartialIssueReasonArabic = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    NotesArabic = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectBOQItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectBOQItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectBOQItems_ProjectBOQs_BOQId",
                        column: x => x.BOQId,
                        principalTable: "ProjectBOQs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OperationalCustodies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustodyNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    WorkerId = table.Column<int>(type: "INTEGER", nullable: false),
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    WarehouseId = table.Column<int>(type: "INTEGER", nullable: false),
                    FactoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    DepartmentId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    ReturnedQuantity = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    ConsumedQuantity = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    IssuedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ReturnedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Purpose = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    PurposeArabic = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    NotesArabic = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    IssuedById = table.Column<int>(type: "INTEGER", nullable: false),
                    ReturnReceivedById = table.Column<int>(type: "INTEGER", nullable: true),
                    CustodyLimit = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationalCustodies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperationalCustodies_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OperationalCustodies_Factories_FactoryId",
                        column: x => x.FactoryId,
                        principalTable: "Factories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OperationalCustodies_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OperationalCustodies_Users_IssuedById",
                        column: x => x.IssuedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OperationalCustodies_Users_ReturnReceivedById",
                        column: x => x.ReturnReceivedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OperationalCustodies_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OperationalCustodies_Workers_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "Workers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Requisitions_BOQId",
                table: "Requisitions",
                column: "BOQId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_ProjectBOQId",
                table: "InventoryTransactions",
                column: "ProjectBOQId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationalCustodies_CustodyNumber",
                table: "OperationalCustodies",
                column: "CustodyNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OperationalCustodies_DepartmentId",
                table: "OperationalCustodies",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationalCustodies_FactoryId",
                table: "OperationalCustodies",
                column: "FactoryId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationalCustodies_IssuedById",
                table: "OperationalCustodies",
                column: "IssuedById");

            migrationBuilder.CreateIndex(
                name: "IX_OperationalCustodies_ItemId",
                table: "OperationalCustodies",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationalCustodies_ReturnReceivedById",
                table: "OperationalCustodies",
                column: "ReturnReceivedById");

            migrationBuilder.CreateIndex(
                name: "IX_OperationalCustodies_WarehouseId",
                table: "OperationalCustodies",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationalCustodies_WorkerId",
                table: "OperationalCustodies",
                column: "WorkerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectBOQItems_BOQId",
                table: "ProjectBOQItems",
                column: "BOQId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectBOQItems_ItemId",
                table: "ProjectBOQItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectBOQs_ApprovedById",
                table: "ProjectBOQs",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectBOQs_BOQNumber",
                table: "ProjectBOQs",
                column: "BOQNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectBOQs_CommanderApprovalId",
                table: "ProjectBOQs",
                column: "CommanderApprovalId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectBOQs_FactoryId",
                table: "ProjectBOQs",
                column: "FactoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectBOQs_OriginalBOQId",
                table: "ProjectBOQs",
                column: "OriginalBOQId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectBOQs_ProjectId",
                table: "ProjectBOQs",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectBOQs_WarehouseId",
                table: "ProjectBOQs",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Workers_DepartmentId",
                table: "Workers",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Workers_FactoryId",
                table: "Workers",
                column: "FactoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Workers_WorkerCode",
                table: "Workers",
                column: "WorkerCode",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTransactions_ProjectBOQs_ProjectBOQId",
                table: "InventoryTransactions",
                column: "ProjectBOQId",
                principalTable: "ProjectBOQs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Requisitions_ProjectBOQs_BOQId",
                table: "Requisitions",
                column: "BOQId",
                principalTable: "ProjectBOQs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTransactions_ProjectBOQs_ProjectBOQId",
                table: "InventoryTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Requisitions_ProjectBOQs_BOQId",
                table: "Requisitions");

            migrationBuilder.DropTable(
                name: "OperationalCustodies");

            migrationBuilder.DropTable(
                name: "ProjectBOQItems");

            migrationBuilder.DropTable(
                name: "Workers");

            migrationBuilder.DropTable(
                name: "ProjectBOQs");

            migrationBuilder.DropIndex(
                name: "IX_Requisitions_BOQId",
                table: "Requisitions");

            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactions_ProjectBOQId",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "BOQId",
                table: "Requisitions");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "Requisitions");

            migrationBuilder.DropColumn(
                name: "ProjectBOQId",
                table: "InventoryTransactions");
        }
    }
}
