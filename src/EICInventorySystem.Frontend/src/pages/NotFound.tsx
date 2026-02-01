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
  ErrorOutline as ErrorIcon,
  Home as HomeIcon,
} from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';

const NotFound: React.FC = () => {
  const theme = useTheme();
  const navigate = useNavigate();

  const handleGoHome = () => {
    navigate('/dashboard');
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
          <ErrorIcon
            sx={{
              fontSize: 120,
              color: theme.palette.error.main,
              mb: 3,
            }}
          />
          <Typography variant="h4" gutterBottom color="error">
            الصفحة غير موجودة
          </Typography>
          <Typography variant="h6" gutterBottom color="textSecondary">
            Page Not Found
          </Typography>

          <Typography variant="body1" color="textSecondary" paragraph sx={{ mt: 3 }}>
            عذراً، الصفحة التي تبحث عنها غير موجودة أو تم نقلها.
          </Typography>
          <Typography variant="body2" color="textSecondary" paragraph>
            Sorry, the page you are looking for does not exist or has been moved.
          </Typography>

          <Box mt={4} display="flex" justifyContent="center">
            <Button
              variant="contained"
              size="large"
              startIcon={<HomeIcon />}
              onClick={handleGoHome}
              sx={{ minWidth: 180 }}
            >
              الرئيسية
            </Button>
          </Box>
        </Paper>
      </Box>
    </Container>
  );
};

export default NotFound;
