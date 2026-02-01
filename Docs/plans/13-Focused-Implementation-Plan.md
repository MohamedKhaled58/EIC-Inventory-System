# Focused Implementation Plan
## مجمع الصناعات الهندسية - نظام إدارة المخازن

**Date:** February 1, 2026  
**Status:** Ready for Implementation  
**Version:** 1.0

---

## Executive Summary

This document provides a **focused implementation plan** for specific system improvements, excluding the BOQ System and Custody System (which are being handled by another AI).

### Scope of Work

This plan focuses on **8 critical improvements**:

1. ✅ **Free Text Input Elimination** - Replace free text typing with search-based autocomplete
2. ✅ **Partial Issuance Automation** - Automate remaining requisition creation
3. ✅ **Advanced Item Details Page** - Excel-like comprehensive item view
4. ✅ **Search-Based Item Selection** - Dropdown autocomplete with real-time filtering
5. ✅ **Forms & UX Improvements** - Controlled inputs with validation
6. ✅ **Archiving Strategy** - Background jobs for data retention
7. ✅ **Auditing Strategy** - Enhanced audit logging
8. ✅ **Performance Optimizations** - Caching, pagination, query optimization

---

## 1. Free Text Input Elimination

### Current Problem

**Location:** [`Requisitions.tsx`](src/EICInventorySystem.Frontend/src/pages/Requisitions.tsx)

```typescript
// Current implementation - FREE TEXT INPUT (PROBLEMATIC!)
const [itemCode, setItemCode] = useState('');
const [itemName, setItemName] = useState('');

<TextField
    label="كود الصنف"
    value={itemCode}
    onChange={(e) => setItemCode(e.target.value)}  // FREE TEXT TYPING!
/>

<TextField
    label="اسم الصنف"
    value={itemName}
    onChange={(e) => setItemName(e.target.value)}  // FREE TEXT TYPING!
/>
```

**Risks:**
- ❌ Human typing errors (wrong codes, typos)
- ❌ Non-existent items can be entered
- ❌ Inconsistent item names
- ❌ No validation against available stock
- ❌ No category filtering
- ❌ No unit information displayed

### Solution: Search-Based Autocomplete

**New Component:** [`ItemAutocomplete.tsx`](src/EICInventorySystem.Frontend/src/components/ItemAutocomplete.tsx)

```typescript
interface ItemAutocompleteProps {
    items: Item[];
    selectedItems: Item | Item[];
    onChange: (item: Item | Item[]) => void;
    multiple?: boolean;
    disabled?: boolean;
    placeholder?: string;
    warehouseId?: number;  // Filter by warehouse
    showStockInfo?: boolean;  // Show available stock
    allowOutOfStock?: boolean;  // Allow selecting out-of-stock items
}

const ItemAutocomplete: React.FC<ItemAutocompleteProps> = ({
    items,
    selectedItems,
    onChange,
    multiple = false,
    disabled = false,
    placeholder = 'ابحث عن الصنف',
    warehouseId,
    showStockInfo = true,
    allowOutOfStock = false
}) => {
    const [searchTerm, setSearchTerm] = useState('');
    const [open, setOpen] = useState(false);

    // Filter items based on search term
    const filteredItems = useMemo(() => {
        return items.filter(item => {
            // Search by item code (exact match preferred)
            const codeMatch = item.itemCode.toLowerCase().includes(searchTerm.toLowerCase());
            // Search by Arabic name
            const arabicMatch = item.itemNameAr.includes(searchTerm);
            // Search by English name
            const englishMatch = item.itemName.toLowerCase().includes(searchTerm.toLowerCase());
            // Search by category
            const categoryMatch = item.categoryAr.includes(searchTerm) || 
                                  item.category.toLowerCase().includes(searchTerm.toLowerCase());
            
            return codeMatch || arabicMatch || englishMatch || categoryMatch;
        });
    }, [items, searchTerm]);

    // Filter by warehouse if specified
    const warehouseFilteredItems = useMemo(() => {
        if (!warehouseId) return filteredItems;
        
        return filteredItems.filter(item => {
            const inventoryRecord = item.inventoryRecords?.find(ir => ir.warehouseId === warehouseId);
            if (!inventoryRecord) return false;
            
            // Check if out of stock and not allowed
            if (!allowOutOfStock && inventoryRecord.availableQuantity <= 0) {
                return false;
            }
            
            return true;
        });
    }, [filteredItems, warehouseId, allowOutOfStock]);

    return (
        <Autocomplete
            multiple={multiple}
            disabled={disabled}
            open={open}
            onOpen={() => setOpen(true)}
            onClose={() => setOpen(false)}
            options={warehouseFilteredItems}
            value={selectedItems}
            onChange={(event, value) => onChange(value as Item | Item[])}
            getOptionLabel={(option) => `${option.itemCode} - ${option.itemNameAr}`}
            filterOptions={(options) => options}  // Custom filtering
            renderInput={(params) => (
                <TextField
                    {...params}
                    label="اختر الصنف"
                    placeholder={placeholder}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    InputProps={{
                        ...params.InputProps,
                        endAdornment: (
                            <InputAdornment position="end">
                                <IconButton onClick={() => setOpen(!open)}>
                                    <SearchIcon />
                                </IconButton>
                            </InputAdornment>
                        ),
                    }}
                />
            )}
            renderOption={(props, option) => (
                <li {...props}>
                    <Box sx={{ display: 'flex', flexDirection: 'column', width: '100%' }}>
                        {/* Item Code and Status */}
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                            <Typography variant="body1" fontWeight="bold" sx={{ minWidth: 120 }}>
                                {option.itemCode}
                            </Typography>
                            <Chip 
                                label={option.isActive ? 'نشط' : 'غير نشط'} 
                                size="small" 
                                color={option.isActive ? 'success' : 'default'}
                            />
                            {option.isCritical && (
                                <Chip label="حرج" size="small" color="error" />
                            )}
                        </Box>
                        
                        {/* Item Names */}
                        <Box sx={{ display: 'flex', gap: 1, alignItems: 'center' }}>
                            <Typography variant="body2" fontWeight="medium">
                                {option.itemNameAr}
                            </Typography>
                            <Typography variant="caption" color="textSecondary">
                                {option.itemName}
                            </Typography>
                        </Box>
                        
                        {/* Category and Unit */}
                        <Box sx={{ display: 'flex', gap: 1, alignItems: 'center' }}>
                            <Chip label={option.categoryAr} size="small" variant="outlined" />
                            <Typography variant="caption" color="textSecondary">
                                الوحدة: {option.unitAr}
                            </Typography>
                        </Box>
                        
                        {/* Stock Information */}
                        {showStockInfo && option.inventoryRecords && option.inventoryRecords.length > 0 && (
                            <Box sx={{ display: 'flex', gap: 1, alignItems: 'center', mt: 0.5 }}>
                                {option.inventoryRecords.map((ir, idx) => (
                                    <Chip
                                        key={idx}
                                        label={`${ir.warehouseNameAr}: ${ir.availableQuantity} ${option.unitAr}`}
                                        size="small"
                                        color={ir.availableQuantity > 0 ? 'success' : 'error'}
                                        variant="outlined"
                                    />
                                ))}
                            </Box>
                        )}
                    </Box>
                </li>
            )}
            noOptionsText="لم يتم العثور على أصناف"
        />
    );
};
```

### Backend API Enhancement

**Add to [`InventoryController.cs`](src/EICInventorySystem.WebAPI/Controllers/InventoryController.cs):**

```csharp
[HttpGet("search")]
public async Task<ActionResult<List<ItemDTO>>> SearchItems(
    [FromQuery] string searchTerm,
    [FromQuery] int? warehouseId = null,
    [FromQuery] bool includeOutOfStock = false,
    [FromQuery] bool includeInactive = false,
    CancellationToken cancellationToken = default)
{
    var query = _context.Items
        .Include(i => i.InventoryRecords)
            .ThenInclude(ir => ir.Warehouse)
        .AsQueryable();

    // Filter by search term
    if (!string.IsNullOrWhiteSpace(searchTerm))
    {
        query = query.Where(i =>
            i.ItemCode.Contains(searchTerm) ||
            i.ItemNameAr.Contains(searchTerm) ||
            i.ItemName.Contains(searchTerm) ||
            i.CategoryAr.Contains(searchTerm) ||
            i.Category.Contains(searchTerm));
    }

    // Filter by warehouse
    if (warehouseId.HasValue)
    {
        query = query.Where(i => i.InventoryRecords.Any(ir => ir.WarehouseId == warehouseId.Value));
    }

    // Filter out inactive items
    if (!includeInactive)
    {
        query = query.Where(i => i.IsActive);
    }

    // Filter out of stock items
    if (!includeOutOfStock)
    {
        query = query.Where(i => i.InventoryRecords.Any(ir => ir.AvailableQuantity > 0));
    }

    var items = await query
        .Select(i => new ItemDTO
        {
            Id = i.Id,
            ItemCode = i.ItemCode,
            ItemName = i.ItemName,
            ItemNameAr = i.ItemNameAr,
            Category = i.Category,
            CategoryAr = i.CategoryAr,
            Unit = i.Unit,
            UnitAr = i.UnitAr,
            IsActive = i.IsActive,
            IsCritical = i.IsCritical,
            InventoryRecords = i.InventoryRecords.Select(ir => new InventoryRecordDTO
            {
                WarehouseId = ir.WarehouseId,
                WarehouseName = ir.Warehouse.Name,
                WarehouseNameAr = ir.Warehouse.NameAr,
                GeneralQuantity = ir.GeneralQuantity,
                CommanderReserveQuantity = ir.CommanderReserveQuantity,
                AvailableQuantity = ir.AvailableQuantity
            }).ToList()
        })
        .OrderBy(i => i.ItemCode)
        .Take(100)  // Limit results
        .ToListAsync(cancellationToken);

    return Ok(items);
}
```

### Forms to Update

Replace free text inputs in ALL forms:

1. ✅ [`Requisitions.tsx`](src/EICInventorySystem.Frontend/src/pages/Requisitions.tsx) - Item selection
2. ✅ [`Transfers.tsx`](src/EICInventorySystem.Frontend/src/pages/Transfers.tsx) - Item selection
3. ✅ [`Returns.tsx`](src/EICInventorySystem.Frontend/src/pages/Returns.tsx) - Item selection
4. ✅ [`Consumptions.tsx`](src/EICInventorySystem.Frontend/src/pages/Consumptions.tsx) - Item selection
5. ✅ [`Inventory.tsx`](src/EICInventorySystem.Frontend/src/pages/Inventory.tsx) - Item search
6. ✅ [`Receipts.tsx`](src/EICInventorySystem.Frontend/src/pages/Receipts.tsx) - Item selection
7. ✅ [`Adjustments.tsx`](src/EICInventorySystem.Frontend/src/pages/Adjustments.tsx) - Item selection

---

## 2. Partial Issuance Automation

### Current Problem

**Location:** [`RequisitionCommands.cs`](src/EICInventorySystem.Application/Commands/RequisitionCommands.cs)

The current system supports `PartiallyIssued` status but **does not automatically create a remaining requisition**.

**Issues:**
- ❌ Manual tracking of remaining items
- ❌ No link between original and remaining requisitions
- ❌ No visibility into why items were not issued
- ❌ Risk of losing track of pending items

### Solution: Automatic Remaining Requisition Creation

**Update [`Requisition.cs`](src/EICInventorySystem.Domain/Entities/Requisition.cs):**

```csharp
public partial class Requisition
{
    // Add new property
    public int? ParentRequisitionId { get; set; }
    public Requisition ParentRequisition { get; set; }
    public List<Requisition> ChildRequisitions { get; set; } = new();

    public void IssuePartial(
        Dictionary<int, decimal> itemQuantities, 
        string reason, 
        int warehouseId, 
        int updatedBy)
    {
        // Update issued quantities for each item
        foreach (var (itemId, quantity) in itemQuantities)
        {
            var requisitionItem = Items.FirstOrDefault(i => i.ItemId == itemId);
            if (requisitionItem != null)
            {
                requisitionItem.IssuedQuantity += quantity;
                requisitionItem.PartialIssueReason = reason;
            }
        }

        // Update status
        Status = RequisitionStatus.PartiallyIssued;
        IssuedDate = DateTime.UtcNow;
        IssuedBy = updatedBy;

        // Create remaining requisition
        CreateRemainingRequisition(reason, warehouseId, updatedBy);
        
        Update(updatedBy);
    }

    private void CreateRemainingRequisition(string reason, int warehouseId, int createdBy)
    {
        // Check if there are remaining items
        var remainingItems = Items.Where(i => !i.IsFullyIssued()).ToList();
        if (!remainingItems.Any()) return;

        // Create new requisition for remaining items
        var remainingRequisition = new Requisition(
            requisitionNumber: GenerateRequisitionNumber(),
            projectId: ProjectId,
            departmentId: DepartmentId,
            warehouseId: warehouseId,
            createdBy: createdBy
        );

        // Set parent relationship
        remainingRequisition.ParentRequisitionId = Id;
        remainingRequisition.IsRemainingRequisition = true;
        remainingRequisition.RemainingReason = reason;

        // Copy remaining items
        foreach (var item in remainingItems)
        {
            var remainingItem = new RequisitionItem(
                requisitionId: remainingRequisition.Id,
                itemId: item.ItemId,
                requestedQuantity: item.GetRemainingQuantity(),
                unitPrice: item.UnitPrice,
                createdBy: createdBy
            );
            remainingRequisition.Items.Add(remainingItem);
        }

        // Add to child requisitions
        ChildRequisitions.Add(remainingRequisition);
    }

    public bool IsRemainingRequisition { get; set; }
    public string RemainingReason { get; set; }
}
```

**Update [`RequisitionItem.cs`](src/EICInventorySystem.Domain/Entities/RequisitionItem.cs):**

```csharp
public partial class RequisitionItem
{
    public string PartialIssueReason { get; set; }

    public bool IsFullyIssued()
    {
        return IssuedQuantity >= RequestedQuantity;
    }

    public decimal GetRemainingQuantity()
    {
        return Math.Max(0, RequestedQuantity - IssuedQuantity);
    }
}
```

**Update [`RequisitionCommands.cs`](src/EICInventorySystem.Application/Commands/RequisitionCommands.cs):**

```csharp
public class IssuePartialRequisitionCommand : IRequest<Result<RequisitionDTO>>
{
    public int RequisitionId { get; set; }
    public Dictionary<int, decimal> ItemQuantities { get; set; }  // ItemId -> IssuedQuantity
    public int WarehouseId { get; set; }
    public string Reason { get; set; }
    public int UpdatedBy { get; set; }
}

public class IssuePartialRequisitionCommandHandler : IRequestHandler<IssuePartialRequisitionCommand, Result<RequisitionDTO>>
{
    private readonly ApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public IssuePartialRequisitionCommandHandler(
        ApplicationDbContext context,
        IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<Result<RequisitionDTO>> Handle(IssuePartialRequisitionCommand request, CancellationToken cancellationToken)
    {
        // Get requisition
        var requisition = await _context.Requisitions
            .Include(r => r.Items)
            .Include(r => r.Project)
            .Include(r => r.Department)
            .Include(r => r.Warehouse)
            .FirstOrDefaultAsync(r => r.Id == request.RequisitionId, cancellationToken);

        if (requisition == null)
        {
            return Result<RequisitionDTO>.Failure("Requisition not found");
        }

        // Validate status
        if (requisition.Status != RequisitionStatus.Approved)
        {
            return Result<RequisitionDTO>.Failure("Only approved requisitions can be issued");
        }

        // Validate warehouse
        if (requisition.WarehouseId != request.WarehouseId)
        {
            return Result<RequisitionDTO>.Failure("Warehouse mismatch");
        }

        // Validate item quantities
        foreach (var (itemId, quantity) in request.ItemQuantities)
        {
            var item = await _context.Items.FindAsync(itemId);
            if (item == null)
            {
                return Result<RequisitionDTO>.Failure($"Item {itemId} not found");
            }

            var requisitionItem = requisition.Items.FirstOrDefault(i => i.ItemId == itemId);
            if (requisitionItem == null)
            {
                return Result<RequisitionDTO>.Failure($"Item {itemId} not in requisition");
            }

            if (quantity <= 0)
            {
                return Result<RequisitionDTO>.Failure($"Quantity must be positive for item {item.ItemCode}");
            }

            if (quantity > requisitionItem.GetRemainingQuantity())
            {
                return Result<RequisitionDTO>.Failure($"Cannot issue more than remaining quantity for item {item.ItemCode}");
            }

            // Check stock availability
            var inventoryRecord = await _context.InventoryRecords
                .FirstOrDefaultAsync(ir => 
                    ir.ItemId == itemId && 
                    ir.WarehouseId == request.WarehouseId, 
                    cancellationToken);

            if (inventoryRecord == null || inventoryRecord.AvailableQuantity < quantity)
            {
                return Result<RequisitionDTO>.Failure($"Insufficient stock for item {item.ItemCode}");
            }
        }

        // Begin transaction
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // Issue items
            foreach (var (itemId, quantity) in request.ItemQuantities)
            {
                var item = await _context.Items.FindAsync(itemId);
                var inventoryRecord = await _context.InventoryRecords
                    .FirstOrDefaultAsync(ir => 
                        ir.ItemId == itemId && 
                        ir.WarehouseId == request.WarehouseId, 
                        cancellationToken);

                // Deduct from stock
                inventoryRecord.GeneralQuantity -= quantity;
                inventoryRecord.GeneralAllocated -= quantity;

                // Record transaction
                var transaction = new InventoryTransaction(
                    itemId: itemId,
                    warehouseId: request.WarehouseId,
                    transactionType: TransactionType.Issue,
                    quantity: -quantity,
                    referenceNumber: requisition.RequisitionNumber,
                    referenceType: "Requisition",
                    referenceId: requisition.Id,
                    notes: $"Partial issue: {request.Reason}",
                    createdBy: request.UpdatedBy
                );
                _context.InventoryTransactions.Add(transaction);

                // Update item transaction history
                var history = new ItemTransactionHistory(
                    itemId: itemId,
                    transactionType: "Issue",
                    quantity: -quantity,
                    referenceNumber: requisition.RequisitionNumber,
                    referenceType: "Requisition",
                    referenceId: requisition.Id,
                    warehouseId: request.WarehouseId,
                    notes: $"Partial issue: {request.Reason}",
                    createdBy: request.UpdatedBy
                );
                _context.ItemTransactionHistories.Add(history);
            }

            // Issue partial requisition (this creates remaining requisition)
            requisition.IssuePartial(
                itemQuantities: request.ItemQuantities,
                reason: request.Reason,
                warehouseId: request.WarehouseId,
                updatedBy: request.UpdatedBy
            );

            // Save changes
            await _context.SaveChangesAsync(cancellationToken);

            // Audit log
            await _auditService.LogAsync(
                action: "Requisition.IssuePartial",
                entityType: "Requisition",
                entityId: requisition.Id,
                userId: request.UpdatedBy,
                details: $"Partially issued requisition {requisition.RequisitionNumber}. Reason: {request.Reason}"
            );

            // Commit transaction
            await transaction.CommitAsync(cancellationToken);

            // Return updated requisition
            var dto = MapToDTO(requisition);
            return Result<RequisitionDTO>.Success(dto);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result<RequisitionDTO>.Failure($"Failed to issue partial requisition: {ex.Message}");
        }
    }
}
```

### Frontend UI

**Add to [`Requisitions.tsx`](src/EICInventorySystem.Frontend/src/pages/Requisitions.tsx):**

```typescript
const PartialIssueDialog: React.FC<PartialIssueDialogProps> = ({ requisition, open, onClose }) => {
    const [itemQuantities, setItemQuantities] = useState<Record<number, decimal>>({});
    const [reason, setReason] = useState('');

    const handleQuantityChange = (itemId: number, quantity: decimal) => {
        setItemQuantities(prev => ({
            ...prev,
            [itemId]: quantity
        }));
    };

    const handleSubmit = async () => {
        try {
            await apiClient.post(`/api/requisitions/${requisition.id}/issue-partial`, {
                itemQuantities,
                warehouseId: requisition.warehouseId,
                reason
            });
            onClose();
        } catch (error) {
            console.error('Failed to issue partial requisition:', error);
        }
    };

    return (
        <Dialog open={open} onClose={onClose} maxWidth="md" fullWidth>
            <DialogTitle>صدر جزئي - {requisition.requisitionNumber}</DialogTitle>
            <DialogContent>
                <Table>
                    <TableHead>
                        <TableRow>
                            <TableCell>كود الصنف</TableCell>
                            <TableCell>الصنف</TableCell>
                            <TableCell>المطلوب</TableCell>
                            <TableCell>المصدر</TableCell>
                            <TableCell>الصدر الآن</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {requisition.items.map(item => (
                            <TableRow key={item.id}>
                                <TableCell>{item.itemCode}</TableCell>
                                <TableCell>{item.itemNameAr}</TableCell>
                                <TableCell>{item.requestedQuantity}</TableCell>
                                <TableCell>{item.issuedQuantity}</TableCell>
                                <TableCell>
                                    <TextField
                                        type="number"
                                        value={itemQuantities[item.itemId] || ''}
                                        onChange={(e) => handleQuantityChange(item.itemId, decimal.Parse(e.target.value))}
                                        inputProps={{
                                            min: 0,
                                            max: item.requestedQuantity - item.issuedQuantity,
                                            step: 0.01
                                        }}
                                        helperText={`المتبقي: ${item.requestedQuantity - item.issuedQuantity}`}
                                    />
                                </TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
                <TextField
                    label="سبب الصدر الجزئي"
                    value={reason}
                    onChange={(e) => setReason(e.target.value)}
                    fullWidth
                    multiline
                    rows={3}
                    required
                    sx={{ mt: 2 }}
                />
            </DialogContent>
            <DialogActions>
                <Button onClick={onClose}>إلغاء</Button>
                <Button 
                    onClick={handleSubmit} 
                    variant="contained" 
                    color="primary"
                    disabled={!reason || Object.keys(itemQuantities).length === 0}
                >
                    صدر جزئي
                </Button>
            </DialogActions>
        </Dialog>
    );
};
```

---

## 3. Advanced Item Details Page

### Current Problem

**Location:** [`Inventory.tsx`](src/EICInventorySystem.Frontend/src/pages/Inventory.tsx)

The current inventory page shows limited item information:
- ❌ No drill-down to item details
- ❌ No transaction history
- ❌ No project allocations
- ❌ No stock movement charts
- ❌ No consumption analysis

### Solution: Comprehensive Item Details Page

**Create new page:** [`ItemDetails.tsx`](src/EICInventorySystem.Frontend/src/pages/ItemDetails.tsx)

```typescript
interface ItemDetailsProps {
    itemId: number;
}

const ItemDetails: React.FC<ItemDetailsProps> = ({ itemId }) => {
    const [item, setItem] = useState<Item | null>(null);
    const [activeTab, setActiveTab] = useState(0);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        loadItemDetails();
    }, [itemId]);

    const loadItemDetails = async () => {
        try {
            const response = await apiClient.get(`/api/inventory/items/${itemId}/details`);
            setItem(response.data);
        } catch (error) {
            console.error('Failed to load item details:', error);
        } finally {
            setLoading(false);
        }
    };

    if (loading) return <CircularProgress />;
    if (!item) return <Typography>الصنف غير موجود</Typography>;

    return (
        <Box sx={{ p: 3 }}>
            {/* Header */}
            <Box sx={{ mb: 3, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <Box>
                    <Typography variant="h4" gutterBottom>
                        {item.itemCode} - {item.itemNameAr}
                    </Typography>
                    <Typography variant="subtitle1" color="textSecondary">
                        {item.itemName}
                    </Typography>
                </Box>
                <Box sx={{ display: 'flex', gap: 1 }}>
                    <Chip label={item.categoryAr} size="medium" />
                    <Chip 
                        label={item.isActive ? 'نشط' : 'غير نشط'} 
                        size="medium" 
                        color={item.isActive ? 'success' : 'default'}
                    />
                    {item.isCritical && (
                        <Chip label="حرج" size="medium" color="error" />
                    )}
                    <Button 
                        variant="outlined" 
                        startIcon={<DownloadIcon />}
                        onClick={() => exportToExcel(item)}
                    >
                        تصدير
                    </Button>
                </Box>
            </Box>

            {/* Tabs */}
            <Tabs value={activeTab} onChange={(e, newValue) => setActiveTab(newValue)}>
                <Tab label="معلومات أساسية" />
                <Tab label="المخزون" />
                <Tab label="تخصيصات المشاريع" />
                <Tab label="تاريخ الحركات" />
                <Tab label="تحليل الاستهلاك" />
            </Tabs>

            {/* Tab Content */}
            <Box sx={{ mt: 2 }}>
                {activeTab === 0 && <BasicInformationTab item={item} />}
                {activeTab === 1 && <StockTab item={item} />}
                {activeTab === 2 && <ProjectAllocationsTab item={item} />}
                {activeTab === 3 && <TransactionHistoryTab item={item} />}
                {activeTab === 4 && <ConsumptionAnalysisTab item={item} />}
            </Box>
        </Box>
    );
};

// Basic Information Tab
const BasicInformationTab: React.FC<{ item: Item }> = ({ item }) => {
    return (
        <Grid container spacing={2}>
            <Grid item xs={12} md={6}>
                <Card>
                    <CardHeader title="معلومات الصنف" />
                    <CardContent>
                        <DetailRow label="الكود" value={item.itemCode} />
                        <DetailRow label="الاسم (عربي)" value={item.itemNameAr} />
                        <DetailRow label="الاسم (إنجليزي)" value={item.itemName} />
                        <DetailRow label="التصنيف" value={item.categoryAr} />
                        <DetailRow label="الوحدة" value={item.unitAr} />
                        <DetailRow label="السعر" value={`${item.unitPrice} ج.م`} />
                        <DetailRow label="نسبة الاحتياطي" value={`${item.reservePercentage}%`} />
                        <DetailRow label="المخزون الأدنى" value={`${item.minimumStock} ${item.unitAr}`} />
                    </CardContent>
                </Card>
            </Grid>
            <Grid item xs={12} md={6}>
                <Card>
                    <CardHeader title="الحالة" />
                    <CardContent>
                        <DetailRow 
                            label="الحالة" 
                            value={item.isActive ? 'نشط' : 'غير نشط'}
                            color={item.isActive ? 'success' : 'error'}
                        />
                        <DetailRow 
                            label="نوع الصنف" 
                            value={item.isCritical ? 'حرج' : 'عادي'}
                            color={item.isCritical ? 'error' : 'success'}
                        />
                        <DetailRow label="تاريخ الإنشاء" value={formatDate(item.createdAt)} />
                        <DetailRow label="آخر تحديث" value={formatDate(item.updatedAt)} />
                    </CardContent>
                </Card>
            </Grid>
        </Grid>
    );
};

// Stock Tab
const StockTab: React.FC<{ item: Item }> = ({ item }) => {
    return (
        <Box>
            <Typography variant="h6" gutterBottom>
                المخزون حسب المخزن
            </Typography>
            <TableContainer component={Paper}>
                <Table>
                    <TableHead>
                        <TableRow>
                            <TableCell>المخزن</TableCell>
                            <TableCell>الكمية العامة</TableCell>
                            <TableCell>الاحتياطي</TableCell>
                            <TableCell>المخصص</TableCell>
                            <TableCell>المتاح</TableCell>
                            <TableCell>الحالة</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {item.inventoryRecords?.map((record, index) => (
                            <TableRow key={index}>
                                <TableCell>{record.warehouseNameAr}</TableCell>
                                <TableCell>{record.generalQuantity}</TableCell>
                                <TableCell>{record.commanderReserveQuantity}</TableCell>
                                <TableCell>{record.generalAllocated}</TableCell>
                                <TableCell>
                                    <Typography 
                                        color={record.availableQuantity > 0 ? 'success' : 'error'}
                                        fontWeight="bold"
                                    >
                                        {record.availableQuantity}
                                    </Typography>
                                </TableCell>
                                <TableCell>
                                    <Chip 
                                        label={getStockStatus(record.availableQuantity, item.minimumStock)}
                                        color={getStockStatusColor(record.availableQuantity, item.minimumStock)}
                                    />
                                </TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </TableContainer>

            {/* Stock Movement Chart */}
            <Box sx={{ mt: 3 }}>
                <Typography variant="h6" gutterBottom>
                    حركة المخزون
                </Typography>
                <StockMovementChart itemId={item.id} />
            </Box>
        </Box>
    );
};

// Project Allocations Tab
const ProjectAllocationsTab: React.FC<{ item: Item }> = ({ item }) => {
    return (
        <Box>
            <Typography variant="h6" gutterBottom>
                تخصيصات المشاريع
            </Typography>
            <TableContainer component={Paper}>
                <Table>
                    <TableHead>
                        <TableRow>
                            <TableCell>المشروع</TableCell>
                            <TableCell>الكمية المخصصة</TableCell>
                            <TableCell>الكمية المستهلكة</TableCell>
                            <TableCell>المتبقي</TableCell>
                            <TableCell>تاريخ التخصيص</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {item.projectAllocations?.map((allocation, index) => (
                            <TableRow key={index}>
                                <TableCell>{allocation.projectNameAr}</TableCell>
                                <TableCell>{allocation.allocatedQuantity}</TableCell>
                                <TableCell>{allocation.consumedQuantity}</TableCell>
                                <TableCell>
                                    <Typography 
                                        color={allocation.remainingQuantity > 0 ? 'success' : 'error'}
                                        fontWeight="bold"
                                    >
                                        {allocation.remainingQuantity}
                                    </Typography>
                                </TableCell>
                                <TableCell>{formatDate(allocation.createdAt)}</TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </TableContainer>
        </Box>
    );
};

// Transaction History Tab
const TransactionHistoryTab: React.FC<{ item: Item }> = ({ item }) => {
    return (
        <Box>
            <Typography variant="h6" gutterBottom>
                تاريخ الحركات
            </Typography>
            <TableContainer component={Paper}>
                <Table>
                    <TableHead>
                        <TableRow>
                            <TableCell>التاريخ</TableCell>
                            <TableCell>نوع الحركة</TableCell>
                            <TableCell>الكمية</TableCell>
                            <TableCell>المخزن</TableCell>
                            <TableCell>رقم المرجع</TableCell>
                            <TableCell>المستخدم</TableCell>
                            <TableCell>ملاحظات</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {item.transactionHistory?.map((transaction, index) => (
                            <TableRow key={index}>
                                <TableCell>{formatDate(transaction.transactionDate)}</TableCell>
                                <TableCell>
                                    <Chip 
                                        label={transaction.transactionTypeAr}
                                        color={getTransactionColor(transaction.transactionType)}
                                    />
                                </TableCell>
                                <TableCell>
                                    <Typography 
                                        color={transaction.quantity > 0 ? 'success' : 'error'}
                                        fontWeight="bold"
                                    >
                                        {transaction.quantity > 0 ? '+' : ''}{transaction.quantity}
                                    </Typography>
                                </TableCell>
                                <TableCell>{transaction.warehouseNameAr}</TableCell>
                                <TableCell>{transaction.referenceNumber}</TableCell>
                                <TableCell>{transaction.userName}</TableCell>
                                <TableCell>{transaction.notes}</TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </TableContainer>
        </Box>
    );
};

// Consumption Analysis Tab
const ConsumptionAnalysisTab: React.FC<{ item: Item }> = ({ item }) => {
    return (
        <Box>
            <Typography variant="h6" gutterBottom>
                تحليل الاستهلاك
            </Typography>
            <Grid container spacing={2}>
                <Grid item xs={12} md={6}>
                    <Card>
                        <CardHeader title="إحصائيات الاستهلاك" />
                        <CardContent>
                            <DetailRow label="إجمالي الاستهلاك" value={`${item.totalConsumption} ${item.unitAr}`} />
                            <DetailRow label="معدل الاستهلاك الشهري" value={`${item.monthlyConsumptionRate} ${item.unitAr}`} />
                            <DetailRow label="أعلى استهلاك" value={`${item.maxConsumption} ${item.unitAr}`} />
                            <DetailRow label="أقل استهلاك" value={`${item.minConsumption} ${item.unitAr}`} />
                        </CardContent>
                    </Card>
                </Grid>
                <Grid item xs={12} md={6}>
                    <Card>
                        <CardHeader title="التنبؤ" />
                        <CardContent>
                            <DetailRow 
                                label="النفاد المتوقع" 
                                value={item.estimatedStockoutDate ? formatDate(item.estimatedStockoutDate) : 'غير معروف'}
                            />
                            <DetailRow 
                                label="يوم المخزون" 
                                value={`${item.daysOfStock} يوم`}
                            />
                        </CardContent>
                    </Card>
                </Grid>
            </Grid>
            
            {/* Consumption Trend Chart */}
            <Box sx={{ mt: 3 }}>
                <ConsumptionTrendChart itemId={item.id} />
            </Box>
        </Box>
    );
};
```

### Backend API

**Add to [`InventoryController.cs`](src/EICInventorySystem.WebAPI/Controllers/InventoryController.cs):**

```csharp
[HttpGet("items/{id}/details")]
public async Task<ActionResult<ItemDetailsDTO>> GetItemDetails(int id, CancellationToken cancellationToken = default)
{
    var item = await _context.Items
        .Include(i => i.InventoryRecords)
            .ThenInclude(ir => ir.Warehouse)
        .Include(i => i.ProjectAllocations)
            .ThenInclude(pa => pa.Project)
        .Include(i => i.TransactionHistory)
            .ThenInclude(th => th.Warehouse)
        .Include(i => i.SupplierItems)
            .ThenInclude(si => si.Supplier)
        .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

    if (item == null)
    {
        return NotFound();
    }

    var dto = new ItemDetailsDTO
    {
        Id = item.Id,
        ItemCode = item.ItemCode,
        ItemName = item.ItemName,
        ItemNameAr = item.ItemNameAr,
        Category = item.Category,
        CategoryAr = item.CategoryAr,
        Unit = item.Unit,
        UnitAr = item.UnitAr,
        UnitPrice = item.UnitPrice,
        ReservePercentage = item.ReservePercentage,
        MinimumStock = item.MinimumStock,
        IsActive = item.IsActive,
        IsCritical = item.IsCritical,
        
        // Inventory Records
        InventoryRecords = item.InventoryRecords.Select(ir => new InventoryRecordDTO
        {
            WarehouseId = ir.WarehouseId,
            WarehouseName = ir.Warehouse.Name,
            WarehouseNameAr = ir.Warehouse.NameAr,
            GeneralQuantity = ir.GeneralQuantity,
            CommanderReserveQuantity = ir.CommanderReserveQuantity,
            GeneralAllocated = ir.GeneralAllocated,
            ReserveAllocated = ir.ReserveAllocated,
            AvailableQuantity = ir.AvailableQuantity
        }).ToList(),
        
        // Project Allocations
        ProjectAllocations = item.ProjectAllocations.Select(pa => new ProjectAllocationDTO
        {
            ProjectId = pa.ProjectId,
            ProjectName = pa.Project.Name,
            ProjectNameAr = pa.Project.NameAr,
            AllocatedQuantity = pa.AllocatedQuantity,
            ConsumedQuantity = pa.ConsumedQuantity,
            RemainingQuantity = pa.RemainingQuantity,
            CreatedAt = pa.CreatedAt
        }).ToList(),
        
        // Transaction History
        TransactionHistory = item.TransactionHistory
            .OrderByDescending(th => th.TransactionDate)
            .Select(th => new TransactionHistoryDTO
            {
                TransactionDate = th.TransactionDate,
                TransactionType = th.TransactionType,
                TransactionTypeAr = GetTransactionTypeAr(th.TransactionType),
                Quantity = th.Quantity,
                WarehouseId = th.WarehouseId,
                WarehouseName = th.Warehouse?.NameAr,
                ReferenceNumber = th.ReferenceNumber,
                ReferenceType = th.ReferenceType,
                ReferenceId = th.ReferenceId,
                Notes = th.Notes,
                CreatedAt = th.CreatedAt
            }).ToList(),
        
        // Consumption Statistics
        TotalConsumption = item.TransactionHistory
            .Where(th => th.TransactionType == "Consumption")
            .Sum(th => Math.Abs(th.Quantity)),
        
        MonthlyConsumptionRate = CalculateMonthlyConsumptionRate(item.Id),
        
        MaxConsumption = item.TransactionHistory
            .Where(th => th.TransactionType == "Consumption")
            .Max(th => Math.Abs(th.Quantity)),
        
        MinConsumption = item.TransactionHistory
            .Where(th => th.TransactionType == "Consumption")
            .Min(th => Math.Abs(th.Quantity)),
        
        EstimatedStockoutDate = CalculateEstimatedStockoutDate(item.Id),
        
        DaysOfStock = CalculateDaysOfStock(item.Id),
        
        CreatedAt = item.CreatedAt,
        UpdatedAt = item.UpdatedAt
    };

    return Ok(dto);
}

private decimal CalculateMonthlyConsumptionRate(int itemId)
{
    var last30Days = DateTime.UtcNow.AddDays(-30);
    var consumption = _context.ItemTransactionHistories
        .Where(th => th.ItemId == itemId && 
                     th.TransactionType == "Consumption" && 
                     th.TransactionDate >= last30Days)
        .Sum(th => Math.Abs(th.Quantity));
    
    return consumption / 30m;
}

private DateTime? CalculateEstimatedStockoutDate(int itemId)
{
    var item = _context.Items
        .Include(i => i.InventoryRecords)
        .FirstOrDefault(i => i.Id == itemId);
    
    if (item == null) return null;
    
    var totalStock = item.InventoryRecords.Sum(ir => ir.AvailableQuantity);
    var monthlyRate = CalculateMonthlyConsumptionRate(itemId);
    
    if (monthlyRate <= 0) return null;
    
    var daysOfStock = totalStock / (monthlyRate / 30m);
    return DateTime.UtcNow.AddDays(daysOfStock);
}

private int CalculateDaysOfStock(int itemId)
{
    var item = _context.Items
        .Include(i => i.InventoryRecords)
        .FirstOrDefault(i => i.Id == itemId);
    
    if (item == null) return 0;
    
    var totalStock = item.InventoryRecords.Sum(ir => ir.AvailableQuantity);
    var monthlyRate = CalculateMonthlyConsumptionRate(itemId);
    
    if (monthlyRate <= 0) return int.MaxValue;
    
    return (int)(totalStock / (monthlyRate / 30m));
}

private string GetTransactionTypeAr(string type)
{
    return type switch
    {
        "Issue" => "إصدار",
        "Receipt" => "استلام",
        "Consumption" => "استهلاك",
        "Transfer" => "نقل",
        "Return" => "إرجاع",
        "Adjustment" => "تعديل",
        _ => type
    };
}
```

---

## 4. Forms & UX Improvements

### Controlled Quantity Input Component

**Create [`QuantityInput.tsx`](src/EICInventorySystem.Frontend/src/components/QuantityInput.tsx):**

```typescript
interface QuantityInputProps {
    value: decimal;
    onChange: (value: decimal) => void;
    label?: string;
    placeholder?: string;
    min?: decimal;
    max?: decimal;
    step?: decimal;
    unit?: string;
    availableQuantity?: decimal;
    reservedQuantity?: decimal;
    disabled?: boolean;
    error?: boolean;
    helperText?: string;
    showStockInfo?: boolean;
}

const QuantityInput: React.FC<QuantityInputProps> = ({
    value,
    onChange,
    label,
    placeholder,
    min = 0,
    max,
    step = 1,
    unit,
    availableQuantity,
    reservedQuantity,
    disabled = false,
    error = false,
    helperText,
    showStockInfo = true
}) => {
    const [localValue, setLocalValue] = useState(value);
    const [validationError, setValidationError] = useState<string | null>(null);

    useEffect(() => {
        setLocalValue(value);
    }, [value]);

    const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const newValue = decimal.Parse(event.target.value);
        
        // Validate min
        if (newValue < min) {
            setValidationError(`الحد الأدنى هو ${min}`);
            return;
        }
        
        // Validate max
        if (max && newValue > max) {
            setValidationError(`الحد الأقصى هو ${max}`);
            return;
        }
        
        // Validate available quantity
        if (availableQuantity && newValue > availableQuantity) {
            setValidationError(`المتاح هو ${availableQuantity} ${unit}`);
            return;
        }
        
        setValidationError(null);
        setLocalValue(newValue);
        onChange(newValue);
    };

    const getStockStatus = () => {
        if (!availableQuantity) return { color: 'default', text: 'غير معروف' };
        
        const percentage = (localValue / availableQuantity) * 100;
        
        if (percentage > 100) return { color: 'error', text: 'تجاوز المتاح' };
        if (percentage > 90) return { color: 'warning', text: 'قريب من الحد الأقصى' };
        if (percentage > 50) return { color: 'info', text: 'متاح' };
        return { color: 'success', text: 'متاح' };
    };

    const stockStatus = getStockStatus();

    return (
        <Box>
            <TextField
                type="number"
                label={label}
                placeholder={placeholder}
                value={localValue}
                onChange={handleChange}
                disabled={disabled}
                error={error || validationError !== null}
                helperText={validationError || helperText}
                InputProps={{
                    inputProps: {
                        min,
                        max,
                        step: step.ToString()
                    },
                    endAdornment: unit ? (
                        <InputAdornment position="end">{unit}</InputAdornment>
                    ) : undefined
                }}
                fullWidth
            />
            
            {showStockInfo && availableQuantity && (
                <Box sx={{ mt: 1, display: 'flex', gap: 1, alignItems: 'center' }}>
                    <Chip 
                        label={`المتاح: ${availableQuantity} ${unit}`}
                        size="small"
                        color={stockStatus.color as any}
                    />
                    {reservedQuantity && reservedQuantity > 0 && (
                        <Chip 
                            label={`الاحتياطي: ${reservedQuantity} ${unit}`}
                            size="small"
                            color="warning"
                        />
                    )}
                </Box>
            )}
        </Box>
    );
};
```

### Error Handling Component

**Create [`ErrorAlert.tsx`](src/EICInventorySystem.Frontend/src/components/ErrorAlert.tsx):**

```typescript
interface FormError {
    code: string;
    messageAr: string;
    messageEn: string;
    severity: 'error' | 'warning' | 'info';
    guidanceAr?: string;
    guidanceEn?: string;
}

const ERROR_MESSAGES: Record<string, FormError> = {
    'INSUFFICIENT_STOCK': {
        code: 'INSUFFICIENT_STOCK',
        messageAr: 'المخزون غير كافي للإصدار',
        messageEn: 'Insufficient stock for issuance',
        severity: 'error',
        guidanceAr: 'يرجى التحقق من المخزون المتاح أو طلب المواد من المخزن الاحتياطي',
        guidanceEn: 'Please check available stock or request from Commander\'s Reserve'
    },
    'COMMANDER_APPROVAL_REQUIRED': {
        code: 'COMMANDER_APPROVAL_REQUIRED',
        messageAr: 'يتطلب موافقة القائد',
        messageEn: 'Commander approval required',
        severity: 'warning',
        guidanceAr: 'يرجى تقديم طلب للموافقة من القائد',
        guidanceEn: 'Please submit request for Commander approval'
    },
    'ITEM_NOT_FOUND': {
        code: 'ITEM_NOT_FOUND',
        messageAr: 'الصنف غير موجود',
        messageEn: 'Item not found',
        severity: 'error',
        guidanceAr: 'يرجى التحقق من كود الصنف',
        guidanceEn: 'Please verify item code'
    },
    'INVALID_QUANTITY': {
        code: 'INVALID_QUANTITY',
        messageAr: 'الكمية غير صحيحة',
        messageEn: 'Invalid quantity',
        severity: 'error',
        guidanceAr: 'يرجى التحقق من الكمية المدخلة',
        guidanceEn: 'Please verify the entered quantity'
    }
};

interface ErrorAlertProps {
    error: FormError | null;
    onClose?: () => void;
}

const ErrorAlert: React.FC<ErrorAlertProps> = ({ error, onClose }) => {
    if (!error) return null;

    return (
        <Alert severity={error.severity} onClose={onClose} sx={{ mb: 2 }}>
            <AlertTitle>{error.messageAr}</AlertTitle>
            {error.guidanceAr && (
                <Typography variant="body2" sx={{ mt: 1 }}>
                    <strong>الإرشاد:</strong> {error.guidanceAr}
                </Typography>
            )}
            {error.guidanceEn && (
                <Typography variant="caption" color="textSecondary" sx={{ mt: 1, display: 'block' }}>
                    <strong>Guidance:</strong> {error.guidanceEn}
                </Typography>
            )}
        </Alert>
    );
};
```

---

## 5. Archiving Strategy

### Background Archiving Service

**Create [`ArchivingService.cs`](src/EICInventorySystem.Infrastructure/Services/ArchivingService.cs):**

```csharp
public interface IArchivingService
{
    Task ArchiveOldTransactionsAsync(CancellationToken cancellationToken = default);
    Task ArchiveCompletedRequisitionsAsync(CancellationToken cancellationToken = default);
    Task ArchiveCompletedTransfersAsync(CancellationToken cancellationToken = default);
    Task ArchiveCompletedReturnsAsync(CancellationToken cancellationToken = default);
}

public class ArchivingService : IArchivingService
{
    private readonly ApplicationDbContext _context;
    private readonly IAuditService _auditService;
    private readonly ILogger<ArchivingService> _logger;

    private const int ArchiveMonthsOld = 6;
    private const int ArchiveBOQMonthsOld = 12;

    public ArchivingService(
        ApplicationDbContext context,
        IAuditService auditService,
        ILogger<ArchivingService> logger)
    {
        _context = context;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task ArchiveOldTransactionsAsync(CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow.AddMonths(-ArchiveMonthsOld);

        var oldTransactions = await _context.InventoryTransactions
            .Where(t => t.TransactionDate < cutoffDate)
            .Where(t => !t.IsArchived)
            .OrderBy(t => t.TransactionDate)
            .Take(1000)  // Batch size
            .ToListAsync(cancellationToken);

        if (!oldTransactions.Any())
        {
            _logger.LogInformation("No transactions to archive");
            return;
        }

        foreach (var transaction in oldTransactions)
        {
            transaction.IsArchived = true;
            transaction.ArchivedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Archived {Count} transactions", oldTransactions.Count);

        await _auditService.LogAsync(
            action: "Archive.Transactions",
            entityType: "InventoryTransaction",
            entityId: 0,
            userId: 0,  // System user
            details: $"Archived {oldTransactions.Count} transactions older than {cutoffDate:yyyy-MM-dd}"
        );
    }

    public async Task ArchiveCompletedRequisitionsAsync(CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow.AddMonths(-ArchiveMonthsOld);

        var completedRequisitions = await _context.Requisitions
            .Where(r => r.Status == RequisitionStatus.Completed)
            .Where(r => r.CompletedDate < cutoffDate)
            .Where(r => !r.IsArchived)
            .OrderBy(r => r.CompletedDate)
            .Take(100)  // Batch size
            .ToListAsync(cancellationToken);

        if (!completedRequisitions.Any())
        {
            _logger.LogInformation("No requisitions to archive");
            return;
        }

        foreach (var requisition in completedRequisitions)
        {
            requisition.IsArchived = true;
            requisition.ArchivedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Archived {Count} requisitions", completedRequisitions.Count);

        await _auditService.LogAsync(
            action: "Archive.Requisitions",
            entityType: "Requisition",
            entityId: 0,
            userId: 0,  // System user
            details: $"Archived {completedRequisitions.Count} completed requisitions older than {cutoffDate:yyyy-MM-dd}"
        );
    }

    public async Task ArchiveCompletedTransfersAsync(CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow.AddMonths(-ArchiveMonthsOld);

        var completedTransfers = await _context.Transfers
            .Where(t => t.Status == TransferStatus.Completed)
            .Where(t => t.CompletedDate < cutoffDate)
            .Where(t => !t.IsArchived)
            .OrderBy(t => t.CompletedDate)
            .Take(100)  // Batch size
            .ToListAsync(cancellationToken);

        if (!completedTransfers.Any())
        {
            _logger.LogInformation("No transfers to archive");
            return;
        }

        foreach (var transfer in completedTransfers)
        {
            transfer.IsArchived = true;
            transfer.ArchivedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Archived {Count} transfers", completedTransfers.Count);

        await _auditService.LogAsync(
            action: "Archive.Transfers",
            entityType: "Transfer",
            entityId: 0,
            userId: 0,  // System user
            details: $"Archived {completedTransfers.Count} completed transfers older than {cutoffDate:yyyy-MM-dd}"
        );
    }

    public async Task ArchiveCompletedReturnsAsync(CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow.AddMonths(-ArchiveMonthsOld);

        var completedReturns = await _context.Returns
            .Where(r => r.Status == ReturnStatus.Completed)
            .Where(r => r.CompletedDate < cutoffDate)
            .Where(r => !r.IsArchived)
            .OrderBy(r => r.CompletedDate)
            .Take(100)  // Batch size
            .ToListAsync(cancellationToken);

        if (!completedReturns.Any())
        {
            _logger.LogInformation("No returns to archive");
            return;
        }

        foreach (var returnRecord in completedReturns)
        {
            returnRecord.IsArchived = true;
            returnRecord.ArchivedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Archived {Count} returns", completedReturns.Count);

        await _auditService.LogAsync(
            action: "Archive.Returns",
            entityType: "Return",
            entityId: 0,
            userId: 0,  // System user
            details: $"Archived {completedReturns.Count} completed returns older than {cutoffDate:yyyy-MM-dd}"
        );
    }
}
```

### Background Service

**Create [`ArchivingBackgroundService.cs`](src/EICInventorySystem.Infrastructure/Services/ArchivingBackgroundService.cs):**

```csharp
public class ArchivingBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ArchivingBackgroundService> _logger;

    public ArchivingBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<ArchivingBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Archiving Background Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Starting archiving process");

                using var scope = _serviceProvider.CreateScope();
                var archivingService = scope.ServiceProvider.GetRequiredService<IArchivingService>();

                // Archive old transactions
                await archivingService.ArchiveOldTransactionsAsync(stoppingToken);

                // Archive completed requisitions
                await archivingService.ArchiveCompletedRequisitionsAsync(stoppingToken);

                // Archive completed transfers
                await archivingService.ArchiveCompletedTransfersAsync(stoppingToken);

                // Archive completed returns
                await archivingService.ArchiveCompletedReturnsAsync(stoppingToken);

                _logger.LogInformation("Archiving process completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during archiving process");
            }

            // Run every day at 2 AM
            var now = DateTime.UtcNow;
            var nextRun = new DateTime(now.Year, now.Month, now.Day, 2, 0, 0, DateTimeKind.Utc);
            if (now > nextRun)
            {
                nextRun = nextRun.AddDays(1);
            }

            var delay = nextRun - now;
            _logger.LogInformation("Next archiving run scheduled for {NextRun}", nextRun);
            
            await Task.Delay(delay, stoppingToken);
        }

        _logger.LogInformation("Archiving Background Service stopped");
    }
}
```

---

## 6. Auditing Strategy

### Enhanced Audit Service

**Update [`AuditService.cs`](src/EICInventorySystem.Infrastructure/Services/AuditService.cs):**

```csharp
public interface IAuditService
{
    Task LogAsync(
        string action,
        string entityType,
        int entityId,
        int userId,
        string details = null,
        Dictionary<string, object> metadata = null,
        CancellationToken cancellationToken = default);

    Task<List<AuditLogDTO>> GetAuditLogsAsync(
        string entityType = null,
        int? entityId = null,
        int? userId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default);
}

public class AuditService : IAuditService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AuditService> _logger;

    public AuditService(
        ApplicationDbContext context,
        ILogger<AuditService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task LogAsync(
        string action,
        string entityType,
        int entityId,
        int userId,
        string details = null,
        Dictionary<string, object> metadata = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var auditLog = new AuditLog
            {
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                UserId = userId,
                Details = details,
                Metadata = metadata != null ? JsonSerializer.Serialize(metadata) : null,
                ActionDate = DateTime.UtcNow,
                IpAddress = GetCurrentUserIpAddress(),
                UserAgent = GetCurrentUserAgent()
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Audit log created: {Action} on {EntityType} {EntityId} by user {UserId}",
                action, entityType, entityId, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create audit log");
            // Don't throw - audit logging should not break the application
        }
    }

    public async Task<List<AuditLogDTO>> GetAuditLogsAsync(
        string entityType = null,
        int? entityId = null,
        int? userId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.AuditLogs
            .Include(al => al.User)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(entityType))
        {
            query = query.Where(al => al.EntityType == entityType);
        }

        if (entityId.HasValue)
        {
            query = query.Where(al => al.EntityId == entityId.Value);
        }

        if (userId.HasValue)
        {
            query = query.Where(al => al.UserId == userId.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(al => al.ActionDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(al => al.ActionDate <= endDate.Value);
        }

        return await query
            .OrderByDescending(al => al.ActionDate)
            .Take(1000)
            .Select(al => new AuditLogDTO
            {
                Id = al.Id,
                Action = al.Action,
                EntityType = al.EntityType,
                EntityId = al.EntityId,
                UserId = al.UserId,
                UserName = al.User?.NameAr,
                Details = al.Details,
                Metadata = al.Metadata,
                ActionDate = al.ActionDate,
                IpAddress = al.IpAddress,
                UserAgent = al.UserAgent
            })
            .ToListAsync(cancellationToken);
    }

    private string GetCurrentUserIpAddress()
    {
        // Implementation depends on hosting environment
        return "127.0.0.1";  // Placeholder
    }

    private string GetCurrentUserAgent()
    {
        // Implementation depends on hosting environment
        return "Unknown";  // Placeholder
    }
}
```

---

## 7. Performance Optimizations

### Caching Strategy

**Create [`CacheService.cs`](src/EICInventorySystem.Infrastructure/Services/CacheService.cs):**

```csharp
public interface ICacheService
{
    Task<T> GetOrCreateAsync<T>(
        string key,
        Func<Task<T>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default);

    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default);
}

public class CacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CacheService> _logger;

    public CacheService(
        IMemoryCache cache,
        ILogger<CacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<Task<T>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(key, out T cachedValue))
        {
            _logger.LogDebug("Cache hit for key: {Key}", key);
            return cachedValue;
        }

        _logger.LogDebug("Cache miss for key: {Key}", key);

        var value = await factory();

        var cacheOptions = new MemoryCacheEntryOptions();
        if (expiration.HasValue)
        {
            cacheOptions.AbsoluteExpirationRelativeToNow = expiration.Value;
        }
        else
        {
            cacheOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
        }

        _cache.Set(key, value, cacheOptions);

        return value;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _cache.Remove(key);
        _logger.LogDebug("Cache removed for key: {Key}", key);
        return Task.CompletedTask;
    }

    public Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        // MemoryCache doesn't support prefix removal by default
        // This would require a custom implementation or using a different cache provider
        _logger.LogDebug("Cache removal requested for prefix: {Prefix}", prefix);
        return Task.CompletedTask;
    }
}
```

### Query Optimization

**Update [`InventoryQueries.cs`](src/EICInventorySystem.Application/Queries/InventoryQueries.cs):**

```csharp
public class InventoryQueries
{
    private readonly ApplicationDbContext _context;
    private readonly ICacheService _cacheService;

    public InventoryQueries(
        ApplicationDbContext context,
        ICacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
    }

    public async Task<List<ItemDTO>> GetAllItemsAsync(
        int page = 1,
        int pageSize = 20,
        string searchTerm = null,
        string category = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"items_page_{page}_size_{pageSize}_search_{searchTerm}_category_{category}_active_{isActive}";

        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async () =>
            {
                var query = _context.Items
                    .Include(i => i.InventoryRecords)
                        .ThenInclude(ir => ir.Warehouse)
                    .AsQueryable();

                // Search
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(i =>
                        i.ItemCode.Contains(searchTerm) ||
                        i.ItemNameAr.Contains(searchTerm) ||
                        i.ItemName.Contains(searchTerm));
                }

                // Category filter
                if (!string.IsNullOrWhiteSpace(category))
                {
                    query = query.Where(i => i.Category == category);
                }

                // Active filter
                if (isActive.HasValue)
                {
                    query = query.Where(i => i.IsActive == isActive.Value);
                }

                return await query
                    .OrderBy(i => i.ItemCode)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(i => new ItemDTO
                    {
                        Id = i.Id,
                        ItemCode = i.ItemCode,
                        ItemName = i.ItemName,
                        ItemNameAr = i.ItemNameAr,
                        Category = i.Category,
                        CategoryAr = i.CategoryAr,
                        Unit = i.Unit,
                        UnitAr = i.UnitAr,
                        UnitPrice = i.UnitPrice,
                        IsActive = i.IsActive,
                        IsCritical = i.IsCritical,
                        MinimumStock = i.MinimumStock,
                        TotalStock = i.InventoryRecords.Sum(ir => ir.GeneralQuantity + ir.CommanderReserveQuantity),
                        TotalAvailable = i.InventoryRecords.Sum(ir => ir.AvailableQuantity),
                        InventoryRecords = i.InventoryRecords.Select(ir => new InventoryRecordDTO
                        {
                            WarehouseId = ir.WarehouseId,
                            WarehouseName = ir.Warehouse.Name,
                            WarehouseNameAr = ir.Warehouse.NameAr,
                            GeneralQuantity = ir.GeneralQuantity,
                            CommanderReserveQuantity = ir.CommanderReserveQuantity,
                            AvailableQuantity = ir.AvailableQuantity
                        }).ToList()
                    })
                    .ToListAsync(cancellationToken);
            },
            TimeSpan.FromMinutes(5),  // Cache for 5 minutes
            cancellationToken);
    }

    public async Task<ItemDetailsDTO> GetItemDetailsAsync(
        int itemId,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"item_details_{itemId}";

        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async () =>
            {
                var item = await _context.Items
                    .Include(i => i.InventoryRecords)
                        .ThenInclude(ir => ir.Warehouse)
                    .Include(i => i.ProjectAllocations)
                        .ThenInclude(pa => pa.Project)
                    .Include(i => i.TransactionHistory)
                        .ThenInclude(th => th.Warehouse)
                    .FirstOrDefaultAsync(i => i.Id == itemId, cancellationToken);

                if (item == null)
                {
                    throw new NotFoundException($"Item with ID {itemId} not found");
                }

                return new ItemDetailsDTO
                {
                    Id = item.Id,
                    ItemCode = item.ItemCode,
                    ItemName = item.ItemName,
                    ItemNameAr = item.ItemNameAr,
                    // ... rest of mapping
                };
            },
            TimeSpan.FromMinutes(10),  // Cache for 10 minutes
            cancellationToken);
    }
}
```

---

## Implementation Roadmap

### Phase 1: Free Text Input Elimination (Week 1)

- [ ] Create `ItemAutocomplete` component
- [ ] Add search API endpoint to `InventoryController`
- [ ] Replace free text inputs in `Requisitions.tsx`
- [ ] Replace free text inputs in `Transfers.tsx`
- [ ] Replace free text inputs in `Returns.tsx`
- [ ] Replace free text inputs in `Consumptions.tsx`
- [ ] Replace free text inputs in `Receipts.tsx`
- [ ] Replace free text inputs in `Adjustments.tsx`
- [ ] Test autocomplete functionality
- [ ] Deploy to development environment

### Phase 2: Partial Issuance Automation (Week 2)

- [ ] Add `ParentRequisitionId` to `Requisition` entity
- [ ] Add `IsRemainingRequisition` and `RemainingReason` to `Requisition` entity
- [ ] Add `PartialIssueReason` to `RequisitionItem` entity
- [ ] Implement `IssuePartial` method in `Requisition` entity
- [ ] Implement `CreateRemainingRequisition` method in `Requisition` entity
- [ ] Create `IssuePartialRequisitionCommand` and handler
- [ ] Add `IssuePartial` endpoint to `RequisitionController`
- [ ] Create `PartialIssueDialog` component
- [ ] Update `Requisitions.tsx` with partial issue UI
- [ ] Test partial issuance workflow
- [ ] Deploy to development environment

### Phase 3: Advanced Item Details Page (Week 3-4)

- [ ] Create `ItemDetails.tsx` page
- [ ] Create `BasicInformationTab` component
- [ ] Create `StockTab` component
- [ ] Create `ProjectAllocationsTab` component
- [ ] Create `TransactionHistoryTab` component
- [ ] Create `ConsumptionAnalysisTab` component
- [ ] Create `StockMovementChart` component
- [ ] Create `ConsumptionTrendChart` component
- [ ] Add `GetItemDetails` endpoint to `InventoryController`
- [ ] Implement consumption rate calculations
- [ ] Implement stockout date prediction
- [ ] Implement days of stock calculation
- [ ] Add export to Excel functionality
- [ ] Test item details page
- [ ] Deploy to development environment

### Phase 4: Forms & UX Improvements (Week 5)

- [ ] Create `QuantityInput` component
- [ ] Create `ErrorAlert` component
- [ ] Create `DropdownSelect` component
- [ ] Update all forms with new components
- [ ] Add validation to all quantity inputs
- [ ] Add error handling to all forms
- [ ] Add success notifications
- [ ] Test all forms
- [ ] Deploy to development environment

### Phase 5: Archiving Strategy (Week 6)

- [ ] Add `IsArchived` and `ArchivedAt` to entities
- [ ] Create `IArchivingService` interface
- [ ] Implement `ArchivingService`
- [ ] Create `ArchivingBackgroundService`
- [ ] Register background service in `Program.cs`
- [ ] Test archiving functionality
- [ ] Deploy to development environment

### Phase 6: Auditing Strategy (Week 7)

- [ ] Update `IAuditService` interface
- [ ] Enhance `AuditService` implementation
- [ ] Add `GetAuditLogs` endpoint to `AuditController`
- [ ] Create audit log UI
- [ ] Add audit export functionality
- [ ] Test audit logging
- [ ] Deploy to development environment

### Phase 7: Performance Optimizations (Week 8)

- [ ] Create `ICacheService` interface
- [ ] Implement `CacheService`
- [ ] Add caching to frequently accessed data
- [ ] Optimize database queries
- [ ] Add pagination to all list views
- [ ] Add eager loading to navigation properties
- [ ] Monitor and optimize slow queries
- [ ] Performance testing
- [ ] Deploy to development environment

---

## Success Metrics

| Metric | Target | Measurement |
|--------|--------|-------------|
| Free Text Inputs Eliminated | 100% | No free text inputs in forms |
| Autocomplete Response Time | < 100ms | API response time |
| Partial Issuance Automation | 100% | Automatic remaining requisition creation |
| Item Details Page Load Time | < 2s | Page load time |
| Transaction History Query Time | < 100ms | Database query time |
| Archiving Success Rate | 100% | All old data archived |
| Audit Log Completeness | 100% | All operations logged |
| Cache Hit Rate | > 80% | Cache performance |
| API Response Time | < 200ms | Average API response time |

---

## Risk Mitigation

| Risk | Mitigation |
|------|-----------|
| Breaking changes to existing forms | Implement feature flags, gradual rollout |
| Performance degradation from caching | Monitor cache hit rates, adjust cache expiration |
| Archiving data loss | Test restore functionality, maintain backups |
| Audit logging overhead | Use async logging, batch operations |
| Database query performance | Add indexes, use query optimization |

---

## Conclusion

This focused implementation plan addresses the 8 critical improvements requested:

1. ✅ **Free Text Input Elimination** - Search-based autocomplete component
2. ✅ **Partial Issuance Automation** - Automatic remaining requisition creation
3. ✅ **Advanced Item Details Page** - Comprehensive Excel-like view
4. ✅ **Search-Based Item Selection** - Dropdown autocomplete with real-time filtering
5. ✅ **Forms & UX Improvements** - Controlled inputs with validation
6. ✅ **Archiving Strategy** - Background jobs for data retention
7. ✅ **Auditing Strategy** - Enhanced audit logging
8. ✅ **Performance Optimizations** - Caching, pagination, query optimization

**Total Estimated Timeline:** 8 weeks

**Next Steps:**
1. Review this plan with user
2. Prioritize phases based on business needs
3. Begin implementation in Code mode starting with Phase 1
4. Follow iterative development approach with regular reviews
