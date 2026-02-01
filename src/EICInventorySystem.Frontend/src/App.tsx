import { Suspense, lazy } from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { ThemeProvider, CssBaseline } from '@mui/material';
import { CircularProgress, Box } from '@mui/material';
import { rtlTheme } from './theme';
import MainLayout from './components/layout/MainLayout';
import { authService } from './services';

// Lazy load pages for better performance
const Login = lazy(() => import('./pages/Login'));
const Dashboard = lazy(() => import('./pages/Dashboard'));
const Projects = lazy(() => import('./pages/Projects'));
const Requisitions = lazy(() => import('./pages/Requisitions'));
const CommanderReserve = lazy(() => import('./pages/CommanderReserve'));
const Inventory = lazy(() => import('./pages/Inventory'));
const Transfers = lazy(() => import('./pages/Transfers'));
const Reports = lazy(() => import('./pages/Reports'));
const Settings = lazy(() => import('./pages/Settings'));
const AuditLog = lazy(() => import('./pages/AuditLog'));
const ProjectBOQ = lazy(() => import('./pages/ProjectBOQ'));
const OperationalCustody = lazy(() => import('./pages/OperationalCustody'));
const Factories = lazy(() => import('./pages/Factories'));
const Unauthorized = lazy(() => import('./pages/Unauthorized'));
const NotFound = lazy(() => import('./pages/NotFound'));

// Protected Route Component
const ProtectedRoute: React.FC<{ children: React.ReactNode; requiredPermission?: string }> = ({
  children,
  requiredPermission,
}) => {
  const isAuthenticated = authService.isAuthenticated();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (requiredPermission && !authService.hasPermission(requiredPermission)) {
    return <Navigate to="/unauthorized" replace />;
  }

  return <>{children}</>;
};

// Loading Component
const LoadingFallback: React.FC = () => (
  <Box
    display="flex"
    justifyContent="center"
    alignItems="center"
    minHeight="100vh"
  >
    <CircularProgress size={60} />
  </Box>
);

const App: React.FC = () => {
  return (
    <ThemeProvider theme={rtlTheme}>
      <CssBaseline />
      <BrowserRouter future={{ v7_startTransition: true, v7_relativeSplatPath: true }}>
        <Suspense fallback={<LoadingFallback />}>
          <Routes>
            {/* Public Routes */}
            <Route path="/login" element={<Login />} />
            <Route path="/unauthorized" element={<Unauthorized />} />
            <Route path="/not-found" element={<NotFound />} />

            {/* Protected Routes */}
            <Route element={<MainLayout />}>
              <Route
                path="/"
                element={
                  <ProtectedRoute>
                    <Navigate to="/dashboard" replace />
                  </ProtectedRoute>
                }
              />
              <Route
                path="/dashboard"
                element={
                  <ProtectedRoute>
                    <Dashboard />
                  </ProtectedRoute>
                }
              />
              <Route
                path="/projects"
                element={
                  <ProtectedRoute requiredPermission="view_own_project">
                    <Projects />
                  </ProtectedRoute>
                }
              />
              <Route
                path="/requisitions"
                element={
                  <ProtectedRoute requiredPermission="create_requisition">
                    <Requisitions />
                  </ProtectedRoute>
                }
              />
              <Route
                path="/commander-reserve"
                element={
                  <ProtectedRoute requiredPermission="view_reserve">
                    <CommanderReserve />
                  </ProtectedRoute>
                }
              />
              <Route
                path="/inventory"
                element={
                  <ProtectedRoute requiredPermission="view_inventory">
                    <Inventory />
                  </ProtectedRoute>
                }
              />
              <Route
                path="/transfers"
                element={
                  <ProtectedRoute requiredPermission="view_transfers">
                    <Transfers />
                  </ProtectedRoute>
                }
              />
              <Route
                path="/reports"
                element={
                  <ProtectedRoute requiredPermission="view_reports">
                    <Reports />
                  </ProtectedRoute>
                }
              />
              <Route
                path="/settings"
                element={
                  <ProtectedRoute requiredPermission="manage_settings">
                    <Settings />
                  </ProtectedRoute>
                }
              />
              <Route
                path="/audit-log"
                element={
                  <ProtectedRoute requiredPermission="view_audit_log">
                    <AuditLog />
                  </ProtectedRoute>
                }
              />
              <Route
                path="/users"
                element={
                  <ProtectedRoute requiredPermission="manage_users">
                    <Settings />
                  </ProtectedRoute>
                }
              />
              <Route
                path="/boq"
                element={
                  <ProtectedRoute>
                    <ProjectBOQ />
                  </ProtectedRoute>
                }
              />
              <Route
                path="/custody"
                element={
                  <ProtectedRoute>
                    <OperationalCustody />
                  </ProtectedRoute>
                }
              />
              <Route
                path="/factories"
                element={
                  <ProtectedRoute>
                    <Factories />
                  </ProtectedRoute>
                }
              />

            </Route>

            {/* Catch all route */}
            <Route path="*" element={<Navigate to="/not-found" replace />} />
          </Routes>
        </Suspense>
      </BrowserRouter>
    </ThemeProvider>
  );
};

export default App;
