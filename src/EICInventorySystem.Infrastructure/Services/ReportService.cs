using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Application.Interfaces;
using EICInventorySystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EICInventorySystem.Infrastructure.Services;

public class ReportService : IReportService
{
    private readonly ApplicationDbContext _context;
    private readonly IInventoryService _inventoryService;
    private readonly ICommanderReserveService _commanderReserveService;

    public ReportService(ApplicationDbContext context, IInventoryService inventoryService, ICommanderReserveService commanderReserveService)
    {
        _context = context;
        _inventoryService = inventoryService;
        _commanderReserveService = commanderReserveService;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> GenerateInventoryReportAsync(int? warehouseId = null, string format = "pdf", CancellationToken cancellationToken = default)
    {
        var records = (await _inventoryService.GetInventoryRecordsAsync(warehouseId, cancellationToken: cancellationToken)).ToList();
        var warehouseName = warehouseId.HasValue
            ? (await _context.Warehouses.FindAsync(new object[] { warehouseId.Value }, cancellationToken))?.Name ?? "All Warehouses"
            : "All Warehouses";

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(9));

                page.Header().Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text("EIC Inventory System").Bold().FontSize(16);
                        row.RelativeItem().AlignRight().Text($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC");
                    });
                    col.Item().PaddingTop(5).Text($"Inventory Report - {warehouseName}").SemiBold().FontSize(14);
                    col.Item().PaddingBottom(10).LineHorizontal(1).LineColor(Colors.Grey.Medium);
                });

                page.Content().Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Background(Colors.Blue.Lighten4).Padding(10).Column(c =>
                        {
                            c.Item().Text("Total Items").FontSize(10);
                            c.Item().Text(records.Count.ToString()).Bold().FontSize(18);
                        });
                        row.ConstantItem(10);
                        row.RelativeItem().Background(Colors.Green.Lighten4).Padding(10).Column(c =>
                        {
                            c.Item().Text("Total Quantity").FontSize(10);
                            c.Item().Text(records.Sum(r => r.TotalQuantity).ToString("N2")).Bold().FontSize(18);
                        });
                        row.ConstantItem(10);
                        row.RelativeItem().Background(Colors.Orange.Lighten4).Padding(10).Column(c =>
                        {
                            c.Item().Text("Low Stock Items").FontSize(10);
                            c.Item().Text(records.Count(r => r.Status == "Low").ToString()).Bold().FontSize(18);
                        });
                        row.ConstantItem(10);
                        row.RelativeItem().Background(Colors.Red.Lighten4).Padding(10).Column(c =>
                        {
                            c.Item().Text("Critical Items").FontSize(10);
                            c.Item().Text(records.Count(r => r.Status == "Critical").ToString()).Bold().FontSize(18);
                        });
                    });

                    col.Item().PaddingTop(15).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1.2f);
                            columns.RelativeColumn(1.2f);
                            columns.RelativeColumn(1.2f);
                            columns.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderStyle).Text("Item Code");
                            header.Cell().Element(HeaderStyle).Text("Item Name");
                            header.Cell().Element(HeaderStyle).Text("Unit");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("Total Qty");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("General");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("Reserve");
                            header.Cell().Element(HeaderStyle).AlignCenter().Text("Status");
                        });

                        foreach (var record in records.OrderBy(r => r.ItemCode))
                        {
                            var bgColor = record.Status == "Critical" ? Colors.Red.Lighten5 :
                                          record.Status == "Low" ? Colors.Orange.Lighten5 : Colors.White;

                            table.Cell().Element(c => CellStyle(c, bgColor)).Text(record.ItemCode);
                            table.Cell().Element(c => CellStyle(c, bgColor)).Text(record.ItemName);
                            table.Cell().Element(c => CellStyle(c, bgColor)).Text(record.Unit);
                            table.Cell().Element(c => CellStyle(c, bgColor)).AlignRight().Text(record.TotalQuantity.ToString("N2"));
                            table.Cell().Element(c => CellStyle(c, bgColor)).AlignRight().Text(record.GeneralQuantity.ToString("N2"));
                            table.Cell().Element(c => CellStyle(c, bgColor)).AlignRight().Text(record.CommanderReserveQuantity.ToString("N2"));
                            table.Cell().Element(c => CellStyle(c, bgColor)).AlignCenter().Text(record.Status);
                        }
                    });
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Page ");
                    x.CurrentPageNumber();
                    x.Span(" of ");
                    x.TotalPages();
                });
            });
        }).GeneratePdf();

        static IContainer HeaderStyle(IContainer container) =>
            container.DefaultTextStyle(x => x.SemiBold()).Padding(5).Background(Colors.Grey.Lighten3).BorderBottom(1).BorderColor(Colors.Grey.Medium);

        static IContainer CellStyle(IContainer container, string bgColor) =>
            container.Padding(4).Background(bgColor).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
    }

    public async Task<byte[]> GenerateRequisitionReportAsync(DateTime? startDate = null, DateTime? endDate = null, string format = "pdf", CancellationToken cancellationToken = default)
    {
        var query = _context.Requisitions
            .Include(r => r.Requester)
            .Include(r => r.Warehouse)
            .Include(r => r.Department)
            .AsQueryable();

        if (startDate.HasValue)
            query = query.Where(r => r.RequestDate >= startDate.Value);
        if (endDate.HasValue)
            query = query.Where(r => r.RequestDate <= endDate.Value);

        var requisitions = await query.OrderByDescending(r => r.RequestDate).Take(500).ToListAsync(cancellationToken);

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(9));

                page.Header().Column(col =>
                {
                    col.Item().Text("Requisition Report").Bold().FontSize(16);
                    col.Item().Text($"Period: {startDate?.ToString("yyyy-MM-dd") ?? "All"} to {endDate?.ToString("yyyy-MM-dd") ?? "Present"}");
                    col.Item().PaddingBottom(10).LineHorizontal(1);
                });

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(1.5f);
                        columns.RelativeColumn(1.2f);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(1.2f);
                        columns.RelativeColumn(1);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(HeaderStyle).Text("Req. Number");
                        header.Cell().Element(HeaderStyle).Text("Date");
                        header.Cell().Element(HeaderStyle).Text("Requester");
                        header.Cell().Element(HeaderStyle).Text("Warehouse");
                        header.Cell().Element(HeaderStyle).Text("Priority");
                        header.Cell().Element(HeaderStyle).AlignRight().Text("Total Qty");
                        header.Cell().Element(HeaderStyle).Text("Status");
                    });

                    foreach (var req in requisitions)
                    {
                        table.Cell().Element(CellStyle).Text(req.Number);
                        table.Cell().Element(CellStyle).Text(req.RequestDate.ToString("yyyy-MM-dd"));
                        table.Cell().Element(CellStyle).Text(req.Requester?.FullName ?? "");
                        table.Cell().Element(CellStyle).Text(req.Warehouse?.Name ?? "");
                        table.Cell().Element(CellStyle).Text(req.Priority.ToString());
                        table.Cell().Element(CellStyle).AlignRight().Text(req.TotalQuantity.ToString("N2") ?? "");
                        table.Cell().Element(CellStyle).Text(req.Status.ToString());
                    }
                });

                page.Footer().AlignCenter().Text(x => { x.CurrentPageNumber(); x.Span(" / "); x.TotalPages(); });
            });
        }).GeneratePdf();

        static IContainer HeaderStyle(IContainer c) => c.DefaultTextStyle(x => x.SemiBold()).Padding(5).Background(Colors.Grey.Lighten3).BorderBottom(1);
        static IContainer CellStyle(IContainer c) => c.Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
    }

    public async Task<byte[]> GenerateTransferReportAsync(DateTime? startDate = null, DateTime? endDate = null, string format = "pdf", CancellationToken cancellationToken = default)
    {
        var query = _context.Transfers
            .Include(t => t.SourceWarehouse)
            .Include(t => t.DestinationWarehouse)
            .Include(t => t.CreatedByUser)
            .AsQueryable();

        if (startDate.HasValue)
            query = query.Where(t => t.TransferDate >= startDate.Value);
        if (endDate.HasValue)
            query = query.Where(t => t.TransferDate <= endDate.Value);

        var transfers = await query.OrderByDescending(t => t.TransferDate).Take(500).ToListAsync(cancellationToken);

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(9));

                page.Header().Column(col =>
                {
                    col.Item().Text("Transfer Report").Bold().FontSize(16);
                    col.Item().Text($"Period: {startDate?.ToString("yyyy-MM-dd") ?? "All"} to {endDate?.ToString("yyyy-MM-dd") ?? "Present"}");
                    col.Item().PaddingBottom(10).LineHorizontal(1);
                });

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(1.5f);
                        columns.RelativeColumn(1.2f);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(1.2f);
                        columns.RelativeColumn(1);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(HeaderStyle).Text("Transfer No.");
                        header.Cell().Element(HeaderStyle).Text("Date");
                        header.Cell().Element(HeaderStyle).Text("From Warehouse");
                        header.Cell().Element(HeaderStyle).Text("To Warehouse");
                        header.Cell().Element(HeaderStyle).AlignRight().Text("Total Qty");
                        header.Cell().Element(HeaderStyle).Text("Status");
                    });

                    foreach (var transfer in transfers)
                    {
                        table.Cell().Element(CellStyle).Text(transfer.TransferNumber);
                        table.Cell().Element(CellStyle).Text(transfer.TransferDate.ToString("yyyy-MM-dd"));
                        table.Cell().Element(CellStyle).Text(transfer.SourceWarehouse?.Name ?? "");
                        table.Cell().Element(CellStyle).Text(transfer.DestinationWarehouse?.Name ?? "");
                        table.Cell().Element(CellStyle).AlignRight().Text(transfer.TotalQuantity.ToString("N2"));
                        table.Cell().Element(CellStyle).Text(transfer.Status);
                    }
                });

                page.Footer().AlignCenter().Text(x => { x.CurrentPageNumber(); x.Span(" / "); x.TotalPages(); });
            });
        }).GeneratePdf();

        static IContainer HeaderStyle(IContainer c) => c.DefaultTextStyle(x => x.SemiBold()).Padding(5).Background(Colors.Grey.Lighten3).BorderBottom(1);
        static IContainer CellStyle(IContainer c) => c.Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
    }

    public async Task<byte[]> GenerateStockMovementReportAsync(int? warehouseId = null, DateTime? startDate = null, DateTime? endDate = null, string format = "pdf", CancellationToken cancellationToken = default)
    {
        var query = _context.ItemTransactionHistories
            .Include(t => t.Item)
            .Include(t => t.Department)
            .AsQueryable();

        if (startDate.HasValue)
            query = query.Where(t => t.TransactionDate >= startDate.Value);
        if (endDate.HasValue)
            query = query.Where(t => t.TransactionDate <= endDate.Value);

        var transactions = await query.OrderByDescending(t => t.TransactionDate).Take(1000).ToListAsync(cancellationToken);

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(9));

                page.Header().Column(col =>
                {
                    col.Item().Text("Stock Movement Report").Bold().FontSize(16);
                    col.Item().Text($"Period: {startDate?.ToString("yyyy-MM-dd") ?? "All"} to {endDate?.ToString("yyyy-MM-dd") ?? "Present"}");
                    col.Item().PaddingBottom(10).LineHorizontal(1);
                });

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(1.2f);
                        columns.RelativeColumn(1.5f);
                        columns.RelativeColumn(2.5f);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(1.5f);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(HeaderStyle).Text("Date");
                        header.Cell().Element(HeaderStyle).Text("Item Code");
                        header.Cell().Element(HeaderStyle).Text("Item Name");
                        header.Cell().Element(HeaderStyle).Text("Type");
                        header.Cell().Element(HeaderStyle).AlignRight().Text("Quantity");
                        header.Cell().Element(HeaderStyle).Text("Worker");
                        header.Cell().Element(HeaderStyle).Text("Reference");
                    });

                    foreach (var tx in transactions)
                    {
                        table.Cell().Element(CellStyle).Text(tx.TransactionDate.ToString("yyyy-MM-dd"));
                        table.Cell().Element(CellStyle).Text(tx.Item?.ItemCode ?? "");
                        table.Cell().Element(CellStyle).Text(tx.Item?.Name ?? "");
                        table.Cell().Element(CellStyle).Text(tx.TransactionType);
                        table.Cell().Element(CellStyle).AlignRight().Text(tx.Quantity.ToString("N2"));
                        table.Cell().Element(CellStyle).Text(tx.WorkerName);
                        table.Cell().Element(CellStyle).Text(tx.ReferenceNumber ?? "");
                    }
                });

                page.Footer().AlignCenter().Text(x => { x.CurrentPageNumber(); x.Span(" / "); x.TotalPages(); });
            });
        }).GeneratePdf();

        static IContainer HeaderStyle(IContainer c) => c.DefaultTextStyle(x => x.SemiBold()).Padding(5).Background(Colors.Grey.Lighten3).BorderBottom(1);
        static IContainer CellStyle(IContainer c) => c.Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
    }

    public async Task<byte[]> GenerateCommanderReserveReportAsync(int? warehouseId = null, string format = "pdf", CancellationToken cancellationToken = default)
    {
        var reserves = (await _commanderReserveService.GetCommanderReserveAsync(warehouseId, cancellationToken: cancellationToken)).ToList();
        var summary = (await _commanderReserveService.GetCommanderReserveSummaryAsync(warehouseId, cancellationToken)).ToList();

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(9));

                page.Header().Column(col =>
                {
                    col.Item().Text("Commander's Reserve Report").Bold().FontSize(16);
                    col.Item().Text($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC");
                    col.Item().PaddingBottom(10).LineHorizontal(1);
                });

                page.Content().Column(col =>
                {
                    // Summary section
                    col.Item().Text("Summary by Warehouse").SemiBold().FontSize(12);
                    col.Item().PaddingTop(5).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1.2f);
                            columns.RelativeColumn(1.2f);
                            columns.RelativeColumn(1.2f);
                            columns.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderStyle).Text("Warehouse");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("Items");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("Total Reserve");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("Allocated");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("Available");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("Low Items");
                        });

                        foreach (var s in summary)
                        {
                            table.Cell().Element(CellStyle).Text(s.WarehouseName);
                            table.Cell().Element(CellStyle).AlignRight().Text(s.TotalItems.ToString());
                            table.Cell().Element(CellStyle).AlignRight().Text(s.TotalReserve.ToString("N2"));
                            table.Cell().Element(CellStyle).AlignRight().Text(s.TotalAllocated.ToString("N2"));
                            table.Cell().Element(CellStyle).AlignRight().Text(s.AvailableReserve.ToString("N2"));
                            table.Cell().Element(CellStyle).AlignRight().Text(s.LowReserveItems.ToString());
                        }
                    });

                    // Details section
                    col.Item().PaddingTop(15).Text("Reserve Details").SemiBold().FontSize(12);
                    col.Item().PaddingTop(5).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(2.5f);
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderStyle).Text("Item Code");
                            header.Cell().Element(HeaderStyle).Text("Item Name");
                            header.Cell().Element(HeaderStyle).Text("Warehouse");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("Reserve");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("Allocated");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("Available");
                            header.Cell().Element(HeaderStyle).Text("Status");
                        });

                        foreach (var r in reserves.OrderBy(r => r.ItemCode))
                        {
                            var bgColor = r.Status == "Low" ? Colors.Orange.Lighten5 : Colors.White;
                            table.Cell().Element(c => CellStyleWithBg(c, bgColor)).Text(r.ItemCode);
                            table.Cell().Element(c => CellStyleWithBg(c, bgColor)).Text(r.ItemName);
                            table.Cell().Element(c => CellStyleWithBg(c, bgColor)).Text(r.WarehouseName);
                            table.Cell().Element(c => CellStyleWithBg(c, bgColor)).AlignRight().Text(r.ReserveQuantity.ToString("N2"));
                            table.Cell().Element(c => CellStyleWithBg(c, bgColor)).AlignRight().Text(r.ReserveAllocated.ToString("N2"));
                            table.Cell().Element(c => CellStyleWithBg(c, bgColor)).AlignRight().Text(r.AvailableReserve.ToString("N2"));
                            table.Cell().Element(c => CellStyleWithBg(c, bgColor)).Text(r.Status);
                        }
                    });
                });

                page.Footer().AlignCenter().Text(x => { x.CurrentPageNumber(); x.Span(" / "); x.TotalPages(); });
            });
        }).GeneratePdf();

        static IContainer HeaderStyle(IContainer c) => c.DefaultTextStyle(x => x.SemiBold()).Padding(5).Background(Colors.Grey.Lighten3).BorderBottom(1);
        static IContainer CellStyle(IContainer c) => c.Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
        static IContainer CellStyleWithBg(IContainer c, string bg) => c.Padding(4).Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
    }
}
