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
    Alert,
    CircularProgress,
    Tooltip,
    Card,
    CardContent,
    Divider,
} from '@mui/material';
import {
    Add as AddIcon,
    Refresh as RefreshIcon,
    Visibility as ViewIcon,
    KeyboardReturn as ReturnIcon,
    Delete as ConsumeIcon,
    SwapHoriz as TransferIcon,
    Warning as OverdueIcon,
    Person as WorkerIcon,
} from '@mui/icons-material';
import { useAuthStore } from '../stores/authStore';
import { custodyService } from '../services';
import {
    ItemAutocomplete,
    WorkerAutocomplete,
    WarehouseAutocomplete,
    FactoryAutocomplete,
    DepartmentAutocomplete,
} from '../components/common';
import type {
    OperationalCustody,
    CustodyStatus,
    CustodyStatistics,
    Worker,
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
            id={`custody-tabpanel-${index}`}
            aria-labelledby={`custody-tab-${index}`}
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

interface Department {
    id: number;
    name: string;
    nameAr: string;
    code: string;
    factoryId: number;
}

const OperationalCustodyPage: React.FC = () => {
    const { user } = useAuthStore();
    const [tabValue, setTabValue] = useState(0);
    const [loading, setLoading] = useState(true);
    const [custodies, setCustodies] = useState<OperationalCustody[]>([]);
    const [statistics, setStatistics] = useState<CustodyStatistics | null>(null);
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState<string | null>(null);

    // Dialog states
    const [issueDialogOpen, setIssueDialogOpen] = useState(false);
    const [viewDialogOpen, setViewDialogOpen] = useState(false);
    const [returnDialogOpen, setReturnDialogOpen] = useState(false);
    const [consumeDialogOpen, setConsumeDialogOpen] = useState(false);
    const [transferDialogOpen, setTransferDialogOpen] = useState(false);
    const [selectedCustody, setSelectedCustody] = useState<OperationalCustody | null>(null);

    // Issue form state
    const [issueForm, setIssueForm] = useState({
        worker: null as Worker | null,
        item: null as Item | null,
        warehouse: null as Warehouse | null,
        factory: null as Factory | null,
        department: null as Department | null,
        quantity: 1,
        purpose: '',
        purposeArabic: '',
        custodyLimit: 30,
        notes: '',
    });

    // Return/Consume/Transfer state
    const [returnQuantity, setReturnQuantity] = useState(0);
    const [consumeQuantity, setConsumeQuantity] = useState(0);
    const [transferWorker, setTransferWorker] = useState<Worker | null>(null);
    const [transferDepartment, setTransferDepartment] = useState<Department | null>(null);
    const [actionNotes, setActionNotes] = useState('');

    const loadData = useCallback(async () => {
        setLoading(true);
        setError(null);
        try {
            const factoryId = user?.factoryId;
            const departmentId = user?.departmentId;
            const [custodiesResponse, statsResponse] = await Promise.all([
                custodyService.getCustodies({ factoryId, departmentId }, 1, 100),
                custodyService.getStatistics(factoryId, departmentId),
            ]);
            setCustodies(custodiesResponse.items || []);
            setStatistics(statsResponse);
        } catch (err: unknown) {
            const message = err instanceof Error ? err.message : 'Failed to load custody data';
            setError(message);
        } finally {
            setLoading(false);
        }
    }, [user?.factoryId, user?.departmentId]);

    useEffect(() => {
        loadData();
    }, [loadData]);

    const handleTabChange = (_event: React.SyntheticEvent, newValue: number) => {
        setTabValue(newValue);
    };

    const handleIssueClick = () => {
        setIssueForm({
            worker: null,
            item: null,
            warehouse: null,
            factory: null,
            department: null,
            quantity: 1,
            purpose: '',
            purposeArabic: '',
            custodyLimit: 30,
            notes: '',
        });
        setIssueDialogOpen(true);
    };

    const handleViewClick = (custody: OperationalCustody) => {
        setSelectedCustody(custody);
        setViewDialogOpen(true);
    };

    const handleReturnClick = (custody: OperationalCustody) => {
        setSelectedCustody(custody);
        setReturnQuantity(custody.remainingQuantity);
        setActionNotes('');
        setReturnDialogOpen(true);
    };

    const handleConsumeClick = (custody: OperationalCustody) => {
        setSelectedCustody(custody);
        setConsumeQuantity(custody.remainingQuantity);
        setActionNotes('');
        setConsumeDialogOpen(true);
    };

    const handleTransferClick = (custody: OperationalCustody) => {
        setSelectedCustody(custody);
        setTransferWorker(null);
        setTransferDepartment(null);
        setActionNotes('');
        setTransferDialogOpen(true);
    };

    const handleIssueSubmit = async () => {
        if (!issueForm.worker || !issueForm.item || !issueForm.warehouse || !issueForm.factory || !issueForm.department) {
            setError('Please fill in all required fields');
            return;
        }

        try {
            await custodyService.issueCustody({
                workerId: issueForm.worker.id,
                itemId: issueForm.item.id,
                warehouseId: issueForm.warehouse.id,
                factoryId: issueForm.factory.id,
                departmentId: issueForm.department.id,
                quantity: issueForm.quantity,
                purpose: issueForm.purpose,
                purposeArabic: issueForm.purposeArabic,
                custodyLimit: issueForm.custodyLimit,
                notes: issueForm.notes,
            });
            setSuccess('Custody issued successfully');
            setIssueDialogOpen(false);
            loadData();
        } catch (err: unknown) {
            const message = err instanceof Error ? err.message : 'Failed to issue custody';
            setError(message);
        }
    };

    const handleReturnSubmit = async () => {
        if (!selectedCustody || returnQuantity <= 0) return;
        try {
            await custodyService.returnCustody({
                id: selectedCustody.id,
                returnQuantity: returnQuantity,
                notes: actionNotes,
            });
            setSuccess('Custody returned successfully');
            setReturnDialogOpen(false);
            loadData();
        } catch (err: unknown) {
            const message = err instanceof Error ? err.message : 'Failed to return custody';
            setError(message);
        }
    };

    const handleConsumeSubmit = async () => {
        if (!selectedCustody || consumeQuantity <= 0) return;
        try {
            await custodyService.consumeCustody({
                id: selectedCustody.id,
                consumeQuantity: consumeQuantity,
                notes: actionNotes,
            });
            setSuccess('Custody consumed successfully');
            setConsumeDialogOpen(false);
            loadData();
        } catch (err: unknown) {
            const message = err instanceof Error ? err.message : 'Failed to consume custody';
            setError(message);
        }
    };

    const handleTransferSubmit = async () => {
        if (!selectedCustody || !transferWorker || !transferDepartment) return;
        try {
            await custodyService.transferCustody({
                id: selectedCustody.id,
                newWorkerId: transferWorker.id,
                newDepartmentId: transferDepartment.id,
                notes: actionNotes,
            });
            setSuccess('Custody transferred successfully');
            setTransferDialogOpen(false);
            loadData();
        } catch (err: unknown) {
            const message = err instanceof Error ? err.message : 'Failed to transfer custody';
            setError(message);
        }
    };

    const getStatusColor = (status: CustodyStatus) => {
        switch (status) {
            case 'Active': return 'primary';
            case 'PartiallyReturned': return 'warning';
            case 'FullyReturned': return 'success';
            case 'Consumed': return 'info';
            case 'Transferred': return 'secondary';
            default: return 'default';
        }
    };

    const filteredCustodies = custodies.filter(custody => {
        if (tabValue === 0) return true;
        if (tabValue === 1) return custody.status === 'Active';
        if (tabValue === 2) return custody.isOverdue;
        if (tabValue === 3) return custody.status === 'FullyReturned' || custody.status === 'Consumed';
        return true;
    });

    const canIssue = user?.role !== 'Auditor' && user?.role !== 'Worker';
    const canProcess = user?.role !== 'Auditor';

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
                        العهد التشغيلية
                    </Typography>
                    <Typography variant="body1" color="textSecondary">
                        Operational Custody
                    </Typography>
                </Box>
                <Box display="flex" gap={2}>
                    <Tooltip title="Refresh">
                        <IconButton onClick={loadData} color="primary">
                            <RefreshIcon />
                        </IconButton>
                    </Tooltip>
                    {canIssue && (
                        <Button
                            variant="contained"
                            startIcon={<AddIcon />}
                            onClick={handleIssueClick}
                        >
                            اصدار عهده
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
                                <Typography variant="h4" color="primary">{statistics.totalActiveCustodies}</Typography>
                                <Typography variant="body2" color="textSecondary">Active Custodies</Typography>
                            </CardContent>
                        </Card>
                    </Grid>
                    <Grid item xs={12} sm={6} md={2}>
                        <Card>
                            <CardContent>
                                <Typography variant="h4" color="error.main">{statistics.totalOverdueCustodies}</Typography>
                                <Typography variant="body2" color="textSecondary">Overdue</Typography>
                            </CardContent>
                        </Card>
                    </Grid>
                    <Grid item xs={12} sm={6} md={2}>
                        <Card>
                            <CardContent>
                                <Typography variant="h4" color="info.main">{statistics.totalWorkersWithCustody}</Typography>
                                <Typography variant="body2" color="textSecondary">Workers</Typography>
                            </CardContent>
                        </Card>
                    </Grid>
                    <Grid item xs={12} sm={6} md={2}>
                        <Card>
                            <CardContent>
                                <Typography variant="h4" color="secondary.main">{statistics.totalItemsInCustody}</Typography>
                                <Typography variant="body2" color="textSecondary">Items</Typography>
                            </CardContent>
                        </Card>
                    </Grid>
                    <Grid item xs={12} sm={6} md={2}>
                        <Card>
                            <CardContent>
                                <Typography variant="h4" color="success.main">{statistics.totalReturnedQuantity.toLocaleString()}</Typography>
                                <Typography variant="body2" color="textSecondary">Returned</Typography>
                            </CardContent>
                        </Card>
                    </Grid>
                    <Grid item xs={12} sm={6} md={2}>
                        <Card>
                            <CardContent>
                                <Typography variant="h4" color="warning.main">{statistics.totalConsumedQuantity.toLocaleString()}</Typography>
                                <Typography variant="body2" color="textSecondary">Consumed</Typography>
                            </CardContent>
                        </Card>
                    </Grid>
                </Grid>
            )}

            {/* Tabs */}
            <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
                <Tabs value={tabValue} onChange={handleTabChange}>
                    <Tab label="All / الكل" />
                    <Tab label="Active / نشط" />
                    <Tab label="Overdue / متأخر" icon={<OverdueIcon color="error" />} iconPosition="end" />
                    <Tab label="Closed / مغلق" />
                </Tabs>
            </Box>

            {/* Custody Table */}
            <TabPanel value={tabValue} index={tabValue}>
                <TableContainer component={Paper}>
                    <Table>
                        <TableHead>
                            <TableRow>
                                <TableCell>رقم العهدة</TableCell>
                                <TableCell>العامل</TableCell>
                                <TableCell>الصنف</TableCell>
                                <TableCell>القسم</TableCell>
                                <TableCell align="right">الكمية</TableCell>
                                <TableCell align="right">المتبقي</TableCell>
                                <TableCell>الحالة</TableCell>
                                <TableCell>تاريخ الإصدار</TableCell>
                                <TableCell align="center">الأيام</TableCell>
                                <TableCell align="center">الإجراءات</TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {filteredCustodies.length === 0 ? (
                                <TableRow>
                                    <TableCell colSpan={10} align="center">
                                        <Typography variant="body2" color="textSecondary">
                                            No custody records found
                                        </Typography>
                                    </TableCell>
                                </TableRow>
                            ) : (
                                filteredCustodies.map(custody => (
                                    <TableRow key={custody.id} hover sx={{ bgcolor: custody.isOverdue ? 'error.light' : 'inherit' }}>
                                        <TableCell>
                                            <Typography variant="body2" fontWeight="bold">
                                                {custody.custodyNumber}
                                            </Typography>
                                        </TableCell>
                                        <TableCell>
                                            <Box display="flex" alignItems="center" gap={1}>
                                                <WorkerIcon fontSize="small" />
                                                <Box>
                                                    <Typography variant="body2">{custody.workerNameArabic}</Typography>
                                                    <Typography variant="caption" color="textSecondary">{custody.workerCode}</Typography>
                                                </Box>
                                            </Box>
                                        </TableCell>
                                        <TableCell>
                                            <Typography variant="body2">{custody.itemNameArabic}</Typography>
                                            <Typography variant="caption" color="textSecondary">{custody.itemCode}</Typography>
                                        </TableCell>
                                        <TableCell>{custody.departmentNameArabic}</TableCell>
                                        <TableCell align="right">{custody.quantity.toLocaleString()} {custody.unit}</TableCell>
                                        <TableCell align="right">{custody.remainingQuantity.toLocaleString()}</TableCell>
                                        <TableCell>
                                            <Chip
                                                label={custody.status}
                                                size="small"
                                                color={getStatusColor(custody.status) as any}
                                            />
                                            {custody.isOverdue && (
                                                <Chip
                                                    icon={<OverdueIcon />}
                                                    label="Overdue"
                                                    size="small"
                                                    color="error"
                                                    sx={{ ml: 0.5 }}
                                                />
                                            )}
                                        </TableCell>
                                        <TableCell>
                                            {new Date(custody.issuedDate).toLocaleDateString('en-GB')}
                                        </TableCell>
                                        <TableCell align="center">
                                            <Typography
                                                variant="body2"
                                                color={custody.isOverdue ? 'error' : 'textPrimary'}
                                                fontWeight={custody.isOverdue ? 'bold' : 'normal'}
                                            >
                                                {custody.daysInCustody}
                                            </Typography>
                                        </TableCell>
                                        <TableCell align="center">
                                            <Box display="flex" gap={0.5} justifyContent="center">
                                                <Tooltip title="View Details">
                                                    <IconButton size="small" onClick={() => handleViewClick(custody)}>
                                                        <ViewIcon />
                                                    </IconButton>
                                                </Tooltip>
                                                {custody.status === 'Active' && canProcess && custody.remainingQuantity > 0 && (
                                                    <>
                                                        <Tooltip title="Return">
                                                            <IconButton size="small" color="success" onClick={() => handleReturnClick(custody)}>
                                                                <ReturnIcon />
                                                            </IconButton>
                                                        </Tooltip>
                                                        <Tooltip title="Consume">
                                                            <IconButton size="small" color="warning" onClick={() => handleConsumeClick(custody)}>
                                                                <ConsumeIcon />
                                                            </IconButton>
                                                        </Tooltip>
                                                        <Tooltip title="Transfer">
                                                            <IconButton size="small" color="info" onClick={() => handleTransferClick(custody)}>
                                                                <TransferIcon />
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

            {/* Issue Custody Dialog */}
            <Dialog open={issueDialogOpen} onClose={() => setIssueDialogOpen(false)} maxWidth="md" fullWidth>
                <DialogTitle>
                    <Box display="flex" alignItems="center" gap={1}>
                        <AddIcon />
                        <Typography>Issue Custody / إصدار عهدة</Typography>
                    </Box>
                </DialogTitle>
                <DialogContent>
                    <Grid container spacing={2} sx={{ mt: 1 }}>
                        <Grid item xs={12} md={6}>
                            <FactoryAutocomplete
                                value={issueForm.factory}
                                onChange={(factory) => setIssueForm({ ...issueForm, factory, department: null, warehouse: null, worker: null })}
                                required
                            />
                        </Grid>
                        <Grid item xs={12} md={6}>
                            <DepartmentAutocomplete
                                value={issueForm.department}
                                onChange={(department) => setIssueForm({ ...issueForm, department, worker: null })}
                                factoryId={issueForm.factory?.id}
                                required
                            />
                        </Grid>
                        <Grid item xs={12} md={6}>
                            <WorkerAutocomplete
                                value={issueForm.worker}
                                onChange={(worker) => setIssueForm({ ...issueForm, worker })}
                                factoryId={issueForm.factory?.id}
                                departmentId={issueForm.department?.id}
                                required
                            />
                        </Grid>
                        <Grid item xs={12} md={6}>
                            <WarehouseAutocomplete
                                value={issueForm.warehouse}
                                onChange={(warehouse) => setIssueForm({ ...issueForm, warehouse })}
                                factoryId={issueForm.factory?.id}
                                required
                            />
                        </Grid>
                        <Grid item xs={12} md={6}>
                            <ItemAutocomplete
                                value={issueForm.item}
                                onChange={(item) => setIssueForm({ ...issueForm, item })}
                                warehouseId={issueForm.warehouse?.id}
                                required
                            />
                        </Grid>
                        <Grid item xs={6} md={3}>
                            <TextField
                                fullWidth
                                label="Quantity / الكمية"
                                type="number"
                                value={issueForm.quantity}
                                onChange={(e) => setIssueForm({ ...issueForm, quantity: parseFloat(e.target.value) || 0 })}
                                inputProps={{ min: 0.01, step: 0.01 }}
                                required
                            />
                        </Grid>
                        <Grid item xs={6} md={3}>
                            <TextField
                                fullWidth
                                label="Custody Limit (Days)"
                                type="number"
                                value={issueForm.custodyLimit}
                                onChange={(e) => setIssueForm({ ...issueForm, custodyLimit: parseInt(e.target.value) || 30 })}
                                inputProps={{ min: 1 }}
                            />
                        </Grid>
                        <Grid item xs={12} md={6}>
                            <TextField
                                fullWidth
                                label="Purpose / الغرض"
                                value={issueForm.purpose}
                                onChange={(e) => setIssueForm({ ...issueForm, purpose: e.target.value })}
                                required
                            />
                        </Grid>
                        <Grid item xs={12} md={6}>
                            <TextField
                                fullWidth
                                label="الغرض بالعربي"
                                value={issueForm.purposeArabic}
                                onChange={(e) => setIssueForm({ ...issueForm, purposeArabic: e.target.value })}
                            />
                        </Grid>
                        <Grid item xs={12}>
                            <TextField
                                fullWidth
                                label="Notes / ملاحظات"
                                multiline
                                rows={2}
                                value={issueForm.notes}
                                onChange={(e) => setIssueForm({ ...issueForm, notes: e.target.value })}
                            />
                        </Grid>
                    </Grid>
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setIssueDialogOpen(false)}>Cancel</Button>
                    <Button onClick={handleIssueSubmit} variant="contained">
                        Issue Custody
                    </Button>
                </DialogActions>
            </Dialog>

            {/* View Custody Dialog */}
            <Dialog open={viewDialogOpen} onClose={() => setViewDialogOpen(false)} maxWidth="md" fullWidth>
                <DialogTitle>
                    Custody Details - {selectedCustody?.custodyNumber}
                </DialogTitle>
                <DialogContent>
                    {selectedCustody && (
                        <Grid container spacing={2} sx={{ mt: 1 }}>
                            <Grid item xs={12} sm={6} md={4}>
                                <Typography variant="body2" color="textSecondary">Worker</Typography>
                                <Typography variant="body1">{selectedCustody.workerNameArabic}</Typography>
                                <Typography variant="caption">{selectedCustody.workerCode}</Typography>
                            </Grid>
                            <Grid item xs={12} sm={6} md={4}>
                                <Typography variant="body2" color="textSecondary">Item</Typography>
                                <Typography variant="body1">{selectedCustody.itemNameArabic}</Typography>
                                <Typography variant="caption">{selectedCustody.itemCode}</Typography>
                            </Grid>
                            <Grid item xs={12} sm={6} md={4}>
                                <Typography variant="body2" color="textSecondary">Status</Typography>
                                <Chip label={selectedCustody.status} color={getStatusColor(selectedCustody.status) as any} />
                            </Grid>
                            <Grid item xs={12}>
                                <Divider sx={{ my: 2 }} />
                            </Grid>
                            <Grid item xs={6} sm={3}>
                                <Typography variant="body2" color="textSecondary">Quantity</Typography>
                                <Typography variant="h6">{selectedCustody.quantity.toLocaleString()}</Typography>
                            </Grid>
                            <Grid item xs={6} sm={3}>
                                <Typography variant="body2" color="textSecondary">Returned</Typography>
                                <Typography variant="h6" color="success.main">{selectedCustody.returnedQuantity.toLocaleString()}</Typography>
                            </Grid>
                            <Grid item xs={6} sm={3}>
                                <Typography variant="body2" color="textSecondary">Consumed</Typography>
                                <Typography variant="h6" color="warning.main">{selectedCustody.consumedQuantity.toLocaleString()}</Typography>
                            </Grid>
                            <Grid item xs={6} sm={3}>
                                <Typography variant="body2" color="textSecondary">Remaining</Typography>
                                <Typography variant="h6" color="primary">{selectedCustody.remainingQuantity.toLocaleString()}</Typography>
                            </Grid>
                            <Grid item xs={12}>
                                <Divider sx={{ my: 2 }} />
                            </Grid>
                            <Grid item xs={12} sm={6}>
                                <Typography variant="body2" color="textSecondary">Purpose</Typography>
                                <Typography variant="body1">{selectedCustody.purposeArabic || selectedCustody.purpose}</Typography>
                            </Grid>
                            <Grid item xs={6} sm={3}>
                                <Typography variant="body2" color="textSecondary">Days in Custody</Typography>
                                <Typography variant="h6" color={selectedCustody.isOverdue ? 'error' : 'textPrimary'}>
                                    {selectedCustody.daysInCustody}
                                </Typography>
                            </Grid>
                            <Grid item xs={6} sm={3}>
                                <Typography variant="body2" color="textSecondary">Custody Limit</Typography>
                                <Typography variant="body1">{selectedCustody.custodyLimit || 'N/A'} days</Typography>
                            </Grid>
                        </Grid>
                    )}
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setViewDialogOpen(false)}>Close</Button>
                </DialogActions>
            </Dialog>

            {/* Return Dialog */}
            <Dialog open={returnDialogOpen} onClose={() => setReturnDialogOpen(false)} maxWidth="sm" fullWidth>
                <DialogTitle>Return Custody</DialogTitle>
                <DialogContent>
                    {selectedCustody && (
                        <Grid container spacing={2} sx={{ mt: 1 }}>
                            <Grid item xs={12}>
                                <Typography variant="body2">
                                    <strong>Custody #:</strong> {selectedCustody.custodyNumber}
                                </Typography>
                                <Typography variant="body2">
                                    <strong>Worker:</strong> {selectedCustody.workerNameArabic}
                                </Typography>
                                <Typography variant="body2">
                                    <strong>Item:</strong> {selectedCustody.itemNameArabic}
                                </Typography>
                                <Typography variant="body2">
                                    <strong>Remaining:</strong> {selectedCustody.remainingQuantity.toLocaleString()}
                                </Typography>
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    fullWidth
                                    label="Return Quantity"
                                    type="number"
                                    value={returnQuantity}
                                    onChange={(e) => setReturnQuantity(parseFloat(e.target.value) || 0)}
                                    inputProps={{ min: 0.01, max: selectedCustody.remainingQuantity }}
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    fullWidth
                                    label="Notes"
                                    multiline
                                    rows={2}
                                    value={actionNotes}
                                    onChange={(e) => setActionNotes(e.target.value)}
                                />
                            </Grid>
                        </Grid>
                    )}
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setReturnDialogOpen(false)}>Cancel</Button>
                    <Button onClick={handleReturnSubmit} variant="contained" color="success">
                        Return
                    </Button>
                </DialogActions>
            </Dialog>

            {/* Consume Dialog */}
            <Dialog open={consumeDialogOpen} onClose={() => setConsumeDialogOpen(false)} maxWidth="sm" fullWidth>
                <DialogTitle>Consume Custody</DialogTitle>
                <DialogContent>
                    {selectedCustody && (
                        <Grid container spacing={2} sx={{ mt: 1 }}>
                            <Grid item xs={12}>
                                <Typography variant="body2">
                                    <strong>Custody #:</strong> {selectedCustody.custodyNumber}
                                </Typography>
                                <Typography variant="body2">
                                    <strong>Worker:</strong> {selectedCustody.workerNameArabic}
                                </Typography>
                                <Typography variant="body2">
                                    <strong>Item:</strong> {selectedCustody.itemNameArabic}
                                </Typography>
                                <Typography variant="body2">
                                    <strong>Remaining:</strong> {selectedCustody.remainingQuantity.toLocaleString()}
                                </Typography>
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    fullWidth
                                    label="Consume Quantity"
                                    type="number"
                                    value={consumeQuantity}
                                    onChange={(e) => setConsumeQuantity(parseFloat(e.target.value) || 0)}
                                    inputProps={{ min: 0.01, max: selectedCustody.remainingQuantity }}
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    fullWidth
                                    label="Notes"
                                    multiline
                                    rows={2}
                                    value={actionNotes}
                                    onChange={(e) => setActionNotes(e.target.value)}
                                />
                            </Grid>
                        </Grid>
                    )}
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setConsumeDialogOpen(false)}>Cancel</Button>
                    <Button onClick={handleConsumeSubmit} variant="contained" color="warning">
                        Consume
                    </Button>
                </DialogActions>
            </Dialog>

            {/* Transfer Dialog */}
            <Dialog open={transferDialogOpen} onClose={() => setTransferDialogOpen(false)} maxWidth="sm" fullWidth>
                <DialogTitle>Transfer Custody</DialogTitle>
                <DialogContent>
                    {selectedCustody && (
                        <Grid container spacing={2} sx={{ mt: 1 }}>
                            <Grid item xs={12}>
                                <Typography variant="body2">
                                    <strong>Custody #:</strong> {selectedCustody.custodyNumber}
                                </Typography>
                                <Typography variant="body2">
                                    <strong>From Worker:</strong> {selectedCustody.workerNameArabic}
                                </Typography>
                                <Typography variant="body2">
                                    <strong>Item:</strong> {selectedCustody.itemNameArabic}
                                </Typography>
                            </Grid>
                            <Grid item xs={12}>
                                <DepartmentAutocomplete
                                    value={transferDepartment}
                                    onChange={setTransferDepartment}
                                    factoryId={selectedCustody.factoryId}
                                    label="Transfer to Department"
                                    labelArabic="نقل إلى قسم"
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <WorkerAutocomplete
                                    value={transferWorker}
                                    onChange={setTransferWorker}
                                    factoryId={selectedCustody.factoryId}
                                    departmentId={transferDepartment?.id}
                                    label="Transfer to Worker"
                                    labelArabic="نقل إلى عامل"
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    fullWidth
                                    label="Notes"
                                    multiline
                                    rows={2}
                                    value={actionNotes}
                                    onChange={(e) => setActionNotes(e.target.value)}
                                />
                            </Grid>
                        </Grid>
                    )}
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setTransferDialogOpen(false)}>Cancel</Button>
                    <Button onClick={handleTransferSubmit} variant="contained" color="info" disabled={!transferWorker || !transferDepartment}>
                        Transfer
                    </Button>
                </DialogActions>
            </Dialog>
        </Container>
    );
};

export default OperationalCustodyPage;
