import React, { useState, useEffect } from 'react';
import {
    TextField,
    InputAdornment,
    Typography,
    Box
} from '@mui/material';

interface QuantityInputProps {
    value: number | string;
    onChange: (value: number | string) => void;
    label?: string;
    labelArabic?: string;
    placeholder?: string;
    min?: number;
    max?: number;
    step?: number;
    unit?: string;
    unitArabic?: string;
    availableQuantity?: number;
    reservedQuantity?: number;
    disabled?: boolean;
    error?: boolean;
    helperText?: string;
    showStockInfo?: boolean;
}

/**
 * Controlled Quantity Input Component
 * Provides validation, stock information, and user guidance
 */
export const QuantityInput: React.FC<QuantityInputProps> = ({
    value,
    onChange,
    label,
    labelArabic = 'الكمية',
    placeholder,
    min = 0,
    max,
    step = 1,
    unit,
    unitArabic = 'الوحدة',
    availableQuantity,
    reservedQuantity,
    disabled = false,
    error = false,
    helperText,
    showStockInfo = true,
}) => {
    const [localValue, setLocalValue] = useState<number>(0);
    const [validationError, setValidationError] = useState<string | null>(null);

    useEffect(() => {
        setLocalValue(typeof value === 'number' ? value : parseFloat(value as string) || 0);
    }, [value]);

    const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const newValue = parseFloat(event.target.value) || 0;

        // Validate min
        if (min !== undefined && newValue < min) {
            setValidationError(`الحد الأدنى هو ${min}`);
            return;
        }

        // Validate max
        if (max !== undefined && newValue > max) {
            setValidationError(`الحد الأقصى هو ${max}`);
            return;
        }

        // Validate available quantity
        if (availableQuantity !== undefined && newValue > availableQuantity) {
            setValidationError(`المتاح هو ${availableQuantity} ${unitArabic || unit}`);
            return;
        }

        setValidationError(null);
        setLocalValue(newValue);
        onChange(newValue);
    };

    const getStockStatus = () => {
        if (!availableQuantity) return { color: 'default' as const, text: 'غير معروف' };

        const percentage = (localValue / availableQuantity) * 100;

        if (percentage > 100) return { color: 'error' as const, text: 'تجاوز المتاح' };
        if (percentage > 90) return { color: 'warning' as const, text: 'قريب من الحد الأقصى' };
        if (percentage > 0 && percentage <= 90) return { color: 'success' as const, text: 'متاح' };
        if (localValue <= 0) return { color: 'info' as const, text: 'صفر' };

        return { color: 'default' as const, text: 'غير معروف' };
    };

    return (
        <Box>
            <TextField
                label={`${labelArabic} / ${label}`}
                placeholder={placeholder}
                type="number"
                value={localValue}
                onChange={handleChange}
                disabled={disabled}
                error={error || validationError !== null}
                helperText={validationError || helperText || (showStockInfo && availableQuantity !== undefined ? `المتاح: ${availableQuantity} ${unitArabic || unit}` : '')}
                InputProps={{
                    inputProps: {
                        min,
                        max,
                        step,
                    },
                    endAdornment: unit ? (
                        <InputAdornment position="end">
                            <Typography variant="body2" color="text.secondary">
                                {unitArabic || unit}
                            </Typography>
                        </InputAdornment>
                    ) : undefined,
                    startAdornment: showStockInfo && availableQuantity !== undefined ? (
                        <InputAdornment position="start">
                            <Box sx={{
                                display: 'flex',
                                alignItems: 'center',
                                gap: 1,
                                mr: 1,
                                p: 0.5,
                                borderRadius: 1,
                                bgcolor: getStockStatus().color === 'success' ? 'success.main' :
                                    getStockStatus().color === 'warning' ? 'warning.main' :
                                        getStockStatus().color === 'error' ? 'error.main' :
                                            getStockStatus().color === 'info' ? 'info.main' : 'action.disabledBackground'
                            }}>
                                <Typography variant="caption" color="white" fontWeight="bold">
                                    {getStockStatus().text}
                                </Typography>
                            </Box>
                        </InputAdornment>
                    ) : undefined,
                }}
            />
            {validationError && (
                <Typography variant="caption" color="error" sx={{ mt: 1 }}>
                    {validationError}
                </Typography>
            )}
        </Box>
    );
};
