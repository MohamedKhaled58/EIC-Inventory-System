import React, { useState, useEffect } from 'react';
import {
    Box,
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
    Tabs,
    Tab,
    Card,
    CardContent,
    Grid,
    Alert,
    CircularProgress,
    Divider
} from '@mui/material';
import {
    Download as DownloadIcon,
    TrendingUp as TrendingUpIcon,
    TrendingDown as TrendingDownIcon,
    Inventory as InventoryIcon,
    Warning as WarningIcon
} from '@mui/icons-material';
import { useLocation, useParams } from 'react-router-dom';
import { apiClient } from '../services/apiClient';
import type { Item } from '../types';

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
            id={`item-tabpanel-${index}`}
            aria-labelledby={`item-tab-${index}`}
            {...other}
        >
            {value === index && <Box sx={{ py: 3 }}>{children}</Box>}
        </div>
    );
}

// Helper functions used by multiple components
const getStockStatus = (available: number, minimumStock: number) => {
    if (available > minimumStock) return { color: 'success' as const, text: 'متاح' };
    if (available > 0 && available <= minimumStock) return { color: 'warning' as const, text: 'منخفض' };
    if (available <= 0) return { color: 'error' as const, text: 'غير متاح' };
    return { color: 'default' as const, text: 'غير معروف' };
};

const getTransactionColor = (type: string) => {
    switch (type) {
        case 'Issue': return 'error';
        case 'Receipt': return 'success';
        case 'Transfer': return 'info';
        case 'Return': return 'warning';
        case 'Adjustment': return 'secondary';
        default: return 'default';
    }
};

const getTransactionTypeAr = (type: string) => {
    switch (type) {
        case 'Issue': return 'إصدار';
        case 'Receipt': return 'استلام';
        case 'Transfer': return 'نقل';
        case 'Return': return 'إرجاع';
        case 'Adjustment': return 'تعديل';
        default: return type;
    }
};

const ItemDetails: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const [activeTab, setActiveTab] = useState(0);
    const [item, setItem] = useState<Item | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        loadItemDetails();
    }, [id]);

    const loadItemDetails = async () => {
        try {
            setLoading(true);
            const response = await apiClient.get<{
                item: Item;
                inventoryRecords: any[];
                projectAllocations: any[];
                transactionHistory: any[];
                consumptionStats: any;
            }>(`/inventory/items/${id}/details`);

            // apiClient returns ApiResponse wrapper
            const data = response as any;
            if (data.success && data.data) {
                setItem(data.data.item);
            } else if (data.item) {
                // Direct response without wrapper
                setItem(data.item);
            } else {
                setError(data.message || 'Failed to load item details');
            }
        } catch (err) {
            setError('Error loading item details');
            console.error('Error loading item details:', err);
        } finally {
            setLoading(false);
        }
    };

    const handleTabChange = (event: React.SyntheticEvent, newValue: number) => {
        setActiveTab(newValue);
    };

    const handleExport = async () => {
        try {
            const response = await apiClient.get<any>(`/inventory/items/${id}/export`);
            const data = response as any;
            if (data.success || data.data) {
                const exportData = data.data || data;
                const url = window.URL.createObjectURL(new Blob([exportData], { type: 'text/csv' }));
                const link = document.createElement('a');
                link.href = url;
                link.download = `item-details-${item?.code || id}.csv`;
                link.click();
            }
        } catch (err) {
            console.error('Export error:', err);
        }
    };

    if (loading) {
        return (
            <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '60vh' }}>
                <CircularProgress />
            </Box>
        );
    }

    if (error) {
        return (
            <Box sx={{ p: 3 }}>
                <Alert severity="error">{error}</Alert>
            </Box>
        );
    }

    if (!item) {
        return null;
    }

    return (
        <Box sx={{ p: 3 }}>
            {/* Header */}
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
                <Box>
                    <Typography variant="h4" gutterBottom>
                        {item.code} - {item.nameAr}
                    </Typography>
                    <Typography variant="subtitle1" color="text.secondary">
                        {item.name}
                    </Typography>
                </Box>
                <Box sx={{ display: 'flex', gap: 1, alignItems: 'center' }}>
                    <Chip label={item.isActive ? 'نشط' : 'غير نشط'} size="medium" color={item.isActive ? 'success' : 'default'} />
                    {item.isCritical && (
                        <Chip label="حرج" size="medium" color="error" />
                    )}
                    <Button
                        variant="outlined"
                        startIcon={<DownloadIcon />}
                        onClick={handleExport}
                    >
                        تصدير
                    </Button>
                </Box>
            </Box>

            {/* Tabs */}
            <Paper sx={{ mb: 3 }}>
                <Tabs
                    value={activeTab}
                    onChange={handleTabChange}
                    aria-label="item details tabs"
                    sx={{ borderBottom: 1, borderColor: 'divider' }}
                >
                    <Tab label="معلومات أساسية" />
                    <Tab label="المخزون" />
                    <Tab label="تخصيصات المشاريع" />
                    <Tab label="تاريخ الحركات" />
                    <Tab label="تحليل الاستهلاك" />
                </Tabs>
            </Paper>

            {/* Tab Content */}
            <Box sx={{ mt: 2 }}>
                <TabPanel value={activeTab} index={0}>
                    <BasicInformationTab item={item} />
                </TabPanel>
                <TabPanel value={activeTab} index={1}>
                    <StockTab item={item} />
                </TabPanel>
                <TabPanel value={activeTab} index={2}>
                    <ProjectAllocationsTab item={item} />
                </TabPanel>
                <TabPanel value={activeTab} index={3}>
                    <TransactionHistoryTab item={item} />
                </TabPanel>
                <TabPanel value={activeTab} index={4}>
                    <ConsumptionAnalysisTab item={item} />
                </TabPanel>
            </Box>
        </Box>
    );
};

// Basic Information Tab Component
const BasicInformationTab: React.FC<{ item: Item }> = ({ item }) => {
    return (
        <Card>
            <CardContent>
                <Typography variant="h6" gutterBottom>
                    معلومات الصنف
                </Typography>
                <Grid container spacing={2}>
                    <Grid item xs={12} md={6}>
                        <Typography variant="body1" fontWeight="bold">
                            الكود:
                        </Typography>
                        <Typography variant="body2">
                            {item.code}
                        </Typography>
                    </Grid>
                    <Grid item xs={12} md={6}>
                        <Typography variant="body1" fontWeight="bold">
                            الاسم (عربي):
                        </Typography>
                        <Typography variant="body2">
                            {item.nameAr}
                        </Typography>
                    </Grid>
                    <Grid item xs={12} md={6}>
                        <Typography variant="body1" fontWeight="bold">
                            الاسم (إنجليزي):
                        </Typography>
                        <Typography variant="body2">
                            {item.name}
                        </Typography>
                    </Grid>
                    <Grid item xs={12} md={6}>
                        <Typography variant="body1" fontWeight="bold">
                            التصنيف:
                        </Typography>
                        <Typography variant="body2">
                            {item.categoryNameAr || item.category}
                        </Typography>
                    </Grid>
                    <Grid item xs={12} md={6}>
                        <Typography variant="body1" fontWeight="bold">
                            الوحدة:
                        </Typography>
                        <Typography variant="body2">
                            {item.unitAr || item.unit}
                        </Typography>
                    </Grid>
                    <Grid item xs={12} md={6}>
                        <Typography variant="body1" fontWeight="bold">
                            السعر:
                        </Typography>
                        <Typography variant="body2">
                            {item.unitPrice ? `${item.unitPrice} ج.م` : 'غير معروف'}
                        </Typography>
                    </Grid>
                    <Grid item xs={12} md={6}>
                        <Typography variant="body1" fontWeight="bold">
                            المخزون الأدنى:
                        </Typography>
                        <Typography variant="body2">
                            {item.minimumStock} {item.unitAr || item.unit}
                        </Typography>
                    </Grid>
                    <Grid item xs={12} md={6}>
                        <Typography variant="body1" fontWeight="bold">
                            نسبة الاحتياطي:
                        </Typography>
                        <Typography variant="body2">
                            {item.reservePercentage}%
                        </Typography>
                    </Grid>
                    <Grid item xs={12} md={6}>
                        <Typography variant="body1" fontWeight="bold">
                            الحالة:
                        </Typography>
                        <Chip
                            label={item.isActive ? 'نشط' : 'غير نشط'}
                            color={item.isActive ? 'success' : 'default'}
                        />
                    </Grid>
                </Grid>
            </CardContent>
        </Card>
    );
};

// Stock Tab Component
const StockTab: React.FC<{ item: Item }> = ({ item }) => {
    return (
        <Card>
            <CardContent>
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
                            {(item as any).inventoryRecords?.map((record: any, index: number) => (
                                <TableRow key={index}>
                                    <TableCell>{record.warehouseNameAr || record.warehouseName}</TableCell>
                                    <TableCell>{record.generalQuantity || 0}</TableCell>
                                    <TableCell>{record.commanderReserveQuantity || 0}</TableCell>
                                    <TableCell>{record.generalAllocated || 0}</TableCell>
                                    <TableCell>
                                        <Typography
                                            color={getStockStatus(record.availableQuantity, item.minimumStock).color}
                                            fontWeight="bold"
                                        >
                                            {record.availableQuantity || 0}
                                        </Typography>
                                    </TableCell>
                                    <TableCell>
                                        <Chip
                                            label={getStockStatus(record.availableQuantity, item.minimumStock).text}
                                            color={getStockStatus(record.availableQuantity, item.minimumStock).color}
                                            size="small"
                                        />
                                    </TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </TableContainer>
            </CardContent>
        </Card>
    );
};

// Project Allocations Tab Component
const ProjectAllocationsTab: React.FC<{ item: Item }> = ({ item }) => {
    return (
        <Card>
            <CardContent>
                <Typography variant="h6" gutterBottom>
                    تخصيصات المشاريع
                </Typography>
                <TableContainer component={Paper}>
                    <Table>
                        <TableHead>
                            <TableRow>
                                <TableCell>المشروع</TableCell>
                                <TableCell>الكمية المخصصة</TableCell>
                                <TableCell>المستهلكة</TableCell>
                                <TableCell>المتبقي</TableCell>
                                <TableCell>تاريخ التخصيص</TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {(item as any).projectAllocations?.map((allocation: any, index: number) => (
                                <TableRow key={index}>
                                    <TableCell>{allocation.projectNameAr || allocation.projectName}</TableCell>
                                    <TableCell>{allocation.allocatedQuantity || 0}</TableCell>
                                    <TableCell>{allocation.consumedQuantity || 0}</TableCell>
                                    <TableCell>
                                        <Typography
                                            color={allocation.remainingQuantity > 0 ? 'success' : 'error'}
                                            fontWeight="bold"
                                        >
                                            {allocation.remainingQuantity || 0}
                                        </Typography>
                                    </TableCell>
                                    <TableCell>{allocation.createdAt ? new Date(allocation.createdAt).toLocaleDateString('ar-EG') : '-'}</TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </TableContainer>
            </CardContent>
        </Card>
    );
};

// Transaction History Tab Component
const TransactionHistoryTab: React.FC<{ item: Item }> = ({ item }) => {
    return (
        <Card>
            <CardContent>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
                    <Typography variant="h6">
                        تاريخ الحركات
                    </Typography>
                    <Button
                        variant="outlined"
                        startIcon={<DownloadIcon />}
                        size="small"
                    >
                        تصدير
                    </Button>
                </Box>
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
                            {(item as any).transactionHistory?.slice(0, 10).map((transaction: any, index: number) => (
                                <TableRow key={index}>
                                    <TableCell>{transaction.transactionDate ? new Date(transaction.transactionDate).toLocaleDateString('ar-EG') : '-'}</TableCell>
                                    <TableCell>
                                        <Chip
                                            label={getTransactionTypeAr(transaction.type)}
                                            color={getTransactionColor(transaction.type) as any}
                                            size="small"
                                        />
                                    </TableCell>
                                    <TableCell>
                                        <Typography
                                            color={transaction.quantity < 0 ? 'error' : 'success'}
                                            fontWeight="bold"
                                        >
                                            {transaction.quantity > 0 ? '+' : ''}{transaction.quantity}
                                        </Typography>
                                    </TableCell>
                                    <TableCell>{transaction.warehouseNameAr || transaction.warehouseName}</TableCell>
                                    <TableCell>{transaction.referenceNumber || '-'}</TableCell>
                                    <TableCell>{transaction.userName || '-'}</TableCell>
                                    <TableCell>{transaction.notes || '-'}</TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </TableContainer>
            </CardContent>
        </Card>
    );
};

// Consumption Analysis Tab Component
const ConsumptionAnalysisTab: React.FC<{ item: Item }> = ({ item }) => {
    const consumptionStats = (item as any).consumptionStats || {};

    return (
        <Grid container spacing={2}>
            <Grid item xs={12} md={6}>
                <Card>
                    <CardContent>
                        <Typography variant="subtitle1" gutterBottom>
                            إحصائيات الاستهلاك
                        </Typography>
                        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
                            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                                <InventoryIcon sx={{ fontSize: 40, color: 'primary.main' }} />
                                <Box>
                                    <Typography variant="h4">
                                        {consumptionStats.totalConsumption || 0}
                                    </Typography>
                                    <Typography variant="body2" color="text.secondary">
                                        إجمالي الاستهلاك
                                    </Typography>
                                </Box>
                            </Box>
                            <Divider />
                            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                                <TrendingUpIcon sx={{ fontSize: 32, color: 'success.main' }} />
                                <Box>
                                    <Typography variant="h4">
                                        {consumptionStats.monthlyConsumptionRate || 0}
                                    </Typography>
                                    <Typography variant="body2" color="text.secondary">
                                        معدل الاستهلاك الشهري
                                    </Typography>
                                </Box>
                            </Box>
                            <Divider />
                            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                                <WarningIcon sx={{ fontSize: 32, color: 'warning.main' }} />
                                <Box>
                                    <Typography variant="h4">
                                        {consumptionStats.minConsumption || 0} - {consumptionStats.maxConsumption || 0}
                                    </Typography>
                                    <Typography variant="body2" color="text.secondary">
                                        نطاق الاستهلاك (أدنى - أعلى)
                                    </Typography>
                                </Box>
                            </Box>
                        </Box>
                    </CardContent>
                </Card>
            </Grid>
            <Grid item xs={12} md={6}>
                <Card>
                    <CardContent>
                        <Typography variant="subtitle1" gutterBottom>
                            التنبؤ
                        </Typography>
                        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
                            <Box>
                                <Typography variant="h4">
                                    {consumptionStats.daysOfStock || 0} يوم
                                </Typography>
                                <Typography variant="body2" color="text.secondary">
                                    المخزون المتوقع
                                </Typography>
                                <Typography variant="caption" color="text.secondary">
                                    بناء على معدل الاستهلاك الحالي
                                </Typography>
                            </Box>
                            <Divider />
                            <Box>
                                <Typography variant="h4">
                                    {consumptionStats.estimatedStockoutDate
                                        ? new Date(consumptionStats.estimatedStockoutDate).toLocaleDateString('ar-EG')
                                        : 'غير معروف'}
                                </Typography>
                                <Typography variant="body2" color="text.secondary">
                                    تاريخ نفاد المخزون المتوقع
                                </Typography>
                            </Box>
                        </Box>
                    </CardContent>
                </Card>
            </Grid>
        </Grid>
    );
};

export default ItemDetails;
