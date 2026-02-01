import React from 'react';
import { Alert, AlertTitle, IconButton } from '@mui/material';
import { Close as CloseIcon } from '@mui/icons-material';

interface ErrorAlertProps {
    open: boolean;
    onClose: () => void;
    severity: 'error' | 'warning' | 'info' | 'success';
    title?: string;
    titleArabic?: string;
    message?: string;
    messageArabic?: string;
    guidance?: string;
    guidanceArabic?: string;
}

/**
 * Error Alert Component
 * Displays user-friendly error messages with guidance
 */
export const ErrorAlert: React.FC<ErrorAlertProps> = ({
    open,
    onClose,
    severity = 'error',
    title,
    titleArabic = 'خطأ',
    message,
    messageArabic = 'حدث خطأ',
    guidance,
    guidanceArabic = 'إرشاد',
}) => {
    const displayTitle = titleArabic || title || (severity === 'error' ? 'خطأ' : severity === 'warning' ? 'تحذير' : 'معلومة');
    const displayMessage = messageArabic || message || (severity === 'error' ? 'حدث خطأ' : 'معلومة');
    const displayGuidance = guidanceArabic || guidance || (severity === 'error' ? 'إرشاد' : 'معلومة');

    if (!open) return null;

    return (
        <Alert
            onClose={onClose}
            severity={severity}
            sx={{ mb: 2 }}
            action={
                <IconButton
                    aria-label="close"
                    color="inherit"
                    size="small"
                    onClick={onClose as any}
                >
                    <CloseIcon fontSize="inherit" />
                </IconButton>
            }
        >
            <AlertTitle>{displayTitle}</AlertTitle>
            {(message || messageArabic) && (
                <>
                    {messageArabic || message}
                    {(guidance || guidanceArabic) && (
                        <div style={{ marginTop: '8px', padding: '12px', backgroundColor: severity === 'error' ? '#feebee' : severity === 'warning' ? '#fff3cd' : severity === 'info' ? '#e3f2fd' : '#e8f5e9', borderRadius: '4px' }}>
                            <strong>الإرشاد:</strong> {guidanceArabic || guidance}
                        </div>
                    )}
                </>
            )}
        </Alert>
    );
};
