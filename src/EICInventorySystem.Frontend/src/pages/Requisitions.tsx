import React, { useState, useEffect, useCallback } from 'react';
import {
    Container,
    Box,
    Typography,
    Paper,
    Tabs,
    Tab,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Button,
    IconButton,
    Chip,
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    Grid,
    TextField,
    FormControl,
    InputLabel,
    Select,
    MenuItem,
    Alert,
    CircularProgress,
    Tooltip,
    Divider,
} from '@mui/material';
import {
    Add as AddIcon,
    Refresh as RefreshIcon,
    Visibility as ViewIcon,
    CheckCircle as ApproveIcon,
    Cancel as RejectIcon,
    Delete as DeleteIcon,
    Assignment as RequisitionIcon,
} from '@mui/icons-material';
import { useAuthStore } from '../stores/authStore';
import { requisitionService } from '../services/requisitionService';
import {
    ItemAutocomplete,
    ProjectAutocomplete,
    WarehouseAutocomplete,
    FactoryAutocomplete,
    DepartmentAutocomplete
} from '../components/common';
import type {
    Requisition,
    RequisitionItem,
    Item,
    Project,
    Warehouse,
} from '../types';

interface TabPanelProps {
    children?: React.ReactNode;
    index: number;
    value: number;
}

function TabPanel(props: TabPanelProps) {
    const { children, value, index, ...other } = props;
    return (
        <div
            role="tabpanel"
            hidden={value !== index}
            id={`req-tabpanel-${index}`}
            aria-labelledby={`req-tab-${index}`}
            {...other}
        >
            {value === index && <Box sx={{ pt: 3 }}>{children}</Box>}
        </div>
    );
}

// Temporary interface for Factory (should be in types)
interface FactoryData {
    id: number;
    name: string;
    nameAr: string;
    code: string;
}

interface DepartmentData {
    id: number;
    name: string;
    nameAr: string;
    code: string;
    factoryId: number;
}

const Requisitions: React.FC = () => {
    const { user } = useAuthStore();
    const [tabValue, setTabValue] = useState(0);
    const [loading, setLoading] = useState(true);
    const [requisitions, setRequisitions] = useState<Requisition[]>([]);
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState<string | null>(null);

    // Dialog states
    const [createDialogOpen, setCreateDialogOpen] = useState(false);
    const [viewDialogOpen, setViewDialogOpen] = useState(false);
    const [approveDialogOpen, setApproveDialogOpen] = useState(false);
    const [rejectDialogOpen, setRejectDialogOpen] = useState(false);
    const [selectedRequisition, setSelectedRequisition] = useState<Requisition | null>(null);

    // Form state
    const [formData, setFormData] = useState({
        factory: null as FactoryData | null,
        department: null as DepartmentData | null,
        project: null as Project | null,
        warehouse: null as Warehouse | null,
        priority: 'Medium' as 'Low' | 'Medium' | 'High' | 'Critical',
        notes: '',
        items: [] as Array<{
            item: Item | null;
            requestedQuantity: number;
            notes: string;
        }>,
    });

    // Workflow dialog state
    const [approvalNotes, setApprovalNotes] = useState('');
    const [rejectionReason, setRejectionReason] = useState('');

    const loadData = useCallback(async () => {
        setLoading(true);
        setError(null);
        try {
            const result = await requisitionService.getRequisitions({}, 1, 100);
            setRequisitions(result.items || []);
        } catch (err: any) {
            const message = err.message || 'Failed to load requisitions';
            setError(message);
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => {
        loadData();
    }, [loadData]);

    const handleTabChange = (_event: React.SyntheticEvent, newValue: number) => {
        setTabValue(newValue);
    };

    const handleCreateClick = () => {
        setFormData({
            factory: null,
            department: null,
            project: null,
            warehouse: null,
            priority: 'Medium',
            notes: '',
            items: [{ item: null, requestedQuantity: 1, notes: '' }],
        });
        setCreateDialogOpen(true);
    };

    const handleViewClick = (req: Requisition) => {
        setSelectedRequisition(req);
        setViewDialogOpen(true);
    };

    const handleAddItem = () => {
        setFormData({
            ...formData,
            items: [...formData.items, { item: null, requestedQuantity: 1, notes: '' }],
        });
    };

    const handleRemoveItem = (index: number) => {
        const newItems = formData.items.filter((_, i) => i !== index);
        setFormData({ ...formData, items: newItems });
    };

    const handleItemChange = (index: number, field: string, value: any) => {
        const newItems = [...formData.items];
        newItems[index] = { ...newItems[index], [field]: value };
        setFormData({ ...formData, items: newItems });
    };

    const handleCreateSubmit = async () => {
        if (!formData.factory || !formData.warehouse) {
            setError('Please select Factory and Warehouse / يرجى اختيار المصنع والمستودع');
            return;
        }

        // Must have either Department OR Project (or both, depending on logic, but usually at least one context)
        if (!formData.department && !formData.project) {
            // For now, let's just warn but allow if backend permits. But usually we need a cost center.
            // Let's assume Department is required if Project is not selected, or vice versa.
        }

        const validItems = formData.items.filter(i => i.item && i.requestedQuantity > 0);
        if (validItems.length === 0) {
            setError('Please add at least one item / يرجى إضافة صنف واحد على الأقل');
            return;
        }

        try {
            await requisitionService.createRequisition({
                type: 'Internal', // Defaulting to internal
                priority: formData.priority,
                warehouseId: formData.warehouse.id,
                projectId: formData.project?.id,
                departmentId: formData.department?.id,
                notes: formData.notes,
                items: validItems.map(i => ({
                    itemId: i.item!.id,
                    requestedQuantity: i.requestedQuantity,
                    notes: i.notes
                })),
            });
            setSuccess('Requisition created successfully / تم إنشاء الطلب بنجاح');
            setCreateDialogOpen(false);
            loadData();
        } catch (err: any) {
            const message = err.message || 'Failed to create requisition';
            setError(message);
        }
    };

    const handleApproveClick = (req: Requisition) => {
        setSelectedRequisition(req);
        setApprovalNotes('');
        setApproveDialogOpen(true);
    };

    const handleApproveConfirm = async () => {
        if (!selectedRequisition) return;
        try {
            // Logic to approve all quantity by default
            const approvalData = {
                items: selectedRequisition.items.map(i => ({
                    itemId: i.itemId,
                    approvedQuantity: i.requestedQuantity // Default to requested
                })),
                notes: approvalNotes
            };

            await requisitionService.approveRequisition(selectedRequisition.id, approvalData);
            setSuccess('Requisition approved successfully');
            setApproveDialogOpen(false);
            loadData();
        } catch (err: any) {
            setError(err.message || 'Failed to approve requisition');
        }
    };

    const handleRejectClick = (req: Requisition) => {
        setSelectedRequisition(req);
        setRejectionReason('');
        setRejectDialogOpen(true);
    };

    const handleRejectConfirm = async () => {
        if (!selectedRequisition || !rejectionReason) return;
        try {
            await requisitionService.rejectRequisition(selectedRequisition.id, rejectionReason);
            setSuccess('Requisition rejected');
            setRejectDialogOpen(false);
            loadData();
        } catch (err: any) {
            setError(err.message || 'Failed to reject requisition');
        }
    };

    const getStatusColor = (status: string) => {
        switch (status) {
            case 'Draft': return 'default';
            case 'Pending': return 'warning';
            case 'Approved': return 'success';
            case 'PartiallyFulfilled': return 'info';
            case 'Rejected': return 'error';
            case 'Completed': return 'success';
            default: return 'default';
        }
    };

    const getPriorityColor = (priority: string) => {
        switch (priority) {
            case 'Low': return 'default';
            case 'Medium': return 'info';
            case 'High': return 'warning';
            case 'Critical': return 'error';
            default: return 'default';
        }
    };

    const filteredRequisitions = requisitions.filter(req => {
        if (tabValue === 0) return true;
        if (tabValue === 1) return req.status === 'Pending';
        if (tabValue === 2) return req.status === 'Approved' || req.status === 'PartiallyFulfilled';
        if (tabValue === 3) return req.status === 'Completed';
        if (tabValue === 4) return req.status === 'Rejected';
        return true;
    });

    const canApprove = user?.role === 'FactoryCommander' || user?.role === 'ComplexCommander' || user?.role === 'DepartmentHead'; // Adjust roles as needed

    return (
        <Container maxWidth="xl">
            {/* Header */}
            <Box display="flex" justifyContent="space-between" alignItems="center" mb={3} mt={3}>
                <Box>
                    <Typography variant="h4" gutterBottom>
                        طلبات الصرف
                    </Typography>
                    <Typography variant="body1" color="textSecondary">
                        Material Requisitions
                    </Typography>
                </Box>
                <Box display="flex" gap={2}>
                    <Tooltip title="Refresh">
                        <IconButton onClick={loadData} color="primary">
                            <RefreshIcon />
                        </IconButton>
                    </Tooltip>
                    <Button
                        variant="contained"
                        startIcon={<AddIcon />}
                        onClick={handleCreateClick}
                    >
                        طلب صرف جديد
                    </Button>
                </Box>
            </Box>

            {/* Alerts */}
            {error && (
                <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError(null)}>
                    {error}
                </Alert>
            )}
            {success && (
                <Alert severity="success" sx={{ mb: 2 }} onClose={() => setSuccess(null)}>
                    {success}
                </Alert>
            )}

            {/* Tabs */}
            <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
                <Tabs value={tabValue} onChange={handleTabChange}>
                    <Tab label="All / الكل" />
                    <Tab label="Pending / معلق" />
                    <Tab label="Approved / معتمد" />
                    <Tab label="Completed / مكتمل" />
                    <Tab label="Rejected / مرفوض" />
                </Tabs>
            </Box>

            {/* Table */}
            <TabPanel value={tabValue} index={tabValue}>
                <TableContainer component={Paper}>
                    <Table>
                        <TableHead>
                            <TableRow>
                                <TableCell>Req #</TableCell>
                                <TableCell>Date</TableCell>
                                <TableCell>Project / Department</TableCell>
                                <TableCell align="center">Items</TableCell>
                                <TableCell>Priority</TableCell>
                                <TableCell>Status</TableCell>
                                <TableCell align="center">Actions</TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {loading ? (
                                <TableRow>
                                    <TableCell colSpan={7} align="center"><CircularProgress /></TableCell>
                                </TableRow>
                            ) : filteredRequisitions.length === 0 ? (
                                <TableRow>
                                    <TableCell colSpan={7} align="center">No requisitions found</TableCell>
                                </TableRow>
                            ) : (
                                filteredRequisitions.map(req => (
                                    <TableRow key={req.id} hover>
                                        <TableCell>
                                            <Typography variant="body2" fontWeight="bold">
                                                {req.requisitionNumber}
                                            </Typography>
                                        </TableCell>
                                        <TableCell>
                                            {new Date(req.requestedDate).toLocaleDateString('en-GB')}
                                        </TableCell>
                                        <TableCell>
                                            {req.projectName && <Chip label={req.projectName} size="small" sx={{ mr: 1 }} />}
                                            {req.departmentName && <Chip label={req.departmentName} size="small" variant="outlined" />}
                                        </TableCell>
                                        <TableCell align="center">{req.totalItems}</TableCell>
                                        <TableCell>
                                            <Chip
                                                label={req.priority}
                                                size="small"
                                                color={getPriorityColor(req.priority) as any}
                                            />
                                        </TableCell>
                                        <TableCell>
                                            <Chip
                                                label={req.status}
                                                size="small"
                                                color={getStatusColor(req.status) as any}
                                            />
                                        </TableCell>
                                        <TableCell align="center">
                                            <Box display="flex" gap={0.5} justifyContent="center">
                                                <Tooltip title="View Details">
                                                    <IconButton size="small" onClick={() => handleViewClick(req)}>
                                                        <ViewIcon />
                                                    </IconButton>
                                                </Tooltip>
                                                {req.status === 'Pending' && canApprove && (
                                                    <>
                                                        <Tooltip title="Approve">
                                                            <IconButton size="small" color="success" onClick={() => handleApproveClick(req)}>
                                                                <ApproveIcon />
                                                            </IconButton>
                                                        </Tooltip>
                                                        <Tooltip title="Reject">
                                                            <IconButton size="small" color="error" onClick={() => handleRejectClick(req)}>
                                                                <RejectIcon />
                                                            </IconButton>
                                                        </Tooltip>
                                                    </>
                                                )}
                                            </Box>
                                        </TableCell>
                                    </TableRow>
                                ))
                            )}
                        </TableBody>
                    </Table>
                </TableContainer>
            </TabPanel>

            {/* Create Dialog */}
            <Dialog open={createDialogOpen} onClose={() => setCreateDialogOpen(false)} maxWidth="lg" fullWidth>
                <DialogTitle>
                    <Box display="flex" alignItems="center" gap={1}>
                        <RequisitionIcon />
                        <Typography>Create New Requisition / طلب صرف جديد</Typography>
                    </Box>
                </DialogTitle>
                <DialogContent>
                    <Grid container spacing={2} sx={{ mt: 1 }}>
                        <Grid item xs={12} md={6}>
                            <FactoryAutocomplete
                                value={formData.factory as any}
                                onChange={(factory) => setFormData({ ...formData, factory: factory as any, department: null, warehouse: null })}
                                required
                            />
                        </Grid>
                        <Grid item xs={12} md={6}>
                            <WarehouseAutocomplete
                                value={formData.warehouse}
                                onChange={(warehouse) => setFormData({ ...formData, warehouse })}
                                factoryId={formData.factory?.id}
                                required
                            />
                        </Grid>
                        <Grid item xs={12} md={6}>
                            <ProjectAutocomplete
                                value={formData.project}
                                onChange={(project) => setFormData({ ...formData, project })}
                                factoryId={formData.factory?.id}
                            />
                        </Grid>
                        <Grid item xs={12} md={6}>
                            <DepartmentAutocomplete
                                value={formData.department as any}
                                onChange={(department) => setFormData({ ...formData, department: department as any })}
                                factoryId={formData.factory?.id}
                            />
                        </Grid>
                        <Grid item xs={12} md={6}>
                            <FormControl fullWidth>
                                <InputLabel>Priority / الأولوية</InputLabel>
                                <Select
                                    value={formData.priority}
                                    label="Priority / الأولوية"
                                    onChange={(e) => setFormData({ ...formData, priority: e.target.value as any })}
                                >
                                    <MenuItem value="Low">Low / منخفض</MenuItem>
                                    <MenuItem value="Medium">Medium / متوسط</MenuItem>
                                    <MenuItem value="High">High / عالي</MenuItem>
                                    <MenuItem value="Critical">Critical / حرج</MenuItem>
                                </Select>
                            </FormControl>
                        </Grid>
                        <Grid item xs={12}>
                            <TextField
                                fullWidth
                                label="Notes / ملاحظات"
                                multiline
                                rows={2}
                                value={formData.notes}
                                onChange={(e) => setFormData({ ...formData, notes: e.target.value })}
                            />
                        </Grid>

                        <Grid item xs={12}>
                            <Divider sx={{ my: 2 }}>
                                <Typography variant="subtitle2">Items / الأصناف</Typography>
                            </Divider>
                        </Grid>

                        {formData.items.map((item, index) => (
                            <Grid item xs={12} key={index}>
                                <Paper variant="outlined" sx={{ p: 2 }}>
                                    <Grid container spacing={2} alignItems="center">
                                        <Grid item xs={12} md={6}>
                                            <ItemAutocomplete
                                                value={item.item}
                                                onChange={(newItem) => handleItemChange(index, 'item', newItem)}
                                                warehouseId={formData.warehouse?.id}
                                                showStockInfo
                                            />
                                        </Grid>
                                        <Grid item xs={6} md={2}>
                                            <TextField
                                                fullWidth
                                                label="Quantity"
                                                type="number"
                                                value={item.requestedQuantity}
                                                onChange={(e) => handleItemChange(index, 'requestedQuantity', parseFloat(e.target.value) || 0)}
                                                inputProps={{ min: 0 }}
                                            />
                                        </Grid>
                                        <Grid item xs={12} md={3}>
                                            <TextField
                                                fullWidth
                                                label="Item Notes"
                                                value={item.notes}
                                                onChange={(e) => handleItemChange(index, 'notes', e.target.value)}
                                            />
                                        </Grid>
                                        <Grid item xs={2} md={1}>
                                            <IconButton color="error" onClick={() => handleRemoveItem(index)} disabled={formData.items.length === 1}>
                                                <DeleteIcon />
                                            </IconButton>
                                        </Grid>
                                    </Grid>
                                </Paper>
                            </Grid>
                        ))}

                        <Grid item xs={12}>
                            <Button startIcon={<AddIcon />} onClick={handleAddItem}>
                                Add Item
                            </Button>
                        </Grid>
                    </Grid>
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setCreateDialogOpen(false)}>Cancel</Button>
                    <Button onClick={handleCreateSubmit} variant="contained" disabled={!formData.warehouse || (!formData.project && !formData.department)}>
                        Submit Request
                    </Button>
                </DialogActions>
            </Dialog>

            {/* View Dialog */}
            <Dialog open={viewDialogOpen} onClose={() => setViewDialogOpen(false)} maxWidth="md" fullWidth>
                <DialogTitle>Requisition Details - {selectedRequisition?.requisitionNumber}</DialogTitle>
                <DialogContent>
                    {selectedRequisition && (
                        <Grid container spacing={2} sx={{ mt: 1 }}>
                            <Grid item xs={6}>
                                <Typography variant="body2" color="textSecondary">Requested Date</Typography>
                                <Typography variant="body1">{new Date(selectedRequisition.requestedDate).toLocaleDateString()}</Typography>
                            </Grid>
                            <Grid item xs={6}>
                                <Typography variant="body2" color="textSecondary">Status</Typography>
                                <Typography variant="body1">{selectedRequisition.status}</Typography>
                            </Grid>
                            <Grid item xs={6}>
                                <Typography variant="body2" color="textSecondary">Department</Typography>
                                <Typography variant="body1">{selectedRequisition.departmentName || '-'}</Typography>
                            </Grid>
                            <Grid item xs={6}>
                                <Typography variant="body2" color="textSecondary">Project</Typography>
                                <Typography variant="body1">{selectedRequisition.projectName || '-'}</Typography>
                            </Grid>
                            <Grid item xs={12}>
                                <TableContainer component={Paper} variant="outlined">
                                    <Table size="small">
                                        <TableHead>
                                            <TableRow>
                                                <TableCell>Item</TableCell>
                                                <TableCell align="right">Requested</TableCell>
                                                <TableCell align="right">Approved</TableCell>
                                                <TableCell align="right">Issued</TableCell>
                                                <TableCell>Unit</TableCell>
                                            </TableRow>
                                        </TableHead>
                                        <TableBody>
                                            {selectedRequisition.items.map((item, idx) => (
                                                <TableRow key={idx}>
                                                    <TableCell>
                                                        <Typography variant="body2">{item.itemNameAr}</Typography>
                                                        <Typography variant="caption">{item.itemCode}</Typography>
                                                    </TableCell>
                                                    <TableCell align="right">{item.requestedQuantity}</TableCell>
                                                    <TableCell align="right">{item.approvedQuantity || '-'}</TableCell>
                                                    <TableCell align="right">{item.issuedQuantity || '-'}</TableCell>
                                                    <TableCell>{item.unitAr}</TableCell>
                                                </TableRow>
                                            ))}
                                        </TableBody>
                                    </Table>
                                </TableContainer>
                            </Grid>
                        </Grid>
                    )}
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setViewDialogOpen(false)}>Close</Button>
                </DialogActions>
            </Dialog>

            {/* Approve Dialog */}
            <Dialog open={approveDialogOpen} onClose={() => setApproveDialogOpen(false)} maxWidth="sm" fullWidth>
                <DialogTitle>Approve Requisition</DialogTitle>
                <DialogContent>
                    <Typography variant="body2" gutterBottom>
                        {selectedRequisition?.requisitionNumber}
                    </Typography>
                    <TextField
                        autoFocus
                        margin="dense"
                        label="Approval Notes"
                        fullWidth
                        multiline
                        rows={3}
                        value={approvalNotes}
                        onChange={(e) => setApprovalNotes(e.target.value)}
                    />
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setApproveDialogOpen(false)}>Cancel</Button>
                    <Button onClick={handleApproveConfirm} variant="contained" color="success">
                        Approve
                    </Button>
                </DialogActions>
            </Dialog>

            {/* Reject Dialog */}
            <Dialog open={rejectDialogOpen} onClose={() => setRejectDialogOpen(false)} maxWidth="sm" fullWidth>
                <DialogTitle>Reject Requisition</DialogTitle>
                <DialogContent>
                    <Typography variant="body2" gutterBottom>
                        {selectedRequisition?.requisitionNumber}
                    </Typography>
                    <TextField
                        autoFocus
                        margin="dense"
                        label="Rejection Reason"
                        fullWidth
                        multiline
                        rows={3}
                        value={rejectionReason}
                        onChange={(e) => setRejectionReason(e.target.value)}
                        required
                    />
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setRejectDialogOpen(false)}>Cancel</Button>
                    <Button onClick={handleRejectConfirm} variant="contained" color="error" disabled={!rejectionReason}>
                        Reject
                    </Button>
                </DialogActions>
            </Dialog>

        </Container>
    );
};

export default Requisitions;
