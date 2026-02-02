using EICInventorySystem.Application.Interfaces;
using EICInventorySystem.Infrastructure.Data;
using EICInventorySystem.Infrastructure.Repositories;
using EICInventorySystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EICInventorySystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(
                configuration.GetConnectionString("DefaultConnection")));

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IFactoryRepository, FactoryRepository>();
        services.AddScoped<IWarehouseRepository, WarehouseRepository>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IRequisitionRepository, RequisitionRepository>();
        services.AddScoped<ITransferRepository, TransferRepository>();
        services.AddScoped<IReceiptRepository, ReceiptRepository>();
        services.AddScoped<IConsumptionRepository, ConsumptionRepository>();
        services.AddScoped<IReturnRepository, ReturnRepository>();
        services.AddScoped<IAdjustmentRepository, AdjustmentRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<ISystemSettingsRepository, SystemSettingsRepository>();

        // Register Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IRequisitionService, RequisitionService>();
        services.AddScoped<ITransferService, TransferService>();
        services.AddScoped<ICommanderReserveService, CommanderReserveService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IExportService, ExportService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<ISecurityService, SecurityService>();
        
        // BOQ & Custody Services
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IProjectBOQService, ProjectBOQService>();
        services.AddScoped<IWorkerService, WorkerService>();
        services.AddScoped<IOperationalCustodyService, OperationalCustodyService>();

        // Register Background Services
        services.AddHostedService<NotificationBackgroundService>();
        services.AddHostedService<InventoryAlertBackgroundService>();
        services.AddHostedService<ReportGenerationBackgroundService>();

        return services;
    }
}
