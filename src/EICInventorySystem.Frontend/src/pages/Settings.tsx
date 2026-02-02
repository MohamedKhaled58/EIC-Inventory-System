import React, { useState, useEffect } from 'react';
import {
  Box,
  Container,
  Typography,
  Paper,
  Grid,
  Card,
  CardContent,
  Button,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Switch,
  FormControlLabel,
  Divider,
  Alert,
  CircularProgress,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  List,
  ListItem,
  ListItemText,
  ListItemSecondaryAction,
  IconButton,
  Chip,
  Tab,
  Tabs,
  Avatar,
  Tooltip,
} from '@mui/material';
import {
  Save as SaveIcon,
  Person as PersonIcon,
  Lock as LockIcon,
  Notifications as NotificationsIcon,
  Language as LanguageIcon,
  Storage as StorageIcon,
  Security as SecurityIcon,
  Delete as DeleteIcon,
  Edit as EditIcon,
  Visibility as VisibilityIcon,
  VisibilityOff as VisibilityOffIcon,
  Refresh as RefreshIcon,
  Download as DownloadIcon,
  Upload as UploadIcon,
} from '@mui/icons-material';
import { useAuthStore } from '../stores/authStore';
import { apiClient } from '../services/apiClient';

interface TabPanelProps {
  children?: React.ReactNode;
  index: number;
  value: number;
}

function TabPanel(props: TabPanelProps) {
  const { children, value, index, ...other } = props;
  return (
    <div
      role="tabpanel"
      hidden={value !== index}
      id={`settings-tabpanel-${index}`}
      aria-labelledby={`settings-tab-${index}`}
      {...other}
    >
      {value === index && <Box sx={{ py: 3 }}>{children}</Box>}
    </div>
  );
}

interface UserSettings {
  userId: number;
  username: string;
  email: string;
  fullName: string;
  fullNameAr: string;
  phoneNumber: string;
  role: string;
  factoryId?: number;
  warehouseId?: number;
  departmentId?: number;
  language: 'AR' | 'EN';
  theme: 'LIGHT' | 'DARK';
  notificationsEnabled: boolean;
  emailNotifications: boolean;
  pushNotifications: boolean;
}

interface SystemSettings {
  systemName: string;
  systemNameAr: string;
  defaultLanguage: 'AR' | 'EN';
  defaultTheme: 'LIGHT' | 'DARK';
  sessionTimeoutMinutes: number;
  maxLoginAttempts: number;
  passwordMinLength: number;
  passwordRequireUppercase: boolean;
  passwordRequireLowercase: boolean;
  passwordRequireNumbers: boolean;
  passwordRequireSpecialChars: boolean;
  passwordExpiryDays: number;
  auditLogRetentionDays: number;
  enableTwoFactorAuth: boolean;
  enableIpWhitelist: boolean;
  allowedIpRanges: string[];
}

interface NotificationPreference {
  id: number;
  type: string;
  typeAr: string;
  enabled: boolean;
  email: boolean;
  push: boolean;
}

const Settings: React.FC = () => {
  const { user } = useAuthStore();
  const [tabValue, setTabValue] = useState(0);
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  // User settings
  const [userSettings, setUserSettings] = useState<UserSettings>({
    userId: user?.userId || 0,
    username: user?.username || '',
    email: user?.email || '',
    fullName: user?.fullName || '',
    fullNameAr: user?.fullNameAr || '',
    phoneNumber: '',
    role: user?.role || '',
    factoryId: user?.factoryId,
    warehouseId: user?.warehouseId,
    departmentId: user?.departmentId,
    language: 'AR',
    theme: 'LIGHT',
    notificationsEnabled: true,
    emailNotifications: true,
    pushNotifications: true,
  });

  // Password change
  const [passwordForm, setPasswordForm] = useState({
    currentPassword: '',
    newPassword: '',
    confirmPassword: '',
  });
  const [showPassword, setShowPassword] = useState({
    current: false,
    new: false,
    confirm: false,
  });

  // System settings (only for admins)
  const [systemSettings, setSystemSettings] = useState<SystemSettings>({
    systemName: 'EIC Inventory System',
    systemNameAr: 'نظام إدارة المخازن',
    defaultLanguage: 'AR',
    defaultTheme: 'LIGHT',
    sessionTimeoutMinutes: 30,
    maxLoginAttempts: 5,
    passwordMinLength: 8,
    passwordRequireUppercase: true,
    passwordRequireLowercase: true,
    passwordRequireNumbers: true,
    passwordRequireSpecialChars: true,
    passwordExpiryDays: 90,
    auditLogRetentionDays: 365,
    enableTwoFactorAuth: false,
    enableIpWhitelist: false,
    allowedIpRanges: [],
  });

  // Notification preferences
  const [notificationPreferences, setNotificationPreferences] = useState<NotificationPreference[]>([
    { id: 1, type: 'Requisition Approved', typeAr: 'موافقة على الطلب', enabled: true, email: true, push: true },
    { id: 2, type: 'Requisition Rejected', typeAr: 'رفض الطلب', enabled: true, email: true, push: true },
    { id: 3, type: 'Low Stock Alert', typeAr: 'تنبيه انخفاض المخزون', enabled: true, email: true, push: true },
    { id: 4, type: 'Transfer Completed', typeAr: 'اكتمال النقل', enabled: true, email: false, push: true },
    { id: 5, type: 'Reserve Release Request', typeAr: 'طلب إطلاق الاحتياطي', enabled: true, email: true, push: true },
  ]);

  useEffect(() => {
    loadSettings();
  }, []);

  const loadSettings = async () => {
    setLoading(true);
    setError(null);
    try {
      const [userRes, sysRes, notifRes] = await Promise.all([
        apiClient.get<{ data: UserSettings }>('/api/settings/user'),
        apiClient.get<{ data: SystemSettings }>('/api/settings/system'),
        apiClient.get<{ data: NotificationPreference[] }>('/api/settings/notifications'),
      ]);

      setUserSettings(userRes.data);
      setSystemSettings(sysRes.data);
      setNotificationPreferences(notifRes.data);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to load settings');
    } finally {
      setLoading(false);
    }
  };

  const handleTabChange = (event: React.SyntheticEvent, newValue: number) => {
    setTabValue(newValue);
  };

  const handleUserSettingsSave = async () => {
    setSaving(true);
    setError(null);
    try {
      await apiClient.put('/api/settings/user', userSettings);
      setSuccess('User settings saved successfully');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to save settings');
    } finally {
      setSaving(false);
    }
  };

  const handlePasswordChange = async () => {
    if (passwordForm.newPassword !== passwordForm.confirmPassword) {
      setError('Passwords do not match');
      return;
    }

    setSaving(true);
    setError(null);
    try {
      await apiClient.post('/api/settings/change-password', {
        currentPassword: passwordForm.currentPassword,
        newPassword: passwordForm.newPassword,
      });
      setSuccess('Password changed successfully');
      setPasswordForm({
        currentPassword: '',
        newPassword: '',
        confirmPassword: '',
      });
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to change password');
    } finally {
      setSaving(false);
    }
  };

  const handleSystemSettingsSave = async () => {
    setSaving(true);
    setError(null);
    try {
      await apiClient.put('/api/settings/system', systemSettings);
      setSuccess('System settings saved successfully');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to save settings');
    } finally {
      setSaving(false);
    }
  };

  const handleNotificationPreferenceToggle = async (id: number, field: 'enabled' | 'email' | 'push') => {
    const updated = notificationPreferences.map(pref =>
      pref.id === id ? { ...pref, [field]: !pref[field] } : pref
    );
    setNotificationPreferences(updated);

    try {
      await apiClient.put(`/api/settings/notifications/${id}`, {
        ...updated.find(p => p.id === id),
      });
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to update notification preference');
    }
  };

  const isAdmin = user?.role === 'ComplexCommander' || user?.role === 'FactoryCommander';

  return (
    <Container maxWidth="xl">
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Box>
          <Typography variant="h4" gutterBottom>
            الإعدادات
          </Typography>
          <Typography variant="body1" color="textSecondary">
            Settings
          </Typography>
        </Box>
        <Tooltip title="Refresh">
          <IconButton onClick={loadSettings} color="primary">
            <RefreshIcon />
          </IconButton>
        </Tooltip>
      </Box>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError(null)}>
          {error}
        </Alert>
      )}

      {success && (
        <Alert severity="success" sx={{ mb: 2 }} onClose={() => setSuccess(null)}>
          {success}
        </Alert>
      )}

      {loading ? (
        <Box display="flex" justifyContent="center" alignItems="center" minHeight="40vh">
          <CircularProgress />
        </Box>
      ) : (
        <>
          <Box sx={{ borderBottom: 1, borderColor: 'divider', mb: 3 }}>
            <Tabs value={tabValue} onChange={handleTabChange} aria-label="Settings Tabs">
              <Tab
                label="Profile / الملف الشخصي"
                id="settings-tab-0"
                aria-controls="settings-tabpanel-0"
                icon={<PersonIcon />}
              />
              <Tab
                label="Security / الأمان"
                id="settings-tab-1"
                aria-controls="settings-tabpanel-1"
                icon={<LockIcon />}
              />
              <Tab
                label="Notifications / الإشعارات"
                id="settings-tab-2"
                aria-controls="settings-tabpanel-2"
                icon={<NotificationsIcon />}
              />
              <Tab
                label="Preferences / التفضيلات"
                id="settings-tab-3"
                aria-controls="settings-tabpanel-3"
                icon={<LanguageIcon />}
              />
              {isAdmin && (
                <Tab
                  label="System / النظام"
                  id="settings-tab-4"
                  aria-controls="settings-tabpanel-4"
                  icon={<StorageIcon />}
                />
              )}
            </Tabs>
          </Box>

          <TabPanel value={tabValue} index={0}>
            <ProfileSettings
              settings={userSettings}
              onChange={setUserSettings}
              onSave={handleUserSettingsSave}
              saving={saving}
            />
          </TabPanel>

          <TabPanel value={tabValue} index={1}>
            <SecuritySettings
              passwordForm={passwordForm}
              setPasswordForm={setPasswordForm}
              showPassword={showPassword}
              setShowPassword={setShowPassword}
              onSave={handlePasswordChange}
              saving={saving}
            />
          </TabPanel>

          <TabPanel value={tabValue} index={2}>
            <NotificationSettings
              preferences={notificationPreferences}
              onToggle={handleNotificationPreferenceToggle}
            />
          </TabPanel>

          <TabPanel value={tabValue} index={3}>
            <PreferenceSettings
              settings={userSettings}
              onChange={setUserSettings}
              onSave={handleUserSettingsSave}
              saving={saving}
            />
          </TabPanel>

          {isAdmin && (
            <TabPanel value={tabValue} index={4}>
              <SystemSettingsPanel
                settings={systemSettings}
                onChange={setSystemSettings}
                onSave={handleSystemSettingsSave}
                saving={saving}
              />
            </TabPanel>
          )}
        </>
      )}
    </Container>
  );
};

interface ProfileSettingsProps {
  settings: UserSettings;
  onChange: (settings: UserSettings) => void;
  onSave: () => void;
  saving: boolean;
}

const ProfileSettings: React.FC<ProfileSettingsProps> = ({ settings, onChange, onSave, saving }) => {
  return (
    <Grid container spacing={3}>
      <Grid item xs={12} md={4}>
        <Card>
          <CardContent>
            <Box display="flex" flexDirection="column" alignItems="center">
              <Avatar sx={{ width: 120, height: 120, mb: 2, bgcolor: 'primary.main' }}>
                {settings?.fullNameAr?.charAt(0) || settings?.fullName?.charAt(0) || 'U'}
              </Avatar>
              <Typography variant="h6" gutterBottom>
                {settings?.fullNameAr || 'N/A'}
              </Typography>
              <Typography variant="body2" color="textSecondary" gutterBottom>
                {settings?.fullName || ''}
              </Typography>
              <Chip label={settings?.role || 'User'} color="primary" size="small" />
            </Box>
          </CardContent>
        </Card>
      </Grid>
      <Grid item xs={12} md={8}>
        <Card>
          <CardContent>
            <Typography variant="h6" gutterBottom>
              Profile Information / معلومات الملف الشخصي
            </Typography>
            {settings ? (
              <Grid container spacing={2}>
                <Grid item xs={12} sm={6}>
                  <TextField
                    fullWidth
                    label="Username / اسم المستخدم"
                    value={settings.username || ''}
                    disabled
                    margin="normal"
                  />
                </Grid>
                <Grid item xs={12} sm={6}>
                  <TextField
                    fullWidth
                    label="Email / البريد الإلكتروني"
                    value={settings.email || ''}
                    disabled
                    margin="normal"
                  />
                </Grid>
                <Grid item xs={12} sm={6}>
                  <TextField
                    fullWidth
                    label="Full Name (Arabic) / الاسم الكامل"
                    value={settings.fullNameAr || ''}
                    onChange={(e) => onChange({ ...settings, fullNameAr: e.target.value })}
                    margin="normal"
                  />
                </Grid>
                <Grid item xs={12} sm={6}>
                  <TextField
                    fullWidth
                    label="Full Name (English)"
                    value={settings.fullName || ''}
                    onChange={(e) => onChange({ ...settings, fullName: e.target.value })}
                    margin="normal"
                  />
                </Grid>
                <Grid item xs={12} sm={6}>
                  <TextField
                    fullWidth
                    label="Phone Number / رقم الهاتف"
                    value={settings.phoneNumber || ''}
                    onChange={(e) => onChange({ ...settings, phoneNumber: e.target.value })}
                    margin="normal"
                  />
                </Grid>
                <Grid item xs={12} sm={6}>
                  <TextField
                    fullWidth
                    label="Role / الدور"
                    value={settings.role || ''}
                    disabled
                    margin="normal"
                  />
                </Grid>
              </Grid>
            ) : (
              <Box py={3} display="flex" justifyContent="center">
                <CircularProgress />
              </Box>
            )}
            <Box mt={3} display="flex" justifyContent="flex-end">
              <Button
                variant="contained"
                startIcon={<SaveIcon />}
                onClick={onSave}
                disabled={saving || !settings}
              >
                {saving ? 'Saving...' : 'Save / حفظ'}
              </Button>
            </Box>
          </CardContent>
        </Card>
      </Grid>
    </Grid>
  );
};

interface SecuritySettingsProps {
  passwordForm: {
    currentPassword: string;
    newPassword: string;
    confirmPassword: string;
  };
  setPasswordForm: (form: any) => void;
  showPassword: {
    current: boolean;
    new: boolean;
    confirm: boolean;
  };
  setShowPassword: (show: any) => void;
  onSave: () => void;
  saving: boolean;
}

const SecuritySettings: React.FC<SecuritySettingsProps> = ({
  passwordForm,
  setPasswordForm,
  showPassword,
  setShowPassword,
  onSave,
  saving,
}) => {
  return (
    <Grid container spacing={3}>
      <Grid item xs={12} md={6}>
        <Card>
          <CardContent>
            <Typography variant="h6" gutterBottom>
              Change Password / تغيير كلمة المرور
            </Typography>
            <Box mt={2}>
              <TextField
                fullWidth
                label="Current Password / كلمة المرور الحالية"
                type={showPassword.current ? 'text' : 'password'}
                value={passwordForm.currentPassword}
                onChange={(e) => setPasswordForm({ ...passwordForm, currentPassword: e.target.value })}
                margin="normal"
                InputProps={{
                  endAdornment: (
                    <IconButton
                      onClick={() => setShowPassword({ ...showPassword, current: !showPassword.current })}
                    >
                      {showPassword.current ? <VisibilityOffIcon /> : <VisibilityIcon />}
                    </IconButton>
                  ),
                }}
              />
              <TextField
                fullWidth
                label="New Password / كلمة المرور الجديدة"
                type={showPassword.new ? 'text' : 'password'}
                value={passwordForm.newPassword}
                onChange={(e) => setPasswordForm({ ...passwordForm, newPassword: e.target.value })}
                margin="normal"
                InputProps={{
                  endAdornment: (
                    <IconButton
                      onClick={() => setShowPassword({ ...showPassword, new: !showPassword.new })}
                    >
                      {showPassword.new ? <VisibilityOffIcon /> : <VisibilityIcon />}
                    </IconButton>
                  ),
                }}
              />
              <TextField
                fullWidth
                label="Confirm New Password / تأكيد كلمة المرور"
                type={showPassword.confirm ? 'text' : 'password'}
                value={passwordForm.confirmPassword}
                onChange={(e) => setPasswordForm({ ...passwordForm, confirmPassword: e.target.value })}
                margin="normal"
                InputProps={{
                  endAdornment: (
                    <IconButton
                      onClick={() => setShowPassword({ ...showPassword, new: !showPassword.confirm })}
                    >
                      {showPassword.confirm ? <VisibilityOffIcon /> : <VisibilityIcon />}
                    </IconButton>
                  ),
                }}
              />
              <Box mt={3} display="flex" justifyContent="flex-end">
                <Button
                  variant="contained"
                  startIcon={<SaveIcon />}
                  onClick={onSave}
                  disabled={saving}
                >
                  {saving ? 'Changing...' : 'Change Password / تغيير كلمة المرور'}
                </Button>
              </Box>
            </Box>
          </CardContent>
        </Card>
      </Grid>
      <Grid item xs={12} md={6}>
        <Card>
          <CardContent>
            <Typography variant="h6" gutterBottom>
              Password Requirements / متطلبات كلمة المرور
            </Typography>
            <List>
              <ListItem>
                <ListItemText
                  primary="Minimum Length / الحد الأدنى للطول"
                  secondary="8 characters"
                />
              </ListItem>
              <ListItem>
                <ListItemText
                  primary="Uppercase Letters / الأحرف الكبيرة"
                  secondary="At least one required"
                />
              </ListItem>
              <ListItem>
                <ListItemText
                  primary="Lowercase Letters / الأحرف الصغيرة"
                  secondary="At least one required"
                />
              </ListItem>
              <ListItem>
                <ListItemText
                  primary="Numbers / الأرقام"
                  secondary="At least one required"
                />
              </ListItem>
              <ListItem>
                <ListItemText
                  primary="Special Characters / الأحرف الخاصة"
                  secondary="At least one required"
                />
              </ListItem>
            </List>
          </CardContent>
        </Card>
      </Grid>
    </Grid>
  );
};

interface NotificationSettingsProps {
  preferences: NotificationPreference[];
  onToggle: (id: number, field: 'enabled' | 'email' | 'push') => void;
}

const NotificationSettings: React.FC<NotificationSettingsProps> = ({ preferences, onToggle }) => {
  return (
    <Card>
      <CardContent>
        <Typography variant="h6" gutterBottom>
          Notification Preferences / تفضيلات الإشعارات
        </Typography>
        <List>
          {preferences.map((pref) => (
            <ListItem key={pref.id} divider>
              <ListItemText
                primary={pref.typeAr}
                secondary={pref.type}
              />
              <ListItemSecondaryAction>
                <Box display="flex" gap={1}>
                  <FormControlLabel
                    control={
                      <Switch
                        checked={pref.enabled}
                        onChange={() => onToggle(pref.id, 'enabled')}
                        color="primary"
                      />
                    }
                    label="Enabled"
                  />
                  <FormControlLabel
                    control={
                      <Switch
                        checked={pref.email}
                        onChange={() => onToggle(pref.id, 'email')}
                        color="primary"
                      />
                    }
                    label="Email"
                  />
                  <FormControlLabel
                    control={
                      <Switch
                        checked={pref.push}
                        onChange={() => onToggle(pref.id, 'push')}
                        color="primary"
                      />
                    }
                    label="Push"
                  />
                </Box>
              </ListItemSecondaryAction>
            </ListItem>
          ))}
        </List>
      </CardContent>
    </Card>
  );
};

interface PreferenceSettingsProps {
  settings: UserSettings;
  onChange: (settings: UserSettings) => void;
  onSave: () => void;
  saving: boolean;
}

const PreferenceSettings: React.FC<PreferenceSettingsProps> = ({ settings, onChange, onSave, saving }) => {
  return (
    <Grid container spacing={3}>
      <Grid item xs={12} md={6}>
        <Card>
          <CardContent>
            <Typography variant="h6" gutterBottom>
              Language & Theme / اللغة والمظهر
            </Typography>
            <FormControl fullWidth margin="normal">
              <InputLabel>Language / اللغة</InputLabel>
              <Select
                value={settings.language}
                label="Language / اللغة"
                onChange={(e) => onChange({ ...settings, language: e.target.value as 'AR' | 'EN' })}
              >
                <MenuItem value="AR">العربية (Arabic)</MenuItem>
                <MenuItem value="EN">English</MenuItem>
              </Select>
            </FormControl>
            <FormControl fullWidth margin="normal">
              <InputLabel>Theme / المظهر</InputLabel>
              <Select
                value={settings.theme}
                label="Theme / المظهر"
                onChange={(e) => onChange({ ...settings, theme: e.target.value as 'LIGHT' | 'DARK' })}
              >
                <MenuItem value="LIGHT">Light / فاتح</MenuItem>
                <MenuItem value="DARK">Dark / داكن</MenuItem>
              </Select>
            </FormControl>
          </CardContent>
        </Card>
      </Grid>
      <Grid item xs={12} md={6}>
        <Card>
          <CardContent>
            <Typography variant="h6" gutterBottom>
              Notification Settings / إعدادات الإشعارات
            </Typography>
            <FormControlLabel
              control={
                <Switch
                  checked={settings.notificationsEnabled}
                  onChange={(e) => onChange({ ...settings, notificationsEnabled: e.target.checked })}
                  color="primary"
                />
              }
              label="Enable Notifications / تفعيل الإشعارات"
            />
            <FormControlLabel
              control={
                <Switch
                  checked={settings.emailNotifications}
                  onChange={(e) => onChange({ ...settings, emailNotifications: e.target.checked })}
                  color="primary"
                />
              }
              label="Email Notifications / إشعارات البريد الإلكتروني"
            />
            <FormControlLabel
              control={
                <Switch
                  checked={settings.pushNotifications}
                  onChange={(e) => onChange({ ...settings, pushNotifications: e.target.checked })}
                  color="primary"
                />
              }
              label="Push Notifications / إشعارات الدفع"
            />
          </CardContent>
        </Card>
      </Grid>
      <Grid item xs={12}>
        <Box display="flex" justifyContent="flex-end">
          <Button
            variant="contained"
            startIcon={<SaveIcon />}
            onClick={onSave}
            disabled={saving}
          >
            {saving ? 'Saving...' : 'Save Preferences / حفظ التفضيلات'}
          </Button>
        </Box>
      </Grid>
    </Grid>
  );
};

interface SystemSettingsPanelProps {
  settings: SystemSettings;
  onChange: (settings: SystemSettings) => void;
  onSave: () => void;
  saving: boolean;
}

const SystemSettingsPanel: React.FC<SystemSettingsPanelProps> = ({ settings, onChange, onSave, saving }) => {
  return (
    <Grid container spacing={3}>
      <Grid item xs={12} md={6}>
        <Card>
          <CardContent>
            <Typography variant="h6" gutterBottom>
              General Settings / الإعدادات العامة
            </Typography>
            <TextField
              fullWidth
              label="System Name / اسم النظام"
              value={settings.systemName}
              onChange={(e) => onChange({ ...settings, systemName: e.target.value })}
              margin="normal"
            />
            <TextField
              fullWidth
              label="System Name (Arabic) / اسم النظام"
              value={settings.systemNameAr}
              onChange={(e) => onChange({ ...settings, systemNameAr: e.target.value })}
              margin="normal"
            />
            <FormControl fullWidth margin="normal">
              <InputLabel>Default Language / اللغة الافتراضية</InputLabel>
              <Select
                value={settings.defaultLanguage}
                label="Default Language / اللغة الافتراضية"
                onChange={(e) => onChange({ ...settings, defaultLanguage: e.target.value as 'AR' | 'EN' })}
              >
                <MenuItem value="AR">العربية (Arabic)</MenuItem>
                <MenuItem value="EN">English</MenuItem>
              </Select>
            </FormControl>
            <FormControl fullWidth margin="normal">
              <InputLabel>Default Theme / المظهر الافتراضي</InputLabel>
              <Select
                value={settings.defaultTheme}
                label="Default Theme / المظهر الافتراضي"
                onChange={(e) => onChange({ ...settings, defaultTheme: e.target.value as 'LIGHT' | 'DARK' })}
              >
                <MenuItem value="LIGHT">Light / فاتح</MenuItem>
                <MenuItem value="DARK">Dark / داكن</MenuItem>
              </Select>
            </FormControl>
          </CardContent>
        </Card>
      </Grid>
      <Grid item xs={12} md={6}>
        <Card>
          <CardContent>
            <Typography variant="h6" gutterBottom>
              Security Settings / إعدادات الأمان
            </Typography>
            <TextField
              fullWidth
              label="Session Timeout (minutes) / مهلة الجلسة"
              type="number"
              value={settings.sessionTimeoutMinutes}
              onChange={(e) => onChange({ ...settings, sessionTimeoutMinutes: parseInt(e.target.value) })}
              margin="normal"
            />
            <TextField
              fullWidth
              label="Max Login Attempts / محاولات تسجيل الدخول القصوى"
              type="number"
              value={settings.maxLoginAttempts}
              onChange={(e) => onChange({ ...settings, maxLoginAttempts: parseInt(e.target.value) })}
              margin="normal"
            />
            <TextField
              fullWidth
              label="Password Expiry (days) / انتهاء صلاحية كلمة المرور"
              type="number"
              value={settings.passwordExpiryDays}
              onChange={(e) => onChange({ ...settings, passwordExpiryDays: parseInt(e.target.value) })}
              margin="normal"
            />
            <TextField
              fullWidth
              label="Audit Log Retention (days) / الاحتفاظ بسجل التدقيق"
              type="number"
              value={settings.auditLogRetentionDays}
              onChange={(e) => onChange({ ...settings, auditLogRetentionDays: parseInt(e.target.value) })}
              margin="normal"
            />
          </CardContent>
        </Card>
      </Grid>
      <Grid item xs={12} md={6}>
        <Card>
          <CardContent>
            <Typography variant="h6" gutterBottom>
              Password Requirements / متطلبات كلمة المرور
            </Typography>
            <TextField
              fullWidth
              label="Minimum Length / الحد الأدنى للطول"
              type="number"
              value={settings.passwordMinLength}
              onChange={(e) => onChange({ ...settings, passwordMinLength: parseInt(e.target.value) })}
              margin="normal"
            />
            <FormControlLabel
              control={
                <Switch
                  checked={settings.passwordRequireUppercase}
                  onChange={(e) => onChange({ ...settings, passwordRequireUppercase: e.target.checked })}
                  color="primary"
                />
              }
              label="Require Uppercase / يتطلب أحرف كبيرة"
            />
            <FormControlLabel
              control={
                <Switch
                  checked={settings.passwordRequireLowercase}
                  onChange={(e) => onChange({ ...settings, passwordRequireLowercase: e.target.checked })}
                  color="primary"
                />
              }
              label="Require Lowercase / يتطلب أحرف صغيرة"
            />
            <FormControlLabel
              control={
                <Switch
                  checked={settings.passwordRequireNumbers}
                  onChange={(e) => onChange({ ...settings, passwordRequireNumbers: e.target.checked })}
                  color="primary"
                />
              }
              label="Require Numbers / يتطلب أرقام"
            />
            <FormControlLabel
              control={
                <Switch
                  checked={settings.passwordRequireSpecialChars}
                  onChange={(e) => onChange({ ...settings, passwordRequireSpecialChars: e.target.checked })}
                  color="primary"
                />
              }
              label="Require Special Characters / يتطلب أحرف خاصة"
            />
          </CardContent>
        </Card>
      </Grid>
      <Grid item xs={12} md={6}>
        <Card>
          <CardContent>
            <Typography variant="h6" gutterBottom>
              Advanced Security / الأمان المتقدم
            </Typography>
            <FormControlLabel
              control={
                <Switch
                  checked={settings.enableTwoFactorAuth}
                  onChange={(e) => onChange({ ...settings, enableTwoFactorAuth: e.target.checked })}
                  color="primary"
                />
              }
              label="Enable Two-Factor Authentication / تفعيل المصادقة الثنائية"
            />
            <FormControlLabel
              control={
                <Switch
                  checked={settings.enableIpWhitelist}
                  onChange={(e) => onChange({ ...settings, enableIpWhitelist: e.target.checked })}
                  color="primary"
                />
              }
              label="Enable IP Whitelist / تفعيل القائمة البيضاء للعناوين"
            />
          </CardContent>
        </Card>
      </Grid>
      <Grid item xs={12}>
        <Box display="flex" justifyContent="flex-end">
          <Button
            variant="contained"
            startIcon={<SaveIcon />}
            onClick={onSave}
            disabled={saving}
          >
            {saving ? 'Saving...' : 'Save System Settings / حفظ إعدادات النظام'}
          </Button>
        </Box>
      </Grid>
    </Grid>
  );
};

export default Settings;
