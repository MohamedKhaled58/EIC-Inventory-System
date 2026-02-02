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

interface TransferItem {
    id: number;
    itemId: number;
    itemCode: string;
    itemName: string;
    itemNameAr: string;
    unit: string;
    requestedQuantity: number;
    approvedQuantity: number;
    shippedQuantity: number;
    receivedQuantity: number;
    notes?: string;
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
    items: TransferItem[];
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
    const [formData, setFormData] = useState<{
        fromWarehouseId: string;
        toWarehouseId: string;
        items: { itemId: string; quantity: string }[];
        reason: string;
        priority: 'LOW' | 'MEDIUM' | 'HIGH' | 'URGENT';
    }>({
        fromWarehouseId: '',
        toWarehouseId: '',
        items: [{ itemId: '', quantity: '' }],
        reason: '',
        priority: 'MEDIUM',
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
            items: [{ itemId: '', quantity: '' }],
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
            if (!formData.fromWarehouseId || !formData.toWarehouseId || formData.items.some(i => !i.itemId || !i.quantity)) {
                setError('Please fill in all required fields');
                return;
            }

            const payload = {
                fromWarehouseId: parseInt(formData.fromWarehouseId),
                toWarehouseId: parseInt(formData.toWarehouseId),
                items: formData.items.map(item => ({
                    itemId: parseInt(item.itemId),
                    requestedQuantity: parseFloat(item.quantity),
                    fromCommanderReserve: false,
                    toCommanderReserve: false,
                })),
                notes: formData.reason,
            };

            await apiClient.post('/api/transfers', payload);
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
            {/* Header */}
            <Box display="flex" justifyContent="space-between" alignItems="center" mb={3} mt={3}>
                <Box>
                    <Typography variant="h4" gutterBottom>
                        عمليات النقل
                    </Typography>
                    <Typography variant="body1" color="textSecondary">
                        Material Transfers
                    </Typography>
                </Box>
                <Box display="flex" gap={2}>
                    <Tooltip title="تحديث">
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
                            طلب نقل جديد
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
            <Dialog open={createDialogOpen} onClose={() => setCreateDialogOpen(false)} maxWidth="lg" fullWidth>
                <DialogTitle>إنشاء طلب نقل جديد</DialogTitle>
                <DialogContent>
                    <Grid container spacing={2} sx={{ mt: 1 }}>
                        <Grid item xs={12} sm={6}>
                            <FormControl fullWidth>
                                <InputLabel>من مستودع</InputLabel>
                                <Select
                                    value={formData.fromWarehouseId}
                                    label="من مستودع"
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
                                <InputLabel>إلى مستودع</InputLabel>
                                <Select
                                    value={formData.toWarehouseId}
                                    label="إلى مستودع"
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
                        <Grid item xs={12} sm={6}>
                            <FormControl fullWidth>
                                <InputLabel>الأولوية</InputLabel>
                                <Select
                                    value={formData.priority}
                                    label="الأولوية"
                                    onChange={(e) => setFormData({ ...formData, priority: e.target.value as any })}
                                >
                                    <MenuItem value="LOW">منخفض</MenuItem>
                                    <MenuItem value="MEDIUM">متوسط</MenuItem>
                                    <MenuItem value="HIGH">عالي</MenuItem>
                                    <MenuItem value="URGENT">حرج</MenuItem>
                                </Select>
                            </FormControl>
                        </Grid>
                        <Grid item xs={12} sm={6}>
                            <TextField
                                fullWidth
                                label="سبب النقل"
                                value={formData.reason}
                                onChange={(e) => setFormData({ ...formData, reason: e.target.value })}
                            />
                        </Grid>

                        <Grid item xs={12}>
                            <Typography variant="h6" gutterBottom sx={{ mt: 2 }}>
                                الأصناف
                            </Typography>
                            {formData.items.map((item, index) => (
                                <Box key={index} sx={{ mb: 2, p: 2, border: '1px solid #e0e0e0', borderRadius: 1, position: 'relative' }}>
                                    <Grid container spacing={2} alignItems="center">
                                        <Grid item xs={12} sm={6}>
                                            <Autocomplete
                                                options={items}
                                                getOptionLabel={(option) => `${option.code} - ${option.nameAr}`}
                                                renderInput={(params) => (
                                                    <TextField {...params} label="الصنف" fullWidth />
                                                )}
                                                value={items.find(i => i.id.toString() === item.itemId) || null}
                                                onChange={(_, value) => {
                                                    const newItems = [...formData.items];
                                                    newItems[index].itemId = value?.id.toString() || '';
                                                    setFormData({ ...formData, items: newItems });
                                                }}
                                            />
                                        </Grid>
                                        <Grid item xs={12} sm={4}>
                                            <TextField
                                                fullWidth
                                                label="الكمية"
                                                type="number"
                                                value={item.quantity}
                                                onChange={(e) => {
                                                    const newItems = [...formData.items];
                                                    newItems[index].quantity = e.target.value;
                                                    setFormData({ ...formData, items: newItems });
                                                }}
                                            />
                                        </Grid>
                                        <Grid item xs={12} sm={2}>
                                            <IconButton
                                                color="error"
                                                onClick={() => {
                                                    const newItems = formData.items.filter((_, i) => i !== index);
                                                    setFormData({ ...formData, items: newItems });
                                                }}
                                                disabled={formData.items.length === 1}
                                            >
                                                <CancelIcon />
                                            </IconButton>
                                        </Grid>
                                    </Grid>
                                </Box>
                            ))}
                            <Button
                                startIcon={<AddIcon />}
                                onClick={() => setFormData({ ...formData, items: [...formData.items, { itemId: '', quantity: '' }] })}
                            >
                                إضافة صنف
                            </Button>
                        </Grid>
                    </Grid>
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setCreateDialogOpen(false)}>إلغاء</Button>
                    <Button onClick={handleCreateSubmit} variant="contained">
                        إنشاء الطلب
                    </Button>
                </DialogActions>
            </Dialog>

            {/* View Transfer Dialog */}
            <Dialog open={viewDialogOpen} onClose={() => setViewDialogOpen(false)} maxWidth="md" fullWidth>
                <DialogTitle>تفاصيل النقل</DialogTitle>
                <DialogContent>
                    {selectedTransfer && (
                        <Grid container spacing={2} sx={{ mt: 1 }}>
                            <Grid item xs={12} sm={6}>
                                <Typography variant="body2" color="textSecondary">
                                    رقم النقل
                                </Typography>
                                <Typography variant="body1">{selectedTransfer.transferNumber}</Typography>
                            </Grid>
                            <Grid item xs={12} sm={6}>
                                <Typography variant="body2" color="textSecondary">
                                    الحالة
                                </Typography>
                                <Chip
                                    label={selectedTransfer.status === 'COMPLETED' ? 'مكتمل' : selectedTransfer.status === 'APPROVED' ? 'معتمد' : selectedTransfer.status === 'IN_TRANSIT' ? 'جاري النقل' : selectedTransfer.status === 'PENDING' ? 'معلق' : selectedTransfer.status === 'REJECTED' ? 'مرفوض' : 'ملغي'}
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
                                <Typography variant="h6" gutterBottom sx={{ mt: 2 }}>
                                    الأصناف ({selectedTransfer.items.length})
                                </Typography>
                                <TableContainer component={Paper} variant="outlined">
                                    <Table size="small">
                                        <TableHead>
                                            <TableRow>
                                                <TableCell>الصنف</TableCell>
                                                <TableCell align="right">الكمية المطلوبة</TableCell>
                                                <TableCell align="right">الكمية المشحونة</TableCell>
                                                <TableCell align="right">الكمية المستلمة</TableCell>
                                            </TableRow>
                                        </TableHead>
                                        <TableBody>
                                            {selectedTransfer.items.map((item) => (
                                                <TableRow key={item.id}>
                                                    <TableCell>
                                                        <Box>
                                                            <Typography variant="body2">{item.itemNameAr}</Typography>
                                                            <Typography variant="caption" color="textSecondary">{item.itemCode}</Typography>
                                                        </Box>
                                                    </TableCell>
                                                    <TableCell align="right">{item.requestedQuantity} {item.unit}</TableCell>
                                                    <TableCell align="right">{item.shippedQuantity} {item.unit}</TableCell>
                                                    <TableCell align="right">{item.receivedQuantity} {item.unit}</TableCell>
                                                </TableRow>
                                            ))}
                                        </TableBody>
                                    </Table>
                                </TableContainer>
                            </Grid>

                            <Grid item xs={12} sm={6}>
                                <Typography variant="body2" color="textSecondary">
                                    الأولوية
                                </Typography>
                                <Chip
                                    label={selectedTransfer.priority === 'URGENT' ? 'حرج' : selectedTransfer.priority === 'HIGH' ? 'عالي' : selectedTransfer.priority === 'MEDIUM' ? 'متوسط' : 'منخفض'}
                                    color={getPriorityColor(selectedTransfer.priority) as any}
                                    size="small"
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <Typography variant="body2" color="textSecondary">
                                    سبب النقل
                                </Typography>
                                <Typography variant="body1">{selectedTransfer.reason}</Typography>
                            </Grid>
                            <Grid item xs={12} sm={6}>
                                <Typography variant="body2" color="textSecondary">
                                    طالب النقل
                                </Typography>
                                <Typography variant="body1">{selectedTransfer.requestedBy}</Typography>
                            </Grid>
                            <Grid item xs={12} sm={6}>
                                <Typography variant="body2" color="textSecondary">
                                    تاريخ الطلب
                                </Typography>
                                <Typography variant="body1">
                                    {new Date(selectedTransfer.requestedDate).toLocaleString('en-GB')}
                                </Typography>
                            </Grid>
                            {selectedTransfer.approvedBy && (
                                <>
                                    <Grid item xs={12} sm={6}>
                                        <Typography variant="body2" color="textSecondary">
                                            تم الاعتماد بواسطة
                                        </Typography>
                                        <Typography variant="body1">{selectedTransfer.approvedBy}</Typography>
                                    </Grid>
                                    <Grid item xs={12} sm={6}>
                                        <Typography variant="body2" color="textSecondary">
                                            تاريخ الاعتماد
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
                                        سبب الرفض
                                    </Typography>
                                    <Typography variant="body1" color="error">
                                        {selectedTransfer.rejectionReason}
                                    </Typography>
                                </Grid>
                            )}
                            {selectedTransfer.notes && (
                                <Grid item xs={12}>
                                    <Typography variant="body2" color="textSecondary">
                                        ملاحظات
                                    </Typography>
                                    <Typography variant="body1">{selectedTransfer.notes}</Typography>
                                </Grid>
                            )}
                        </Grid>
                    )}
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setViewDialogOpen(false)}>إغلاق</Button>
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
                            <TableCell>رقم النقل</TableCell>
                            <TableCell>من</TableCell>
                            <TableCell>إلى</TableCell>
                            <TableCell>عدد الأصناف</TableCell>
                            <TableCell>الأولوية</TableCell>
                            <TableCell>الحالة</TableCell>
                            <TableCell>طالب النقل</TableCell>
                            <TableCell>التاريخ</TableCell>
                            <TableCell align="center">الإجراءات</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {transfers.length === 0 ? (
                            <TableRow>
                                <TableCell colSpan={9} align="center">
                                    <Typography variant="body2" color="textSecondary">
                                        لا توجد عمليات نقل
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
                                        {transfer.items.length} أصناف
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
                <DialogTitle>رفض النقل</DialogTitle>
                <DialogContent>
                    {selectedTransfer && (
                        <Box>
                            <Typography variant="body2" gutterBottom>
                                <strong>رقم النقل:</strong> {selectedTransfer.transferNumber}
                            </Typography>
                        </Box>
                    )}
                    <TextField
                        autoFocus
                        margin="dense"
                        label="سبب الرفض"
                        fullWidth
                        multiline
                        rows={3}
                        value={rejectionReason}
                        onChange={(e) => setRejectionReason(e.target.value)}
                        required
                    />
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setRejectDialogOpen(false)}>إلغاء</Button>
                    <Button onClick={handleRejectConfirm} variant="contained" color="error" disabled={!rejectionReason}>
                        تأكيد الرفض
                    </Button>
                </DialogActions>
            </Dialog>
        </>
    );
};

export default Transfers;
