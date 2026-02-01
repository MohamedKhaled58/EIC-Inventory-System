using EICInventorySystem.Domain.Enums;

namespace EICInventorySystem.Domain.Entities;

/// <summary>
/// Project Bill of Quantities (BOQ) - defines material requirements for a project
/// Supports full and partial issuance with automatic splitting
/// Tracks Commander Reserve requirements
/// </summary>
public class ProjectBOQ : BaseEntity
{
    public string BOQNumber { get; private set; } = null!;
    public int ProjectId { get; private set; }
    public int FactoryId { get; private set; }
    public int WarehouseId { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public DateTime? RequiredDate { get; private set; }
    public DateTime? ApprovedDate { get; private set; }
    public DateTime? IssuedDate { get; private set; }
    public DateTime? CompletedDate { get; private set; }
    public BOQStatus Status { get; private set; }
    public BOQPriority Priority { get; private set; }

    // Quantities (computed from items)
    public decimal TotalQuantity { get; private set; }
    public decimal IssuedQuantity { get; private set; }
    public decimal RemainingQuantity => TotalQuantity - IssuedQuantity;
    public int TotalItems { get; private set; }

    // Commander Reserve tracking
    public bool RequiresCommanderReserve { get; private set; }
    public decimal CommanderReserveQuantity { get; private set; }
    public int? CommanderApprovalId { get; private set; }
    public DateTime? CommanderApprovalDate { get; private set; }
    public string? CommanderApprovalNotes { get; private set; }
    public string? CommanderApprovalNotesArabic { get; private set; }

    // Partial issue tracking - links to original BOQ if this is a split/remaining BOQ
    public int? OriginalBOQId { get; private set; }
    public bool IsRemainingBOQ { get; private set; }
    public string? PartialIssueReason { get; private set; }
    public string? PartialIssueReasonArabic { get; private set; }

    // Approval tracking
    public int? ApprovedById { get; private set; }
    public string? ApprovalNotes { get; private set; }
    public string? ApprovalNotesArabic { get; private set; }

    // Notes
    public string? Notes { get; private set; }
    public string? NotesArabic { get; private set; }

    // Navigation properties
    public Project Project { get; private set; } = null!;
    public Factory Factory { get; private set; } = null!;
    public Warehouse Warehouse { get; private set; } = null!;
    public User? ApprovedBy { get; private set; }
    public User? CommanderApprover { get; private set; }
    public ProjectBOQ? OriginalBOQ { get; private set; }
    public ICollection<ProjectBOQ> RemainingBOQs { get; private set; } = new List<ProjectBOQ>();
    public ICollection<ProjectBOQItem> Items { get; private set; } = new List<ProjectBOQItem>();
    public ICollection<InventoryTransaction> Transactions { get; private set; } = new List<InventoryTransaction>();

    private ProjectBOQ() { }

    public ProjectBOQ(
        string boqNumber,
        int projectId,
        int factoryId,
        int warehouseId,
        DateTime requiredDate,
        BOQPriority priority,
        int createdBy,
        string? notes = null,
        string? notesArabic = null) : base(createdBy)
    {
        BOQNumber = boqNumber;
        ProjectId = projectId;
        FactoryId = factoryId;
        WarehouseId = warehouseId;
        CreatedDate = DateTime.UtcNow;
        RequiredDate = requiredDate;
        Status = BOQStatus.Draft;
        Priority = priority;
        TotalQuantity = 0;
        IssuedQuantity = 0;
        TotalItems = 0;
        RequiresCommanderReserve = false;
        CommanderReserveQuantity = 0;
        IsRemainingBOQ = false;
        Notes = notes;
        NotesArabic = notesArabic;
    }

    /// <summary>
    /// Creates a remaining BOQ from partial issuance
    /// </summary>
    public static ProjectBOQ CreateRemainingBOQ(
        string boqNumber,
        ProjectBOQ originalBOQ,
        string partialIssueReason,
        string partialIssueReasonArabic,
        int createdBy)
    {
        var remainingBOQ = new ProjectBOQ(
            boqNumber,
            originalBOQ.ProjectId,
            originalBOQ.FactoryId,
            originalBOQ.WarehouseId,
            originalBOQ.RequiredDate ?? DateTime.UtcNow.AddDays(7),
            originalBOQ.Priority,
            createdBy,
            originalBOQ.Notes,
            originalBOQ.NotesArabic);

        remainingBOQ.OriginalBOQId = originalBOQ.Id;
        remainingBOQ.IsRemainingBOQ = true;
        remainingBOQ.PartialIssueReason = partialIssueReason;
        remainingBOQ.PartialIssueReasonArabic = partialIssueReasonArabic;
        remainingBOQ.Status = BOQStatus.Pending; // Remaining BOQs go to pending automatically

        return remainingBOQ;
    }

    public void UpdateTotals(int updatedBy)
    {
        TotalQuantity = Items?.Sum(i => i.RequestedQuantity) ?? 0;
        IssuedQuantity = Items?.Sum(i => i.IssuedQuantity) ?? 0;
        TotalItems = Items?.Count ?? 0;
        CommanderReserveQuantity = Items?.Sum(i => i.CommanderReserveQuantity) ?? 0;
        RequiresCommanderReserve = CommanderReserveQuantity > 0;
        Update(updatedBy);
    }

    public void Submit(int updatedBy)
    {
        if (Status != BOQStatus.Draft)
            throw new InvalidOperationException("Only draft BOQs can be submitted");

        Status = BOQStatus.Pending;
        Update(updatedBy);
    }

    public void Approve(int approverId, int updatedBy, string? approvalNotes = null, string? approvalNotesArabic = null)
    {
        if (Status != BOQStatus.Pending)
            throw new InvalidOperationException("Only pending BOQs can be approved");

        ApprovedById = approverId;
        ApprovedDate = DateTime.UtcNow;
        ApprovalNotes = approvalNotes;
        ApprovalNotesArabic = approvalNotesArabic;
        Status = BOQStatus.Approved;
        Update(updatedBy);
    }

    public void ApproveCommanderReserve(int commanderId, int updatedBy, string? approvalNotes = null, string? approvalNotesArabic = null)
    {
        CommanderApprovalId = commanderId;
        CommanderApprovalDate = DateTime.UtcNow;
        CommanderApprovalNotes = approvalNotes;
        CommanderApprovalNotesArabic = approvalNotesArabic;
        Update(updatedBy);
    }

    public void Reject(int approverId, string rejectionReason, string rejectionReasonArabic, int updatedBy)
    {
        ApprovedById = approverId;
        ApprovedDate = DateTime.UtcNow;
        ApprovalNotes = rejectionReason;
        ApprovalNotesArabic = rejectionReasonArabic;
        Status = BOQStatus.Cancelled;
        Update(updatedBy);
    }

    public void IssueFullly(int updatedBy)
    {
        if (Status != BOQStatus.Approved)
            throw new InvalidOperationException("Only approved BOQs can be issued");

        IssuedDate = DateTime.UtcNow;
        IssuedQuantity = TotalQuantity;
        Status = BOQStatus.FullyIssued;
        CompletedDate = DateTime.UtcNow;
        Update(updatedBy);
    }

    public void IssuePartially(decimal issuedQty, int updatedBy)
    {
        if (Status != BOQStatus.Approved)
            throw new InvalidOperationException("Only approved BOQs can be issued");

        IssuedDate = DateTime.UtcNow;
        IssuedQuantity = issuedQty;
        Status = BOQStatus.PartiallyIssued;
        Update(updatedBy);
    }

    public void Cancel(int updatedBy, string? reason = null, string? reasonArabic = null)
    {
        Notes = reason ?? Notes;
        NotesArabic = reasonArabic ?? NotesArabic;
        Status = BOQStatus.Cancelled;
        Update(updatedBy);
    }

    /// <summary>
    /// Check if BOQ can be fully issued (all items have sufficient stock)
    /// </summary>
    public bool CanBeFullyIssued()
    {
        return Items?.All(i => i.RemainingQuantity == 0 || i.HasSufficientStock()) ?? false;
    }
    
    /// <summary>
    /// Check if this BOQ requires Commander approval (uses Commander Reserve)
    /// </summary>
    public bool RequiresCommanderApproval()
    {
        return RequiresCommanderReserve && !CommanderApprovalId.HasValue;
    }
}
