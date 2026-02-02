using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EICInventorySystem.Infrastructure.Data;
using EICInventorySystem.Domain.Entities;

namespace EICInventorySystem.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("inventory")]
        public async Task<ActionResult<IEnumerable<InventoryReportDto>>> GetInventoryReport()
        {
            var data = await _context.InventoryRecords
                .Include(i => i.Item)
                .Include(i => i.Warehouse)
                .Select(r => new InventoryReportDto
                {
                    ItemCode = r.Item.ItemCode,
                    ItemName = r.Item.Name,
                    ItemNameAr = r.Item.NameArabic,
                    Warehouse = r.Warehouse.Name,
                    WarehouseAr = r.Warehouse.NameArabic,
                    Total = r.TotalQuantity,
                    General = r.GeneralQuantity,
                    Reserve = r.CommanderReserveQuantity,
                    Allocated = r.GeneralAllocated,
                    Available = r.GeneralQuantity - r.GeneralAllocated, // Simplified logic
                    Status = r.TotalQuantity <= r.ReorderPoint ? "LOW_STOCK" : "IN_STOCK"
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("movement")]
        public async Task<ActionResult<IEnumerable<MovementReportDto>>> GetMovementReport()
        {
            // Simplified movement logic: Using ItemTransactionHistory
            // For a real report, we'd filter by date range. Getting last 30 days for now.
            var history = await _context.ItemTransactionHistories
                .Include(h => h.Item)
                .OrderByDescending(h => h.TransactionDate)
                .Take(1000) // Limit for performance
                .ToListAsync();

            var grouped = history.GroupBy(h => h.ItemId).Select(g => new MovementReportDto
            {
                ItemCode = g.First().Item.ItemCode,
                ItemName = g.First().Item.Name,
                ItemNameAr = g.First().Item.NameArabic,
                Opening = 0, // Would need calculation from previous period
                Received = g.Where(x => x.TransactionType == "IN" || x.TransactionType == "Purchase").Sum(x => x.Quantity),
                Issued = g.Where(x => x.TransactionType == "OUT" || x.TransactionType == "Consumption").Sum(x => x.Quantity),
                TransferredIn = g.Where(x => x.TransactionType == "TransferIn").Sum(x => x.Quantity),
                TransferredOut = g.Where(x => x.TransactionType == "TransferOut").Sum(x => x.Quantity),
                NetChange = g.Sum(x => (x.TransactionType == "IN" || x.TransactionType == "Purchase" || x.TransactionType == "TransferIn") ? x.Quantity : -x.Quantity),
                Closing = 0 // Would be Opening + NetChange
            }).ToList();

            return Ok(grouped);
        }

        [HttpGet("requisitions")]
        public async Task<ActionResult<IEnumerable<RequisitionReportDto>>> GetRequisitionsReport()
        {
            var data = await _context.Requisitions
                .Include(r => r.Department)
                .Include(r => r.Project)
                .Include(r => r.Items)
                .Select(r => new RequisitionReportDto
                {
                    RequisitionNumber = r.Number,
                    Department = r.Department.Name,
                    DepartmentAr = r.Department.NameArabic,
                    Project = r.Project != null ? r.Project.Name : "",
                    ProjectAr = r.Project != null ? r.Project.NameArabic : "",
                    ItemCount = r.Items.Count,
                    TotalQuantity = r.TotalQuantity,
                    Status = r.Status.ToString(), // Enum to string
                    RequestedDate = r.RequestDate,
                    ApprovedDate = r.ApprovalDate ?? r.RequestDate, // Fallback if needed or leave nullable
                    IssuedDate = null 
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("transfers")]
        public async Task<ActionResult<IEnumerable<TransferReportDto>>> GetTransfersReport()
        {
            var data = await _context.Transfers
                .Include(t => t.SourceWarehouse)
                .Include(t => t.DestinationWarehouse)
                .Include(t => t.Items)
                .Select(t => new TransferReportDto
                {
                    TransferNumber = t.TransferNumber,
                    FromWarehouse = t.SourceWarehouse.Name,
                    FromWarehouseAr = t.SourceWarehouse.NameArabic,
                    ToWarehouse = t.DestinationWarehouse.Name,
                    ToWarehouseAr = t.DestinationWarehouse.NameArabic,
                    ItemCount = t.Items.Count,
                    TotalQuantity = t.TotalQuantity,
                    Status = t.Status.ToString(), // Enum to string
                    RequestedDate = t.CreatedAt,
                    CompletedDate = t.UpdatedAt 
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("consumption")]
        public async Task<ActionResult<IEnumerable<ConsumptionReportDto>>> GetConsumptionReport()
        {
            // Project consumption report
            var data = await _context.Projects
                .Select(p => new ConsumptionReportDto
                {
                    ProjectNumber = p.Code,
                    ProjectName = p.Name,
                    ProjectNameAr = p.NameArabic,
                    Budget = p.Budget,
                    Consumed = p.SpentAmount,
                    Remaining = p.Budget - p.SpentAmount,
                    ConsumptionPercentage = p.Budget > 0 ? (p.SpentAmount / p.Budget) * 100 : 0,
                    ItemCount = 0, 
                    Status = p.Status.ToString()
                })
                .ToListAsync();

            return Ok(data);
        }

        /// <summary>
        /// Export inventory report
        /// </summary>
        [HttpGet("inventory/export")]
        public async Task<IActionResult> ExportInventoryReport(
            [FromQuery] string format = "EXCEL",
            [FromQuery] int? warehouseId = null,
            [FromQuery] int? factoryId = null,
            [FromQuery] int? itemId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int? projectId = null,
            [FromQuery] int? departmentId = null)
        {
            var query = _context.InventoryRecords
                .Include(i => i.Item)
                .Include(i => i.Warehouse)
                .AsQueryable();

            if (warehouseId.HasValue)
                query = query.Where(i => i.WarehouseId == warehouseId.Value);
            if (itemId.HasValue)
                query = query.Where(i => i.ItemId == itemId.Value);

            var data = await query
                .Select(r => new InventoryReportDto
                {
                    ItemCode = r.Item.ItemCode,
                    ItemName = r.Item.Name,
                    ItemNameAr = r.Item.NameArabic,
                    Warehouse = r.Warehouse.Name,
                    WarehouseAr = r.Warehouse.NameArabic,
                    Total = r.TotalQuantity,
                    General = r.GeneralQuantity,
                    Reserve = r.CommanderReserveQuantity,
                    Allocated = r.GeneralAllocated,
                    Available = r.GeneralQuantity - r.GeneralAllocated,
                    Status = r.TotalQuantity <= r.ReorderPoint ? "LOW_STOCK" : "IN_STOCK"
                })
                .ToListAsync();

            return ExportToFile(data, "InventoryReport", format);
        }

        /// <summary>
        /// Export movement report
        /// </summary>
        [HttpGet("movement/export")]
        public async Task<IActionResult> ExportMovementReport(
            [FromQuery] string format = "EXCEL",
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var query = _context.ItemTransactionHistories
                .Include(h => h.Item)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(h => h.TransactionDate >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(h => h.TransactionDate <= endDate.Value);

            var history = await query
                .OrderByDescending(h => h.TransactionDate)
                .Take(1000)
                .ToListAsync();

            var grouped = history.GroupBy(h => h.ItemId).Select(g => new MovementReportDto
            {
                ItemCode = g.First().Item.ItemCode,
                ItemName = g.First().Item.Name,
                ItemNameAr = g.First().Item.NameArabic,
                Opening = 0,
                Received = g.Where(x => x.TransactionType == "IN" || x.TransactionType == "Purchase").Sum(x => x.Quantity),
                Issued = g.Where(x => x.TransactionType == "OUT" || x.TransactionType == "Consumption").Sum(x => x.Quantity),
                TransferredIn = g.Where(x => x.TransactionType == "TransferIn").Sum(x => x.Quantity),
                TransferredOut = g.Where(x => x.TransactionType == "TransferOut").Sum(x => x.Quantity),
                NetChange = g.Sum(x => (x.TransactionType == "IN" || x.TransactionType == "Purchase" || x.TransactionType == "TransferIn") ? x.Quantity : -x.Quantity),
                Closing = 0
            }).ToList();

            return ExportToFile(grouped, "MovementReport", format);
        }

        /// <summary>
        /// Export requisitions report
        /// </summary>
        [HttpGet("requisitions/export")]
        public async Task<IActionResult> ExportRequisitionsReport(
            [FromQuery] string format = "EXCEL",
            [FromQuery] int? departmentId = null,
            [FromQuery] int? projectId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var query = _context.Requisitions
                .Include(r => r.Department)
                .Include(r => r.Project)
                .Include(r => r.Items)
                .AsQueryable();

            if (departmentId.HasValue)
                query = query.Where(r => r.DepartmentId == departmentId.Value);
            if (projectId.HasValue)
                query = query.Where(r => r.ProjectId == projectId.Value);
            if (startDate.HasValue)
                query = query.Where(r => r.RequestDate >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(r => r.RequestDate <= endDate.Value);

            var data = await query
                .Select(r => new RequisitionReportDto
                {
                    RequisitionNumber = r.Number,
                    Department = r.Department.Name,
                    DepartmentAr = r.Department.NameArabic,
                    Project = r.Project != null ? r.Project.Name : "",
                    ProjectAr = r.Project != null ? r.Project.NameArabic : "",
                    ItemCount = r.Items.Count,
                    TotalQuantity = r.TotalQuantity,
                    Status = r.Status.ToString(),
                    RequestedDate = r.RequestDate,
                    ApprovedDate = r.ApprovalDate ?? r.RequestDate,
                    IssuedDate = null
                })
                .ToListAsync();

            return ExportToFile(data, "RequisitionsReport", format);
        }

        /// <summary>
        /// Export transfers report
        /// </summary>
        [HttpGet("transfers/export")]
        public async Task<IActionResult> ExportTransfersReport(
            [FromQuery] string format = "EXCEL",
            [FromQuery] int? warehouseId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var query = _context.Transfers
                .Include(t => t.SourceWarehouse)
                .Include(t => t.DestinationWarehouse)
                .Include(t => t.Items)
                .AsQueryable();

            if (warehouseId.HasValue)
                query = query.Where(t => t.SourceWarehouseId == warehouseId.Value || t.DestinationWarehouseId == warehouseId.Value);
            if (startDate.HasValue)
                query = query.Where(t => t.CreatedAt >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(t => t.CreatedAt <= endDate.Value);

            var data = await query
                .Select(t => new TransferReportDto
                {
                    TransferNumber = t.TransferNumber,
                    FromWarehouse = t.SourceWarehouse.Name,
                    FromWarehouseAr = t.SourceWarehouse.NameArabic,
                    ToWarehouse = t.DestinationWarehouse.Name,
                    ToWarehouseAr = t.DestinationWarehouse.NameArabic,
                    ItemCount = t.Items.Count,
                    TotalQuantity = t.TotalQuantity,
                    Status = t.Status.ToString(),
                    RequestedDate = t.CreatedAt,
                    CompletedDate = t.UpdatedAt
                })
                .ToListAsync();

            return ExportToFile(data, "TransfersReport", format);
        }

        /// <summary>
        /// Export consumption report
        /// </summary>
        [HttpGet("consumption/export")]
        public async Task<IActionResult> ExportConsumptionReport(
            [FromQuery] string format = "EXCEL",
            [FromQuery] int? projectId = null)
        {
            var query = _context.Projects.AsQueryable();

            if (projectId.HasValue)
                query = query.Where(p => p.Id == projectId.Value);

            var data = await query
                .Select(p => new ConsumptionReportDto
                {
                    ProjectNumber = p.Code,
                    ProjectName = p.Name,
                    ProjectNameAr = p.NameArabic,
                    Budget = p.Budget,
                    Consumed = p.SpentAmount,
                    Remaining = p.Budget - p.SpentAmount,
                    ConsumptionPercentage = p.Budget > 0 ? (p.SpentAmount / p.Budget) * 100 : 0,
                    ItemCount = 0,
                    Status = p.Status.ToString()
                })
                .ToListAsync();

            return ExportToFile(data, "ConsumptionReport", format);
        }

        private IActionResult ExportToFile<T>(List<T> data, string fileName, string format)
        {
            var csv = new System.Text.StringBuilder();
            
            if (data.Count == 0)
            {
                return File(System.Text.Encoding.UTF8.GetBytes("No data available"), 
                    "text/csv", $"{fileName}.csv");
            }

            // Get headers from properties
            var properties = typeof(T).GetProperties();
            csv.AppendLine(string.Join(",", properties.Select(p => p.Name)));

            // Add data rows
            foreach (var item in data)
            {
                var values = properties.Select(p => 
                {
                    var value = p.GetValue(item)?.ToString() ?? "";
                    // Escape CSV values
                    if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
                    {
                        value = "\"" + value.Replace("\"", "\"\"") + "\"";
                    }
                    return value;
                });
                csv.AppendLine(string.Join(",", values));
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
            var contentType = format.ToUpper() == "PDF" ? "application/pdf" : "text/csv";
            var extension = format.ToUpper() == "PDF" ? "pdf" : "csv";
            
            return File(bytes, contentType, $"{fileName}_{DateTime.Now:yyyyMMdd}.{extension}");
        }
    }

    // DTOs
    public class InventoryReportDto
    {
        public string ItemCode { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public string ItemNameAr { get; set; } = string.Empty;
        public string Warehouse { get; set; } = string.Empty;
        public string WarehouseAr { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public decimal General { get; set; }
        public decimal Reserve { get; set; }
        public decimal Allocated { get; set; }
        public decimal Available { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class MovementReportDto
    {
        public string ItemCode { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public string ItemNameAr { get; set; } = string.Empty;
        public decimal Opening { get; set; }
        public decimal Received { get; set; }
        public decimal Issued { get; set; }
        public decimal TransferredIn { get; set; }
        public decimal TransferredOut { get; set; }
        public decimal NetChange { get; set; }
        public decimal Closing { get; set; }
    }

    public class RequisitionReportDto
    {
        public string RequisitionNumber { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string DepartmentAr { get; set; } = string.Empty;
        public string Project { get; set; } = string.Empty;
        public string ProjectAr { get; set; } = string.Empty;
        public int ItemCount { get; set; }
        public decimal TotalQuantity { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime RequestedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? IssuedDate { get; set; }
    }

    public class TransferReportDto
    {
        public string TransferNumber { get; set; } = string.Empty;
        public string FromWarehouse { get; set; } = string.Empty;
        public string FromWarehouseAr { get; set; } = string.Empty;
        public string ToWarehouse { get; set; } = string.Empty;
        public string ToWarehouseAr { get; set; } = string.Empty;
        public int ItemCount { get; set; }
        public decimal TotalQuantity { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime RequestedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
    }

    public class ConsumptionReportDto
    {
        public string ProjectNumber { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string ProjectNameAr { get; set; } = string.Empty;
        public decimal Budget { get; set; }
        public decimal Consumed { get; set; }
        public decimal Remaining { get; set; }
        public decimal ConsumptionPercentage { get; set; }
        public int ItemCount { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
