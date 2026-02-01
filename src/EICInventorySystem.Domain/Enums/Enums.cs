namespace EICInventorySystem.Domain.Enums;

/// <summary>
/// Requisition types
/// </summary>
public enum RequisitionType
{
    Standard,
    Emergency,
    Project,
    Maintenance,
    CommanderReserve
}

/// <summary>
/// Requisition priority
/// </summary>
public enum RequisitionPriority
{
    Low,
    Medium,
    High,
    Critical
}

/// <summary>
/// Requisition status
/// </summary>
public enum RequisitionStatus
{
    Draft,
    Pending,
    Approved,
    Rejected,
    PartiallyIssued,
    Issued,
    PartiallyReceived,
    Completed,
    Cancelled
}

/// <summary>
/// Transfer status
/// </summary>
public enum TransferStatus
{
    Draft,
    Pending,
    Approved,
    Rejected,
    Shipped,
    PartiallyReceived,
    Completed,
    Cancelled
}

/// <summary>
/// Adjustment status
/// </summary>
public enum AdjustmentStatus
{
    Draft,
    Pending,
    Approved,
    Rejected,
    Completed,
    Cancelled
}

/// <summary>
/// Adjustment type
/// </summary>
public enum AdjustmentType
{
    PhysicalCount,
    Damage,
    Loss,
    Correction,
    Addition,
    Deduction
}

/// <summary>
/// Notification type
/// </summary>
public enum NotificationType
{
    Info,
    Warning,
    Error,
    Success,
    Approval,
    Rejection,
    Alert,
    Reminder,
    CommanderReserve,
    Requisition,
    Transfer,
    InventoryAlert,
    System
}

/// <summary>
/// Notification priority
/// </summary>
public enum NotificationPriority
{
    Low,
    Normal,
    High,
    Critical
}

/// <summary>
/// Consumption status
/// </summary>
public enum ConsumptionStatus
{
    Draft,
    Pending,
    Approved,
    Rejected,
    Completed,
    Cancelled
}

/// <summary>
/// Receipt status
/// </summary>
public enum ReceiptStatus
{
    Draft,
    Pending,
    InProgress,
    PartiallyReceived,
    Completed,
    Cancelled
}

/// <summary>
/// Return status
/// </summary>
public enum ReturnStatus
{
    Draft,
    Pending,
    Approved,
    Rejected,
    Completed,
    Cancelled
}

public enum WarehouseType
{
    Central,
    Factory,
    Project
}

public enum ProjectStatus
{
    Planned,
    Planning,
    InProgress,
    Active,
    OnHold,
    Completed,
    Cancelled
}

/// <summary>
/// BOQ status for project Bill of Quantities
/// </summary>
public enum BOQStatus
{
    Draft,           // BOQ created, not submitted
    Pending,         // BOQ submitted, awaiting approval
    Approved,        // BOQ approved, ready for issuance
    PartiallyIssued, // Some items issued, remaining items tracked
    FullyIssued,     // All items issued
    Completed,       // All items received and confirmed
    Cancelled        // BOQ cancelled
}

/// <summary>
/// BOQ priority levels
/// </summary>
public enum BOQPriority
{
    Low,
    Medium,
    High,
    Critical
}

/// <summary>
/// Operational custody status
/// </summary>
public enum CustodyStatus
{
    Active,            // Item is with worker
    PartiallyReturned, // Some quantity returned
    FullyReturned,     // All quantity returned
    Consumed,          // Item consumed/used
    Transferred        // Transferred to another worker
}

/// <summary>
/// Source of a requisition
/// </summary>
public enum RequisitionSource
{
    Standard,          // Normal daily requisition
    BOQ,               // From project BOQ
    DailyOperational,  // Daily operational needs
    Emergency          // Emergency (may use Commander Reserve)
}

