import React, { useState, useEffect } from 'react';
import {
    Box,
    Container,
    Typography,
    Paper,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Button,
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    TextField,
    Grid,
    Alert,
    CircularProgress,
    IconButton,
    Tooltip,
    Chip,
    Select,
    MenuItem,
    FormControl,
    InputLabel,
    Autocomplete,
    Card,
    CardContent,
    Tabs,
    Tab,
} from '@mui/material';
import {
    Add as AddIcon,
    Refresh as RefreshIcon,
    CheckCircle as CheckCircleIcon,
    Cancel as CancelIcon,
    Pending as PendingIcon,
    ArrowForward as ArrowForwardIcon,
    LocalShipping as LocalShippingIcon,
    Warehouse as WarehouseIcon,
    Factory as FactoryIcon,
} from '@mui/icons-material';
import { useAuthStore } from '../stores/authStore';
import { apiClient } from '../services/apiClient';

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
            id={`transfer-tabpanel-${index}`}
            aria-labelledby={`transfer-tab-${index}`}
            {...other}
        >
            {value === index && <Box sx={{ py: 3 }}>{children}</Box>}
        </div>
    );
}

interface TransferRequest {
    id: number;
    transferNumber: string;
    fromWarehouseId: number;
    fromWarehouseName: string;
    fromWarehouseNameAr: string;
    toWarehouseId: number;
    toWarehouseName: string;
    toWarehouseNameAr: string;
    itemId: number;
    itemCode: string;
    itemName: string;
    itemNameAr: string;
    quantity: number;
    unit: string;
    reason: string;
    priority: 'LOW' | 'MEDIUM' | 'HIGH' | 'URGENT';
    status: 'PENDING' | 'APPROVED' | 'REJECTED' | 'IN_TRANSIT' | 'COMPLETED' | 'CANCELLED';
    requestedBy: string;
    requestedDate: string;
    approvedBy?: string;
    approvedDate?: string;
    rejectionReason?: string;
    completedBy?: string;
    completedDate?: string;
    notes?: string;
}

interface Warehouse {
    id: number;
    name: string;
    nameAr: string;
    type: 'CENTRAL' | 'FACTORY';
    factoryId?: number;
}

interface Item {
    id: number;
    code: string;
    name: string;
    nameAr: string;
    unit: string;
}

const Transfers: React.FC = () => {
    const { user } = useAuthStore();
    const [tabValue, setTabValue] = useState(0);
    const [loading, setLoading] = useState(true);
    const [transfers, setTransfers] = useState<TransferRequest[]>([]);
    const [warehouses, setWarehouses] = useState<Warehouse[]>([]);
    const [items, setItems] = useState<Item[]>([]);
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState<string | null>(null);

    // Dialog states
    const [createDialogOpen, setCreateDialogOpen] = useState(false);
    const [viewDialogOpen, setViewDialogOpen] = useState(false);
    const [selectedTransfer, setSelectedTransfer] = useState<TransferRequest | null>(null);

    // Form state
    const [formData, setFormData] = useState({
        fromWarehouseId: '',
        toWarehouseId: '',
        itemId: '',
        quantity: '',
        reason: '',
        priority: 'MEDIUM' as 'LOW' | 'MEDIUM' | 'HIGH' | 'URGENT',
    });

    useEffect(() => {
        loadData();
    }, []);

    const loadData = async () => {
        setLoading(true);
        setError(null);
        try {
            const [transfersRes, warehousesRes, itemsRes] = await Promise.all([
                apiClient.get<any>('/api/transfers'),
                apiClient.get<any>('/api/warehouses'),
                apiClient.get<any>('/api/items'),
            ]);

            // Handle both array and wrapped object responses
            const extractData = (res: any) => Array.isArray(res.data) ? res.data : (res.data?.data || []);
            setTransfers(extractData(transfersRes));
            setWarehouses(extractData(warehousesRes));
            setItems(extractData(itemsRes));
        } catch (err: any) {
            setError(err.response?.data?.message || 'Failed to load transfer data');
        } finally {
            setLoading(false);
        }
    };

    const handleTabChange = (event: React.SyntheticEvent, newValue: number) => {
        setTabValue(newValue);
    };

    const handleCreateClick = () => {
        setFormData({
            fromWarehouseId: '',
            toWarehouseId: '',
            itemId: '',
            quantity: '',
            reason: '',
            priority: 'MEDIUM',
        });
        setCreateDialogOpen(true);
    };

    const handleViewClick = (transfer: TransferRequest) => {
        setSelectedTransfer(transfer);
        setViewDialogOpen(true);
    };

    const handleCreateSubmit = async () => {
        try {
            await apiClient.post('/api/transfers', {
                fromWarehouseId: parseInt(formData.fromWarehouseId),
                toWarehouseId: parseInt(formData.toWarehouseId),
                itemId: parseInt(formData.itemId),
                quantity: parseFloat(formData.quantity),
                reason: formData.reason,
                priority: formData.priority,
            });
            setSuccess('Transfer request created successfully');
            setCreateDialogOpen(false);
            loadData();
        } catch (err: any) {
            setError(err.response?.data?.message || 'Failed to create transfer request');
        }
    };

    const handleApprove = async (transferId: number) => {
        try {
            await apiClient.post(`/api/transfers/${transferId}/approve`);
            setSuccess('Transfer approved successfully');
            loadData();
        } catch (err: any) {
            setError(err.response?.data?.message || 'Failed to approve transfer');
        }
    };

    const handleReject = async (transferId: number, reason: string) => {
        try {
            await apiClient.post(`/api/transfers/${transferId}/reject`, { reason });
            setSuccess('Transfer rejected');
            loadData();
        } catch (err: any) {
            setError(err.response?.data?.message || 'Failed to reject transfer');
        }
    };

    const handleStartTransit = async (transferId: number) => {
        try {
            await apiClient.post(`/api/transfers/${transferId}/start-transit`);
            setSuccess('Transfer marked as in transit');
            loadData();
        } catch (err: any) {
            setError(err.response?.data?.message || 'Failed to start transit');
        }
    };

    const handleComplete = async (transferId: number) => {
        try {
            await apiClient.post(`/api/transfers/${transferId}/complete`);
            setSuccess('Transfer completed successfully');
            loadData();
        } catch (err: any) {
            setError(err.response?.data?.message || 'Failed to complete transfer');
        }
    };

    const handleCancel = async (transferId: number) => {
        try {
            await apiClient.post(`/api/transfers/${transferId}/cancel`);
            setSuccess('Transfer cancelled');
            loadData();
        } catch (err: any) {
            setError(err.response?.data?.message || 'Failed to cancel transfer');
        }
    };

    const getStatusColor = (status: string) => {
        switch (status) {
            case 'COMPLETED':
                return 'success';
            case 'APPROVED':
                return 'info';
            case 'IN_TRANSIT':
                return 'primary';
            case 'PENDING':
                return 'warning';
            case 'REJECTED':
            case 'CANCELLED':
                return 'error';
            default:
                return 'default';
        }
    };

    const getStatusIcon = (status: string): React.ReactElement | undefined => {
        switch (status) {
            case 'COMPLETED':
                return <CheckCircleIcon />;
            case 'PENDING':
                return <PendingIcon />;
            case 'REJECTED':
            case 'CANCELLED':
                return <CancelIcon />;
            default:
                return undefined;
        }
    };

    const getPriorityColor = (priority: string) => {
        switch (priority) {
            case 'URGENT':
                return 'error';
            case 'HIGH':
                return 'warning';
            case 'MEDIUM':
                return 'info';
            case 'LOW':
                return 'default';
            default:
                return 'default';
        }
    };

    const canCreateTransfer = user?.role !== 'Worker' && user?.role !== 'Auditor';
    const canApproveTransfer = user?.role === 'FactoryCommander' || user?.role === 'ComplexCommander' || user?.role === 'CentralWarehouseKeeper';
    const canProcessTransfer = user?.role === 'FactoryWarehouseKeeper' || user?.role === 'CentralWarehouseKeeper';

    const filteredTransfers = (transfers || []).filter((transfer) => {
        if (tabValue === 0) return true; // All
        if (tabValue === 1) return transfer.status === 'PENDING';
        if (tabValue === 2) return transfer.status === 'APPROVED' || transfer.status === 'IN_TRANSIT';
        if (tabValue === 3) return transfer.status === 'COMPLETED';
        return true;
    });

    if (loading) {
        return (
            <Box display="flex" justifyContent="center" alignItems="center" minHeight="60vh">
                <CircularProgress />
            </Box>
        );
    }

    return (
        <Container maxWidth="xl">
            <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
                <Box>
                    <Typography variant="h4" gutterBottom>
                        عمليات النقل
                    </Typography>
                    <Typography variant="body1" color="textSecondary">
                        Material Transfers
                    </Typography>
                </Box>
                <Box display="flex" gap={2}>
                    <Tooltip title="Refresh">
                        <IconButton onClick={loadData} color="primary">
                            <RefreshIcon />
                        </IconButton>
                    </Tooltip>
                    {canCreateTransfer && (
                        <Button
                            variant="contained"
                            startIcon={<AddIcon />}
                            onClick={handleCreateClick}
                        >
                            New Transfer
                        </Button>
                    )}
                </Box>
            </Box>

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

            <Box sx={{ borderBottom: 1, borderColor: 'divider', mb: 3 }}>
                <Tabs value={tabValue} onChange={handleTabChange} aria-label="Transfer Tabs">
                    <Tab label="All / الكل" id="transfer-tab-0" aria-controls="transfer-tabpanel-0" />
                    <Tab label="Pending / معلق" id="transfer-tab-1" aria-controls="transfer-tabpanel-1" />
                    <Tab label="In Progress / قيد التنفيذ" id="transfer-tab-2" aria-controls="transfer-tabpanel-2" />
                    <Tab label="Completed / مكتمل" id="transfer-tab-3" aria-controls="transfer-tabpanel-3" />
                </Tabs>
            </Box>

            <TabPanel value={tabValue} index={0}>
                <TransferTable
                    transfers={filteredTransfers}
                    onView={handleViewClick}
                    onApprove={canApproveTransfer ? handleApprove : undefined}
                    onReject={canApproveTransfer ? handleReject : undefined}
                    onStartTransit={canProcessTransfer ? handleStartTransit : undefined}
                    onComplete={canProcessTransfer ? handleComplete : undefined}
                    onCancel={canProcessTransfer ? handleCancel : undefined}
                    getStatusColor={getStatusColor}
                    getStatusIcon={getStatusIcon}
                    getPriorityColor={getPriorityColor}
                />
            </TabPanel>

            <TabPanel value={tabValue} index={1}>
                <TransferTable
                    transfers={filteredTransfers}
                    onView={handleViewClick}
                    onApprove={canApproveTransfer ? handleApprove : undefined}
                    onReject={canApproveTransfer ? handleReject : undefined}
                    onStartTransit={canProcessTransfer ? handleStartTransit : undefined}
                    onComplete={canProcessTransfer ? handleComplete : undefined}
                    onCancel={canProcessTransfer ? handleCancel : undefined}
                    getStatusColor={getStatusColor}
                    getStatusIcon={getStatusIcon}
                    getPriorityColor={getPriorityColor}
                />
            </TabPanel>

            <TabPanel value={tabValue} index={2}>
                <TransferTable
                    transfers={filteredTransfers}
                    onView={handleViewClick}
                    onApprove={canApproveTransfer ? handleApprove : undefined}
                    onReject={canApproveTransfer ? handleReject : undefined}
                    onStartTransit={canProcessTransfer ? handleStartTransit : undefined}
                    onComplete={canProcessTransfer ? handleComplete : undefined}
                    onCancel={canProcessTransfer ? handleCancel : undefined}
                    getStatusColor={getStatusColor}
                    getStatusIcon={getStatusIcon}
                    getPriorityColor={getPriorityColor}
                />
            </TabPanel>

            <TabPanel value={tabValue} index={3}>
                <TransferTable
                    transfers={filteredTransfers}
                    onView={handleViewClick}
                    onApprove={canApproveTransfer ? handleApprove : undefined}
                    onReject={canApproveTransfer ? handleReject : undefined}
                    onStartTransit={canProcessTransfer ? handleStartTransit : undefined}
                    onComplete={canProcessTransfer ? handleComplete : undefined}
                    onCancel={canProcessTransfer ? handleCancel : undefined}
                    getStatusColor={getStatusColor}
                    getStatusIcon={getStatusIcon}
                    getPriorityColor={getPriorityColor}
                />
            </TabPanel>

            {/* Create Transfer Dialog */}
            <Dialog open={createDialogOpen} onClose={() => setCreateDialogOpen(false)} maxWidth="md" fullWidth>
                <DialogTitle>Create New Transfer Request</DialogTitle>
                <DialogContent>
                    <Grid container spacing={2} sx={{ mt: 1 }}>
                        <Grid item xs={12} sm={6}>
                            <FormControl fullWidth>
                                <InputLabel>From Warehouse</InputLabel>
                                <Select
                                    value={formData.fromWarehouseId}
                                    label="From Warehouse"
                                    onChange={(e) => setFormData({ ...formData, fromWarehouseId: e.target.value })}
                                >
                                    {warehouses.map((wh) => (
                                        <MenuItem key={wh.id} value={wh.id}>
                                            {wh.nameAr} ({wh.name})
                                        </MenuItem>
                                    ))}
                                </Select>
                            </FormControl>
                        </Grid>
                        <Grid item xs={12} sm={6}>
                            <FormControl fullWidth>
                                <InputLabel>To Warehouse</InputLabel>
                                <Select
                                    value={formData.toWarehouseId}
                                    label="To Warehouse"
                                    onChange={(e) => setFormData({ ...formData, toWarehouseId: e.target.value })}
                                >
                                    {warehouses.map((wh) => (
                                        <MenuItem key={wh.id} value={wh.id}>
                                            {wh.nameAr} ({wh.name})
                                        </MenuItem>
                                    ))}
                                </Select>
                            </FormControl>
                        </Grid>
                        <Grid item xs={12}>
                            <Autocomplete
                                options={items}
                                getOptionLabel={(option) => `${option.code} - ${option.nameAr}`}
                                renderInput={(params) => (
                                    <TextField {...params} label="Item" fullWidth />
                                )}
                                onChange={(_, value) => setFormData({ ...formData, itemId: value?.id.toString() || '' })}
                            />
                        </Grid>
                        <Grid item xs={12} sm={6}>
                            <TextField
                                fullWidth
                                label="Quantity"
                                type="number"
                                value={formData.quantity}
                                onChange={(e) => setFormData({ ...formData, quantity: e.target.value })}
                            />
                        </Grid>
                        <Grid item xs={12} sm={6}>
                            <FormControl fullWidth>
                                <InputLabel>Priority</InputLabel>
                                <Select
                                    value={formData.priority}
                                    label="Priority"
                                    onChange={(e) => setFormData({ ...formData, priority: e.target.value as any })}
                                >
                                    <MenuItem value="LOW">Low</MenuItem>
                                    <MenuItem value="MEDIUM">Medium</MenuItem>
                                    <MenuItem value="HIGH">High</MenuItem>
                                    <MenuItem value="URGENT">Urgent</MenuItem>
                                </Select>
                            </FormControl>
                        </Grid>
                        <Grid item xs={12}>
                            <TextField
                                fullWidth
                                label="Reason"
                                multiline
                                rows={3}
                                value={formData.reason}
                                onChange={(e) => setFormData({ ...formData, reason: e.target.value })}
                            />
                        </Grid>
                    </Grid>
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setCreateDialogOpen(false)}>Cancel</Button>
                    <Button onClick={handleCreateSubmit} variant="contained">
                        Create Transfer
                    </Button>
                </DialogActions>
            </Dialog>

            {/* View Transfer Dialog */}
            <Dialog open={viewDialogOpen} onClose={() => setViewDialogOpen(false)} maxWidth="md" fullWidth>
                <DialogTitle>Transfer Details</DialogTitle>
                <DialogContent>
                    {selectedTransfer && (
                        <Grid container spacing={2} sx={{ mt: 1 }}>
                            <Grid item xs={12} sm={6}>
                                <Typography variant="body2" color="textSecondary">
                                    Transfer Number
                                </Typography>
                                <Typography variant="body1">{selectedTransfer.transferNumber}</Typography>
                            </Grid>
                            <Grid item xs={12} sm={6}>
                                <Typography variant="body2" color="textSecondary">
                                    Status
                                </Typography>
                                <Chip
                                    label={selectedTransfer.status}
                                    color={getStatusColor(selectedTransfer.status) as any}
                                    size="small"
                                    icon={getStatusIcon(selectedTransfer.status)}
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <Box display="flex" alignItems="center" gap={2}>
                                    <WarehouseIcon />
                                    <Typography variant="body1">
                                        {selectedTransfer.fromWarehouseNameAr}
                                    </Typography>
                                    <ArrowForwardIcon />
                                    <FactoryIcon />
                                    <Typography variant="body1">
                                        {selectedTransfer.toWarehouseNameAr}
                                    </Typography>
                                </Box>
                            </Grid>
                            <Grid item xs={12}>
                                <Typography variant="body2" color="textSecondary">
                                    Item
                                </Typography>
                                <Typography variant="body1">
                                    {selectedTransfer.itemNameAr} ({selectedTransfer.itemCode})
                                </Typography>
                            </Grid>
                            <Grid item xs={12} sm={6}>
                                <Typography variant="body2" color="textSecondary">
                                    Quantity
                                </Typography>
                                <Typography variant="body1">
                                    {selectedTransfer.quantity.toLocaleString()} {selectedTransfer.unit}
                                </Typography>
                            </Grid>
                            <Grid item xs={12} sm={6}>
                                <Typography variant="body2" color="textSecondary">
                                    Priority
                                </Typography>
                                <Chip
                                    label={selectedTransfer.priority}
                                    color={getPriorityColor(selectedTransfer.priority) as any}
                                    size="small"
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <Typography variant="body2" color="textSecondary">
                                    Reason
                                </Typography>
                                <Typography variant="body1">{selectedTransfer.reason}</Typography>
                            </Grid>
                            <Grid item xs={12} sm={6}>
                                <Typography variant="body2" color="textSecondary">
                                    Requested By
                                </Typography>
                                <Typography variant="body1">{selectedTransfer.requestedBy}</Typography>
                            </Grid>
                            <Grid item xs={12} sm={6}>
                                <Typography variant="body2" color="textSecondary">
                                    Requested Date
                                </Typography>
                                <Typography variant="body1">
                                    {new Date(selectedTransfer.requestedDate).toLocaleString('en-GB')}
                                </Typography>
                            </Grid>
                            {selectedTransfer.approvedBy && (
                                <>
                                    <Grid item xs={12} sm={6}>
                                        <Typography variant="body2" color="textSecondary">
                                            Approved By
                                        </Typography>
                                        <Typography variant="body1">{selectedTransfer.approvedBy}</Typography>
                                    </Grid>
                                    <Grid item xs={12} sm={6}>
                                        <Typography variant="body2" color="textSecondary">
                                            Approved Date
                                        </Typography>
                                        <Typography variant="body1">
                                            {selectedTransfer.approvedDate && new Date(selectedTransfer.approvedDate).toLocaleString('en-GB')}
                                        </Typography>
                                    </Grid>
                                </>
                            )}
                            {selectedTransfer.rejectionReason && (
                                <Grid item xs={12}>
                                    <Typography variant="body2" color="textSecondary">
                                        Rejection Reason
                                    </Typography>
                                    <Typography variant="body1" color="error">
                                        {selectedTransfer.rejectionReason}
                                    </Typography>
                                </Grid>
                            )}
                            {selectedTransfer.notes && (
                                <Grid item xs={12}>
                                    <Typography variant="body2" color="textSecondary">
                                        Notes
                                    </Typography>
                                    <Typography variant="body1">{selectedTransfer.notes}</Typography>
                                </Grid>
                            )}
                        </Grid>
                    )}
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setViewDialogOpen(false)}>Close</Button>
                </DialogActions>
            </Dialog>
        </Container>
    );
};

interface TransferTableProps {
    transfers: TransferRequest[];
    onView: (transfer: TransferRequest) => void;
    onApprove?: (id: number) => void;
    onReject?: (id: number, reason: string) => void;
    onStartTransit?: (id: number) => void;
    onComplete?: (id: number) => void;
    onCancel?: (id: number) => void;
    getStatusColor: (status: string) => string;
    getStatusIcon: (status: string) => React.ReactNode;
    getPriorityColor: (priority: string) => string;
}

const TransferTable: React.FC<TransferTableProps> = ({
    transfers,
    onView,
    onApprove,
    onReject,
    onStartTransit,
    onComplete,
    onCancel,
    getStatusColor,
    getStatusIcon,
    getPriorityColor,
}) => {
    const [rejectDialogOpen, setRejectDialogOpen] = useState(false);
    const [selectedTransfer, setSelectedTransfer] = useState<TransferRequest | null>(null);
    const [rejectionReason, setRejectionReason] = useState('');

    const handleRejectClick = (transfer: TransferRequest) => {
        setSelectedTransfer(transfer);
        setRejectionReason('');
        setRejectDialogOpen(true);
    };

    const handleRejectConfirm = () => {
        if (selectedTransfer && onReject) {
            onReject(selectedTransfer.id, rejectionReason);
            setRejectDialogOpen(false);
            setSelectedTransfer(null);
            setRejectionReason('');
        }
    };

    return (
        <>
            <TableContainer component={Paper}>
                <Table>
                    <TableHead>
                        <TableRow>
                            <TableCell>Transfer #</TableCell>
                            <TableCell>From</TableCell>
                            <TableCell>To</TableCell>
                            <TableCell>Item</TableCell>
                            <TableCell align="right">Quantity</TableCell>
                            <TableCell>Priority</TableCell>
                            <TableCell>Status</TableCell>
                            <TableCell>Requested By</TableCell>
                            <TableCell>Date</TableCell>
                            <TableCell align="center">Actions</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {transfers.length === 0 ? (
                            <TableRow>
                                <TableCell colSpan={10} align="center">
                                    <Typography variant="body2" color="textSecondary">
                                        No transfers found
                                    </Typography>
                                </TableCell>
                            </TableRow>
                        ) : (
                            transfers.map((transfer) => (
                                <TableRow key={transfer.id} hover>
                                    <TableCell>{transfer.transferNumber}</TableCell>
                                    <TableCell>
                                        <Box display="flex" alignItems="center" gap={1}>
                                            <WarehouseIcon fontSize="small" />
                                            <Typography variant="body2">
                                                {transfer.fromWarehouseNameAr}
                                            </Typography>
                                        </Box>
                                    </TableCell>
                                    <TableCell>
                                        <Box display="flex" alignItems="center" gap={1}>
                                            <FactoryIcon fontSize="small" />
                                            <Typography variant="body2">
                                                {transfer.toWarehouseNameAr}
                                            </Typography>
                                        </Box>
                                    </TableCell>
                                    <TableCell>
                                        <Box>
                                            <Typography variant="body2">{transfer.itemNameAr}</Typography>
                                            <Typography variant="caption" color="textSecondary">
                                                {transfer.itemCode}
                                            </Typography>
                                        </Box>
                                    </TableCell>
                                    <TableCell align="right">
                                        {transfer.quantity.toLocaleString()} {transfer.unit}
                                    </TableCell>
                                    <TableCell>
                                        <Chip
                                            label={transfer.priority}
                                            color={getPriorityColor(transfer.priority) as any}
                                            size="small"
                                        />
                                    </TableCell>
                                    <TableCell>
                                        <Chip
                                            label={transfer.status}
                                            color={getStatusColor(transfer.status) as any}
                                            size="small"
                                        />
                                    </TableCell>
                                    <TableCell>{transfer.requestedBy}</TableCell>
                                    <TableCell>
                                        {new Date(transfer.requestedDate).toLocaleDateString('en-GB')}
                                    </TableCell>
                                    <TableCell align="center">
                                        <Box display="flex" gap={1} justifyContent="center">
                                            <Tooltip title="View Details">
                                                <IconButton size="small" onClick={() => onView(transfer)}>
                                                    <LocalShippingIcon />
                                                </IconButton>
                                            </Tooltip>
                                            {transfer.status === 'PENDING' && onApprove && (
                                                <Tooltip title="Approve">
                                                    <IconButton
                                                        size="small"
                                                        color="success"
                                                        onClick={() => onApprove(transfer.id)}
                                                    >
                                                        <CheckCircleIcon />
                                                    </IconButton>
                                                </Tooltip>
                                            )}
                                            {transfer.status === 'PENDING' && onReject && (
                                                <Tooltip title="Reject">
                                                    <IconButton
                                                        size="small"
                                                        color="error"
                                                        onClick={() => handleRejectClick(transfer)}
                                                    >
                                                        <CancelIcon />
                                                    </IconButton>
                                                </Tooltip>
                                            )}
                                            {transfer.status === 'APPROVED' && onStartTransit && (
                                                <Tooltip title="Start Transit">
                                                    <IconButton
                                                        size="small"
                                                        color="primary"
                                                        onClick={() => onStartTransit(transfer.id)}
                                                    >
                                                        <LocalShippingIcon />
                                                    </IconButton>
                                                </Tooltip>
                                            )}
                                            {transfer.status === 'IN_TRANSIT' && onComplete && (
                                                <Tooltip title="Complete">
                                                    <IconButton
                                                        size="small"
                                                        color="success"
                                                        onClick={() => onComplete(transfer.id)}
                                                    >
                                                        <CheckCircleIcon />
                                                    </IconButton>
                                                </Tooltip>
                                            )}
                                            {(transfer.status === 'PENDING' || transfer.status === 'APPROVED') && onCancel && (
                                                <Tooltip title="Cancel">
                                                    <IconButton
                                                        size="small"
                                                        color="error"
                                                        onClick={() => onCancel(transfer.id)}
                                                    >
                                                        <CancelIcon />
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

            {/* Reject Dialog */}
            <Dialog open={rejectDialogOpen} onClose={() => setRejectDialogOpen(false)} maxWidth="sm" fullWidth>
                <DialogTitle>Reject Transfer</DialogTitle>
                <DialogContent>
                    {selectedTransfer && (
                        <Box>
                            <Typography variant="body2" gutterBottom>
                                <strong>Transfer #:</strong> {selectedTransfer.transferNumber}
                            </Typography>
                            <Typography variant="body2" gutterBottom>
                                <strong>Item:</strong> {selectedTransfer.itemNameAr}
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
                                placeholder="Please provide a reason for rejection..."
                                required
                            />
                        </Box>
                    )}
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setRejectDialogOpen(false)}>Cancel</Button>
                    <Button onClick={handleRejectConfirm} variant="contained" color="error">
                        Reject
                    </Button>
                </DialogActions>
            </Dialog>
        </>
    );
};

export default Transfers;
