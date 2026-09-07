import { useState } from 'react';
import { Link, Outlet, useNavigate, useLocation } from 'react-router-dom';
import { Car, MapPin, History, Wallet, Users, Tag, LogOut, Truck, Menu, X, ArrowUpFromLine } from 'lucide-react';
import { useAuth } from '../context/AuthContext';

export default function Layout() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [sidebarOpen, setSidebarOpen] = useState(false);

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const navItem = (to, label, Icon) => {
    const isActive = location.pathname === to;
    return (
      <Link
        to={to}
        onClick={() => setSidebarOpen(false)}
        className={`flex items-center gap-3 px-4 py-3 rounded-xl text-sm font-medium transition-all ${
          isActive
            ? 'bg-white text-gray-900 shadow-lg'
            : 'text-gray-400 hover:bg-gray-800 hover:text-white'
        }`}
      >
        <Icon className="w-[18px] h-[18px]" />
        {label}
      </Link>
    );
  };

  return (
    <div className="min-h-screen bg-gray-100 flex">
      {sidebarOpen && (
        <div
          className="fixed inset-0 bg-black/50 z-40 lg:hidden"
          onClick={() => setSidebarOpen(false)}
        />
      )}

      <aside
        className={`w-64 bg-gray-950 flex flex-col fixed h-screen z-50 transition-transform duration-300 ${
          sidebarOpen ? 'translate-x-0' : '-translate-x-full'
        } lg:translate-x-0`}
      >
        <div className="flex items-center justify-between px-6 py-6 border-b border-gray-800">
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 bg-white rounded-xl flex items-center justify-center">
              <Car className="w-5 h-5 text-gray-950" />
            </div>
            <span className="text-white text-lg font-bold tracking-tight">RideGoo</span>
          </div>
          <button
            onClick={() => setSidebarOpen(false)}
            className="lg:hidden text-gray-400 hover:text-white"
          >
            <X className="w-5 h-5" />
          </button>
        </div>

        <nav className="flex-1 px-3 py-6 space-y-1 overflow-y-auto">
          {user?.role === 'Customer' && (
            <>
              {navItem('/', 'Buyurtma berish', MapPin)}
              {navItem('/my-orders', 'Buyurtmalarim', History)}
              {navItem('/wallet', 'Hamyon', Wallet)}
            </>
          )}

          {user?.role === 'Admin' && (
            <>
              {navItem('/admin/users', 'Foydalanuvchilar', Users)}
              {navItem('/admin/promo-codes', 'Promo-kodlar', Tag)}
              {navItem('/admin/withdrawals', 'Pul yechish', ArrowUpFromLine)}
            </>
          )}

          {user?.role === 'Driver' && <>{navItem('/driver', 'Buyurtmalar', Truck)}</>}
        </nav>

        <div className="px-3 py-4 border-t border-gray-800">
          <div className="flex items-center gap-3 px-3 py-2 mb-2">
            <div className="w-9 h-9 bg-gray-800 rounded-full flex items-center justify-center text-white text-sm font-bold">
              {user?.fullName?.charAt(0)}
            </div>
            <div className="min-w-0">
              <p className="text-sm font-medium text-white truncate">{user?.fullName}</p>
              <p className="text-xs text-gray-500">{user?.role}</p>
            </div>
          </div>
          <button
            onClick={handleLogout}
            className="w-full flex items-center gap-3 px-4 py-2.5 rounded-xl text-sm font-medium text-gray-400 hover:bg-gray-800 hover:text-white transition-all"
          >
            <LogOut className="w-[18px] h-[18px]" />
            Chiqish
          </button>
        </div>
      </aside>

      <div className="lg:ml-64 flex-1 min-h-screen w-full">
        <div className="lg:hidden flex items-center gap-3 px-4 py-4 bg-white border-b border-gray-200 sticky top-0 z-30">
          <button onClick={() => setSidebarOpen(true)} className="text-gray-700">
            <Menu className="w-6 h-6" />
          </button>
          <span className="font-bold text-gray-900">RideGoo</span>
        </div>

        <main className="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8 py-6 lg:py-10">
          <Outlet />
        </main>
      </div>
    </div>
  );
}