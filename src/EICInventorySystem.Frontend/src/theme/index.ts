import { createTheme, ThemeOptions } from '@mui/material/styles';

// RTL theme configuration for Arabic-first UI
const themeOptions: ThemeOptions = {
    direction: 'rtl',
    palette: {
        primary: {
            main: '#1a237e', // Deep blue - military/professional
            light: '#534bae',
            dark: '#000051',
            contrastText: '#ffffff',
        },
        secondary: {
            main: '#8f8b63', // Khaki/Sand
            light: '#a8a482',
            dark: '#736f4d',
            contrastText: '#ffffff',
        },
        error: {
            main: '#d32f2f',
        },
        warning: {
            main: '#ed6c02',
        },
        info: {
            main: '#0288d1',
        },
        success: {
            main: '#2e7d32',
        },
        background: {
            default: '#f5f7f5', // Light Greenish Grey
            paper: '#ffffff',
        },
        text: {
            primary: '#313a31', // Dark Green/Black
            secondary: '#587058',
        },
    },
    typography: {
        fontFamily: '"Cairo", "Segoe UI", "Roboto", "Helvetica", "Arial", sans-serif',
        h1: {
            fontSize: '2.5rem',
            fontWeight: 700,
        },
        h2: {
            fontSize: '2rem',
            fontWeight: 600,
        },
        h3: {
            fontSize: '1.75rem',
            fontWeight: 600,
        },
        h4: {
            fontSize: '1.5rem',
            fontWeight: 600,
        },
        h5: {
            fontSize: '1.25rem',
            fontWeight: 500,
        },
        h6: {
            fontSize: '1rem',
            fontWeight: 500,
        },
        body1: {
            fontSize: '1rem',
        },
        body2: {
            fontSize: '0.875rem',
        },
        button: {
            textTransform: 'none',
            fontWeight: 600,
        },
    },
    components: {
        MuiButton: {
            styleOverrides: {
                root: {
                    borderRadius: 0, // Squared military style
                    padding: '6px 16px',
                    fontWeight: 600,
                    boxShadow: 'none',
                    border: '1px solid transparent',
                    '&:hover': {
                        boxShadow: 'none',
                        border: '1px solid currentColor',
                    },
                },
                containedPrimary: {
                    backgroundColor: '#4e5b31', // Olive Drab
                    '&:hover': {
                        backgroundColor: '#3d4726',
                    },
                },
                containedSecondary: {
                    backgroundColor: '#8f8b63', // Khaki
                    color: '#1a1a1a',
                    '&:hover': {
                        backgroundColor: '#7a7652',
                    },
                },
            },
        },
        MuiCard: {
            styleOverrides: {
                root: {
                    borderRadius: 0,
                    border: '1px solid #c8cdc8', // Light military border
                    boxShadow: 'none',
                    backgroundColor: '#ffffff',
                },
            },
        },
        MuiPaper: {
            styleOverrides: {
                root: {
                    borderRadius: 0,
                    boxShadow: 'none',
                    border: '1px solid #d4d8d4',
                },
            },
        },
        MuiTextField: {
            defaultProps: {
                size: 'small', // Excel-like density
            },
            styleOverrides: {
                root: {
                    '& .MuiOutlinedInput-root': {
                        borderRadius: 0,
                        backgroundColor: '#ffffff',
                        '& fieldset': {
                            borderColor: '#a0a090',
                        },
                    },
                },
            },
        },
        MuiTableContainer: {
            styleOverrides: {
                root: {
                    border: '1px solid #8f8b63', // Khaki border
                    borderRadius: 0,
                    boxShadow: 'none',
                },
            },
        },
        MuiTable: {
            styleOverrides: {
                root: {
                    borderCollapse: 'collapse', // Excel grid style
                },
            },
        },
        MuiTableHead: {
            styleOverrides: {
                root: {
                    backgroundColor: '#e8e8e0', // Light Khaki/Grey background
                    '& .MuiTableCell-head': {
                        color: '#2c332c',
                        fontWeight: 700,
                        borderRight: '1px solid #c0c0b0', // Vertical grid lines
                        borderBottom: '2px solid #8f8b63',
                        fontSize: '0.9rem',
                    },
                },
            },
        },
        MuiTableBody: {
            styleOverrides: {
                root: {
                    '& .MuiTableRow-root:nth-of-type(even)': {
                        backgroundColor: '#f9f9f7', // Alternating rows
                    },
                    '& .MuiTableRow-root:hover': {
                        backgroundColor: '#eef0ee !important', // Hover state
                    },
                },
            },
        },
        MuiTableCell: {
            styleOverrides: {
                root: {
                    borderBottom: '1px solid #d0d0c0',
                    borderRight: '1px solid #e0e0d0', // Vertical grid lines
                    padding: '6px 12px', // Dense padding
                    fontSize: '0.875rem',
                    fontFamily: '"Segoe UI", "Cairo", sans-serif', // Excel default font
                },
            },
        },
        MuiChip: {
            styleOverrides: {
                root: {
                    borderRadius: 0, // Squared
                    fontWeight: 600,
                    height: 24,
                },
            },
        },
    },
    shape: {
        borderRadius: 0,
    },
};

export const rtlTheme = createTheme(themeOptions);

export default rtlTheme;
