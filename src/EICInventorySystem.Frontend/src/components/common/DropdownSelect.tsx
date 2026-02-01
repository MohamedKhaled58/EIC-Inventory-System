import React from 'react';
import {
    Select,
    MenuItem,
    FormControl,
    InputLabel,
    Box,
    Typography,
    Chip
} from '@mui/material';

interface DropdownSelectProps<T> {
    label?: string;
    labelArabic?: string;
    value: T;
    onChange: (value: T) => void;
    options: { value: T; label: string; labelArabic?: string }[];
    disabled?: boolean;
    required?: boolean;
    error?: boolean;
    helperText?: string;
    fullWidth?: boolean;
}

/**
 * Generic Dropdown Select Component
 * Provides consistent dropdown selection with bilingual labels
 */
export const DropdownSelect = <T extends string | number | boolean>({
    label,
    labelArabic = 'اختر',
    value,
    onChange,
    options,
    disabled = false,
    required = false,
    error = false,
    helperText,
    fullWidth = true,
}: DropdownSelectProps<T>) => {
    return (
        <FormControl fullWidth error={error} required={required} disabled={disabled}>
            <InputLabel>{labelArabic} / {label}</InputLabel>
            <Select
                value={value}
                onChange={(e) => onChange(e.target.value as T)}
                disabled={disabled}
                fullWidth
            >
                {options.map((option) => (
                    <MenuItem key={option.value} value={option.value}>
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                            <Typography variant="body2">{option.labelArabic || option.label}</Typography>
                            {option.labelArabic && option.labelArabic !== option.label && (
                                <Typography variant="caption" color="text.secondary">
                                    {option.label}
                                </Typography>
                            )}
                        </Box>
                    </MenuItem>
                ))}
            </Select>
            {helperText && (
                <Typography variant="caption" color="text.secondary" sx={{ mt: 1 }}>
                    {helperText}
                </Typography>
            )}
        </FormControl>
    );
};
