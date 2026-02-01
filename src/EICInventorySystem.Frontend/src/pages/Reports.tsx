import React, { useState, useEffect } from 'react';
import {
    Box,
    Container,
    Typography,
    Paper,
    Grid,
    Card,
    CardContent,
    Button,
    Select,
    MenuItem,
    FormControl,
    InputLabel,
    TextField,
    CircularProgress,
    Alert,
    Tabs,
    Tab,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Chip,
    IconButton,
    Tooltip,
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    Divider,
} from '@mui/material';
import {
    Download as DownloadIcon,
    Print as PrintIcon,
    Refresh as RefreshIcon,
    Assessment as AssessmentIcon,
    Inventory as InventoryIcon,
    TrendingUp as TrendingUpIcon,
    TrendingDown as TrendingDownIcon,
    Warning as WarningIcon,
    CheckCircle as CheckCircleIcon,
    LocalShipping as LocalShippingIcon,
    Receipt as ReceiptIcon,
    Description as DescriptionIcon,
    BarChart as BarChartIcon,
    PieChart as PieChartIcon,
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
            id={`report-tabpanel-${index}`}
            aria-labelledby={`report-tab-${index}`}
            {...other}
        >
            {value === index && <Box sx={{ py: 3 }}>{children}</Box>}
        </div>
    );
}

interface InventoryReport {
    itemId: number;
    itemCode: string;
    itemName: string;
    itemNameAr: string;
    unit: string;
    totalQuantity: number;
    generalQuantity: number;
    commanderReserveQuantity: number;
    allocatedQuantity: number;
    availableQuantity: number;
    reorderPoint: number;
    minimumReserveRequired: number;
    status: 'OK' | 'LOW' | 'CRITICAL';
    warehouseName: string;
    warehouseNameAr: string;
}

interface MovementReport {
    itemId: number;
    itemCode: string;
    itemName: string;
    itemNameAr: string;
    unit: string;
    totalReceived: number;
    totalIssued: number;
    totalTransferredIn: number;
    totalTransferredOut: number;
    netChange: number;
    openingBalance: number;
    closingBalance: number;
}

interface RequisitionReport {
    requisitionNumber: string;
    departmentName: string;
    departmentNameAr: string;
    projectName?: string;
    projectNameAr?: string;
    itemCount: number;
    totalQuantity: number;
    status: string;
    requestedDate: string;
    approvedDate?: string;
    issuedDate?: string;
}

interface TransferReport {
    transferNumber: string;
    fromWarehouseName: string;
    fromWarehouseNameAr: string;
    toWarehouseName: string;
    toWarehouseNameAr: string;
    itemCount: number;
    totalQuantity: number;
    status: string;
    requestedDate: string;
    completedDate?: string;
}

interface ConsumptionReport {
    projectId: number;
    projectNumber: string;
    projectName: string;
    projectNameAr: string;
    budget: number;
    consumed: number;
    remaining: number;
    consumptionPercentage: number;
    itemCount: number;
    status: 'ON_TRACK' | 'OVER_BUDGET' | 'NEAR_LIMIT';
}

const Reports: React.FC = () => {
    const { user } = useAuthStore();
    const [tabValue, setTabValue] = useState(0);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState<string | null>(null);

    // Report data
    const [inventoryReport, setInventoryReport] = useState<InventoryReport[]>([]);
    const [movementReport, setMovementReport] = useState<MovementReport[]>([]);
    const [requisitionReport, setRequisitionReport] = useState<RequisitionReport[]>([]);
    const [transferReport, setTransferReport] = useState<TransferReport[]>([]);
    const [consumptionReport, setConsumptionReport] = useState<ConsumptionReport[]>([]);

    // Filters
    const [filters, setFilters] = useState({
        warehouseId: '',
        factoryId: '',
        itemId: '',
        startDate: '',
        endDate: '',
        projectId: '',
        departmentId: '',
    });

    // Export dialog
    const [exportDialogOpen, setExportDialogOpen] = useState(false);
    const [exportFormat, setExportFormat] = useState<'PDF' | 'EXCEL'>('PDF');

    useEffect(() => {
        loadReports();
    }, []);

    const loadReports = async () => {
        setLoading(true);
        setError(null);
        try {
            const [invRes, moveRes, reqRes, transRes, consRes] = await Promise.all([
                apiClient.get<InventoryReport[]>('/api/reports/inventory'),
                apiClient.get<MovementReport[]>('/api/reports/movement'),
                apiClient.get<RequisitionReport[]>('/api/reports/requisitions'),
                apiClient.get<TransferReport[]>('/api/reports/transfers'),
                apiClient.get<ConsumptionReport[]>('/api/reports/consumption'),
            ]);

            setInventoryReport(invRes);
            setMovementReport(moveRes);
            setRequisitionReport(reqRes);
            setTransferReport(transRes);
            setConsumptionReport(consRes);
        } catch (err: any) {
            setError(err.response?.data?.message || 'Failed to load reports');
        } finally {
            setLoading(false);
        }
    };

    const handleTabChange = (event: React.SyntheticEvent, newValue: number) => {
        setTabValue(newValue);
    };

    const handleExport = async () => {
        setLoading(true);
        try {
            const endpoint = getReportEndpoint(tabValue);
            await apiClient.get(`${endpoint}/export`, {
                params: { format: exportFormat, ...filters },
                responseType: 'blob',
            });
            setSuccess(`Report exported as ${exportFormat}`);
            setExportDialogOpen(false);
        } catch (err: any) {
            setError(err.response?.data?.message || 'Failed to export report');
        } finally {
            setLoading(false);
        }
    };

    const getReportEndpoint = (tab: number) => {
        switch (tab) {
            case 0: return '/api/reports/inventory';
            case 1: return '/api/reports/movement';
            case 2: return '/api/reports/requisitions';
            case 3: return '/api/reports/transfers';
            case 4: return '/api/reports/consumption';
            default: return '/api/reports/inventory';
        }
    };

    const getStatusColor = (status: string) => {
        switch (status) {
            case 'OK':
            case 'ON_TRACK':
            case 'COMPLETED':
                return 'success';
            case 'LOW':
            case 'NEAR_LIMIT':
            case 'PENDING':
                return 'warning';
            case 'CRITICAL':
            case 'OVER_BUDGET':
            case 'REJECTED':
                return 'error';
            default:
                return 'default';
        }
    };

    const getStatusIcon = (status: string) => {
        switch (status) {
            case 'OK':
            case 'ON_TRACK':
            case 'COMPLETED':
                return <CheckCircleIcon />;
            case 'LOW':
            case 'NEAR_LIMIT':
            case 'PENDING':
                return <WarningIcon />;
            case 'CRITICAL':
            case 'OVER_BUDGET':
            case 'REJECTED':
                return <TrendingDownIcon />;
            default:
                return null;
        }
    };

    const getInventorySummary = () => {
        const totalItems = inventoryReport.length;
        const criticalItems = inventoryReport.filter(i => i.status === 'CRITICAL').length;
        const lowItems = inventoryReport.filter(i => i.status === 'LOW').length;
        const okItems = inventoryReport.filter(i => i.status === 'OK').length;

        return { totalItems, criticalItems, lowItems, okItems };
    };

    const getConsumptionSummary = () => {
        const totalBudget = consumptionReport.reduce((sum, c) => sum + c.budget, 0);
        const totalConsumed = consumptionReport.reduce((sum, c) => sum + c.consumed, 0);
        const overBudget = consumptionReport.filter(c => c.status === 'OVER_BUDGET').length;
        const nearLimit = consumptionReport.filter(c => c.status === 'NEAR_LIMIT').length;

        return { totalBudget, totalConsumed, overBudget, nearLimit };
    };

    const summary = getInventorySummary();
    const consumptionSummary = getConsumptionSummary();

    return (
        <Container maxWidth="xl">
            <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
                <Box>
                    <Typography variant="h4" gutterBottom>
                        التقارير
                    </Typography>
                    <Typography variant="body1" color="textSecondary">
                        Reports & Analytics
                    </Typography>
                </Box>
                <Box display="flex" gap={2}>
                    <Tooltip title="تحديث">
                        <IconButton onClick={loadReports} color="primary">
                            <RefreshIcon />
                        </IconButton>
                    </Tooltip>
                    <Button
                        variant="outlined"
                        startIcon={<DownloadIcon />}
                        onClick={() => setExportDialogOpen(true)}
                    >
                        تصدير
                    </Button>
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

            {/* Summary Cards */}
            <Grid container spacing={3} mb={3}>
                <Grid item xs={12} sm={6} md={3}>
                    <Card>
                        <CardContent>
                            <Box display="flex" alignItems="center" justifyContent="space-between">
                                <Box>
                                    <Typography variant="body2" color="textSecondary">
                                        إجمالي الأصناف
                                    </Typography>
                                    <Typography variant="h4">
                                        {summary.totalItems}
                                    </Typography>
                                </Box>
                                <InventoryIcon color="primary" fontSize="large" />
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
                                        مخزون حرج
                                    </Typography>
                                    <Typography variant="h4" color="error">
                                        {summary.criticalItems}
                                    </Typography>
                                </Box>
                                <WarningIcon color="error" fontSize="large" />
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
                                        مخزون منخفض
                                    </Typography>
                                    <Typography variant="h4" color="warning">
                                        {summary.lowItems}
                                    </Typography>
                                </Box>
                                <TrendingDownIcon color="warning" fontSize="large" />
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
                                        تجاوز الميزانية
                                    </Typography>
                                    <Typography variant="h4" color="error">
                                        {consumptionSummary.overBudget}
                                    </Typography>
                                </Box>
                                <AssessmentIcon color="error" fontSize="large" />
                            </Box>
                        </CardContent>
                    </Card>
                </Grid>
            </Grid>

            <Box sx={{ borderBottom: 1, borderColor: 'divider', mb: 3 }}>
                <Tabs value={tabValue} onChange={handleTabChange} aria-label="Report Tabs">
                    <Tab
                        label="Inventory / المخزون"
                        id="report-tab-0"
                        aria-controls="report-tabpanel-0"
                        icon={<InventoryIcon />}
                    />
                    <Tab
                        label="Movement / الحركة"
                        id="report-tab-1"
                        aria-controls="report-tabpanel-1"
                        icon={<LocalShippingIcon />}
                    />
                    <Tab
                        label="Requisitions / الطلبات"
                        id="report-tab-2"
                        aria-controls="report-tabpanel-2"
                        icon={<ReceiptIcon />}
                    />
                    <Tab
                        label="Transfers / النقل"
                        id="report-tab-3"
                        aria-controls="report-tabpanel-3"
                        icon={<LocalShippingIcon />}
                    />
                    <Tab
                        label="Consumption / الاستهلاك"
                        id="report-tab-4"
                        aria-controls="report-tabpanel-4"
                        icon={<AssessmentIcon />}
                    />
                </Tabs>
            </Box>

            {loading && (
                <Box display="flex" justifyContent="center" alignItems="center" minHeight="40vh">
                    <CircularProgress />
                </Box>
            )}

            <TabPanel value={tabValue} index={0}>
                <InventoryReportTable data={inventoryReport} />
            </TabPanel>

            <TabPanel value={tabValue} index={1}>
                <MovementReportTable data={movementReport} />
            </TabPanel>

            <TabPanel value={tabValue} index={2}>
                <RequisitionReportTable data={requisitionReport} />
            </TabPanel>

            <TabPanel value={tabValue} index={3}>
                <TransferReportTable data={transferReport} />
            </TabPanel>

            <TabPanel value={tabValue} index={4}>
                <ConsumptionReportTable data={consumptionReport} />
            </TabPanel>

            {/* Export Dialog */}
            <Dialog open={exportDialogOpen} onClose={() => setExportDialogOpen(false)} maxWidth="sm" fullWidth>
                <DialogTitle>تصدير التقرير</DialogTitle>
                <DialogContent>
                    <FormControl fullWidth sx={{ mt: 2 }}>
                        <InputLabel>الصيغة</InputLabel>
                        <Select
                            value={exportFormat}
                            label="الصيغة"
                            onChange={(e) => setExportFormat(e.target.value as 'PDF' | 'EXCEL')}
                        >
                            <MenuItem value="PDF">PDF</MenuItem>
                            <MenuItem value="EXCEL">Excel</MenuItem>
                        </Select>
                    </FormControl>
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setExportDialogOpen(false)}>إلغاء</Button>
                    <Button onClick={handleExport} variant="contained" startIcon={<DownloadIcon />}>
                        تصدير
                    </Button>
                </DialogActions>
            </Dialog>
        </Container>
    );
};

interface InventoryReportTableProps {
    data: InventoryReport[];
}

const InventoryReportTable: React.FC<InventoryReportTableProps> = ({ data }) => {
    const getStatusColor = (status: string) => {
        switch (status) {
            case 'OK': return 'success';
            case 'LOW': return 'warning';
            case 'CRITICAL': return 'error';
            default: return 'default';
        }
    };

    const getStatusIcon = (status: string) => {
        switch (status) {
            case 'OK': return <CheckCircleIcon />;
            case 'LOW': return <WarningIcon />;
            case 'CRITICAL': return <TrendingDownIcon />;
            default: return null;
        }
    };

    return (
        <TableContainer component={Paper}>
            <Table>
                <TableHead>
                    <TableRow>
                        <TableCell>كود الصنف</TableCell>
                        <TableCell>اسم الصنف</TableCell>
                        <TableCell>المستودع</TableCell>
                        <TableCell align="right">الإجمالي</TableCell>
                        <TableCell align="right">العام</TableCell>
                        <TableCell align="right">الاحتياطي ⭐</TableCell>
                        <TableCell align="right">المحجوز</TableCell>
                        <TableCell align="right">المتاح</TableCell>
                        <TableCell>الحالة</TableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                    {data.length === 0 ? (
                        <TableRow>
                            <TableCell colSpan={9} align="center">
                                <Typography variant="body2" color="textSecondary">
                                    لا توجد بيانات مخزون متاحة
                                </Typography>
                            </TableCell>
                        </TableRow>
                    ) : (
                        data.map((item) => (
                            <TableRow key={`${item.itemId}-${item.warehouseName}`} hover>
                                <TableCell>{item.itemCode}</TableCell>
                                <TableCell>
                                    <Box>
                                        <Typography variant="body2">{item.itemNameAr}</Typography>
                                        <Typography variant="caption" color="textSecondary">
                                            {item.itemName}
                                        </Typography>
                                    </Box>
                                </TableCell>
                                <TableCell>{item.warehouseNameAr}</TableCell>
                                <TableCell align="right">
                                    {(item.totalQuantity || 0).toLocaleString()} {item.unit}
                                </TableCell>
                                <TableCell align="right">
                                    {(item.generalQuantity || 0).toLocaleString()} {item.unit}
                                </TableCell>
                                <TableCell align="right" sx={{ backgroundColor: '#fff8e1' }}>
                                    {(item.commanderReserveQuantity || 0).toLocaleString()} {item.unit}
                                </TableCell>
                                <TableCell align="right">
                                    {(item.allocatedQuantity || 0).toLocaleString()} {item.unit}
                                </TableCell>
                                <TableCell align="right">
                                    {(item.availableQuantity || 0).toLocaleString()} {item.unit}
                                </TableCell>
                                <TableCell>
                                    <Chip
                                        label={item.status}
                                        color={getStatusColor(item.status) as any}
                                        size="small"
                                        icon={getStatusIcon(item.status) as any}
                                    />
                                </TableCell>
                            </TableRow>
                        ))
                    )}
                </TableBody>
            </Table>
        </TableContainer>
    );
};

interface MovementReportTableProps {
    data: MovementReport[];
}

const MovementReportTable: React.FC<MovementReportTableProps> = ({ data }) => {
    return (
        <TableContainer component={Paper}>
            <Table>
                <TableHead>
                    <TableRow>
                        <TableCell>كود الصنف</TableCell>
                        <TableCell>اسم الصنف</TableCell>
                        <TableCell align="right">افتتاحي</TableCell>
                        <TableCell align="right">وارد</TableCell>
                        <TableCell align="right">منصرف</TableCell>
                        <TableCell align="right">محول وارد</TableCell>
                        <TableCell align="right">محول صادر</TableCell>
                        <TableCell align="right">صافي التغيير</TableCell>
                        <TableCell align="right">إغلاق</TableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                    {data.length === 0 ? (
                        <TableRow>
                            <TableCell colSpan={9} align="center">
                                <Typography variant="body2" color="textSecondary">
                                    لا توجد بيانات حركة متاحة
                                </Typography>
                            </TableCell>
                        </TableRow>
                    ) : (
                        data.map((item) => (
                            <TableRow key={item.itemId} hover>
                                <TableCell>{item.itemCode}</TableCell>
                                <TableCell>
                                    <Box>
                                        <Typography variant="body2">{item.itemNameAr}</Typography>
                                        <Typography variant="caption" color="textSecondary">
                                            {item.itemName}
                                        </Typography>
                                    </Box>
                                </TableCell>
                                <TableCell align="right">
                                    {(item.openingBalance || 0).toLocaleString()} {item.unit}
                                </TableCell>
                                <TableCell align="right" sx={{ color: 'success.main' }}>
                                    +{(item.totalReceived || 0).toLocaleString()} {item.unit}
                                </TableCell>
                                <TableCell align="right" sx={{ color: 'error.main' }}>
                                    -{(item.totalIssued || 0).toLocaleString()} {item.unit}
                                </TableCell>
                                <TableCell align="right" sx={{ color: 'info.main' }}>
                                    +{(item.totalTransferredIn || 0).toLocaleString()} {item.unit}
                                </TableCell>
                                <TableCell align="right" sx={{ color: 'warning.main' }}>
                                    -{(item.totalTransferredOut || 0).toLocaleString()} {item.unit}
                                </TableCell>
                                <TableCell align="right">
                                    <Typography
                                        color={item.netChange >= 0 ? 'success.main' : 'error.main'}
                                        fontWeight="bold"
                                    >
                                        {item.netChange >= 0 ? '+' : ''}
                                        {(item.netChange || 0).toLocaleString()} {item.unit}
                                    </Typography>
                                </TableCell>
                                <TableCell align="right" sx={{ fontWeight: 'bold' }}>
                                    {(item.closingBalance || 0).toLocaleString()} {item.unit}
                                </TableCell>
                            </TableRow>
                        ))
                    )}
                </TableBody>
            </Table>
        </TableContainer>
    );
};

interface RequisitionReportTableProps {
    data: RequisitionReport[];
}

const RequisitionReportTable: React.FC<RequisitionReportTableProps> = ({ data }) => {
    const getStatusColor = (status: string) => {
        switch (status) {
            case 'COMPLETED': return 'success';
            case 'APPROVED': return 'info';
            case 'PENDING': return 'warning';
            case 'REJECTED': return 'error';
            default: return 'default';
        }
    };

    return (
        <TableContainer component={Paper}>
            <Table>
                <TableHead>
                    <TableRow>
                        <TableCell>رقم الطلب</TableCell>
                        <TableCell>القسم</TableCell>
                        <TableCell>المشروع</TableCell>
                        <TableCell align="right">الأصناف</TableCell>
                        <TableCell align="right">الكمية</TableCell>
                        <TableCell>الحالة</TableCell>
                        <TableCell>تاريخ الطلب</TableCell>
                        <TableCell>تاريخ الموافقة</TableCell>
                        <TableCell>تاريخ الصرف</TableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                    {data.length === 0 ? (
                        <TableRow>
                            <TableCell colSpan={9} align="center">
                                <Typography variant="body2" color="textSecondary">
                                    لا توجد بيانات طلبات متاحة
                                </Typography>
                            </TableCell>
                        </TableRow>
                    ) : (
                        data.map((req) => (
                            <TableRow key={req.requisitionNumber} hover>
                                <TableCell>{req.requisitionNumber}</TableCell>
                                <TableCell>
                                    <Box>
                                        <Typography variant="body2">{req.departmentNameAr}</Typography>
                                        <Typography variant="caption" color="textSecondary">
                                            {req.departmentName}
                                        </Typography>
                                    </Box>
                                </TableCell>
                                <TableCell>
                                    {req.projectNameAr ? (
                                        <Box>
                                            <Typography variant="body2">{req.projectNameAr}</Typography>
                                            <Typography variant="caption" color="textSecondary">
                                                {req.projectName}
                                            </Typography>
                                        </Box>
                                    ) : (
                                        <Typography variant="body2" color="textSecondary">
                                            -
                                        </Typography>
                                    )}
                                </TableCell>
                                <TableCell align="right">{req.itemCount}</TableCell>
                                <TableCell align="right">{(req.totalQuantity || 0).toLocaleString()}</TableCell>
                                <TableCell>
                                    <Chip
                                        label={req.status}
                                        color={getStatusColor(req.status) as any}
                                        size="small"
                                    />
                                </TableCell>
                                <TableCell>
                                    {new Date(req.requestedDate).toLocaleDateString('en-GB')}
                                </TableCell>
                                <TableCell>
                                    {req.approvedDate
                                        ? new Date(req.approvedDate).toLocaleDateString('en-GB')
                                        : '-'}
                                </TableCell>
                                <TableCell>
                                    {req.issuedDate
                                        ? new Date(req.issuedDate).toLocaleDateString('en-GB')
                                        : '-'}
                                </TableCell>
                            </TableRow>
                        ))
                    )}
                </TableBody>
            </Table>
        </TableContainer>
    );
};

interface TransferReportTableProps {
    data: TransferReport[];
}

const TransferReportTable: React.FC<TransferReportTableProps> = ({ data }) => {
    const getStatusColor = (status: string) => {
        switch (status) {
            case 'COMPLETED': return 'success';
            case 'IN_TRANSIT': return 'primary';
            case 'APPROVED': return 'info';
            case 'PENDING': return 'warning';
            case 'REJECTED': return 'error';
            default: return 'default';
        }
    };

    return (
        <TableContainer component={Paper}>
            <Table>
                <TableHead>
                    <TableRow>
                        <TableCell>رقم النقل</TableCell>
                        <TableCell>من</TableCell>
                        <TableCell>إلى</TableCell>
                        <TableCell align="right">الأصناف</TableCell>
                        <TableCell align="right">الكمية</TableCell>
                        <TableCell>الحالة</TableCell>
                        <TableCell>تاريخ الطلب</TableCell>
                        <TableCell>تاريخ الإكتمال</TableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                    {data.length === 0 ? (
                        <TableRow>
                            <TableCell colSpan={8} align="center">
                                <Typography variant="body2" color="textSecondary">
                                    لا توجد بيانات نقل متاحة
                                </Typography>
                            </TableCell>
                        </TableRow>
                    ) : (
                        data.map((transfer) => (
                            <TableRow key={transfer.transferNumber} hover>
                                <TableCell>{transfer.transferNumber}</TableCell>
                                <TableCell>
                                    <Box>
                                        <Typography variant="body2">{transfer.fromWarehouseNameAr}</Typography>
                                        <Typography variant="caption" color="textSecondary">
                                            {transfer.fromWarehouseName}
                                        </Typography>
                                    </Box>
                                </TableCell>
                                <TableCell>
                                    <Box>
                                        <Typography variant="body2">{transfer.toWarehouseNameAr}</Typography>
                                        <Typography variant="caption" color="textSecondary">
                                            {transfer.toWarehouseName}
                                        </Typography>
                                    </Box>
                                </TableCell>
                                <TableCell align="right">{transfer.itemCount}</TableCell>
                                <TableCell align="right">{(transfer.totalQuantity || 0).toLocaleString()}</TableCell>
                                <TableCell>
                                    <Chip
                                        label={transfer.status}
                                        color={getStatusColor(transfer.status) as any}
                                        size="small"
                                    />
                                </TableCell>
                                <TableCell>
                                    {new Date(transfer.requestedDate).toLocaleDateString('en-GB')}
                                </TableCell>
                                <TableCell>
                                    {transfer.completedDate
                                        ? new Date(transfer.completedDate).toLocaleDateString('en-GB')
                                        : '-'}
                                </TableCell>
                            </TableRow>
                        ))
                    )}
                </TableBody>
            </Table>
        </TableContainer>
    );
};

interface ConsumptionReportTableProps {
    data: ConsumptionReport[];
}

const ConsumptionReportTable: React.FC<ConsumptionReportTableProps> = ({ data }) => {
    const getStatusColor = (status: string) => {
        switch (status) {
            case 'ON_TRACK': return 'success';
            case 'NEAR_LIMIT': return 'warning';
            case 'OVER_BUDGET': return 'error';
            default: return 'default';
        }
    };

    return (
        <TableContainer component={Paper}>
            <Table>
                <TableHead>
                    <TableRow>
                        <TableCell>رقم المشروع</TableCell>
                        <TableCell>اسم المشروع</TableCell>
                        <TableCell align="right">الميزانية</TableCell>
                        <TableCell align="right">المستهلك</TableCell>
                        <TableCell align="right">المتبقي</TableCell>
                        <TableCell align="right">النسبة</TableCell>
                        <TableCell align="right">الأصناف</TableCell>
                        <TableCell>الحالة</TableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                    {data.length === 0 ? (
                        <TableRow>
                            <TableCell colSpan={8} align="center">
                                <Typography variant="body2" color="textSecondary">
                                    لا توجد بيانات استهلاك متاحة
                                </Typography>
                            </TableCell>
                        </TableRow>
                    ) : (
                        data.map((project) => (
                            <TableRow key={project.projectId} hover>
                                <TableCell>{project.projectNumber}</TableCell>
                                <TableCell>
                                    <Box>
                                        <Typography variant="body2">{project.projectNameAr}</Typography>
                                        <Typography variant="caption" color="textSecondary">
                                            {project.projectName}
                                        </Typography>
                                    </Box>
                                </TableCell>
                                <TableCell align="right">
                                    {(project.budget || 0).toLocaleString()}
                                </TableCell>
                                <TableCell align="right">
                                    {(project.consumed || 0).toLocaleString()}
                                </TableCell>
                                <TableCell align="right">
                                    {(project.remaining || 0).toLocaleString()}
                                </TableCell>
                                <TableCell align="right">
                                    <Typography
                                        color={
                                            (project.consumptionPercentage || 0) > 100
                                                ? 'error.main'
                                                : (project.consumptionPercentage || 0) > 80
                                                    ? 'warning.main'
                                                    : 'success.main'
                                        }
                                        fontWeight="bold"
                                    >
                                        {(project.consumptionPercentage || 0).toFixed(1)}%
                                    </Typography>
                                </TableCell>
                                <TableCell align="right">{project.itemCount}</TableCell>
                                <TableCell>
                                    <Chip
                                        label={project.status}
                                        color={getStatusColor(project.status) as any}
                                        size="small"
                                    />
                                </TableCell>
                            </TableRow>
                        ))
                    )}
                </TableBody>
            </Table>
        </TableContainer>
    );
};

export default Reports;
