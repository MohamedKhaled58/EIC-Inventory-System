import React, { useState, useEffect } from 'react';
import { Autocomplete, TextField, CircularProgress, Box, Typography, Chip } from '@mui/material';
import { apiClient } from '../../services/apiClient';
import type { Project } from '../../types';

interface ProjectAutocompleteProps {
    value: Project | null;
    onChange: (project: Project | null) => void;
    label?: string;
    labelArabic?: string;
    placeholder?: string;
    error?: boolean;
    helperText?: string;
    disabled?: boolean;
    required?: boolean;
    factoryId?: number;
    departmentId?: number;
    status?: string;
    fullWidth?: boolean;
}

/**
 * Zero Manual Typing Project Autocomplete
 * Loads and filters projects by code and name
 */
export const ProjectAutocomplete: React.FC<ProjectAutocompleteProps> = ({
    value,
    onChange,
    label = 'Select Project',
    labelArabic = 'اختر المشروع',
    placeholder = 'Type project code or name...',
    error = false,
    helperText,
    disabled = false,
    required = false,
    factoryId,
    departmentId,
    status = 'Active',
    fullWidth = true,
}) => {
    const [open, setOpen] = useState(false);
    const [options, setOptions] = useState<Project[]>([]);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        if (!open) return;

        const loadProjects = async () => {
            setLoading(true);
            try {
                const params = new URLSearchParams();
                if (factoryId) params.append('factoryId', factoryId.toString());
                if (departmentId) params.append('departmentId', departmentId.toString());
                if (status) params.append('status', status);
                params.append('pageSize', '100');

                const response = await apiClient.get<{ items: Project[] }>(`/api/projects?${params.toString()}`);
                setOptions(response.items || []);
            } catch (err) {
                console.error('Error loading projects:', err);
                setOptions([]);
            } finally {
                setLoading(false);
            }
        };

        loadProjects();
    }, [open, factoryId, departmentId, status]);

    const getStatusColor = (projectStatus: string) => {
        switch (projectStatus) {
            case 'Active': return 'success';
            case 'Planning': return 'info';
            case 'OnHold': return 'warning';
            case 'Completed': return 'default';
            case 'Cancelled': return 'error';
            default: return 'default';
        }
    };

    return (
        <Autocomplete
            value={value}
            onChange={(_, newValue) => onChange(newValue)}
            open={open}
            onOpen={() => setOpen(true)}
            onClose={() => setOpen(false)}
            options={options}
            getOptionLabel={(option) => `${option.projectNumber} - ${option.nameAr}`}
            isOptionEqualToValue={(option, val) => option.id === val.id}
            loading={loading}
            disabled={disabled}
            fullWidth={fullWidth}
            filterOptions={(opts, state) => {
                const inputLower = state.inputValue.toLowerCase();
                return opts.filter(
                    (opt) =>
                        opt.projectNumber.toLowerCase().includes(inputLower) ||
                        opt.name.toLowerCase().includes(inputLower) ||
                        opt.nameAr.includes(state.inputValue)
                );
            }}
            renderOption={(props, option) => (
                <Box component="li" {...props} key={option.id}>
                    <Box sx={{ flexGrow: 1 }}>
                        <Box display="flex" alignItems="center" gap={1}>
                            <Typography variant="body1" fontWeight="bold">
                                {option.projectNumber}
                            </Typography>
                            <Chip
                                label={option.status}
                                size="small"
                                color={getStatusColor(option.status) as any}
                            />
                        </Box>
                        <Typography variant="body2">
                            {option.nameAr}
                        </Typography>
                        <Typography variant="caption" color="text.secondary">
                            {option.factoryName} | {option.managerName}
                        </Typography>
                    </Box>
                </Box>
            )}
            renderInput={(params) => (
                <TextField
                    {...params}
                    label={`${labelArabic} / ${label}`}
                    placeholder={placeholder}
                    error={error}
                    helperText={helperText}
                    required={required}
                    InputProps={{
                        ...params.InputProps,
                        endAdornment: (
                            <>
                                {loading ? <CircularProgress color="inherit" size={20} /> : null}
                                {params.InputProps.endAdornment}
                            </>
                        ),
                    }}
                />
            )}
            noOptionsText="No projects found"
        />
    );
};
