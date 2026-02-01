import React, { useState, useEffect } from 'react';
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
    FormControlLabel,
    Switch,
    Divider,
    List,
    ListItem,
    ListItemText,
    ListItemSecondaryAction,
    Tooltip,
    Alert,
    CircularProgress
} from '@mui/material';
import {
    Add as AddIcon,
    Edit as EditIcon,
    Business as FactoryIcon,
    Category as DepartmentIcon,
    LocationOn as LocationIcon
} from '@mui/icons-material';
import { factoryService, Factory, Department } from '../services/factoryService';

const Factories: React.FC = () => {
    const [factories, setFactories] = useState<Factory[]>([]);
    const [departments, setDepartments] = useState<Department[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

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

    const loadData = async () => {
        setLoading(true);
        setError(null);
        try {
            const [factoriesData, departmentsData] = await Promise.all([
                factoryService.getFactories(),
                factoryService.getDepartments()
            ]);
            setFactories(factoriesData);
            setDepartments(departmentsData);
        } catch (err: any) {
            setError(err.message || 'Failed to load data');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadData();
    }, []);

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
            } else {
                await factoryService.createFactory(factoryFormData);
            }
            setFactoryDialogOpen(false);
            loadData();
        } catch (err: any) {
            setError(err.message || 'Failed to save factory');
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
            } else {
                await factoryService.createDepartment({
                    factoryId: selectedFactoryId,
                    ...departmentFormData
                });
            }
            setDepartmentDialogOpen(false);
            loadData();
        } catch (err: any) {
            setError(err.message || 'Failed to save department');
        }
    };

    if (loading && !factories.length) {
        return (
            <Box display="flex" justifyContent="center" alignItems="center" minHeight="60vh">
                <CircularProgress />
            </Box>
        );
    }

    return (
        <Container maxWidth="xl" sx={{ mt: 3, mb: 3 }}>
            <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
                <Box>
                    <Typography variant="h4" gutterBottom>
                        Factories & Departments / المصانع والأقسام
                    </Typography>
                    <Typography variant="body1" color="textSecondary">
                        Manage your organization structure
                    </Typography>
                </Box>
                <Button
                    variant="contained"
                    startIcon={<AddIcon />}
                    onClick={handleAddFactory}
                >
                    Add Factory
                </Button>
            </Box>

            {error && (
                <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError(null)}>
                    {error}
                </Alert>
            )}

            <Grid container spacing={3}>
                {factories.map((factory) => (
                    <Grid item xs={12} md={6} key={factory.id}>
                        <Card elevation={3}>
                            <CardHeader
                                avatar={
                                    <FactoryIcon color="primary" fontSize="large" />
                                }
                                action={
                                    <IconButton onClick={() => handleEditFactory(factory)}>
                                        <EditIcon />
                                    </IconButton>
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
                                            <Chip label="Inactive" color="error" size="small" />
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

                                <Box display="flex" justifyContent="space-between" alignItems="center" mb={1}>
                                    <Typography variant="subtitle1" fontWeight="bold">
                                        Departments / الأقسام
                                    </Typography>
                                    <Button
                                        size="small"
                                        startIcon={<AddIcon />}
                                        onClick={() => handleAddDepartment(factory.id)}
                                    >
                                        Add Dept
                                    </Button>
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
                                                    <IconButton size="small" onClick={() => handleEditDepartment(dept)}>
                                                        <EditIcon fontSize="small" />
                                                    </IconButton>
                                                </ListItemSecondaryAction>
                                            </ListItem>
                                        ))}
                                    {departments.filter(d => d.factoryId === factory.id).length === 0 && (
                                        <Typography variant="body2" color="text.secondary" align="center" sx={{ py: 2 }}>
                                            No departments added yet
                                        </Typography>
                                    )}
                                </List>
                            </CardContent>
                        </Card>
                    </Grid>
                ))}
            </Grid>

            {/* Factory Dialog */}
            <Dialog open={factoryDialogOpen} onClose={() => setFactoryDialogOpen(false)} maxWidth="sm" fullWidth>
                <DialogTitle>{editingFactory ? 'Edit Factory' : 'Add New Factory'}</DialogTitle>
                <DialogContent>
                    <Box display="flex" flexDirection="column" gap={2} mt={1}>
                        <TextField
                            label="Factory Code"
                            value={factoryFormData.code}
                            onChange={(e) => setFactoryFormData({ ...factoryFormData, code: e.target.value })}
                            fullWidth
                            required
                        />
                        <TextField
                            label="Name (Arabic)"
                            value={factoryFormData.nameAr}
                            onChange={(e) => setFactoryFormData({ ...factoryFormData, nameAr: e.target.value })} // Fixed: was updating 'name'
                            fullWidth
                            required
                        />
                        <TextField
                            label="Name (English)"
                            value={factoryFormData.name}
                            onChange={(e) => setFactoryFormData({ ...factoryFormData, name: e.target.value })}
                            fullWidth
                        />
                        <TextField
                            label="Location"
                            value={factoryFormData.location}
                            onChange={(e) => setFactoryFormData({ ...factoryFormData, location: e.target.value })}
                            fullWidth
                        />
                        <FormControlLabel
                            control={
                                <Switch
                                    checked={factoryFormData.isActive}
                                    onChange={(e) => setFactoryFormData({ ...factoryFormData, isActive: e.target.checked })}
                                />
                            }
                            label="Active"
                        />
                    </Box>
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setFactoryDialogOpen(false)}>Cancel</Button>
                    <Button onClick={handleFactorySubmit} variant="contained" disabled={!factoryFormData.code || !factoryFormData.nameAr}>
                        Save
                    </Button>
                </DialogActions>
            </Dialog>

            {/* Department Dialog */}
            <Dialog open={departmentDialogOpen} onClose={() => setDepartmentDialogOpen(false)} maxWidth="sm" fullWidth>
                <DialogTitle>{editingDepartment ? 'Edit Department' : 'Add New Department'}</DialogTitle>
                <DialogContent>
                    <Box display="flex" flexDirection="column" gap={2} mt={1}>
                        <TextField
                            label="Department Code"
                            value={departmentFormData.code}
                            onChange={(e) => setDepartmentFormData({ ...departmentFormData, code: e.target.value })}
                            fullWidth
                            required
                        />
                        <TextField
                            label="Name (Arabic)"
                            value={departmentFormData.nameAr}
                            onChange={(e) => setDepartmentFormData({ ...departmentFormData, nameAr: e.target.value })}
                            fullWidth
                            required
                        />
                        <TextField
                            label="Name (English)"
                            value={departmentFormData.name}
                            onChange={(e) => setDepartmentFormData({ ...departmentFormData, name: e.target.value })}
                            fullWidth
                        />
                    </Box>
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setDepartmentDialogOpen(false)}>Cancel</Button>
                    <Button onClick={handleDepartmentSubmit} variant="contained" disabled={!departmentFormData.code || !departmentFormData.nameAr}>
                        Save
                    </Button>
                </DialogActions>
            </Dialog>
        </Container>
    );
};

export default Factories;
