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
    Card,
    CardContent,
    Divider,
    LinearProgress,
} from '@mui/material';
import {
    Add as AddIcon,
    Refresh as RefreshIcon,
    Visibility as ViewIcon,
    Edit as EditIcon,
    Delete as DeleteIcon,
    Send as SubmitIcon,
    CheckCircle as ApproveIcon,
    Cancel as RejectIcon,
    LocalShipping as IssueIcon,
    Description as BOQIcon,
} from '@mui/icons-material';
import { useAuthStore } from '../stores/authStore';
import { projectBOQService } from '../services';
import {
    ItemAutocomplete,
    ProjectAutocomplete,
    WarehouseAutocomplete,
    FactoryAutocomplete,
} from '../components/common';
import type {
    ProjectBOQ,
    ProjectBOQItem,
    BOQStatus,
    BOQPriority,
    BOQStatistics,
    Project,
    Warehouse,
    Item,
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
            id={`boq-tabpanel-${index}`}
            aria-labelledby={`boq-tab-${index}`}
            {...other}
        >
            {value === index && <Box sx={{ pt: 3 }}>{children}</Box>}
        </div>
    );
}

interface Factory {
    id: number;
    name: string;
    nameAr: string;
    code: string;
    location: string;
    isActive: boolean;
}

const ProjectBOQPage: React.FC = () => {
    const { user } = useAuthStore();
    const [tabValue, setTabValue] = useState(0);
    const [loading, setLoading] = useState(true);
    const [boqs, setBOQs] = useState<ProjectBOQ[]>([]);
    const [statistics, setStatistics] = useState<BOQStatistics | null>(null);
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState<string | null>(null);

    // Dialog states
    const [createDialogOpen, setCreateDialogOpen] = useState(false);
    const [viewDialogOpen, setViewDialogOpen] = useState(false);
    const [approveDialogOpen, setApproveDialogOpen] = useState(false);
    const [rejectDialogOpen, setRejectDialogOpen] = useState(false);
    const [issueDialogOpen, setIssueDialogOpen] = useState(false);
    const [selectedBOQ, setSelectedBOQ] = useState<ProjectBOQ | null>(null);

    // Form state
    const [formData, setFormData] = useState({
        project: null as Project | null,
        factory: null as Factory | null,
        warehouse: null as Warehouse | null,
        requiredDate: '',
        priority: 'Medium' as BOQPriority,
        notes: '',
        notesArabic: '',
        items: [] as Array<{
            item: Item | null;
            requestedQuantity: number;
            useCommanderReserve: boolean;
            commanderReserveQuantity: number;
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
            const factoryId = user?.factoryId;
            const [boqsResponse, statsResponse] = await Promise.all([
                projectBOQService.getBOQs({ factoryId }, 1, 100),
                projectBOQService.getStatistics(factoryId),
            ]);
            setBOQs(boqsResponse.items || []);
            setStatistics(statsResponse);
        } catch (err: unknown) {
            const message = err instanceof Error ? err.message : 'Failed to load BOQ data';
            setError(message);
        } finally {
            setLoading(false);
        }
    }, [user?.factoryId]);

    useEffect(() => {
        loadData();
    }, [loadData]);

    const handleTabChange = (_event: React.SyntheticEvent, newValue: number) => {
        setTabValue(newValue);
    };

    const handleCreateClick = () => {
        setFormData({
            project: null,
            factory: null,
            warehouse: null,
            requiredDate: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000).toISOString().split('T')[0],
            priority: 'Medium',
            notes: '',
            notesArabic: '',
            items: [{ item: null, requestedQuantity: 1, useCommanderReserve: false, commanderReserveQuantity: 0, notes: '' }],
        });
        setCreateDialogOpen(true);
    };

    const handleViewClick = (boq: ProjectBOQ) => {
        setSelectedBOQ(boq);
        setViewDialogOpen(true);
    };

    const handleAddItem = () => {
        setFormData({
            ...formData,
            items: [...formData.items, { item: null, requestedQuantity: 1, useCommanderReserve: false, commanderReserveQuantity: 0, notes: '' }],
        });
    };

    const handleRemoveItem = (index: number) => {
        const newItems = formData.items.filter((_, i) => i !== index);
        setFormData({ ...formData, items: newItems });
    };

    const handleItemChange = (index: number, field: string, value: unknown) => {
        const newItems = [...formData.items];
        newItems[index] = { ...newItems[index], [field]: value };
        setFormData({ ...formData, items: newItems });
    };

    const handleCreateSubmit = async () => {
        if (!formData.project || !formData.factory || !formData.warehouse) {
            setError('Please fill in all required fields');
            return;
        }

        const validItems = formData.items.filter(i => i.item && i.requestedQuantity > 0);
        if (validItems.length === 0) {
            setError('Please add at least one item');
            return;
        }

        try {
            await projectBOQService.createBOQ({
                projectId: formData.project.id,
                factoryId: formData.factory.id,
                warehouseId: formData.warehouse.id,
                requiredDate: formData.requiredDate,
                priority: formData.priority,
                notes: formData.notes,
                notesArabic: formData.notesArabic,
                items: validItems.map(i => ({
                    itemId: i.item!.id,
                    requestedQuantity: i.requestedQuantity,
                    useCommanderReserve: i.useCommanderReserve,
                    commanderReserveQuantity: i.commanderReserveQuantity,
                    notes: i.notes,
                })),
            });
            setSuccess('BOQ created successfully');
            setCreateDialogOpen(false);
            loadData();
        } catch (err: unknown) {
            const message = err instanceof Error ? err.message : 'Failed to create BOQ';
            setError(message);
        }
    };

    const handleSubmitBOQ = async (boq: ProjectBOQ) => {
        try {
            await projectBOQService.submitBOQ(boq.id);
            setSuccess('BOQ submitted for approval');
            loadData();
        } catch (err: unknown) {
            const message = err instanceof Error ? err.message : 'Failed to submit BOQ';
            setError(message);
        }
    };

    const handleApproveClick = (boq: ProjectBOQ) => {
        setSelectedBOQ(boq);
        setApprovalNotes('');
        setApproveDialogOpen(true);
    };

    const handleApproveConfirm = async () => {
        if (!selectedBOQ) return;
        try {
            await projectBOQService.approveBOQ({
                id: selectedBOQ.id,
                approvalNotes: approvalNotes,
            });
            setSuccess('BOQ approved successfully');
            setApproveDialogOpen(false);
            loadData();
        } catch (err: unknown) {
            const message = err instanceof Error ? err.message : 'Failed to approve BOQ';
            setError(message);
        }
    };

    const handleRejectClick = (boq: ProjectBOQ) => {
        setSelectedBOQ(boq);
        setRejectionReason('');
        setRejectDialogOpen(true);
    };

    const handleRejectConfirm = async () => {
        if (!selectedBOQ || !rejectionReason) return;
        try {
            await projectBOQService.rejectBOQ({
                id: selectedBOQ.id,
                rejectionReason: rejectionReason,
            });
            setSuccess('BOQ rejected');
            setRejectDialogOpen(false);
            loadData();
        } catch (err: unknown) {
            const message = err instanceof Error ? err.message : 'Failed to reject BOQ';
            setError(message);
        }
    };

    const handleIssueClick = (boq: ProjectBOQ) => {
        setSelectedBOQ(boq);
        setIssueDialogOpen(true);
    };

    const handleIssueConfirm = async () => {
        if (!selectedBOQ) return;
        try {
            await projectBOQService.issueBOQ({
                id: selectedBOQ.id,
                allowPartialIssue: true,
                items: selectedBOQ.items.map(item => ({
                    itemId: item.itemId,
                    issueQuantity: Math.min(item.remainingQuantity, item.availableStock),
                })),
            });
            setSuccess('BOQ issued successfully');
            setIssueDialogOpen(false);
            loadData();
        } catch (err: unknown) {
            const message = err instanceof Error ? err.message : 'Failed to issue BOQ';
            setError(message);
        }
    };

    const handleDeleteBOQ = async (boq: ProjectBOQ) => {
        if (!window.confirm('Are you sure you want to delete this BOQ?')) return;
        try {
            await projectBOQService.deleteBOQ(boq.id);
            setSuccess('BOQ deleted successfully');
            loadData();
        } catch (err: unknown) {
            const message = err instanceof Error ? err.message : 'Failed to delete BOQ';
            setError(message);
        }
    };

    const getStatusColor = (status: BOQStatus) => {
        switch (status) {
            case 'Draft': return 'default';
            case 'Pending': return 'warning';
            case 'Approved': return 'info';
            case 'PartiallyIssued': return 'primary';
            case 'FullyIssued': return 'success';
            case 'Completed': return 'success';
            case 'Cancelled': return 'error';
            default: return 'default';
        }
    };

    const getPriorityColor = (priority: BOQPriority) => {
        switch (priority) {
            case 'Low': return 'default';
            case 'Medium': return 'info';
            case 'High': return 'warning';
            case 'Critical': return 'error';
            default: return 'default';
        }
    };

    const filteredBOQs = boqs.filter(boq => {
        if (tabValue === 0) return true;
        if (tabValue === 1) return boq.status === 'Draft';
        if (tabValue === 2) return boq.status === 'Pending';
        if (tabValue === 3) return boq.status === 'Approved';
        if (tabValue === 4) return boq.status === 'PartiallyIssued' || boq.status === 'FullyIssued';
        return true;
    });

    const canCreate = user?.role !== 'Auditor';
    const canApprove = user?.role === 'FactoryCommander' || user?.role === 'ComplexCommander' || user?.role === 'DepartmentHead';
    const canIssue = user?.role === 'FactoryWarehouseKeeper' || user?.role === 'CentralWarehouseKeeper';

    if (loading) {
        return (
            <Box display="flex" justifyContent="center" alignItems="center" minHeight="60vh">
                <CircularProgress />
            </Box>
        );
    }

    return (
        <Container maxWidth="xl">
            {/* Header */}
            <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
                <Box>
                    <Typography variant="h4" gutterBottom>
                        قائمة الكميات للمشاريع
                    </Typography>
                    <Typography variant="body1" color="textSecondary">
                        Project Bill of Quantities (BOQ)
                    </Typography>
                </Box>
                <Box display="flex" gap={2}>
                    <Tooltip title="Refresh">
                        <IconButton onClick={loadData} color="primary">
                            <RefreshIcon />
                        </IconButton>
                    </Tooltip>
                    {canCreate && (
                        <Button
                            variant="contained"
                            startIcon={<AddIcon />}
                            onClick={handleCreateClick}
                        >
                            إنشاء قائمة كميات جديدة
                        </Button>
                    )}
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

            {/* Statistics Cards */}
            {statistics && (
                <Grid container spacing={2} mb={3}>
                    <Grid item xs={12} sm={6} md={2}>
                        <Card>
                            <CardContent>
                                <Typography variant="h4" color="primary">{statistics.totalBOQs}</Typography>
                                <Typography variant="body2" color="textSecondary">Total BOQs</Typography>
                            </CardContent>
                        </Card>
                    </Grid>
                    <Grid item xs={12} sm={6} md={2}>
                        <Card>
                            <CardContent>
                                <Typography variant="h4" color="warning.main">{statistics.pendingBOQs}</Typography>
                                <Typography variant="body2" color="textSecondary">Pending</Typography>
                            </CardContent>
                        </Card>
                    </Grid>
                    <Grid item xs={12} sm={6} md={2}>
                        <Card>
                            <CardContent>
                                <Typography variant="h4" color="info.main">{statistics.approvedBOQs}</Typography>
                                <Typography variant="body2" color="textSecondary">Approved</Typography>
                            </CardContent>
                        </Card>
                    </Grid>
                    <Grid item xs={12} sm={6} md={2}>
                        <Card>
                            <CardContent>
                                <Typography variant="h4" color="success.main">{statistics.fullyIssuedBOQs}</Typography>
                                <Typography variant="body2" color="textSecondary">Issued</Typography>
                            </CardContent>
                        </Card>
                    </Grid>
                    <Grid item xs={12} sm={6} md={4}>
                        <Card>
                            <CardContent>
                                <Typography variant="body2" color="textSecondary">Issue Progress</Typography>
                                <Box display="flex" alignItems="center" gap={2}>
                                    <LinearProgress
                                        variant="determinate"
                                        value={statistics.totalRequestedQuantity > 0
                                            ? (statistics.totalIssuedQuantity / statistics.totalRequestedQuantity) * 100
                                            : 0}
                                        sx={{ flexGrow: 1, height: 10, borderRadius: 5 }}
                                    />
                                    <Typography variant="body2">
                                        {statistics.totalIssuedQuantity.toLocaleString()} / {statistics.totalRequestedQuantity.toLocaleString()}
                                    </Typography>
                                </Box>
                            </CardContent>
                        </Card>
                    </Grid>
                </Grid>
            )}

            {/* Tabs */}
            <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
                <Tabs value={tabValue} onChange={handleTabChange}>
                    <Tab label="All / الكل" />
                    <Tab label="Draft / مسودة" />
                    <Tab label="Pending / معلق" />
                    <Tab label="Approved / معتمد" />
                    <Tab label="Issued / مصروف" />
                </Tabs>
            </Box>

            {/* BOQ Table */}
            <TabPanel value={tabValue} index={tabValue}>
                <TableContainer component={Paper}>
                    <Table>
                        <TableHead>
                            <TableRow>
                                <TableCell>رقم القائمة</TableCell>
                                <TableCell>المشروع</TableCell>
                                <TableCell>المصنع</TableCell>
                                <TableCell align="center">الأصناف</TableCell>
                                <TableCell align="right">الكمية المطلوبة</TableCell>
                                <TableCell align="right">الكمية المصروفة</TableCell>
                                <TableCell>الأولوية</TableCell>
                                <TableCell>الحالة</TableCell>
                                <TableCell>التاريخ المطلوب</TableCell>
                                <TableCell align="center">الإجراءات</TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {filteredBOQs.length === 0 ? (
                                <TableRow>
                                    <TableCell colSpan={10} align="center">
                                        <Typography variant="body2" color="textSecondary">
                                            No BOQs found
                                        </Typography>
                                    </TableCell>
                                </TableRow>
                            ) : (
                                filteredBOQs.map(boq => (
                                    <TableRow key={boq.id} hover>
                                        <TableCell>
                                            <Typography variant="body2" fontWeight="bold">
                                                {boq.boqNumber}
                                            </Typography>
                                        </TableCell>
                                        <TableCell>
                                            <Typography variant="body2">{boq.projectNameArabic}</Typography>
                                            <Typography variant="caption" color="textSecondary">{boq.projectCode}</Typography>
                                        </TableCell>
                                        <TableCell>{boq.factoryNameArabic}</TableCell>
                                        <TableCell align="center">{boq.totalItems}</TableCell>
                                        <TableCell align="right">{boq.totalQuantity.toLocaleString()}</TableCell>
                                        <TableCell align="right">{boq.issuedQuantity.toLocaleString()}</TableCell>
                                        <TableCell>
                                            <Chip
                                                label={boq.priority}
                                                size="small"
                                                color={getPriorityColor(boq.priority) as any}
                                            />
                                        </TableCell>
                                        <TableCell>
                                            <Chip
                                                label={boq.status}
                                                size="small"
                                                color={getStatusColor(boq.status) as any}
                                            />
                                            {boq.requiresCommanderReserve && (
                                                <Chip
                                                    label="CR"
                                                    size="small"
                                                    color="secondary"
                                                    sx={{ ml: 0.5 }}
                                                    title="Requires Commander Reserve"
                                                />
                                            )}
                                        </TableCell>
                                        <TableCell>
                                            {boq.requiredDate ? new Date(boq.requiredDate).toLocaleDateString('en-GB') : '-'}
                                        </TableCell>
                                        <TableCell align="center">
                                            <Box display="flex" gap={0.5} justifyContent="center">
                                                <Tooltip title="View Details">
                                                    <IconButton size="small" onClick={() => handleViewClick(boq)}>
                                                        <ViewIcon />
                                                    </IconButton>
                                                </Tooltip>
                                                {boq.status === 'Draft' && (
                                                    <>
                                                        <Tooltip title="Edit">
                                                            <IconButton size="small" color="primary">
                                                                <EditIcon />
                                                            </IconButton>
                                                        </Tooltip>
                                                        <Tooltip title="Submit for Approval">
                                                            <IconButton size="small" color="info" onClick={() => handleSubmitBOQ(boq)}>
                                                                <SubmitIcon />
                                                            </IconButton>
                                                        </Tooltip>
                                                        <Tooltip title="Delete">
                                                            <IconButton size="small" color="error" onClick={() => handleDeleteBOQ(boq)}>
                                                                <DeleteIcon />
                                                            </IconButton>
                                                        </Tooltip>
                                                    </>
                                                )}
                                                {boq.status === 'Pending' && canApprove && (
                                                    <>
                                                        <Tooltip title="Approve">
                                                            <IconButton size="small" color="success" onClick={() => handleApproveClick(boq)}>
                                                                <ApproveIcon />
                                                            </IconButton>
                                                        </Tooltip>
                                                        <Tooltip title="Reject">
                                                            <IconButton size="small" color="error" onClick={() => handleRejectClick(boq)}>
                                                                <RejectIcon />
                                                            </IconButton>
                                                        </Tooltip>
                                                    </>
                                                )}
                                                {boq.status === 'Approved' && canIssue && (
                                                    <Tooltip title="Issue Items">
                                                        <IconButton size="small" color="primary" onClick={() => handleIssueClick(boq)}>
                                                            <IssueIcon />
                                                        </IconButton>
                                                    </Tooltip>
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

            {/* Create BOQ Dialog */}
            <Dialog open={createDialogOpen} onClose={() => setCreateDialogOpen(false)} maxWidth="lg" fullWidth>
                <DialogTitle>
                    <Box display="flex" alignItems="center" gap={1}>
                        <BOQIcon />
                        <Typography>Create New BOQ / إنشاء قائمة كميات جديدة</Typography>
                    </Box>
                </DialogTitle>
                <DialogContent>
                    <Grid container spacing={2} sx={{ mt: 1 }}>
                        <Grid item xs={12} md={6}>
                            <FactoryAutocomplete
                                value={formData.factory}
                                onChange={(factory) => setFormData({ ...formData, factory, warehouse: null })}
                                required
                            />
                        </Grid>
                        <Grid item xs={12} md={6}>
                            <ProjectAutocomplete
                                value={formData.project}
                                onChange={(project) => setFormData({ ...formData, project })}
                                factoryId={formData.factory?.id}
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
                        <Grid item xs={12} md={3}>
                            <TextField
                                fullWidth
                                label="Required Date / التاريخ المطلوب"
                                type="date"
                                value={formData.requiredDate}
                                onChange={(e) => setFormData({ ...formData, requiredDate: e.target.value })}
                                InputLabelProps={{ shrink: true }}
                            />
                        </Grid>
                        <Grid item xs={12} md={3}>
                            <FormControl fullWidth>
                                <InputLabel>Priority / الأولوية</InputLabel>
                                <Select
                                    value={formData.priority}
                                    label="Priority / الأولوية"
                                    onChange={(e) => setFormData({ ...formData, priority: e.target.value as BOQPriority })}
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
                                        <Grid item xs={12} md={5}>
                                            <ItemAutocomplete
                                                value={item.item}
                                                onChange={(newItem) => handleItemChange(index, 'item', newItem)}
                                                warehouseId={formData.warehouse?.id}
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
                                        <Grid item xs={6} md={2}>
                                            <FormControl fullWidth>
                                                <InputLabel>Reserve</InputLabel>
                                                <Select
                                                    value={item.useCommanderReserve ? 'yes' : 'no'}
                                                    label="Reserve"
                                                    onChange={(e) => handleItemChange(index, 'useCommanderReserve', e.target.value === 'yes')}
                                                >
                                                    <MenuItem value="no">No</MenuItem>
                                                    <MenuItem value="yes">Yes</MenuItem>
                                                </Select>
                                            </FormControl>
                                        </Grid>
                                        <Grid item xs={10} md={2}>
                                            <TextField
                                                fullWidth
                                                label="Notes"
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
                    <Button onClick={handleCreateSubmit} variant="contained">
                        Create BOQ
                    </Button>
                </DialogActions>
            </Dialog>

            {/* View BOQ Dialog */}
            <Dialog open={viewDialogOpen} onClose={() => setViewDialogOpen(false)} maxWidth="lg" fullWidth>
                <DialogTitle>
                    BOQ Details - {selectedBOQ?.boqNumber}
                </DialogTitle>
                <DialogContent>
                    {selectedBOQ && (
                        <Grid container spacing={2} sx={{ mt: 1 }}>
                            <Grid item xs={12} sm={6} md={3}>
                                <Typography variant="body2" color="textSecondary">Project</Typography>
                                <Typography variant="body1">{selectedBOQ.projectNameArabic}</Typography>
                                <Typography variant="caption">{selectedBOQ.projectCode}</Typography>
                            </Grid>
                            <Grid item xs={12} sm={6} md={3}>
                                <Typography variant="body2" color="textSecondary">Factory</Typography>
                                <Typography variant="body1">{selectedBOQ.factoryNameArabic}</Typography>
                            </Grid>
                            <Grid item xs={12} sm={6} md={3}>
                                <Typography variant="body2" color="textSecondary">Warehouse</Typography>
                                <Typography variant="body1">{selectedBOQ.warehouseNameArabic}</Typography>
                            </Grid>
                            <Grid item xs={12} sm={6} md={3}>
                                <Typography variant="body2" color="textSecondary">Status</Typography>
                                <Chip label={selectedBOQ.status} color={getStatusColor(selectedBOQ.status) as any} />
                            </Grid>
                            <Grid item xs={12}>
                                <Divider sx={{ my: 2 }} />
                                <Typography variant="h6" gutterBottom>Items</Typography>
                                <TableContainer>
                                    <Table size="small">
                                        <TableHead>
                                            <TableRow>
                                                <TableCell>رقم الصنف</TableCell>
                                                <TableCell>اسم الصنف</TableCell>
                                                <TableCell align="right">المطلوب</TableCell>
                                                <TableCell align="right">المصروف</TableCell>
                                                <TableCell align="right">المتبقي</TableCell>
                                                <TableCell align="right">المتاح</TableCell>
                                                <TableCell align="right">العجز</TableCell>
                                            </TableRow>
                                        </TableHead>
                                        <TableBody>
                                            {selectedBOQ.items.map((item: ProjectBOQItem) => (
                                                <TableRow key={item.id}>
                                                    <TableCell>{item.itemCode}</TableCell>
                                                    <TableCell>{item.itemNameArabic}</TableCell>
                                                    <TableCell align="right">{item.requestedQuantity.toLocaleString()}</TableCell>
                                                    <TableCell align="right">{item.issuedQuantity.toLocaleString()}</TableCell>
                                                    <TableCell align="right">{item.remainingQuantity.toLocaleString()}</TableCell>
                                                    <TableCell align="right">{item.availableStock.toLocaleString()}</TableCell>
                                                    <TableCell align="right">
                                                        {item.shortfall > 0 ? (
                                                            <Typography color="error">{item.shortfall.toLocaleString()}</Typography>
                                                        ) : '-'}
                                                    </TableCell>
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
                <DialogTitle>Approve BOQ</DialogTitle>
                <DialogContent>
                    <Typography variant="body2" gutterBottom>
                        <strong>BOQ #:</strong> {selectedBOQ?.boqNumber}
                    </Typography>
                    <Typography variant="body2" gutterBottom>
                        <strong>Project:</strong> {selectedBOQ?.projectNameArabic}
                    </Typography>
                    <TextField
                        autoFocus
                        margin="dense"
                        label="Approval Notes (Optional)"
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
                <DialogTitle>Reject BOQ</DialogTitle>
                <DialogContent>
                    <Typography variant="body2" gutterBottom>
                        <strong>BOQ #:</strong> {selectedBOQ?.boqNumber}
                    </Typography>
                    <TextField
                        autoFocus
                        margin="dense"
                        label="Rejection Reason *"
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

            {/* Issue Dialog */}
            <Dialog open={issueDialogOpen} onClose={() => setIssueDialogOpen(false)} maxWidth="md" fullWidth>
                <DialogTitle>Issue BOQ Items</DialogTitle>
                <DialogContent>
                    {selectedBOQ && (
                        <>
                            <Typography variant="body2" gutterBottom>
                                <strong>BOQ #:</strong> {selectedBOQ.boqNumber}
                            </Typography>
                            <Typography variant="body2" gutterBottom>
                                <strong>Project:</strong> {selectedBOQ.projectNameArabic}
                            </Typography>
                            <TableContainer sx={{ mt: 2 }}>
                                <Table size="small">
                                    <TableHead>
                                        <TableRow>
                                            <TableCell>Item</TableCell>
                                            <TableCell align="right">Remaining</TableCell>
                                            <TableCell align="right">Available</TableCell>
                                            <TableCell align="right">To Issue</TableCell>
                                        </TableRow>
                                    </TableHead>
                                    <TableBody>
                                        {selectedBOQ.items.map((item: ProjectBOQItem) => {
                                            const toIssue = Math.min(item.remainingQuantity, item.availableStock);
                                            return (
                                                <TableRow key={item.id}>
                                                    <TableCell>
                                                        <Typography variant="body2">{item.itemNameArabic}</Typography>
                                                        <Typography variant="caption" color="textSecondary">{item.itemCode}</Typography>
                                                    </TableCell>
                                                    <TableCell align="right">{item.remainingQuantity.toLocaleString()}</TableCell>
                                                    <TableCell align="right">{item.availableStock.toLocaleString()}</TableCell>
                                                    <TableCell align="right">
                                                        <Typography color={toIssue < item.remainingQuantity ? 'warning.main' : 'success.main'}>
                                                            {toIssue.toLocaleString()}
                                                        </Typography>
                                                    </TableCell>
                                                </TableRow>
                                            );
                                        })}
                                    </TableBody>
                                </Table>
                            </TableContainer>
                        </>
                    )}
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setIssueDialogOpen(false)}>Cancel</Button>
                    <Button onClick={handleIssueConfirm} variant="contained" color="primary">
                        Issue Items
                    </Button>
                </DialogActions>
            </Dialog>
        </Container>
    );
};

export default ProjectBOQPage;
