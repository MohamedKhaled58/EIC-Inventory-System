import React, { useState, useEffect, useCallback } from 'react';
import {
    Container,
    Box,
    Typography,
    Paper,
    Grid,
    Card,
    CardContent,
    CardHeader,
    Button,
    IconButton,
    Chip,
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    TextField,
    FormControl,
    InputLabel,
    Select,
    MenuItem,
    FormControlLabel,
    Switch,
    Divider,
    List,
    ListItem,
    ListItemText,
    ListItemSecondaryAction,
    Alert,
    CircularProgress,
    Tooltip,
} from '@mui/material';
import {
    Add as AddIcon,
    Edit as EditIcon,
    Refresh as RefreshIcon,
    Business as FactoryIcon,
    LocationOn as LocationIcon,
    Warehouse as WarehouseIcon,
} from '@mui/icons-material';
import { factoryService, Factory, Department, Warehouse } from '../services/factoryService';

const Factories: React.FC = () => {
    // State for Factories and Departments
    const [factories, setFactories] = useState<Factory[]>([]);
    const [departments, setDepartments] = useState<Department[]>([]);
    const [warehouses, setWarehouses] = useState<Warehouse[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState<string | null>(null);

    // Factory Dialog State
    const [factoryDialogOpen, setFactoryDialogOpen] = useState(false);
    const [editingFactory, setEditingFactory] = useState<Factory | null>(null);
    const [factoryFormData, setFactoryFormData] = useState({
        name: '',
        nameAr: '',
        code: '',
        location: '',
        isActive: true
    });

    // Department Dialog State
    const [departmentDialogOpen, setDepartmentDialogOpen] = useState(false);
    const [editingDepartment, setEditingDepartment] = useState<Department | null>(null);
    const [selectedFactoryId, setSelectedFactoryId] = useState<number | null>(null);
    const [departmentFormData, setDepartmentFormData] = useState({
        name: '',
        nameAr: '',
        code: ''
    });

    // Warehouse Dialog State
    const [warehouseDialogOpen, setWarehouseDialogOpen] = useState(false);
    const [editingWarehouse, setEditingWarehouse] = useState<Warehouse | null>(null);
    const [warehouseFormData, setWarehouseFormData] = useState({
        name: '',
        nameAr: '',
        code: '',
        location: '',
        type: 'Factory' as 'Central' | 'Factory',
        isActive: true
    });

    const loadData = useCallback(async () => {
        setLoading(true);
        setError(null);
        try {
            const [factoriesData, departmentsData, warehousesData] = await Promise.all([
                factoryService.getFactories(),
                factoryService.getDepartments(),
                factoryService.getWarehouses()
            ]);
            setFactories(factoriesData);
            setDepartments(departmentsData);
            setWarehouses(warehousesData);
        } catch (err: unknown) {
            const message = err instanceof Error ? err.message : 'Failed to load data';
            setError(message);
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => {
        loadData();
    }, [loadData]);

    // Factory Handlers
    const handleAddFactory = () => {
        setEditingFactory(null);
        setFactoryFormData({ name: '', nameAr: '', code: '', location: '', isActive: true });
        setFactoryDialogOpen(true);
    };

    const handleEditFactory = (factory: Factory) => {
        setEditingFactory(factory);
        setFactoryFormData({
            name: factory.name,
            nameAr: factory.nameAr,
            code: factory.code,
            location: factory.location,
            isActive: factory.isActive
        });
        setFactoryDialogOpen(true);
    };

    const handleFactorySubmit = async () => {
        try {
            if (editingFactory) {
                await factoryService.updateFactory({
                    id: editingFactory.id,
                    ...factoryFormData
                });
                setSuccess('Factory updated successfully');
            } else {
                await factoryService.createFactory(factoryFormData);
                setSuccess('Factory created successfully');
            }
            setFactoryDialogOpen(false);
            loadData();
        } catch (err: unknown) {
            const message = err instanceof Error ? err.message : 'Failed to save factory';
            setError(message);
        }
    };

    // Department Handlers
    const handleAddDepartment = (factoryId: number) => {
        setSelectedFactoryId(factoryId);
        setEditingDepartment(null);
        setDepartmentFormData({ name: '', nameAr: '', code: '' });
        setDepartmentDialogOpen(true);
    };

    const handleEditDepartment = (department: Department) => {
        setEditingDepartment(department);
        setSelectedFactoryId(department.factoryId);
        setDepartmentFormData({
            name: department.name,
            nameAr: department.nameAr,
            code: department.code
        });
        setDepartmentDialogOpen(true);
    };

    const handleDepartmentSubmit = async () => {
        if (!selectedFactoryId) return;
        try {
            if (editingDepartment) {
                await factoryService.updateDepartment({
                    id: editingDepartment.id,
                    factoryId: selectedFactoryId,
                    ...departmentFormData
                });
                setSuccess('Department updated successfully');
            } else {
                await factoryService.createDepartment({
                    factoryId: selectedFactoryId,
                    ...departmentFormData
                });
                setSuccess('Department created successfully');
            }
            setDepartmentDialogOpen(false);
            loadData();
        } catch (err: unknown) {
            const message = err instanceof Error ? err.message : 'Failed to save department';
            setError(message);
        }
    };

    // Warehouse Handlers
    const handleAddWarehouse = (factoryId: number) => {
        setSelectedFactoryId(factoryId);
        setEditingWarehouse(null);
        setWarehouseFormData({
            name: '',
            nameAr: '',
            code: '',
            location: '',
            type: 'Factory',
            isActive: true
        });
        setWarehouseDialogOpen(true);
    };

    const handleEditWarehouse = (warehouse: Warehouse) => {
        setEditingWarehouse(warehouse);
        setSelectedFactoryId(warehouse.factoryId ?? null);
        setWarehouseFormData({
            name: warehouse.name,
            nameAr: warehouse.nameAr,
            code: warehouse.code,
            location: warehouse.location,
            type: warehouse.type,
            isActive: warehouse.isActive
        });
        setWarehouseDialogOpen(true);
    };

    const handleWarehouseSubmit = async () => {
        if (!selectedFactoryId) return;
        try {
            if (editingWarehouse) {
                await factoryService.updateWarehouse({
                    id: editingWarehouse.id,
                    factoryId: selectedFactoryId,
                    ...warehouseFormData
                });
                setSuccess('Warehouse updated successfully');
            } else {
                await factoryService.createWarehouse({
                    factoryId: selectedFactoryId,
                    ...warehouseFormData
                });
                setSuccess('Warehouse created successfully');
            }
            setWarehouseDialogOpen(false);
            loadData();
        } catch (err: unknown) {
            const message = err instanceof Error ? err.message : 'Failed to save warehouse';
            setError(message);
        }
    };

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
                        المصانع والأقسام والمستودعات
                    </Typography>
                    <Typography variant="body1" color="textSecondary">
                        Factories, Departments & Warehouses Management
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
                        onClick={handleAddFactory}
                    >
                        إضافة مصنع جديد
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

            {/* Statistics Cards */}
            <Grid container spacing={2} mb={3}>
                <Grid item xs={12} sm={6} md={3}>
                    <Card>
                        <CardContent>
                            <Typography variant="h4" color="primary">{factories.length}</Typography>
                            <Typography variant="body2" color="textSecondary">المصانع / Factories</Typography>
                        </CardContent>
                    </Card>
                </Grid>
                <Grid item xs={12} sm={6} md={3}>
                    <Card>
                        <CardContent>
                            <Typography variant="h4" color="info.main">{departments.length}</Typography>
                            <Typography variant="body2" color="textSecondary">الأقسام / Departments</Typography>
                        </CardContent>
                    </Card>
                </Grid>
                <Grid item xs={12} sm={6} md={3}>
                    <Card>
                        <CardContent>
                            <Typography variant="h4" color="success.main">{warehouses.length}</Typography>
                            <Typography variant="body2" color="textSecondary">المستودعات / Warehouses</Typography>
                        </CardContent>
                    </Card>
                </Grid>
                <Grid item xs={12} sm={6} md={3}>
                    <Card>
                        <CardContent>
                            <Typography variant="h4" color="warning.main">
                                {factories.filter(f => f.isActive).length}
                            </Typography>
                            <Typography variant="body2" color="textSecondary">نشط / Active</Typography>
                        </CardContent>
                    </Card>
                </Grid>
            </Grid>

            {/* Factories Grid */}
            <Grid container spacing={3}>
                {factories.map((factory) => (
                    <Grid item xs={12} md={6} key={factory.id}>
                        <Card elevation={3}>
                            <CardHeader
                                avatar={
                                    <FactoryIcon color="primary" fontSize="large" />
                                }
                                action={
                                    <Tooltip title="Edit Factory">
                                        <IconButton onClick={() => handleEditFactory(factory)}>
                                            <EditIcon />
                                        </IconButton>
                                    </Tooltip>
                                }
                                title={
                                    <Box display="flex" alignItems="center" gap={1}>
                                        <Typography variant="h6">{factory.nameAr}</Typography>
                                        <Chip
                                            label={factory.code}
                                            size="small"
                                            variant="outlined"
                                        />
                                        {!factory.isActive && (
                                            <Chip label="غير نشط" color="error" size="small" />
                                        )}
                                    </Box>
                                }
                                subheader={factory.name}
                            />
                            <CardContent>
                                {factory.location && (
                                    <Box display="flex" alignItems="center" gap={1} mb={2} color="text.secondary">
                                        <LocationIcon fontSize="small" />
                                        <Typography variant="body2">{factory.location}</Typography>
                                    </Box>
                                )}

                                <Divider sx={{ my: 2 }} />

                                {/* Departments Section */}
                                <Box display="flex" justifyContent="space-between" alignItems="center" mb={1}>
                                    <Typography variant="subtitle1" fontWeight="bold">
                                        الأقسام / Departments
                                    </Typography>
                                    <Tooltip title="Add Department">
                                        <Button
                                            size="small"
                                            startIcon={<AddIcon />}
                                            onClick={() => handleAddDepartment(factory.id)}
                                        >
                                            إضافة
                                        </Button>
                                    </Tooltip>
                                </Box>

                                <List dense>
                                    {departments
                                        .filter(d => d.factoryId === factory.id)
                                        .map((dept) => (
                                            <ListItem key={dept.id} divider>
                                                <ListItemText
                                                    primary={dept.nameAr}
                                                    secondary={`${dept.name} (${dept.code})`}
                                                />
                                                <ListItemSecondaryAction>
                                                    <Tooltip title="Edit Department">
                                                        <IconButton size="small" onClick={() => handleEditDepartment(dept)}>
                                                            <EditIcon fontSize="small" />
                                                        </IconButton>
                                                    </Tooltip>
                                                </ListItemSecondaryAction>
                                            </ListItem>
                                        ))}
                                    {departments.filter(d => d.factoryId === factory.id).length === 0 && (
                                        <Typography variant="body2" color="text.secondary" align="center" sx={{ py: 2 }}>
                                            لا يوجد أقسام / No departments
                                        </Typography>
                                    )}
                                </List>

                                <Divider sx={{ my: 2 }} />

                                {/* Warehouses Section */}
                                <Box display="flex" justifyContent="space-between" alignItems="center" mb={1}>
                                    <Typography variant="subtitle1" fontWeight="bold">
                                        المستودعات / Warehouses
                                    </Typography>
                                    <Tooltip title="Add Warehouse">
                                        <Button
                                            size="small"
                                            startIcon={<AddIcon />}
                                            onClick={() => handleAddWarehouse(factory.id)}
                                        >
                                            إضافة
                                        </Button>
                                    </Tooltip>
                                </Box>

                                <List dense>
                                    {warehouses
                                        .filter(w => w.factoryId === factory.id)
                                        .map((wh) => (
                                            <ListItem key={wh.id} divider>
                                                <ListItemText
                                                    primary={
                                                        <Box display="flex" alignItems="center" gap={1}>
                                                            <WarehouseIcon fontSize="small" color="action" />
                                                            <span>{wh.nameAr}</span>
                                                            <Chip
                                                                label={wh.type === 'Central' ? 'مركزي' : 'مصنع'}
                                                                size="small"
                                                                color={wh.type === 'Central' ? 'primary' : 'default'}
                                                            />
                                                        </Box>
                                                    }
                                                    secondary={`${wh.name} (${wh.code})`}
                                                />
                                                <ListItemSecondaryAction>
                                                    <Tooltip title="Edit Warehouse">
                                                        <IconButton size="small" onClick={() => handleEditWarehouse(wh)}>
                                                            <EditIcon fontSize="small" />
                                                        </IconButton>
                                                    </Tooltip>
                                                </ListItemSecondaryAction>
                                            </ListItem>
                                        ))}
                                    {warehouses.filter(w => w.factoryId === factory.id).length === 0 && (
                                        <Typography variant="body2" color="text.secondary" align="center" sx={{ py: 2 }}>
                                            لا يوجد مستودعات / No warehouses
                                        </Typography>
                                    )}
                                </List>
                            </CardContent>
                        </Card>
                    </Grid>
                ))}
            </Grid>

            {factories.length === 0 && (
                <Paper sx={{ p: 4, textAlign: 'center' }}>
                    <Typography variant="h6" color="textSecondary">
                        لا يوجد مصانع / No factories found
                    </Typography>
                    <Button
                        variant="contained"
                        startIcon={<AddIcon />}
                        onClick={handleAddFactory}
                        sx={{ mt: 2 }}
                    >
                        إضافة مصنع جديد
                    </Button>
                </Paper>
            )}

            {/* Factory Dialog */}
            <Dialog open={factoryDialogOpen} onClose={() => setFactoryDialogOpen(false)} maxWidth="sm" fullWidth>
                <DialogTitle>
                    <Box display="flex" alignItems="center" gap={1}>
                        <FactoryIcon />
                        <Typography>{editingFactory ? 'تعديل مصنع / Edit Factory' : 'إضافة مصنع جديد / Add New Factory'}</Typography>
                    </Box>
                </DialogTitle>
                <DialogContent>
                    <Grid container spacing={2} sx={{ mt: 1 }}>
                        <Grid item xs={12} md={6}>
                            <TextField
                                label="كود المصنع / Factory Code"
                                value={factoryFormData.code}
                                onChange={(e) => setFactoryFormData({ ...factoryFormData, code: e.target.value })}
                                fullWidth
                                required
                            />
                        </Grid>
                        <Grid item xs={12} md={6}>
                            <TextField
                                label="الموقع / Location"
                                value={factoryFormData.location}
                                onChange={(e) => setFactoryFormData({ ...factoryFormData, location: e.target.value })}
                                fullWidth
                            />
                        </Grid>
                        <Grid item xs={12} md={6}>
                            <TextField
                                label="الاسم بالعربي / Name (Arabic)"
                                value={factoryFormData.nameAr}
                                onChange={(e) => setFactoryFormData({ ...factoryFormData, nameAr: e.target.value })}
                                fullWidth
                                required
                            />
                        </Grid>
                        <Grid item xs={12} md={6}>
                            <TextField
                                label="الاسم بالإنجليزي / Name (English)"
                                value={factoryFormData.name}
                                onChange={(e) => setFactoryFormData({ ...factoryFormData, name: e.target.value })}
                                fullWidth
                            />
                        </Grid>
                        <Grid item xs={12}>
                            <FormControlLabel
                                control={
                                    <Switch
                                        checked={factoryFormData.isActive}
                                        onChange={(e) => setFactoryFormData({ ...factoryFormData, isActive: e.target.checked })}
                                    />
                                }
                                label="نشط / Active"
                            />
                        </Grid>
                    </Grid>
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setFactoryDialogOpen(false)}>إلغاء / Cancel</Button>
                    <Button onClick={handleFactorySubmit} variant="contained" disabled={!factoryFormData.code || !factoryFormData.nameAr}>
                        حفظ / Save
                    </Button>
                </DialogActions>
            </Dialog>

            {/* Department Dialog */}
            <Dialog open={departmentDialogOpen} onClose={() => setDepartmentDialogOpen(false)} maxWidth="sm" fullWidth>
                <DialogTitle>
                    <Box display="flex" alignItems="center" gap={1}>
                        <FactoryIcon />
                        <Typography>{editingDepartment ? 'تعديل قسم / Edit Department' : 'إضافة قسم جديد / Add New Department'}</Typography>
                    </Box>
                </DialogTitle>
                <DialogContent>
                    <Grid container spacing={2} sx={{ mt: 1 }}>
                        <Grid item xs={12}>
                            <TextField
                                label="كود القسم / Department Code"
                                value={departmentFormData.code}
                                onChange={(e) => setDepartmentFormData({ ...departmentFormData, code: e.target.value })}
                                fullWidth
                                required
                            />
                        </Grid>
                        <Grid item xs={12} md={6}>
                            <TextField
                                label="الاسم بالعربي / Name (Arabic)"
                                value={departmentFormData.nameAr}
                                onChange={(e) => setDepartmentFormData({ ...departmentFormData, nameAr: e.target.value })}
                                fullWidth
                                required
                            />
                        </Grid>
                        <Grid item xs={12} md={6}>
                            <TextField
                                label="الاسم بالإنجليزي / Name (English)"
                                value={departmentFormData.name}
                                onChange={(e) => setDepartmentFormData({ ...departmentFormData, name: e.target.value })}
                                fullWidth
                            />
                        </Grid>
                    </Grid>
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setDepartmentDialogOpen(false)}>إلغاء / Cancel</Button>
                    <Button onClick={handleDepartmentSubmit} variant="contained" disabled={!departmentFormData.code || !departmentFormData.nameAr}>
                        حفظ / Save
                    </Button>
                </DialogActions>
            </Dialog>

            {/* Warehouse Dialog */}
            <Dialog open={warehouseDialogOpen} onClose={() => setWarehouseDialogOpen(false)} maxWidth="sm" fullWidth>
                <DialogTitle>
                    <Box display="flex" alignItems="center" gap={1}>
                        <WarehouseIcon />
                        <Typography>{editingWarehouse ? 'تعديل مستودع / Edit Warehouse' : 'إضافة مستودع جديد / Add New Warehouse'}</Typography>
                    </Box>
                </DialogTitle>
                <DialogContent>
                    <Grid container spacing={2} sx={{ mt: 1 }}>
                        <Grid item xs={12} md={6}>
                            <TextField
                                label="كود المستودع / Warehouse Code"
                                value={warehouseFormData.code}
                                onChange={(e) => setWarehouseFormData({ ...warehouseFormData, code: e.target.value })}
                                fullWidth
                                required
                            />
                        </Grid>
                        <Grid item xs={12} md={6}>
                            <FormControl fullWidth>
                                <InputLabel>نوع المستودع / Warehouse Type</InputLabel>
                                <Select
                                    value={warehouseFormData.type}
                                    label="نوع المستودع / Warehouse Type"
                                    onChange={(e) => setWarehouseFormData({ ...warehouseFormData, type: e.target.value as 'Central' | 'Factory' })}
                                >
                                    <MenuItem value="Factory">مستودع مصنع / Factory</MenuItem>
                                    <MenuItem value="Central">مستودع مركزي / Central</MenuItem>
                                </Select>
                            </FormControl>
                        </Grid>
                        <Grid item xs={12} md={6}>
                            <TextField
                                label="الاسم بالعربي / Name (Arabic)"
                                value={warehouseFormData.nameAr}
                                onChange={(e) => setWarehouseFormData({ ...warehouseFormData, nameAr: e.target.value })}
                                fullWidth
                                required
                            />
                        </Grid>
                        <Grid item xs={12} md={6}>
                            <TextField
                                label="الاسم بالإنجليزي / Name (English)"
                                value={warehouseFormData.name}
                                onChange={(e) => setWarehouseFormData({ ...warehouseFormData, name: e.target.value })}
                                fullWidth
                            />
                        </Grid>
                        <Grid item xs={12}>
                            <TextField
                                label="الموقع / Location"
                                value={warehouseFormData.location}
                                onChange={(e) => setWarehouseFormData({ ...warehouseFormData, location: e.target.value })}
                                fullWidth
                            />
                        </Grid>
                        <Grid item xs={12}>
                            <FormControlLabel
                                control={
                                    <Switch
                                        checked={warehouseFormData.isActive}
                                        onChange={(e) => setWarehouseFormData({ ...warehouseFormData, isActive: e.target.checked })}
                                    />
                                }
                                label="نشط / Active"
                            />
                        </Grid>
                    </Grid>
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setWarehouseDialogOpen(false)}>إلغاء / Cancel</Button>
                    <Button onClick={handleWarehouseSubmit} variant="contained" disabled={!warehouseFormData.code || !warehouseFormData.nameAr}>
                        حفظ / Save
                    </Button>
                </DialogActions>
            </Dialog>
        </Container>
    );
};

export default Factories;
