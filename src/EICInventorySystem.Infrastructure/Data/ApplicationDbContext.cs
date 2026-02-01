using EICInventorySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EICInventorySystem.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // Core entities
    public DbSet<User> Users => Set<User>();
    public DbSet<Factory> Factories => Set<Factory>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<Worker> Workers => Set<Worker>();

    // Inventory entities
    public DbSet<InventoryRecord> InventoryRecords => Set<InventoryRecord>();
    public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();
    public DbSet<ItemTransactionHistory> ItemTransactionHistories => Set<ItemTransactionHistory>();
    public DbSet<StockAdjustment> StockAdjustments => Set<StockAdjustment>();

    // Transaction entities
    public DbSet<Requisition> Requisitions => Set<Requisition>();
    public DbSet<RequisitionItem> RequisitionItems => Set<RequisitionItem>();
    public DbSet<Transfer> Transfers => Set<Transfer>();
    public DbSet<TransferItem> TransferItems => Set<TransferItem>();
    public DbSet<Receipt> Receipts => Set<Receipt>();
    public DbSet<ReceiptItem> ReceiptItems => Set<ReceiptItem>();
    public DbSet<Consumption> Consumptions => Set<Consumption>();
    public DbSet<ConsumptionItem> ConsumptionItems => Set<ConsumptionItem>();
    public DbSet<Return> Returns => Set<Return>();
    public DbSet<ReturnItem> ReturnItems => Set<ReturnItem>();
    public DbSet<Adjustment> Adjustments => Set<Adjustment>();
    public DbSet<AdjustmentItem> AdjustmentItems => Set<AdjustmentItem>();

    // Purchase entities
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();

    // BOQ & Custody entities
    public DbSet<ProjectBOQ> ProjectBOQs => Set<ProjectBOQ>();
    public DbSet<ProjectBOQItem> ProjectBOQItems => Set<ProjectBOQItem>();
    public DbSet<OperationalCustody> OperationalCustodies => Set<OperationalCustody>();

    // System entities
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<SystemConfiguration> SystemConfigurations => Set<SystemConfiguration>();
    public DbSet<SystemSettings> SystemSettings => Set<SystemSettings>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Username).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.FullName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.FullNameArabic).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Role).HasMaxLength(50).IsRequired();

            entity.HasOne(e => e.Factory)
                .WithMany(f => f.Users)
                .HasForeignKey(e => e.FactoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Department)
                .WithMany(d => d.Users)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Warehouse)
                .WithMany(w => w.AssignedUsers)
                .HasForeignKey(e => e.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Project)
                .WithMany()
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Factory configuration
        modelBuilder.Entity<Factory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.NameArabic).HasMaxLength(200).IsRequired();
            entity.HasIndex(e => e.Code).IsUnique();

            entity.HasOne(e => e.Commander)
                .WithMany()
                .HasForeignKey(e => e.CommanderId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Warehouse configuration
        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.NameArabic).HasMaxLength(200).IsRequired();
            entity.HasIndex(e => e.Code).IsUnique();

            entity.HasOne(e => e.Factory)
                .WithMany(f => f.Warehouses)
                .HasForeignKey(e => e.FactoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Keeper)
                .WithMany()
                .HasForeignKey(e => e.KeeperId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Item configuration
        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ItemCode).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(300).IsRequired();
            entity.Property(e => e.NameArabic).HasMaxLength(300).IsRequired();
            entity.Property(e => e.Unit).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.ItemCode).IsUnique();
            entity.Property(e => e.StandardCost).HasPrecision(18, 4);
            entity.Property(e => e.Weight).HasPrecision(18, 4);
            entity.Property(e => e.ReservePercentage).HasPrecision(18, 4);
            entity.Property(e => e.ReservedQuantity).HasPrecision(18, 4);
        });

        // InventoryRecord configuration
        modelBuilder.Entity<InventoryRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.WarehouseId, e.ItemId }).IsUnique();
            entity.Property(e => e.TotalQuantity).HasPrecision(18, 4);
            entity.Property(e => e.GeneralQuantity).HasPrecision(18, 4);
            entity.Property(e => e.CommanderReserveQuantity).HasPrecision(18, 4);
            entity.Property(e => e.GeneralAllocated).HasPrecision(18, 4);
            entity.Property(e => e.ReserveAllocated).HasPrecision(18, 4);
            entity.Property(e => e.MinimumReserveRequired).HasPrecision(18, 4);
            entity.Property(e => e.ReorderPoint).HasPrecision(18, 4);

            entity.HasOne(e => e.Warehouse)
                .WithMany(w => w.InventoryRecords)
                .HasForeignKey(e => e.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Item)
                .WithMany(i => i.InventoryRecords)
                .HasForeignKey(e => e.ItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // AuditLog configuration
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Action).HasMaxLength(50).IsRequired();
            entity.Property(e => e.EntityType).HasMaxLength(100).IsRequired();
            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => e.UserId);

            entity.HasOne(e => e.User)
                .WithMany(u => u.AuditLogs)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Notification configuration
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Message).HasMaxLength(2000).IsRequired();
            entity.Property(e => e.Type).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => new { e.UserId, e.IsRead });

            entity.HasOne(e => e.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Requisition configuration
        modelBuilder.Entity<Requisition>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Number).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.Number).IsUnique();
            entity.Property(e => e.TotalQuantity).HasPrecision(18, 4);
            entity.Property(e => e.TotalValue).HasPrecision(18, 4);
            entity.Property(e => e.IssuedQuantity).HasPrecision(18, 4);
            entity.Property(e => e.CommanderReserveQuantity).HasPrecision(18, 4);

            entity.HasOne(e => e.Requester)
                .WithMany(u => u.CreatedRequisitions)
                .HasForeignKey(e => e.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Approver)
                .WithMany(u => u.ApprovedRequisitions)
                .HasForeignKey(e => e.ApproverId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.CommanderApprover)
                .WithMany() // User doesn't have a specific collection for CommanderApprovedRequisitions, so keeping generic
                .HasForeignKey(e => e.CommanderApprovalId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Department)
                .WithMany(d => d.Requisitions)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Project)
                .WithMany()
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Warehouse)
                .WithMany()
                .HasForeignKey(e => e.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.BOQ)
                .WithMany()
                .HasForeignKey(e => e.BOQId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Project configuration
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.NameArabic).HasMaxLength(200).IsRequired();
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.Budget).HasPrecision(18, 4);
            entity.Property(e => e.SpentAmount).HasPrecision(18, 4);

            entity.HasOne(e => e.Factory)
                .WithMany(f => f.Projects)
                .HasForeignKey(e => e.FactoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Manager)
                .WithMany()
                .HasForeignKey(e => e.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Ignore TeamMembers for now - requires a join table
            entity.Ignore(e => e.TeamMembers);
        });

        // Department configuration
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.NameArabic).HasMaxLength(200).IsRequired();
            entity.HasIndex(e => e.Code).IsUnique();

            entity.HasOne(e => e.Factory)
                .WithMany(f => f.Departments)
                .HasForeignKey(e => e.FactoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Head)
                .WithMany()
                .HasForeignKey(e => e.HeadId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Supplier configuration
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.CreditLimit).HasPrecision(18, 4);
            entity.Property(e => e.CurrentBalance).HasPrecision(18, 4);
        });

        // Transfer configuration
        modelBuilder.Entity<Transfer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TransferNumber).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.TransferNumber).IsUnique();
            entity.Property(e => e.TotalQuantity).HasPrecision(18, 4);
            entity.Property(e => e.ShippedQuantity).HasPrecision(18, 4);
            entity.Property(e => e.ReceivedQuantity).HasPrecision(18, 4);
            entity.Property(e => e.CommanderReserveQuantity).HasPrecision(18, 4);
            entity.Property(e => e.TotalValue).HasPrecision(18, 4);

            entity.HasOne(e => e.CreatedByUser)
                .WithMany(u => u.CreatedTransfers)
                .HasForeignKey(e => e.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ApprovedByUser)
                .WithMany(u => u.ApprovedTransfers)
                .HasForeignKey(e => e.ApprovedBy)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.CommanderApprover)
                .WithMany() // User doesn't have a specific collection for CommanderApprovedTransfers, so keeping generic
                .HasForeignKey(e => e.CommanderApprovalId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.SourceWarehouse)
                .WithMany(w => w.OutgoingTransfers)
                .HasForeignKey(e => e.SourceWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.DestinationWarehouse)
                .WithMany(w => w.IncomingTransfers)
                .HasForeignKey(e => e.DestinationWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ItemTransactionHistory configuration
        modelBuilder.Entity<ItemTransactionHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ItemId);
            entity.HasIndex(e => e.TransactionDate);
            entity.Property(e => e.TransactionType).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Quantity).HasPrecision(18, 4);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 4);
            entity.Property(e => e.TotalValue).HasPrecision(18, 4);

            entity.HasOne(e => e.Item)
                .WithMany(i => i.TransactionHistory)
                .HasForeignKey(e => e.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Worker)
                .WithMany()
                .HasForeignKey(e => e.WorkerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Approver)
                .WithMany()
                .HasForeignKey(e => e.ApproverId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Project)
                .WithMany(p => p.TransactionHistory)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Department)
                .WithMany(d => d.TransactionHistory)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // SystemConfiguration
        modelBuilder.Entity<SystemConfiguration>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Key).HasMaxLength(100).IsRequired();
            entity.HasIndex(e => e.Key).IsUnique();
        });

        // Return configuration
        modelBuilder.Entity<Return>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Number).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.Number).IsUnique();
            entity.Property(e => e.TotalQuantity).HasPrecision(18, 4);
            entity.Property(e => e.TotalValue).HasPrecision(18, 4);

            entity.HasOne(e => e.Approver)
                .WithMany()
                .HasForeignKey(e => e.ApproverId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Receiver)
                .WithMany()
                .HasForeignKey(e => e.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Warehouse)
                .WithMany()
                .HasForeignKey(e => e.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Project)
                .WithMany()
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Department)
                .WithMany()
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Receipt configuration
        modelBuilder.Entity<Receipt>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Number).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.Number).IsUnique();
            entity.Property(e => e.TotalQuantity).HasPrecision(18, 4);
            entity.Property(e => e.TotalValue).HasPrecision(18, 4);

            entity.HasOne(e => e.Receiver)
                .WithMany()
                .HasForeignKey(e => e.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Supplier)
                .WithMany(s => s.Receipts)
                .HasForeignKey(e => e.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Warehouse)
                .WithMany(w => w.Receipts)
                .HasForeignKey(e => e.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.PurchaseOrder)
                .WithMany(po => po.Receipts)
                .HasForeignKey(e => e.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Consumption configuration
        modelBuilder.Entity<Consumption>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Number).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.Number).IsUnique();
            entity.Property(e => e.TotalQuantity).HasPrecision(18, 4);
            entity.Property(e => e.TotalValue).HasPrecision(18, 4);

            entity.HasOne(e => e.Approver)
                .WithMany()
                .HasForeignKey(e => e.ApprovedBy)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Warehouse)
                .WithMany()
                .HasForeignKey(e => e.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Project)
                .WithMany(p => p.Consumptions)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Department)
                .WithMany()
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Adjustment configuration
        modelBuilder.Entity<Adjustment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Number).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.Number).IsUnique();
            entity.Property(e => e.TotalAdjustment).HasPrecision(18, 4);
            entity.Property(e => e.TotalValue).HasPrecision(18, 4);

            entity.HasOne(e => e.Approver)
                .WithMany()
                .HasForeignKey(e => e.ApprovedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Report configuration
        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.CreatedByUser)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // PurchaseOrder configuration
        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Number).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.Number).IsUnique();
            entity.Property(e => e.TotalQuantity).HasPrecision(18, 4);
            entity.Property(e => e.TotalValue).HasPrecision(18, 4);
            entity.Property(e => e.ReceivedQuantity).HasPrecision(18, 4);
            entity.Property(e => e.ReceivedValue).HasPrecision(18, 4);

            entity.HasOne(e => e.Approver)
                .WithMany()
                .HasForeignKey(e => e.ApproverId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Supplier)
                .WithMany(s => s.PurchaseOrders)
                .HasForeignKey(e => e.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // InventoryTransaction configuration
        modelBuilder.Entity<InventoryTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Quantity).HasPrecision(18, 4);
            entity.Property(e => e.GeneralQuantity).HasPrecision(18, 4);
            entity.Property(e => e.CommanderReserveQuantity).HasPrecision(18, 4);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 4);
            entity.Property(e => e.TotalValue).HasPrecision(18, 4);
            entity.Property(e => e.PreviousGeneralQuantity).HasPrecision(18, 4);
            entity.Property(e => e.PreviousReserveQuantity).HasPrecision(18, 4);
            entity.Property(e => e.NewGeneralQuantity).HasPrecision(18, 4);
            entity.Property(e => e.NewReserveQuantity).HasPrecision(18, 4);

            entity.HasOne(e => e.User)
                .WithMany(u => u.Transactions)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        // SystemSettings configuration
        modelBuilder.Entity<SystemSettings>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CommanderReservePercentage).HasPrecision(18, 4);
            entity.Property(e => e.LowStockThresholdPercentage).HasPrecision(18, 4);
        });

        // ProjectAllocation configuration
        modelBuilder.Entity<ProjectAllocation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AllocatedQuantity).HasPrecision(18, 4);
            entity.Property(e => e.CommanderReserveQuantity).HasPrecision(18, 4);
            entity.Property(e => e.ConsumedQuantity).HasPrecision(18, 4);
            entity.Property(e => e.ReturnedQuantity).HasPrecision(18, 4);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 4);
            entity.Property(e => e.TotalValue).HasPrecision(18, 4);

            entity.HasOne(e => e.Project)
                .WithMany(p => p.Allocations)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Item)
                .WithMany()
                .HasForeignKey(e => e.ItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // RequisitionItem configuration
        modelBuilder.Entity<RequisitionItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RequestedQuantity).HasPrecision(18, 4);
            entity.Property(e => e.IssuedQuantity).HasPrecision(18, 4);
            entity.Property(e => e.CommanderReserveQuantity).HasPrecision(18, 4);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 4);
            entity.Property(e => e.TotalValue).HasPrecision(18, 4);

            entity.HasOne(e => e.Requisition)
                .WithMany(r => r.Items)
                .HasForeignKey(e => e.RequisitionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Item)
                .WithMany(i => i.RequisitionItems)
                .HasForeignKey(e => e.ItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // TransferItem configuration
        modelBuilder.Entity<TransferItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Quantity).HasPrecision(18, 4);
            entity.Property(e => e.ShippedQuantity).HasPrecision(18, 4);
            entity.Property(e => e.ReceivedQuantity).HasPrecision(18, 4);
            entity.Property(e => e.CommanderReserveQuantity).HasPrecision(18, 4);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 4);
            entity.Property(e => e.TotalValue).HasPrecision(18, 4);

            entity.HasOne(e => e.Transfer)
                .WithMany(t => t.Items)
                .HasForeignKey(e => e.TransferId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Item)
                .WithMany(i => i.TransferItems)
                .HasForeignKey(e => e.ItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ConsumptionItem configuration
        modelBuilder.Entity<ConsumptionItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Quantity).HasPrecision(18, 4);
            entity.Property(e => e.CommanderReserveQuantity).HasPrecision(18, 4);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 4);
            entity.Property(e => e.TotalValue).HasPrecision(18, 4);

            entity.HasOne(e => e.Consumption)
                .WithMany(c => c.Items)
                .HasForeignKey(e => e.ConsumptionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Item)
                .WithMany(i => i.ConsumptionItems)
                .HasForeignKey(e => e.ItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // AdjustmentItem configuration
        modelBuilder.Entity<AdjustmentItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CurrentQuantity).HasPrecision(18, 4);
            entity.Property(e => e.AdjustedQuantity).HasPrecision(18, 4);
            entity.Property(e => e.AdjustmentAmount).HasPrecision(18, 4);
            entity.Property(e => e.GeneralAdjustment).HasPrecision(18, 4);
            entity.Property(e => e.ReserveAdjustment).HasPrecision(18, 4);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 4);
            entity.Property(e => e.TotalValue).HasPrecision(18, 4);

            entity.HasOne(e => e.Adjustment)
                .WithMany(a => a.Items)
                .HasForeignKey(e => e.AdjustmentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Item)
                .WithMany(i => i.AdjustmentItems)
                .HasForeignKey(e => e.ItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ReceiptItem configuration
        modelBuilder.Entity<ReceiptItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrderedQuantity).HasPrecision(18, 4);
            entity.Property(e => e.ReceivedQuantity).HasPrecision(18, 4);
            entity.Property(e => e.GeneralQuantity).HasPrecision(18, 4);
            entity.Property(e => e.CommanderReserveQuantity).HasPrecision(18, 4);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 4);
            entity.Property(e => e.TotalValue).HasPrecision(18, 4);

            entity.HasOne(e => e.Receipt)
                .WithMany(r => r.Items)
                .HasForeignKey(e => e.ReceiptId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Item)
                .WithMany(i => i.ReceiptItems)
                .HasForeignKey(e => e.ItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // PurchaseOrderItem configuration
        modelBuilder.Entity<PurchaseOrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Quantity).HasPrecision(18, 4);
            entity.Property(e => e.ReceivedQuantity).HasPrecision(18, 4);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 4);
            entity.Property(e => e.TotalValue).HasPrecision(18, 4);

            entity.HasOne(e => e.PurchaseOrder)
                .WithMany(po => po.Items)
                .HasForeignKey(e => e.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Item)
                .WithMany(i => i.PurchaseOrderItems)
                .HasForeignKey(e => e.ItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ReturnItem configuration
        modelBuilder.Entity<ReturnItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Quantity).HasPrecision(18, 4);
            entity.Property(e => e.CommanderReserveQuantity).HasPrecision(18, 4);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 4);
            entity.Property(e => e.TotalValue).HasPrecision(18, 4);

            entity.HasOne(e => e.Return)
                .WithMany(r => r.Items)
                .HasForeignKey(e => e.ReturnId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Item)
                .WithMany(i => i.ReturnItems)
                .HasForeignKey(e => e.ItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // StockAdjustment configuration
        modelBuilder.Entity<StockAdjustment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PreviousQuantity).HasPrecision(18, 4);
            entity.Property(e => e.NewQuantity).HasPrecision(18, 4);
            entity.Property(e => e.AdjustmentAmount).HasPrecision(18, 4);

            entity.HasOne(e => e.Item)
                .WithMany()
                .HasForeignKey(e => e.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.InventoryRecord)
                .WithMany()
                .HasForeignKey(e => e.InventoryRecordId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Worker configuration
        modelBuilder.Entity<Worker>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.WorkerCode).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.NameArabic).HasMaxLength(200).IsRequired();
            entity.Property(e => e.MilitaryRank).HasMaxLength(100);
            entity.Property(e => e.MilitaryRankArabic).HasMaxLength(100);
            entity.Property(e => e.NationalId).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.HasIndex(e => e.WorkerCode).IsUnique();

            entity.HasOne(e => e.Factory)
                .WithMany()
                .HasForeignKey(e => e.FactoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Department)
                .WithMany(d => d.Workers)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // OperationalCustody configuration
        modelBuilder.Entity<OperationalCustody>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CustodyNumber).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Quantity).HasPrecision(18, 4);
            entity.Property(e => e.ReturnedQuantity).HasPrecision(18, 4);
            entity.Property(e => e.ConsumedQuantity).HasPrecision(18, 4);
            entity.Property(e => e.CustodyLimit).HasPrecision(18, 4);
            entity.Property(e => e.Purpose).HasMaxLength(500).IsRequired();
            entity.Property(e => e.PurposeArabic).HasMaxLength(500).IsRequired();
            entity.Property(e => e.Notes).HasMaxLength(2000);
            entity.Property(e => e.NotesArabic).HasMaxLength(2000);
            entity.HasIndex(e => e.CustodyNumber).IsUnique();

            entity.HasOne(e => e.Worker)
                .WithMany(w => w.Custodies)
                .HasForeignKey(e => e.WorkerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Item)
                .WithMany()
                .HasForeignKey(e => e.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Warehouse)
                .WithMany()
                .HasForeignKey(e => e.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Factory)
                .WithMany()
                .HasForeignKey(e => e.FactoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Department)
                .WithMany(d => d.Custodies)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.IssuedBy)
                .WithMany()
                .HasForeignKey(e => e.IssuedById)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ReturnReceivedBy)
                .WithMany()
                .HasForeignKey(e => e.ReturnReceivedById)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ProjectBOQ configuration
        modelBuilder.Entity<ProjectBOQ>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BOQNumber).HasMaxLength(50).IsRequired();
            entity.Property(e => e.TotalQuantity).HasPrecision(18, 4);
            entity.Property(e => e.IssuedQuantity).HasPrecision(18, 4);
            entity.Property(e => e.CommanderReserveQuantity).HasPrecision(18, 4);
            entity.Property(e => e.Notes).HasMaxLength(2000);
            entity.Property(e => e.NotesArabic).HasMaxLength(2000);
            entity.Property(e => e.ApprovalNotes).HasMaxLength(2000);
            entity.Property(e => e.ApprovalNotesArabic).HasMaxLength(2000);
            entity.Property(e => e.CommanderApprovalNotes).HasMaxLength(2000);
            entity.Property(e => e.CommanderApprovalNotesArabic).HasMaxLength(2000);
            entity.Property(e => e.PartialIssueReason).HasMaxLength(2000);
            entity.Property(e => e.PartialIssueReasonArabic).HasMaxLength(2000);
            entity.HasIndex(e => e.BOQNumber).IsUnique();

            entity.HasOne(e => e.Project)
                .WithMany()
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Factory)
                .WithMany()
                .HasForeignKey(e => e.FactoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Warehouse)
                .WithMany()
                .HasForeignKey(e => e.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ApprovedBy)
                .WithMany()
                .HasForeignKey(e => e.ApprovedById)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.CommanderApprover)
                .WithMany()
                .HasForeignKey(e => e.CommanderApprovalId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.OriginalBOQ)
                .WithMany(b => b.RemainingBOQs)
                .HasForeignKey(e => e.OriginalBOQId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ProjectBOQItem configuration
        modelBuilder.Entity<ProjectBOQItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RequestedQuantity).HasPrecision(18, 4);
            entity.Property(e => e.IssuedQuantity).HasPrecision(18, 4);
            entity.Property(e => e.CommanderReserveQuantity).HasPrecision(18, 4);
            entity.Property(e => e.AvailableStock).HasPrecision(18, 4);
            entity.Property(e => e.Notes).HasMaxLength(2000);
            entity.Property(e => e.NotesArabic).HasMaxLength(2000);
            entity.Property(e => e.PartialIssueReason).HasMaxLength(2000);
            entity.Property(e => e.PartialIssueReasonArabic).HasMaxLength(2000);

            entity.HasOne(e => e.BOQ)
                .WithMany(b => b.Items)
                .HasForeignKey(e => e.BOQId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Item)
                .WithMany()
                .HasForeignKey(e => e.ItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

