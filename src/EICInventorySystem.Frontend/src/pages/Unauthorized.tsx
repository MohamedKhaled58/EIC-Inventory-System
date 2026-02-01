import React from 'react';
import {
  Box,
  Container,
  Typography,
  Button,
  Paper,
  useTheme,
} from '@mui/material';
import {
  Lock as LockIcon,
  Home as HomeIcon,
  Logout as LogoutIcon,
} from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';

const Unauthorized: React.FC = () => {
  const theme = useTheme();
  const navigate = useNavigate();

  const handleGoHome = () => {
    navigate('/dashboard');
  };

  const handleLogout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    navigate('/login');
  };

  return (
    <Container maxWidth="md">
      <Box
        display="flex"
        flexDirection="column"
        alignItems="center"
        justifyContent="center"
        minHeight="80vh"
        textAlign="center"
      >
        <Paper
          elevation={3}
          sx={{
            p: 6,
            borderRadius: 2,
            maxWidth: 600,
            width: '100%',
          }}
        >
          <LockIcon
            sx={{
              fontSize: 120,
              color: theme.palette.warning.main,
              mb: 3,
            }}
          />
          <Typography variant="h4" gutterBottom color="warning">
            غير مصرح
          </Typography>
          <Typography variant="h6" gutterBottom color="textSecondary">
            Unauthorized Access
          </Typography>

          <Typography variant="body1" color="textSecondary" paragraph sx={{ mt: 3 }}>
            ليس لديك الصلاحية للوصول إلى هذه الصفحة.
          </Typography>
          <Typography variant="body2" color="textSecondary" paragraph>
            You do not have permission to access this page.
          </Typography>

          <Box mt={4} display="flex" gap={2} justifyContent="center" flexWrap="wrap">
            <Button
              variant="contained"
              size="large"
              startIcon={<HomeIcon />}
              onClick={handleGoHome}
              sx={{ minWidth: 180 }}
            >
              الرئيسية
            </Button>
            <Button
              variant="outlined"
              size="large"
              startIcon={<LogoutIcon />}
              onClick={handleLogout}
              sx={{ minWidth: 180 }}
            >
              تسجيل الخروج
            </Button>
          </Box>
        </Paper>
      </Box>
    </Container>
  );
};

export default Unauthorized;
