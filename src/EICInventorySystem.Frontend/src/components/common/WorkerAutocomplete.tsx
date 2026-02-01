import React, { useState, useEffect, useCallback } from 'react';
import { Autocomplete, TextField, CircularProgress, Box, Typography, Chip } from '@mui/material';
import { debounce } from 'lodash';
import { workerService } from '../../services';
import type { Worker } from '../../types';

interface WorkerAutocompleteProps {
    value: Worker | null;
    onChange: (worker: Worker | null) => void;
    label?: string;
    labelArabic?: string;
    placeholder?: string;
    error?: boolean;
    helperText?: string;
    disabled?: boolean;
    required?: boolean;
    factoryId?: number;
    departmentId?: number;
    fullWidth?: boolean;
}

/**
 * Zero Manual Typing Worker Autocomplete
 * Searches workers by code, name, or national ID
 * Displays active custody count and department
 */
export const WorkerAutocomplete: React.FC<WorkerAutocompleteProps> = ({
    value,
    onChange,
    label = 'Select Worker',
    labelArabic = 'اختر العامل',
    placeholder = 'Type worker code or name...',
    error = false,
    helperText,
    disabled = false,
    required = false,
    factoryId,
    departmentId,
    fullWidth = true,
}) => {
    const [inputValue, setInputValue] = useState('');
    const [options, setOptions] = useState<Worker[]>([]);
    const [loading, setLoading] = useState(false);

    // Debounced search function
    const searchWorkers = useCallback(
        debounce(async (searchTerm: string) => {
            if (!searchTerm || searchTerm.length < 2) {
                setOptions([]);
                return;
            }

            setLoading(true);
            try {
                const response = await workerService.searchWorkers(searchTerm, factoryId, departmentId, 10);
                setOptions(response || []);
            } catch (err) {
                console.error('Error searching workers:', err);
                setOptions([]);
            } finally {
                setLoading(false);
            }
        }, 300),
        [factoryId, departmentId]
    );

    useEffect(() => {
        searchWorkers(inputValue);
    }, [inputValue, searchWorkers]);

    return (
        <Autocomplete
            value={value}
            onChange={(_, newValue) => onChange(newValue)}
            inputValue={inputValue}
            onInputChange={(_, newInputValue) => setInputValue(newInputValue)}
            options={options}
            getOptionLabel={(option) => `${option.workerCode} - ${option.nameArabic}`}
            isOptionEqualToValue={(option, val) => option.id === val.id}
            loading={loading}
            disabled={disabled}
            fullWidth={fullWidth}
            renderOption={(props, option) => (
                <Box component="li" {...props} key={option.id}>
                    <Box sx={{ flexGrow: 1 }}>
                        <Box display="flex" alignItems="center" gap={1}>
                            <Typography variant="body1" fontWeight="bold">
                                {option.workerCode}
                            </Typography>
                            {option.militaryRank && (
                                <Chip label={option.militaryRankArabic || option.militaryRank} size="small" variant="outlined" />
                            )}
                        </Box>
                        <Typography variant="body2">
                            {option.nameArabic}
                        </Typography>
                        <Typography variant="caption" color="text.secondary">
                            {option.departmentNameArabic} | {option.activeCustodyCount > 0 && `عهد: ${option.activeCustodyCount}`}
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
            noOptionsText={inputValue.length < 2 ? 'Type at least 2 characters' : 'No workers found'}
        />
    );
};
