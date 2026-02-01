using EICInventorySystem.Domain.Entities;
using EICInventorySystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace EICInventorySystem.Infrastructure.Data;

public static class SeedData
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Check if data already seeded
        if (await context.Users.AnyAsync())
        {
            return;
        }

        // 1. Seed Users (Moved up because Factories need createdBy user ID)
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");
        var adminUser = new User(
            "admin",
            "admin@eic-inventory.com",
            passwordHash,
            "System Administrator",
            "مدير النظام",
            "ComplexCommander",
            0
        );

        // 2. Seed Factories
        // Factory(code, name, nameArabic, description, descriptionArabic, location, locationArabic, createdBy, commanderId)
        var factory1 = new Factory(
            "FACT-001",
            "Main Production Factory",
            "المصنع الرئيسي للإنتاج",
            "Main production facility for armored vehicles",
            "منشأة الإنتاج الرئيسية للمركبات المدرعة",
            "Cairo Industrial Zone",
            "المنطقة الصناعية بالقاهرة",
            adminUser.Id
        );

        var factory2 = new Factory(
            "FACT-002",
            "Secondary Production Factory",
            "المصنع الثانوي للإنتاج",
            "Secondary production facility for military equipment",
            "منشأة الإنتاج الثانوية للمعدات العسكرية",
            "Alexandria Industrial Zone",
            "المنطقة الصناعية بالإسكندرية",
            0
        );

        var factories = new List<Factory> { factory1, factory2 };
        await context.Factories.AddRangeAsync(factories);
        await context.SaveChangesAsync();

        // 3. Seed Warehouses
        // Warehouse(code, name, nameArabic, type, location, locationArabic, createdBy, factoryId, keeperId)
        var warehouse1 = new Warehouse(
            "WH-CENT-001",
            "Central Warehouse 1",
            "المخزن المركزي 1",
            "WarehouseType.Central.ToString()",
            "Main Factory Complex",
            "مجمع المصنع الرئيسي",
            0,
            factory1.Id
        );

        var warehouse2 = new Warehouse(
            "WH-CENT-002",
            "Central Warehouse 2",
            "المخزن المركزي 2",
            "WarehouseType.Central.ToString()",
            "Secondary Factory Complex",
            "مجمع المصنع الثانوي",
            0,
            factory2.Id
        );

        var warehouse3 = new Warehouse(
            "WH-FACT-001",
            "Factory Warehouse 1",
            "مخزن المصنع 1",
            "WarehouseType.Factory.ToString()",
            "Main Factory Building A",
            "مبنى المصنع الرئيسي أ",
            0,
            factory1.Id
        );

        var warehouse4 = new Warehouse(
            "WH-FACT-002",
            "Factory Warehouse 2",
            "مخزن المصنع 2",
            "WarehouseType.Factory.ToString()",
            "Secondary Factory Building B",
            "مبنى المصنع الثانوي ب",
            0,
            factory2.Id
        );

        var warehouses = new List<Warehouse> { warehouse1, warehouse2, warehouse3, warehouse4 };
        await context.Warehouses.AddRangeAsync(warehouses);
        await context.SaveChangesAsync();

        // 4. Seed Departments
        // Department(code, name, nameArabic, description, descriptionArabic, factoryId, createdBy, headId)
        var dept1 = new Department(
            "DEPT-PROD",
            "Production Department",
            "قسم الإنتاج",
            "Main production department",
            "قسم الإنتاج الرئيسي",
            factory1.Id,
            0
        );

        var dept2 = new Department(
            "DEPT-QC",
            "Quality Control Department",
            "قسم مراقبة الجودة",
            "Quality control and inspection department",
            "قسم مراقبة الجودة والتفتيش",
            factory1.Id,
            0
        );

        var dept3 = new Department(
            "DEPT-MAINT",
            "Maintenance Department",
            "قسم الصيانة",
            "Equipment maintenance department",
            "قسم صيانة المعدات",
            factory1.Id,
            0
        );

        var departments = new List<Department> { dept1, dept2, dept3 };
        await context.Departments.AddRangeAsync(departments);
        await context.SaveChangesAsync();


        var factoryCmdUser = new User(
            "factory1cmd",
            "factory1cmd@eic-inventory.com",
            BCrypt.Net.BCrypt.HashPassword("Factory@123"),
            "Factory 1 Commander",
            "قائد المصنع 1",
            "FactoryCommander",
            0,
            factoryId: factory1.Id
        );

        var warehouseUser = new User(
            "warehouse1",
            "warehouse1@eic-inventory.com",
            BCrypt.Net.BCrypt.HashPassword("Warehouse@123"),
            "Central Warehouse Keeper",
            "أمين المخزن المركزي",
            "CentralWarehouseKeeper",
            0
        );

        var deptHeadUser = new User(
            "dept1head",
            "dept1head@eic-inventory.com",
            BCrypt.Net.BCrypt.HashPassword("Dept@123"),
            "Production Department Head",
            "رئيس قسم الإنتاج",
            "DepartmentHead",
            0,
            factoryId: factory1.Id,
            departmentId: dept1.Id
        );

        var users = new List<User> { adminUser, factoryCmdUser, warehouseUser, deptHeadUser };
        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();

        // 5. Seed Items
        var item1 = new Item(
            "ITM-001",
            "Steel Plate",
            "لوح فولاذ",
            "High-grade steel plate for vehicle armor",
            "لوح فولاذ عالي الجودة للدروع المركبات",
            "Raw Materials",
            "المواد الخام",
            "KG",
            "كيلوجرام",
            50.00m,
            1.0m,
            "KG",
            2000,
            1000,
            10000,
            20.0m,
            0,
            barcode: "1234567890123"
        );

        var item2 = new Item(
            "ITM-002",
            "Aluminum Sheet",
            "ورق ألمنيوم",
            "Aluminum sheet for vehicle body",
            "ورق ألمنيوم لجسم المركبة",
            "Raw Materials",
            "المواد الخام",
            "KG",
            "كيلوجرام",
            30.00m,
            1.0m,
            "KG",
            1000,
            500,
            5000,
            20.0m,
            0,
            barcode: "1234567890124"
        );

        var item3 = new Item(
            "ITM-003",
            "Hydraulic Pump",
            "مضخة هيدروليكية",
            "Heavy-duty hydraulic pump for military vehicles",
            "مضخة هيدروليكية ثقيلة للمركبات العسكرية",
            "Components",
            "المكونات",
            "PCS",
            "قطعة",
            500.00m,
            5.0m,
            "KG",
            75,
            50,
            200,
            20.0m,
            0,
            barcode: "1234567890125"
        );

        var items = new List<Item> { item1, item2, item3 };
        await context.Items.AddRangeAsync(items);
        await context.SaveChangesAsync();

        // 6. Seed Inventory Records
        var inv1 = new InventoryRecord(
            warehouse1.Id,
            item1.Id,
            5000,
            3750,
            1250,
            1000,
            2000,
            warehouseUser.Id
        );

        var inv2 = new InventoryRecord(
            warehouse1.Id,
            item2.Id,
            3000,
            2250,
            750,
            600,
            1000,
            warehouseUser.Id
        );

        var inv3 = new InventoryRecord(
            warehouse3.Id,
            item1.Id,
            2000,
            1500,
            500,
            400,
            800,
            warehouseUser.Id
        );

        var inventoryRecords = new List<InventoryRecord> { inv1, inv2, inv3 };
        await context.InventoryRecords.AddRangeAsync(inventoryRecords);
        await context.SaveChangesAsync();

        // 7. Seed System Settings
        var settings = new SystemSettings(
            "EIC Inventory System",
            "نظام إدارة المخازن",
            "ar",
            "EGP",
            "dd/MM/yyyy",
            true,
            adminUser.Id
        );

        await context.SystemSettings.AddAsync(settings);
        await context.SaveChangesAsync();
    }
}
