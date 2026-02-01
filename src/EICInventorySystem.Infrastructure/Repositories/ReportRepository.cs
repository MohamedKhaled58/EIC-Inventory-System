using EICInventorySystem.Application.Interfaces;
using EICInventorySystem.Infrastructure.Data;
using EICInventorySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EICInventorySystem.Infrastructure.Repositories;

public class ReportRepository : Repository<Report>, IReportRepository
{
    public ReportRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Report>> GetByUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.CreatedByUserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Report>> GetByTypeAsync(ReportType type, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.Type == type)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Report>> GetByStatusAsync(ReportStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.Status == status)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Report>> GetPendingReportsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.Status == ReportStatus.Pending || r.Status == ReportStatus.Processing)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateStatusAsync(int reportId, ReportStatus status, CancellationToken cancellationToken = default)
    {
        var report = await _dbSet.FindAsync(new object[] { reportId }, cancellationToken);
        if (report != null)
        {
            if (status == ReportStatus.Processing)
            {
                report.MarkAsProcessing();
            }
            else if (status == ReportStatus.Completed)
            {
                report.MarkAsCompleted(report.ResultPath ?? "", 0);
            }
            else if (status == ReportStatus.Failed)
            {
                report.MarkAsFailed("Status updated to failed");
            }
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task UpdateResultAsync(int reportId, string resultPath, CancellationToken cancellationToken = default)
    {
        var report = await _dbSet.FindAsync(new object[] { reportId }, cancellationToken);
        if (report != null)
        {
            // Get file size if file exists
            long fileSizeBytes = 0;
            if (File.Exists(resultPath))
            {
                fileSizeBytes = new FileInfo(resultPath).Length;
            }
            report.MarkAsCompleted(resultPath, fileSizeBytes);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task UpdateErrorAsync(int reportId, string errorMessage, CancellationToken cancellationToken = default)
    {
        var report = await _dbSet.FindAsync(new object[] { reportId }, cancellationToken);
        if (report != null)
        {
            report.MarkAsFailed(errorMessage);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<int> GetReportCountByTypeAsync(ReportType type, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(r => r.Type == type);

        if (startDate.HasValue)
            query = query.Where(r => r.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(r => r.CreatedAt <= endDate.Value);

        return await query.CountAsync(cancellationToken);
    }

    public async Task<IEnumerable<Report>> GetRecentReportsAsync(int count, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .OrderByDescending(r => r.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task CleanupOldReportsAsync(DateTime cutoffDate, CancellationToken cancellationToken = default)
    {
        var oldReports = await _dbSet
            .Where(r => r.CreatedAt < cutoffDate && (r.Status == ReportStatus.Completed || r.Status == ReportStatus.Failed))
            .ToListAsync(cancellationToken);

        _dbSet.RemoveRange(oldReports);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
