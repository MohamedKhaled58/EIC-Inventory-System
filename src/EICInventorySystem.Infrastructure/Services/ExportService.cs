using EICInventorySystem.Application.Interfaces;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Drawing;
using System.Text;

namespace EICInventorySystem.Infrastructure.Services;

public class ExportService : IExportService
{
    public ExportService()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> ExportToExcelAsync<T>(IEnumerable<T> data, string sheetName, CancellationToken cancellationToken = default)
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add(sheetName);

        var dataList = data.ToList();
        if (!dataList.Any())
        {
            worksheet.Cells[1, 1].Value = "No data available";
            return await Task.FromResult(package.GetAsByteArray());
        }

        var properties = typeof(T).GetProperties()
            .Where(p => p.PropertyType.IsPrimitive || 
                        p.PropertyType == typeof(string) || 
                        p.PropertyType == typeof(decimal) || 
                        p.PropertyType == typeof(DateTime) ||
                        p.PropertyType == typeof(DateTime?) ||
                        p.PropertyType == typeof(int?) ||
                        p.PropertyType == typeof(decimal?))
            .ToArray();

        // Header row with styling
        for (int i = 0; i < properties.Length; i++)
        {
            var cell = worksheet.Cells[1, i + 1];
            cell.Value = FormatPropertyName(properties[i].Name);
            cell.Style.Font.Bold = true;
            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(68, 114, 196));
            cell.Style.Font.Color.SetColor(Color.White);
            cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        // Data rows
        for (int row = 0; row < dataList.Count; row++)
        {
            for (int col = 0; col < properties.Length; col++)
            {
                var value = properties[col].GetValue(dataList[row]);
                var cell = worksheet.Cells[row + 2, col + 1];
                
                if (value is DateTime dt)
                {
                    cell.Value = dt;
                    cell.Style.Numberformat.Format = "yyyy-mm-dd hh:mm";
                }
                else if (value is decimal dec)
                {
                    cell.Value = dec;
                    cell.Style.Numberformat.Format = "#,##0.00";
                }
                else
                {
                    cell.Value = value;
                }

                // Alternate row colors
                if (row % 2 == 1)
                {
                    cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(242, 242, 242));
                }
            }
        }

        // Auto-fit columns with max width
        worksheet.Cells.AutoFitColumns();
        for (int i = 1; i <= properties.Length; i++)
        {
            if (worksheet.Column(i).Width > 50)
                worksheet.Column(i).Width = 50;
        }

        // Freeze header row
        worksheet.View.FreezePanes(2, 1);

        // Add table formatting
        if (dataList.Count > 0)
        {
            var tableRange = worksheet.Cells[1, 1, dataList.Count + 1, properties.Length];
            tableRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            tableRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            tableRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            tableRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        }

        return await Task.FromResult(package.GetAsByteArray());
    }

    public async Task<byte[]> ExportToPdfAsync<T>(IEnumerable<T> data, string title, CancellationToken cancellationToken = default)
    {
        var dataList = data.ToList();
        var properties = typeof(T).GetProperties()
            .Where(p => p.PropertyType.IsPrimitive || 
                        p.PropertyType == typeof(string) || 
                        p.PropertyType == typeof(decimal) || 
                        p.PropertyType == typeof(DateTime) ||
                        p.PropertyType == typeof(DateTime?) ||
                        p.PropertyType == typeof(int?) ||
                        p.PropertyType == typeof(decimal?))
            .Take(10) // Limit columns for PDF readability
            .ToArray();

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
                        row.RelativeItem().Text(title).Bold().FontSize(16);
                        row.RelativeItem().AlignRight().Text($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC");
                    });
                    col.Item().Text($"Total Records: {dataList.Count}");
                    col.Item().PaddingBottom(10).LineHorizontal(1).LineColor(Colors.Grey.Medium);
                });

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        foreach (var _ in properties)
                            columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        foreach (var prop in properties)
                        {
                            header.Cell()
                                .DefaultTextStyle(x => x.SemiBold())
                                .Padding(5)
                                .Background(Colors.Grey.Lighten3)
                                .BorderBottom(1)
                                .BorderColor(Colors.Grey.Medium)
                                .Text(FormatPropertyName(prop.Name));
                        }
                    });

                    foreach (var item in dataList)
                    {
                        foreach (var prop in properties)
                        {
                            var value = prop.GetValue(item);
                            var displayValue = value switch
                            {
                                DateTime dt => dt.ToString("yyyy-MM-dd"),
                                decimal dec => dec.ToString("N2"),
                                null => "",
                                _ => value.ToString() ?? ""
                            };

                            table.Cell()
                                .Padding(4)
                                .BorderBottom(1)
                                .BorderColor(Colors.Grey.Lighten2)
                                .Text(displayValue);
                        }
                    }
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
    }

    public async Task<byte[]> ExportToCsvAsync<T>(IEnumerable<T> data, CancellationToken cancellationToken = default)
    {
        var sb = new StringBuilder();
        var dataList = data.ToList();

        if (!dataList.Any())
        {
            return await Task.FromResult(Encoding.UTF8.GetBytes("No data available"));
        }

        var properties = typeof(T).GetProperties()
            .Where(p => p.PropertyType.IsPrimitive || 
                        p.PropertyType == typeof(string) || 
                        p.PropertyType == typeof(decimal) || 
                        p.PropertyType == typeof(DateTime) ||
                        p.PropertyType == typeof(DateTime?) ||
                        p.PropertyType == typeof(int?) ||
                        p.PropertyType == typeof(decimal?))
            .ToArray();

        // Add BOM for Excel UTF-8 compatibility
        sb.Append('\uFEFF');

        // Header row
        sb.AppendLine(string.Join(",", properties.Select(p => $"\"{FormatPropertyName(p.Name)}\"")));

        // Data rows
        foreach (var item in dataList)
        {
            var values = properties.Select(p =>
            {
                var value = p.GetValue(item);
                var stringValue = value switch
                {
                    DateTime dt => dt.ToString("yyyy-MM-dd HH:mm:ss"),
                    decimal dec => dec.ToString("F2"),
                    null => "",
                    _ => value.ToString() ?? ""
                };
                // Escape quotes and wrap in quotes
                return $"\"{stringValue.Replace("\"", "\"\"")}\"";
            });
            sb.AppendLine(string.Join(",", values));
        }

        return await Task.FromResult(Encoding.UTF8.GetBytes(sb.ToString()));
    }

    private static string FormatPropertyName(string name)
    {
        // Convert PascalCase to Title Case with spaces
        var result = new StringBuilder();
        for (int i = 0; i < name.Length; i++)
        {
            if (i > 0 && char.IsUpper(name[i]))
                result.Append(' ');
            result.Append(name[i]);
        }
        return result.ToString();
    }
}
