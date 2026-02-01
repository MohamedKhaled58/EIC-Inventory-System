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
    Chip,
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
    Tabs,
    Tab,
    Card,
    CardContent,
    LinearProgress,
} from '@mui/material';
import {
    Star as StarIcon,
    Lock as LockIcon,
    Warning as WarningIcon,
    CheckCircle as CheckCircleIcon,
    ArrowUpward as ArrowUpwardIcon,
    ArrowDownward as ArrowDownwardIcon,
    Refresh as RefreshIcon,
    History as HistoryIcon,
    Assessment as AssessmentIcon,
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
            id={`commander-reserve-tabpanel-${index}`}
            aria-labelledby={`commander-reserve-tab-${index}`}
            {...other}
        >
            {value === index && <Box sx={{ py: 3 }}>{children}</Box>}
        </div>
    );
}

interface ReserveItem {
    id: number;
    itemCode: string;
    itemName: string;
    itemNameAr: string;
    warehouseId: number;
    warehouseName: string;
    totalQuantity: number;
    generalQuantity: number;
    commanderReserveQuantity: number;
    minimumReserveRequired: number;
    availableQuantity: number;
    unit: string;
    status: 'OK' | 'LOW' | 'CRITICAL';
}

interface ReserveRequest {
    id: number;
    requestNumber: string;
    itemId: number;
    itemName: string;
    itemNameAr: string;
    requestedQuantity: number;
    availableReserve: number;
    requestingDepartment: string;
    requestingDepartmentAr: string;
    justification: string;
    priority: 'LOW' | 'MEDIUM' | 'HIGH' | 'URGENT';
    status: 'PENDING' | 'APPROVED' | 'REJECTED';
    requestedBy: string;
    requestedDate: string;
    reviewedBy?: string;
    reviewedDate?: string;
    reviewNotes?: string;
}

interface ReserveTransaction {
    id: number;
    transactionNumber: string;
    itemId: number;
    itemName: string;
    itemNameAr: string;
    quantity: number;
    type: 'ALLOCATION' | 'RELEASE' | 'RESTOCK';
    reason: string;
    performedBy: string;
    performedDate: string;
    approvedBy?: string;
}

const CommanderReserve: React.FC = () => {
    const { user } = useAuthStore();
    const [tabValue, setTabValue] = useState(0);
    const [loading, setLoading] = useState(true);
    const [reserveItems, setReserveItems] = useState<ReserveItem[]>([]);
    const [pendingRequests, setPendingRequests] = useState<ReserveRequest[]>([]);
    const [transactions, setTransactions] = useState<ReserveTransaction[]>([]);
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState<string | null>(null);

    // Dialog states
    const [approveDialogOpen, setApproveDialogOpen] = useState(false);
    const [rejectDialogOpen, setRejectDialogOpen] = useState(false);
    const [selectedRequest, setSelectedRequest] = useState<ReserveRequest | null>(null);
    const [reviewNotes, setReviewNotes] = useState('');

    const canApproveReserve = user?.role === 'FactoryCommander' || user?.role === 'ComplexCommander';

    useEffect(() => {
        loadData();
    }, []);

    const loadData = async () => {
        setLoading(true);
        setError(null);
        try {
            const [itemsRes, requestsRes, transactionsRes] = await Promise.all([
                apiClient.get<{ data: ReserveItem[] }>('/api/commander-reserve/items'),
                apiClient.get<{ data: ReserveRequest[] }>('/api/commander-reserve/requests/pending'),
                apiClient.get<{ data: ReserveTransaction[] }>('/api/commander-reserve/transactions'),
            ]);

            setReserveItems(itemsRes.data);
            setPendingRequests(requestsRes.data);
            setTransactions(transactionsRes.data);
        } catch (err: any) {
            setError(err.response?.data?.message || 'Failed to load commander reserve data');
        } finally {
            setLoading(false);
        }
    };

    const handleTabChange = (event: React.SyntheticEvent, newValue: number) => {
        setTabValue(newValue);
    };

    const handleApproveClick = (request: ReserveRequest) => {
        setSelectedRequest(request);
        setReviewNotes('');
        setApproveDialogOpen(true);
    };

    const handleRejectClick = (request: ReserveRequest) => {
        setSelectedRequest(request);
        setReviewNotes('');
        setRejectDialogOpen(true);
    };

    const handleApproveConfirm = async () => {
        if (!selectedRequest) return;

        try {
            await apiClient.post(`/api/commander-reserve/requests/${selectedRequest.id}/approve`, {
                reviewNotes,
            });
            setSuccess('Reserve request approved successfully');
            setApproveDialogOpen(false);
            setSelectedRequest(null);
            setReviewNotes('');
            loadData();
        } catch (err: any) {
            setError(err.response?.data?.message || 'Failed to approve request');
        }
    };

    const handleRejectConfirm = async () => {
        if (!selectedRequest) return;

        try {
            await apiClient.post(`/api/commander-reserve/requests/${selectedRequest.id}/reject`, {
                reviewNotes,
            });
            setSuccess('Reserve request rejected');
            setRejectDialogOpen(false);
            setSelectedRequest(null);
            setReviewNotes('');
            loadData();
        } catch (err: any) {
            setError(err.response?.data?.message || 'Failed to reject request');
        }
    };

    const getStatusColor = (status: string) => {
        switch (status) {
            case 'OK':
            case 'APPROVED':
                return 'success';
            case 'LOW':
            case 'PENDING':
                return 'warning';
            case 'CRITICAL':
            case 'REJECTED':
                return 'error';
            default:
                return 'default';
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

    const renderOverview = () => {
        const totalReserve = reserveItems.reduce((sum, item) => sum + item.commanderReserveQuantity, 0);
        const criticalItems = reserveItems.filter((item) => item.status === 'CRITICAL').length;
        const lowItems = reserveItems.filter((item) => item.status === 'LOW').length;
        const pendingCount = pendingRequests.length;

        return (
            <Grid container spacing={3}>
                <Grid item xs={12} sm={6} md={3}>
                    <Card>
                        <CardContent>
                            <Box display="flex" alignItems="center" justifyContent="space-between">
                                <Box>
                                    <Typography variant="body2" color="textSecondary">
                                        Total Reserve
                                    </Typography>
                                    <Typography variant="h4" color="primary">
                                        {totalReserve.toLocaleString()}
                                    </Typography>
                                </Box>
                                <StarIcon color="primary" sx={{ fontSize: 40 }} />
                            </Box>
                        </CardContent>
                    </Card>
                </Grid>

                <Grid item xs={12} sm={6} md={3}>
                    <Card>
                        <CardContent>
                            <Box display="flex" alignItems="center" justifyContent="space-between">
                                <Box>
                                    <Typography variant="body2" color="textSecondary">
                                        Critical Items
                                    </Typography>
                                    <Typography variant="h4" color="error">
                                        {criticalItems}
                                    </Typography>
                                </Box>
                                <WarningIcon color="error" sx={{ fontSize: 40 }} />
                            </Box>
                        </CardContent>
                    </Card>
                </Grid>

                <Grid item xs={12} sm={6} md={3}>
                    <Card>
                        <CardContent>
                            <Box display="flex" alignItems="center" justifyContent="space-between">
                                <Box>
                                    <Typography variant="body2" color="textSecondary">
                                        Low Reserve Items
                                    </Typography>
                                    <Typography variant="h4" color="warning">
                                        {lowItems}
                                    </Typography>
                                </Box>
                                <ArrowDownwardIcon color="warning" sx={{ fontSize: 40 }} />
                            </Box>
                        </CardContent>
                    </Card>
                </Grid>

                <Grid item xs={12} sm={6} md={3}>
                    <Card>
                        <CardContent>
                            <Box display="flex" alignItems="center" justifyContent="space-between">
                                <Box>
                                    <Typography variant="body2" color="textSecondary">
                                        Pending Requests
                                    </Typography>
                                    <Typography variant="h4" color="info">
                                        {pendingCount}
                                    </Typography>
                                </Box>
                                <LockIcon color="info" sx={{ fontSize: 40 }} />
                            </Box>
                        </CardContent>
                    </Card>
                </Grid>
            </Grid>
        );
    };

    const renderReserveItems = () => (
        <TableContainer component={Paper}>
            <Table>
                <TableHead>
                    <TableRow>
                        <TableCell>Item Code</TableCell>
                        <TableCell>Item Name</TableCell>
                        <TableCell>Warehouse</TableCell>
                        <TableCell align="right">Total Stock</TableCell>
                        <TableCell align="right">General Stock</TableCell>
                        <TableCell align="right" sx={{ backgroundColor: '#8b6914', color: 'white' }}>
                            Commander's Reserve ⭐
                        </TableCell>
                        <TableCell align="right">Min Required</TableCell>
                        <TableCell>Status</TableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                    {reserveItems.map((item) => (
                        <TableRow key={item.id} hover>
                            <TableCell>{item.itemCode}</TableCell>
                            <TableCell>
                                <Box>
                                    <Typography variant="body2">{item.itemNameAr}</Typography>
                                    <Typography variant="caption" color="textSecondary">
                                        {item.itemName}
                                    </Typography>
                                </Box>
                            </TableCell>
                            <TableCell>{item.warehouseName}</TableCell>
                            <TableCell align="right">
                                {item.totalQuantity.toLocaleString()} {item.unit}
                            </TableCell>
                            <TableCell align="right">
                                {item.generalQuantity.toLocaleString()} {item.unit}
                            </TableCell>
                            <TableCell align="right" sx={{ backgroundColor: '#fff8e1' }}>
                                <Box display="flex" alignItems="center" justifyContent="flex-end" gap={1}>
                                    <StarIcon sx={{ color: '#8b6914', fontSize: 16 }} />
                                    <Typography variant="body2" fontWeight="bold">
                                        {item.commanderReserveQuantity.toLocaleString()} {item.unit}
                                    </Typography>
                                </Box>
                            </TableCell>
                            <TableCell align="right">
                                {item.minimumReserveRequired.toLocaleString()} {item.unit}
                            </TableCell>
                            <TableCell>
                                <Chip
                                    label={item.status}
                                    color={getStatusColor(item.status) as any}
                                    size="small"
                                    icon={item.status === 'OK' ? <CheckCircleIcon /> : <WarningIcon />}
                                />
                            </TableCell>
                        </TableRow>
                    ))}
                </TableBody>
            </Table>
        </TableContainer>
    );

    const renderPendingRequests = () => (
        <TableContainer component={Paper}>
            <Table>
                <TableHead>
                    <TableRow>
                        <TableCell>Request #</TableCell>
                        <TableCell>Item</TableCell>
                        <TableCell>Department</TableCell>
                        <TableCell align="right">Requested</TableCell>
                        <TableCell align="right">Available</TableCell>
                        <TableCell>Priority</TableCell>
                        <TableCell>Justification</TableCell>
                        <TableCell>Requested By</TableCell>
                        <TableCell>Date</TableCell>
                        {canApproveReserve && <TableCell align="center">Actions</TableCell>}
                    </TableRow>
                </TableHead>
                <TableBody>
                    {pendingRequests.length === 0 ? (
                        <TableRow>
                            <TableCell colSpan={canApproveReserve ? 10 : 9} align="center">
                                <Typography variant="body2" color="textSecondary">
                                    No pending requests
                                </Typography>
                            </TableCell>
                        </TableRow>
                    ) : (
                        pendingRequests.map((request) => (
                            <TableRow key={request.id} hover>
                                <TableCell>{request.requestNumber}</TableCell>
                                <TableCell>
                                    <Box>
                                        <Typography variant="body2">{request.itemNameAr}</Typography>
                                        <Typography variant="caption" color="textSecondary">
                                            {request.itemName}
                                        </Typography>
                                    </Box>
                                </TableCell>
                                <TableCell>
                                    <Box>
                                        <Typography variant="body2">{request.requestingDepartmentAr}</Typography>
                                        <Typography variant="caption" color="textSecondary">
                                            {request.requestingDepartment}
                                        </Typography>
                                    </Box>
                                </TableCell>
                                <TableCell align="right">
                                    {request.requestedQuantity.toLocaleString()}
                                </TableCell>
                                <TableCell align="right">
                                    {request.availableReserve.toLocaleString()}
                                </TableCell>
                                <TableCell>
                                    <Chip
                                        label={request.priority}
                                        color={getPriorityColor(request.priority) as any}
                                        size="small"
                                    />
                                </TableCell>
                                <TableCell>
                                    <Tooltip title={request.justification}>
                                        <Typography
                                            variant="body2"
                                            sx={{
                                                maxWidth: 200,
                                                overflow: 'hidden',
                                                textOverflow: 'ellipsis',
                                                whiteSpace: 'nowrap',
                                            }}
                                        >
                                            {request.justification}
                                        </Typography>
                                    </Tooltip>
                                </TableCell>
                                <TableCell>{request.requestedBy}</TableCell>
                                <TableCell>
                                    {new Date(request.requestedDate).toLocaleDateString('en-GB')}
                                </TableCell>
                                {canApproveReserve && (
                                    <TableCell align="center">
                                        <Box display="flex" gap={1} justifyContent="center">
                                            <Tooltip title="Approve">
                                                <IconButton
                                                    color="success"
                                                    size="small"
                                                    onClick={() => handleApproveClick(request)}
                                                >
                                                    <CheckCircleIcon />
                                                </IconButton>
                                            </Tooltip>
                                            <Tooltip title="Reject">
                                                <IconButton
                                                    color="error"
                                                    size="small"
                                                    onClick={() => handleRejectClick(request)}
                                                >
                                                    <WarningIcon />
                                                </IconButton>
                                            </Tooltip>
                                        </Box>
                                    </TableCell>
                                )}
                            </TableRow>
                        ))
                    )}
                </TableBody>
            </Table>
        </TableContainer>
    );

    const renderTransactions = () => (
        <TableContainer component={Paper}>
            <Table>
                <TableHead>
                    <TableRow>
                        <TableCell>Transaction #</TableCell>
                        <TableCell>Item</TableCell>
                        <TableCell align="right">Quantity</TableCell>
                        <TableCell>Type</TableCell>
                        <TableCell>Reason</TableCell>
                        <TableCell>Performed By</TableCell>
                        <TableCell>Date</TableCell>
                        <TableCell>Approved By</TableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                    {transactions.map((transaction) => (
                        <TableRow key={transaction.id} hover>
                            <TableCell>{transaction.transactionNumber}</TableCell>
                            <TableCell>
                                <Box>
                                    <Typography variant="body2">{transaction.itemNameAr}</Typography>
                                    <Typography variant="caption" color="textSecondary">
                                        {transaction.itemName}
                                    </Typography>
                                </Box>
                            </TableCell>
                            <TableCell align="right">
                                <Box display="flex" alignItems="center" justifyContent="flex-end" gap={1}>
                                    {transaction.type === 'RELEASE' ? (
                                        <ArrowDownwardIcon color="error" sx={{ fontSize: 16 }} />
                                    ) : (
                                        <ArrowUpwardIcon color="success" sx={{ fontSize: 16 }} />
                                    )}
                                    <Typography variant="body2">
                                        {transaction.quantity.toLocaleString()}
                                    </Typography>
                                </Box>
                            </TableCell>
                            <TableCell>
                                <Chip
                                    label={transaction.type}
                                    color={
                                        transaction.type === 'RELEASE'
                                            ? 'error'
                                            : transaction.type === 'ALLOCATION'
                                                ? 'warning'
                                                : 'success'
                                    }
                                    size="small"
                                />
                            </TableCell>
                            <TableCell>{transaction.reason}</TableCell>
                            <TableCell>{transaction.performedBy}</TableCell>
                            <TableCell>
                                {new Date(transaction.performedDate).toLocaleDateString('en-GB')}
                            </TableCell>
                            <TableCell>{transaction.approvedBy || '-'}</TableCell>
                        </TableRow>
                    ))}
                </TableBody>
            </Table>
        </TableContainer>
    );

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
                        احتياطي قائد المصنع
                    </Typography>
                    <Typography variant="body1" color="textSecondary">
                        Commander's Reserve Management
                    </Typography>
                </Box>
                <Tooltip title="Refresh">
                    <IconButton onClick={loadData} color="primary">
                        <RefreshIcon />
                    </IconButton>
                </Tooltip>
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
                <Tabs value={tabValue} onChange={handleTabChange} aria-label="Commander Reserve Tabs">
                    <Tab
                        icon={<AssessmentIcon />}
                        label="Overview / نظرة عامة"
                        id="commander-reserve-tab-0"
                        aria-controls="commander-reserve-tabpanel-0"
                    />
                    <Tab
                        icon={<StarIcon />}
                        label="Reserve Items / عناصر الاحتياطي"
                        id="commander-reserve-tab-1"
                        aria-controls="commander-reserve-tabpanel-1"
                    />
                    <Tab
                        icon={<LockIcon />}
                        label="Pending Requests / الطلبات المعلقة"
                        id="commander-reserve-tab-2"
                        aria-controls="commander-reserve-tabpanel-2"
                    />
                    <Tab
                        icon={<HistoryIcon />}
                        label="History / السجل"
                        id="commander-reserve-tab-3"
                        aria-controls="commander-reserve-tabpanel-3"
                    />
                </Tabs>
            </Box>

            <TabPanel value={tabValue} index={0}>
                {renderOverview()}
            </TabPanel>

            <TabPanel value={tabValue} index={1}>
                {renderReserveItems()}
            </TabPanel>

            <TabPanel value={tabValue} index={2}>
                {!canApproveReserve && pendingRequests.length > 0 && (
                    <Alert severity="info" sx={{ mb: 2 }}>
                        Only Factory Commander or Complex Commander can approve reserve requests.
                    </Alert>
                )}
                {renderPendingRequests()}
            </TabPanel>

            <TabPanel value={tabValue} index={3}>
                {renderTransactions()}
            </TabPanel>

            {/* Approve Dialog */}
            <Dialog open={approveDialogOpen} onClose={() => setApproveDialogOpen(false)} maxWidth="sm" fullWidth>
                <DialogTitle>Approve Reserve Request</DialogTitle>
                <DialogContent>
                    {selectedRequest && (
                        <Box>
                            <Typography variant="body2" gutterBottom>
                                <strong>Item:</strong> {selectedRequest.itemNameAr}
                            </Typography>
                            <Typography variant="body2" gutterBottom>
                                <strong>Quantity:</strong> {selectedRequest.requestedQuantity.toLocaleString()}
                            </Typography>
                            <Typography variant="body2" gutterBottom>
                                <strong>Department:</strong> {selectedRequest.requestingDepartmentAr}
                            </Typography>
                            <Typography variant="body2" gutterBottom>
                                <strong>Justification:</strong> {selectedRequest.justification}
                            </Typography>
                            <TextField
                                autoFocus
                                margin="dense"
                                label="Review Notes"
                                fullWidth
                                multiline
                                rows={3}
                                value={reviewNotes}
                                onChange={(e) => setReviewNotes(e.target.value)}
                                placeholder="Optional notes for approval..."
                            />
                        </Box>
                    )}
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
                <DialogTitle>Reject Reserve Request</DialogTitle>
                <DialogContent>
                    {selectedRequest && (
                        <Box>
                            <Typography variant="body2" gutterBottom>
                                <strong>Item:</strong> {selectedRequest.itemNameAr}
                            </Typography>
                            <Typography variant="body2" gutterBottom>
                                <strong>Quantity:</strong> {selectedRequest.requestedQuantity.toLocaleString()}
                            </Typography>
                            <Typography variant="body2" gutterBottom>
                                <strong>Department:</strong> {selectedRequest.requestingDepartmentAr}
                            </Typography>
                            <Typography variant="body2" gutterBottom>
                                <strong>Justification:</strong> {selectedRequest.justification}
                            </Typography>
                            <TextField
                                autoFocus
                                margin="dense"
                                label="Rejection Reason *"
                                fullWidth
                                multiline
                                rows={3}
                                value={reviewNotes}
                                onChange={(e) => setReviewNotes(e.target.value)}
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
        </Container>
    );
};

export default CommanderReserve;
