import React, { useState, useEffect } from 'react';
import { Autocomplete, TextField, CircularProgress, Box, Typography, Chip } from '@mui/material';
import { apiClient } from '../../services/apiClient';
import type { Warehouse } from '../../types';

interface WarehouseAutocompleteProps {
    value: Warehouse | null;
    onChange: (warehouse: Warehouse | null) => void;
    label?: string;
    labelArabic?: string;
    placeholder?: string;
    error?: boolean;
    helperText?: string;
    disabled?: boolean;
    required?: boolean;
    factoryId?: number;
    type?: 'Central' | 'Factory' | 'All';
    fullWidth?: boolean;
}

/**
 * Zero Manual Typing Warehouse Autocomplete
 * Loads warehouses filtered by type or factory
 */
export const WarehouseAutocomplete: React.FC<WarehouseAutocompleteProps> = ({
    value,
    onChange,
    label = 'Select Warehouse',
    labelArabic = 'اختر المخزن',
    placeholder = 'Select a warehouse...',
    error = false,
    helperText,
    disabled = false,
    required = false,
    factoryId,
    type = 'All',
    fullWidth = true,
}) => {
    const [open, setOpen] = useState(false);
    const [options, setOptions] = useState<Warehouse[]>([]);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        if (!open) return;

        const loadWarehouses = async () => {
            setLoading(true);
            try {
                const params = new URLSearchParams();
                if (factoryId) params.append('factoryId', factoryId.toString());
                if (type !== 'All') params.append('type', type);

                const response = await apiClient.get<{ data: Warehouse[] } | Warehouse[]>(`/api/warehouses?${params.toString()}`);
                const warehouses = Array.isArray(response) ? response : response.data;
                setOptions(warehouses || []);
            } catch (err) {
                console.error('Error loading warehouses:', err);
                setOptions([]);
            } finally {
                setLoading(false);
            }
        };

        loadWarehouses();
    }, [open, factoryId, type]);

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
            groupBy={(option) => option.type === 'Central' ? 'المخزن المركزي' : 'مخازن المصانع'}
            renderOption={(props, option) => (
                <Box component="li" {...props} key={option.id}>
                    <Box sx={{ flexGrow: 1 }}>
                        <Box display="flex" alignItems="center" gap={1}>
                            <Typography variant="body1" fontWeight="bold">
                                {option.code}
                            </Typography>
                            <Chip
                                label={option.type === 'Central' ? 'مركزي' : 'مصنع'}
                                size="small"
                                color={option.type === 'Central' ? 'primary' : 'secondary'}
                                variant="outlined"
                            />
                        </Box>
                        <Typography variant="body2">
                            {option.nameAr}
                        </Typography>
                        {option.location && (
                            <Typography variant="caption" color="text.secondary">
                                {option.location}
                            </Typography>
                        )}
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
            noOptionsText="No warehouses found"
        />
    );
};
