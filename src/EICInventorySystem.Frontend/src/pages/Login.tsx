import * as React from 'react';
import {
    Container,
    Box,
    Paper,
    TextField,
    Button,
    Typography,
    Alert,
    CircularProgress,
} from '@mui/material';
import {
    Login as LoginIcon,
    MilitaryTech as MilitaryTechIcon,
} from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { authService } from '../services';

const Login: React.FC = () => {
    const navigate = useNavigate();
    const [username, setUsername] = React.useState('');
    const [password, setPassword] = React.useState('');
    const [error, setError] = React.useState('');
    const [loading, setLoading] = React.useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');
        setLoading(true);

        try {
            await authService.login({ username, password });
            navigate('/dashboard');
        } catch (err: any) {
            setError(err.message || 'فشل تسجيل الدخول. يرجى المحاولة مرة أخرى.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <Container maxWidth="sm">
            <Box
                display="flex"
                flexDirection="column"
                alignItems="center"
                justifyContent="center"
                minHeight="100vh"
            >
                <Paper
                    elevation={3}
                    sx={{
                        p: 4,
                        width: '100%',
                        maxWidth: 450,
                    }}
                >
                    <Box display="flex" flexDirection="column" alignItems="center" mb={3}>
                        <MilitaryTechIcon
                            sx={{
                                fontSize: 64,
                                color: 'primary.main',
                                mb: 2,
                            }}
                        />
                        <Typography variant="h4" gutterBottom align="center">
                            نظام إدارة مخازن
                        </Typography>
                        <Typography variant="h6" color="textSecondary" align="center">
                            Inventory Management System
                        </Typography>
                        <Typography variant="body2" color="textSecondary" align="center" sx={{ mt: 1 }}>
                            مجمع الصناعات الهندسية
                        </Typography>
                    </Box>

                    {error && (
                        <Alert severity="error" sx={{ mb: 3 }}>
                            {error}
                        </Alert>
                    )}

                    <form onSubmit={handleSubmit}>
                        <TextField
                            fullWidth
                            label="اسم المستخدم"
                            placeholder="Username"
                            value={username}
                            onChange={(e) => setUsername(e.target.value)}
                            margin="normal"
                            required
                            autoFocus
                            disabled={loading}
                        />
                        <TextField
                            fullWidth
                            label="كلمة المرور"
                            placeholder="Password"
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            margin="normal"
                            required
                            disabled={loading}
                        />
                        <Button
                            type="submit"
                            fullWidth
                            variant="contained"
                            size="large"
                            startIcon={loading ? <CircularProgress size={20} /> : <LoginIcon />}
                            sx={{ mt: 3, mb: 2 }}
                            disabled={loading}
                        >
                            {loading ? 'جاري تسجيل الدخول...' : 'تسجيل الدخول'}
                        </Button>
                    </form>

                    <Typography variant="caption" color="textSecondary" display="block" align="center" sx={{ mt: 2 }}>
                        Engineering Industries Complex - Military Inventory System
                    </Typography>
                </Paper>
            </Box>
        </Container>
    );
};

export default Login;
