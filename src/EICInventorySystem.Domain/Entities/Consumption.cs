namespace EICInventorySystem.Domain.Entities;

/// <summary>
/// Consumption entity for tracking material usage in projects
/// </summary>
public class Consumption : BaseEntity
{
    public string Number { get; private set; } = null!;
    public int ProjectId { get; private set; }
    public int? DepartmentId { get; private set; }
    public int WarehouseId { get; private set; }
    public DateTime ConsumptionDate { get; private set; }
    public ConsumptionStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public string? NotesArabic { get; private set; }
    public int? ApprovedBy { get; private set; }
    public DateTime? ApprovalDate { get; private set; }
    public string? ApprovalNotes { get; private set; }
    public decimal TotalQuantity { get; private set; }
    public decimal TotalValue { get; private set; }
    public string? VehicleNumber { get; private set; } // For vehicle-related projects
    public string? WorkOrderNumber { get; private set; }

    // Navigation properties
    public Project Project { get; private set; } = null!;
    public Department? Department { get; private set; }
    public Warehouse Warehouse { get; private set; } = null!;
    public User? Approver { get; private set; }
    public ICollection<ConsumptionItem> Items { get; private set; } = new List<ConsumptionItem>();

    private Consumption() { }

    public Consumption(
        string number,
        int projectId,
        int? departmentId,
        int warehouseId,
        DateTime consumptionDate,
        int createdBy,
        string? notes = null,
        string? notesArabic = null,
        string? vehicleNumber = null,
        string? workOrderNumber = null) : base(createdBy)
    {
        Number = number;
        ProjectId = projectId;
        DepartmentId = departmentId;
        WarehouseId = warehouseId;
        ConsumptionDate = consumptionDate;
        Status = ConsumptionStatus.Draft;
        Notes = notes;
        NotesArabic = notesArabic;
        TotalQuantity = 0;
        TotalValue = 0;
        VehicleNumber = vehicleNumber;
        WorkOrderNumber = workOrderNumber;
    }

    public void Submit(int updatedBy)
    {
        Status = ConsumptionStatus.Pending;
        Update(updatedBy);
    }

    public void Approve(int approverId, string? approvalNotes = null, int updatedBy = 0)
    {
        ApprovedBy = approverId;
        ApprovalDate = DateTime.UtcNow;
        ApprovalNotes = approvalNotes;
        Status = ConsumptionStatus.Approved;
        Update(updatedBy);
    }

    public void Reject(int approverId, string rejectionReason, int updatedBy)
    {
        ApprovedBy = approverId;
        ApprovalDate = DateTime.UtcNow;
        ApprovalNotes = rejectionReason;
        Status = ConsumptionStatus.Rejected;
        Update(updatedBy);
    }

    public void Complete(int updatedBy)
    {
        Status = ConsumptionStatus.Completed;
        Update(updatedBy);
    }

    public void Cancel(int updatedBy)
    {
        Status = ConsumptionStatus.Cancelled;
        Update(updatedBy);
    }

    public void UpdateTotals(decimal totalQuantity, decimal totalValue, int updatedBy)
    {
        TotalQuantity = totalQuantity;
        TotalValue = totalValue;
        Update(updatedBy);
    }

    public void UpdateVehicleNumber(string vehicleNumber, int updatedBy)
    {
        VehicleNumber = vehicleNumber;
        Update(updatedBy);
    }

    public bool IsPending()
    {
        return Status == ConsumptionStatus.Pending;
    }

    public bool IsApproved()
    {
        return Status == ConsumptionStatus.Approved || Status == ConsumptionStatus.Completed;
    }

    public bool IsRejected()
    {
        return Status == ConsumptionStatus.Rejected;
    }

    public bool IsCompleted()
    {
        return Status == ConsumptionStatus.Completed;
    }

    public bool IsVehicleRelated()
    {
        return !string.IsNullOrEmpty(VehicleNumber);
    }
}
