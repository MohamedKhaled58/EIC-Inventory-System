namespace EICInventorySystem.Application.Interfaces;

public interface IExportService
{
    Task<byte[]> ExportToExcelAsync<T>(IEnumerable<T> data, string sheetName, CancellationToken cancellationToken = default);
    Task<byte[]> ExportToPdfAsync<T>(IEnumerable<T> data, string title, CancellationToken cancellationToken = default);
    Task<byte[]> ExportToCsvAsync<T>(IEnumerable<T> data, CancellationToken cancellationToken = default);
}
