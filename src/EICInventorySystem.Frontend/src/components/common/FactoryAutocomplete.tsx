import React, { useState, useEffect } from 'react';
import { Autocomplete, TextField, CircularProgress, Box, Typography, Chip } from '@mui/material';
import { apiClient } from '../../services/apiClient';

interface Factory {
    id: number;
    name: string;
    nameAr: string;
    code: string;
    location: string;
    isActive: boolean;
}

interface FactoryAutocompleteProps {
    value: Factory | null;
    onChange: (factory: Factory | null) => void;
    label?: string;
    labelArabic?: string;
    placeholder?: string;
    error?: boolean;
    helperText?: string;
    disabled?: boolean;
    required?: boolean;
    onlyActive?: boolean;
    fullWidth?: boolean;
}

/**
 * Zero Manual Typing Factory Autocomplete
 * Loads all factories with optional active filter
 */
export const FactoryAutocomplete: React.FC<FactoryAutocompleteProps> = ({
    value,
    onChange,
    label = 'Select Factory',
    labelArabic = 'اختر المصنع',
    placeholder = 'Select a factory...',
    error = false,
    helperText,
    disabled = false,
    required = false,
    onlyActive = true,
    fullWidth = true,
}) => {
    const [open, setOpen] = useState(false);
    const [options, setOptions] = useState<Factory[]>([]);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        if (!open) return;

        const loadFactories = async () => {
            setLoading(true);
            try {
                const params = new URLSearchParams();
                if (onlyActive) params.append('isActive', 'true');

                const response = await apiClient.get<{ data: Factory[] } | Factory[]>(`/api/factories?${params.toString()}`);
                const factories = Array.isArray(response) ? response : response.data;
                setOptions(factories || []);
            } catch (err) {
                console.error('Error loading factories:', err);
                setOptions([]);
            } finally {
                setLoading(false);
            }
        };

        loadFactories();
    }, [open, onlyActive]);

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
                    <Box sx={{ flexGrow: 1 }}>
                        <Box display="flex" alignItems="center" gap={1}>
                            <Typography variant="body1" fontWeight="bold">
                                {option.code}
                            </Typography>
                            {!option.isActive && (
                                <Chip label="غير نشط" size="small" color="error" variant="outlined" />
                            )}
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
            noOptionsText="No factories found"
        />
    );
};
