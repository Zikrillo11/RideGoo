import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { Toaster } from 'react-hot-toast';
import { AuthProvider, useAuth } from './context/AuthContext';
import Layout from './components/Layout';
import Login from './pages/auth/Login';
import Register from './pages/auth/Register';
import OrderCreate from './pages/customer/OrderCreate';
import Wallet from './pages/customer/Wallet';
import MyOrders from './pages/customer/MyOrders';
import AdminUsers from './pages/admin/AdminUsers';
import AdminPromoCodes from './pages/admin/AdminPromoCodes';
import AdminWithdrawals from './pages/admin/AdminWithdrawals';
import DriverPanel from './pages/driver/DriverPanel';

function ProtectedRoute({ children }) {
  const { user, loading } = useAuth();

  if (loading) return <p>Yuklanmoqda...</p>;
  if (!user) return <Navigate to="/login" replace />;

  return children;
}

function AppRoutes() {
  return (
    <Routes>
      <Route path="/login" element={<Login />} />
      <Route path="/register" element={<Register />} />

      <Route
        element={
          <ProtectedRoute>
            <Layout />
          </ProtectedRoute>
        }
      >
        <Route path="/" element={<OrderCreate />} />
        <Route path="/wallet" element={<Wallet />} />
        <Route path="/my-orders" element={<MyOrders />} />
        <Route path="/admin/users" element={<AdminUsers />} />
        <Route path="/admin/promo-codes" element={<AdminPromoCodes />} />
        <Route path="/admin/withdrawals" element={<AdminWithdrawals />} />
        <Route path="/driver" element={<DriverPanel />} />
      </Route>
    </Routes>
  );
}

export default function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Toaster position="top-center" toastOptions={{ duration: 3000 }} />
        <AppRoutes />
      </AuthProvider>
    </BrowserRouter>
  );
}