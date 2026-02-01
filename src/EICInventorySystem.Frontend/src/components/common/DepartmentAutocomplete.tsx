import React, { useState, useEffect } from 'react';
import { Autocomplete, TextField, CircularProgress, Box, Typography } from '@mui/material';
import { apiClient } from '../../services/apiClient';

interface Department {
    id: number;
    name: string;
    nameAr: string;
    code: string;
    factoryId: number;
}

interface DepartmentAutocompleteProps {
    value: Department | null;
    onChange: (department: Department | null) => void;
    label?: string;
    labelArabic?: string;
    placeholder?: string;
    error?: boolean;
    helperText?: string;
    disabled?: boolean;
    required?: boolean;
    factoryId?: number;
    fullWidth?: boolean;
}

/**
 * Zero Manual Typing Department Autocomplete
 * Loads departments filtered by factory
 */
export const DepartmentAutocomplete: React.FC<DepartmentAutocompleteProps> = ({
    value,
    onChange,
    label = 'Select Department',
    labelArabic = 'اختر القسم',
    placeholder = 'Select a department...',
    error = false,
    helperText,
    disabled = false,
    required = false,
    factoryId,
    fullWidth = true,
}) => {
    const [open, setOpen] = useState(false);
    const [options, setOptions] = useState<Department[]>([]);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        if (!open) return;

        const loadDepartments = async () => {
            setLoading(true);
            try {
                const params = new URLSearchParams();
                if (factoryId) params.append('factoryId', factoryId.toString());

                const response = await apiClient.get<{ data: Department[] } | Department[]>(`/api/departments?${params.toString()}`);
                const departments = Array.isArray(response) ? response : response.data;
                setOptions(departments || []);
            } catch (err) {
                console.error('Error loading departments:', err);
                setOptions([]);
            } finally {
                setLoading(false);
            }
        };

        loadDepartments();
    }, [open, factoryId]);

    // Reset value if factory changes
    useEffect(() => {
        if (value && factoryId && value.factoryId !== factoryId) {
            onChange(null);
        }
    }, [factoryId, value, onChange]);

    return (
        <Autocomplete
            value={value}
            onChange={(_, newValue) => onChange(newValue)}
            open={open}
            onOpen={() => setOpen(true)}
            onClose={() => setOpen(false)}
            options={options}
            getOptionLabel={(option) => `${option.code} - ${option.nameAr}`}
            isOptionEqualToValue={(option, val) => option.id === val.id}
            loading={loading}
            disabled={disabled}
            fullWidth={fullWidth}
            renderOption={(props, option) => (
                <Box component="li" {...props} key={option.id}>
                    <Box>
                        <Typography variant="body1" fontWeight="bold">
                            {option.code}
                        </Typography>
                        <Typography variant="body2">
                            {option.nameAr}
                        </Typography>
                        <Typography variant="caption" color="text.secondary">
                            {option.name}
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
            noOptionsText="No departments found"
        />
    );
};
