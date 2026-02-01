import React, { useState, useEffect, useCallback } from 'react';
import {
    Autocomplete,
    TextField,
    CircularProgress,
    Box,
    Typography,
    Chip,
    IconButton,
    InputAdornment
} from '@mui/material';
import { Search as SearchIcon } from '@mui/icons-material';
import { debounce } from 'lodash';
import { inventoryService } from '../../services';
import type { Item } from '../../types';

interface ItemAutocompleteProps {
    value: Item | null;
    onChange: (item: Item | null) => void;
    label?: string;
    labelArabic?: string;
    placeholder?: string;
    error?: boolean;
    helperText?: string;
    disabled?: boolean;
    required?: boolean;
    warehouseId?: number; // Optional: filter by warehouse availability
    fullWidth?: boolean;
    showStockInfo?: boolean; // Show stock information
    allowOutOfStock?: boolean; // Allow selecting out-of-stock items
}

/**
 * Enhanced Item Autocomplete with Stock Information
 * Zero Manual Typing - Search-based selection only
 * Displays stock availability when warehouse is specified
 */
export const ItemAutocomplete: React.FC<ItemAutocompleteProps> = ({
    value,
    onChange,
    label = 'Select Item',
    labelArabic = 'اختر الصنف',
    placeholder = 'Type part number or name...',
    error = false,
    helperText,
    disabled = false,
    required = false,
    warehouseId,
    fullWidth = true,
    showStockInfo = true,
    allowOutOfStock = false,
}) => {
    const [inputValue, setInputValue] = useState('');
    const [options, setOptions] = useState<Item[]>([]);
    const [loading, setLoading] = useState(false);
    const [open, setOpen] = useState(false);

    // Debounced search function
    const searchItems = useCallback(
        debounce(async (searchTerm: string) => {
            if (!searchTerm || searchTerm.length < 2) {
                setOptions([]);
                return;
            }

            setLoading(true);
            try {
                const response = await inventoryService.searchItems(searchTerm, warehouseId);
                setOptions(response || []);
            } catch (err) {
                console.error('Error searching items:', err);
                setOptions([]);
            } finally {
                setLoading(false);
            }
        }, 300),
        [warehouseId]
    );

    useEffect(() => {
        if (open) {
            searchItems(inputValue);
        }
    }, [inputValue, searchItems, open]);

    const getStockStatus = (item: Item) => {
        if (!item.availableQuantity) return { color: 'default' as const, text: 'غير معروف' };

        if (item.availableQuantity > 0) return { color: 'success' as const, text: 'متاح' };
        return { color: 'error' as const, text: 'غير متاح' };
    };

    return (
        <Autocomplete
            value={value}
            onChange={(_, newValue) => onChange(newValue)}
            inputValue={inputValue}
            onInputChange={(_, newInputValue) => setInputValue(newInputValue)}
            open={open}
            onOpen={() => setOpen(true)}
            onClose={() => setOpen(false)}
            options={options}
            getOptionLabel={(option) => `${option.code} - ${option.nameAr}`}
            isOptionEqualToValue={(option, val) => option.id === val.id}
            loading={loading}
            disabled={disabled}
            fullWidth={fullWidth}
            filterOptions={(options) => options} // Custom filtering
            renderOption={(props, option) => (
                <Box component="li" {...props} key={option.id}>
                    <Box sx={{ display: 'flex', flexDirection: 'column', width: '100%' }}>
                        {/* Item Code and Status */}
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                            <Typography variant="body1" fontWeight="bold" sx={{ minWidth: 120 }}>
                                {option.code}
                            </Typography>
                            <Chip
                                label={option.isActive ? 'نشط' : 'غير نشط'}
                                size="small"
                                color={option.isActive ? 'success' : 'default'}
                            />
                            {option.isCritical && (
                                <Chip label="حرج" size="small" color="error" />
                            )}
                        </Box>

                        {/* Item Names */}
                        <Box sx={{ display: 'flex', gap: 1, alignItems: 'center' }}>
                            <Typography variant="body2" fontWeight="medium">
                                {option.nameAr}
                            </Typography>
                            <Typography variant="caption" color="textSecondary">
                                {option.name}
                            </Typography>
                        </Box>

                        {/* Category and Unit */}
                        <Box sx={{ display: 'flex', gap: 1, alignItems: 'center' }}>
                            <Chip
                                label={option.categoryNameAr || option.categoryName}
                                size="small"
                                variant="outlined"
                            />
                            <Typography variant="caption" color="textSecondary">
                                الوحدة: {option.unitAr || option.unit}
                            </Typography>
                        </Box>

                        {/* Stock Information */}
                        {showStockInfo && option.availableQuantity !== undefined && (
                            <Box sx={{ display: 'flex', gap: 1, alignItems: 'center', mt: 0.5 }}>
                                <Chip
                                    label={`المتاح: ${option.availableQuantity} ${option.unitAr || option.unit}`}
                                    size="small"
                                    color={getStockStatus(option).color}
                                    variant="outlined"
                                />
                                {option.reservedQuantity && option.reservedQuantity > 0 && (
                                    <Chip
                                        label={`الاحتياطي: ${option.reservedQuantity} ${option.unitAr || option.unit}`}
                                        size="small"
                                        color="warning"
                                        variant="outlined"
                                    />
                                )}
                                {option.commanderReserveQuantity && option.commanderReserveQuantity > 0 && (
                                    <Chip
                                        label={`احتياطي القائد: ${option.commanderReserveQuantity} ${option.unitAr || option.unit}`}
                                        size="small"
                                        color="info"
                                        variant="outlined"
                                    />
                                )}
                            </Box>
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
                            <InputAdornment position="end">
                                <IconButton onClick={() => setOpen(!open)} edge="end">
                                    <SearchIcon />
                                </IconButton>
                                {loading && <CircularProgress color="inherit" size={20} />}
                            </InputAdornment>
                        ),
                    }}
                />
            )}
            noOptionsText={inputValue.length < 2 ? 'اكتب حرفين على الأقل' : 'لم يتم العثور على أصناف'}
        />
    );
};
