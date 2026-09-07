import { useState, useEffect } from 'react';
import toast from 'react-hot-toast';
import { Power, MapPin, Clock, CheckCircle2, Navigation, Wallet as WalletIcon, ArrowUpFromLine } from 'lucide-react';
import api from '../../services/api';
import { useAuth } from '../../context/AuthContext';
import { useSignalR } from '../../hooks/useSignalR';
import { SkeletonStatCard, SkeletonList, SkeletonLine } from '../../components/Skeleton';

const statusLabels = {
  Completed: 'Yakunlandi',
  CancelledByCustomer: 'Bekor qilindi',
  CancelledByDriver: 'Bekor qilindi',
};

const withdrawalStatusStyles = {
  Pending: 'bg-amber-50 text-amber-700',
  Approved: 'bg-green-50 text-green-700',
  Rejected: 'bg-red-50 text-red-700',
};

const withdrawalStatusLabels = {
  Pending: 'Kutilmoqda',
  Approved: 'Tasdiqlandi',
  Rejected: 'Rad etildi',
};

export default function DriverPanel() {
  const { user } = useAuth();
  const [driverInfo, setDriverInfo] = useState(null);
  const [myOrders, setMyOrders] = useState([]);
  const [pendingOrders, setPendingOrders] = useState([]);
  const [wallet, setWallet] = useState(null);
  const [withdrawals, setWithdrawals] = useState([]);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(true);
  const [toggling, setToggling] = useState(false);

  const [showWithdrawForm, setShowWithdrawForm] = useState(false);
  const [withdrawAmount, setWithdrawAmount] = useState('');
  const [withdrawCard, setWithdrawCard] = useState('');
  const [submittingWithdraw, setSubmittingWithdraw] = useState(false);

  const loadDriverData = async () => {
    setLoading(true);
    setError('');
    try {
      const driverRes = await api.get('/Drivers/my');
      setDriverInfo(driverRes.data);

      const [ordersRes, pendingRes, walletRes, withdrawalsRes] = await Promise.all([
        api.get(`/Orders/driver/${driverRes.data.id}`, { params: { pageNumber: 1, pageSize: 10 } }),
        api.get('/Orders/pending', { params: { pageNumber: 1, pageSize: 10 } }),
        api.get('/Wallet/my'),
        api.get('/Withdrawals/my'),
      ]);
      setMyOrders(ordersRes.data.items);
      setPendingOrders(pendingRes.data.items);
      setWallet(walletRes.data);
      setWithdrawals(withdrawalsRes.data);
    } catch (err) {
      const message = err.response?.data?.message || 'Malumotlarni yuklashda xatolik yuz berdi.';
      setError(message);
      toast.error(message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadDriverData();
  }, []);

  useSignalR(
    (data) => {
      toast.success(`Yangi buyurtma: ${data.fromAddress} → ${data.toAddress}`, { duration: 5000 });
      loadDriverData();
    },
    null
  );

  const handleToggleOnline = async () => {
    if (!driverInfo) return;
    setToggling(true);
    try {
      const newStatus = driverInfo.status === 'Online' ? 'Offline' : 'Online';
      await api.put(`/Drivers/${driverInfo.id}`, {
        licenseNumber: driverInfo.licenseNumber,
        status: newStatus,
      });
      toast.success(newStatus === 'Online' ? 'Siz endi online holatdasiz!' : 'Siz offline holatga otdingiz.');
      await loadDriverData();
    } catch (err) {
      toast.error(err.response?.data?.message || 'Holatni ozgartirishda xatolik.');
    } finally {
      setToggling(false);
    }
  };

  const handleAccept = async (orderId) => {
    try {
      await api.post(`/Orders/${orderId}/accept`, null, { params: { driverId: driverInfo.id } });
      toast.success('Buyurtma qabul qilindi!');
      await loadDriverData();
    } catch (err) {
      toast.error(err.response?.data?.message || 'Qabul qilishda xatolik.');
    }
  };

  const statusMessages = {
    DriverArrived: 'Yetib kelganingiz belgilandi!',
    InProgress: 'Safar boshlandi!',
    Completed: 'Safar yakunlandi!',
  };

  const handleUpdateStatus = async (orderId, status) => {
    try {
      await api.put(`/Orders/${orderId}/status`, { status });
      toast.success(statusMessages[status] || 'Holat yangilandi.');
      await loadDriverData();
    } catch (err) {
      toast.error(err.response?.data?.message || 'Holatni yangilashda xatolik.');
    }
  };

  const handleWithdrawSubmit = async (e) => {
    e.preventDefault();
    setSubmittingWithdraw(true);
    try {
      await api.post('/Withdrawals', {
        amount: Number(withdrawAmount),
        cardNumber: withdrawCard || null,
      });
      toast.success('Pul yechish sorovi yuborildi!');
      setWithdrawAmount('');
      setWithdrawCard('');
      setShowWithdrawForm(false);
      await loadDriverData();
    } catch (err) {
      toast.error(err.response?.data?.message || 'Sorov yuborishda xatolik yuz berdi.');
    } finally {
      setSubmittingWithdraw(false);
    }
  };

  if (loading) {
    return (
      <div>
        <div className="flex items-center justify-between mb-8">
          <div className="space-y-2">
            <SkeletonLine width="w-48" height="h-7" />
            <SkeletonLine width="w-32" height="h-4" />
          </div>
          <SkeletonLine width="w-28" height="h-11" />
        </div>

        <div className="grid grid-cols-3 gap-4 mb-8">
          <SkeletonStatCard />
          <SkeletonStatCard />
          <SkeletonStatCard />
        </div>

        <SkeletonLine width="w-40" height="h-5" />
        <div className="bg-white rounded-2xl border border-gray-200 mt-3">
          <SkeletonList rows={3} />
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="p-4 bg-red-50 border border-red-100 rounded-xl text-red-600 text-sm">
        {error}
      </div>
    );
  }

  const activeOrders = myOrders.filter((o) => o.status !== 'Completed' && o.status !== 'CancelledByCustomer' && o.status !== 'CancelledByDriver');
  const historyOrders = myOrders.filter((o) => o.status === 'Completed' || o.status === 'CancelledByCustomer' || o.status === 'CancelledByDriver');

  return (
    <div>
      <div className="flex items-center justify-between mb-8">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Haydovchi paneli</h1>
          <p className="text-gray-500 mt-1">Xush kelibsiz, {user?.fullName}</p>
        </div>

        <button
          onClick={handleToggleOnline}
          disabled={toggling}
          className={`flex items-center gap-2 px-5 py-3 rounded-xl font-semibold text-sm transition-all ${
            driverInfo?.status === 'Online'
              ? 'bg-green-600 text-white hover:bg-green-700'
              : 'bg-gray-200 text-gray-600 hover:bg-gray-300'
          }`}
        >
          <Power className="w-4 h-4" />
          {driverInfo?.status === 'Online' ? 'Online' : 'Offline'}
        </button>
      </div>

      <div className="grid grid-cols-3 gap-4 mb-8">
        <div className="bg-white rounded-2xl border border-gray-200 p-5">
          <p className="text-xs text-gray-400 uppercase tracking-wide mb-1">Reyting</p>
          <p className="text-2xl font-bold text-gray-900">{driverInfo?.averageRating?.toFixed(1)}</p>
        </div>
        <div className="bg-white rounded-2xl border border-gray-200 p-5">
          <p className="text-xs text-gray-400 uppercase tracking-wide mb-1">Jami safarlar</p>
          <p className="text-2xl font-bold text-gray-900">{driverInfo?.totalTrips}</p>
        </div>
        <div className="bg-white rounded-2xl border border-gray-200 p-5">
          <p className="text-xs text-gray-400 uppercase tracking-wide mb-1">Mashina</p>
          <p className="text-sm font-semibold text-gray-900 mt-2">
            {driverInfo?.vehicle ? `${driverInfo.vehicle.brand} ${driverInfo.vehicle.model}` : 'Biriktirilmagan'}
          </p>
        </div>
      </div>

      {/* Hamyon bolimi */}
      <div className="mb-8">
        <div className="bg-gray-900 rounded-2xl p-6 text-white mb-4">
          <div className="flex items-center justify-between">
            <div>
              <div className="flex items-center gap-2 mb-2">
                <WalletIcon className="w-5 h-5" />
                <span className="text-xs text-gray-400 uppercase tracking-wide">Hamyon balansi</span>
              </div>
              <p className="text-3xl font-bold">{wallet?.balance.toLocaleString()}</p>
              <p className="text-gray-400 text-sm mt-1">{wallet?.currency}</p>
            </div>
            <button
              onClick={() => setShowWithdrawForm(!showWithdrawForm)}
              className="flex items-center gap-1.5 px-4 py-2.5 bg-white text-gray-900 text-sm font-semibold rounded-lg hover:bg-gray-100 transition-colors"
            >
              <ArrowUpFromLine className="w-4 h-4" /> Pul yechish
            </button>
          </div>

          {showWithdrawForm && (
            <form onSubmit={handleWithdrawSubmit} className="mt-4 pt-4 border-t border-gray-700 space-y-3">
              <input
                type="number"
                value={withdrawAmount}
                onChange={(e) => setWithdrawAmount(e.target.value)}
                placeholder="Summa (som)"
                required
                min="1"
                className="w-full px-3 py-2.5 bg-white/10 border border-white/20 rounded-lg text-sm text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-white/50"
              />
              <input
                type="text"
                value={withdrawCard}
                onChange={(e) => setWithdrawCard(e.target.value)}
                placeholder="Karta raqami (ixtiyoriy)"
                className="w-full px-3 py-2.5 bg-white/10 border border-white/20 rounded-lg text-sm text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-white/50"
              />
              <button
                type="submit"
                disabled={submittingWithdraw}
                className="w-full py-2.5 bg-white text-gray-900 text-sm font-semibold rounded-lg hover:bg-gray-100 transition-colors disabled:opacity-50"
              >
                {submittingWithdraw ? 'Yuborilmoqda...' : 'Sorov yuborish'}
              </button>
            </form>
          )}
        </div>

        {withdrawals.length > 0 && (
          <div className="bg-white rounded-2xl border border-gray-200">
            <div className="px-6 py-4 border-b border-gray-100">
              <h3 className="text-sm font-semibold text-gray-900">Pul yechish tarixi</h3>
            </div>
            <div className="divide-y divide-gray-100">
              {withdrawals.map((w) => (
                <div key={w.id} className="flex items-center justify-between px-6 py-3">
                  <div>
                    <p className="text-sm font-medium text-gray-900">{w.amount.toLocaleString()} som</p>
                    <p className="text-xs text-gray-400">{new Date(w.createdAt).toLocaleString()}</p>
                  </div>
                  <span className={`px-3 py-1 rounded-full text-xs font-medium ${withdrawalStatusStyles[w.status]}`}>
                    {withdrawalStatusLabels[w.status]}
                  </span>
                </div>
              ))}
            </div>
          </div>
        )}
      </div>

      {activeOrders.length > 0 && (
        <div className="mb-8">
          <h3 className="text-sm font-semibold text-gray-900 mb-3">Joriy safaringiz</h3>
          <div className="bg-white rounded-2xl border border-gray-200 divide-y divide-gray-100">
            {activeOrders.map((order) => (
              <OrderRow key={order.id} order={order} onAccept={handleAccept} onUpdateStatus={handleUpdateStatus} />
            ))}
          </div>
        </div>
      )}

      {driverInfo?.status === 'Online' && activeOrders.length === 0 && (
        <div className="mb-8">
          <h3 className="text-sm font-semibold text-gray-900 mb-3">Kutilayotgan buyurtmalar</h3>
          {pendingOrders.length === 0 ? (
            <div className="bg-white rounded-2xl border border-gray-200 p-12 text-center">
              <p className="text-gray-400">Hozircha kutilayotgan buyurtma yoq.</p>
            </div>
          ) : (
            <div className="bg-white rounded-2xl border border-gray-200 divide-y divide-gray-100">
              {pendingOrders.map((order) => (
                <OrderRow key={order.id} order={order} onAccept={handleAccept} onUpdateStatus={handleUpdateStatus} />
              ))}
            </div>
          )}
        </div>
      )}

      {driverInfo?.status !== 'Online' && activeOrders.length === 0 && (
        <div className="bg-amber-50 border border-amber-200 rounded-2xl p-6 text-center mb-8">
          <p className="text-amber-700 text-sm">Buyurtmalarni korish uchun Online holatga oting.</p>
        </div>
      )}

      {historyOrders.length > 0 && (
        <div>
          <h3 className="text-sm font-semibold text-gray-900 mb-3">Tarix</h3>
          <div className="bg-white rounded-2xl border border-gray-200 divide-y divide-gray-100">
            {historyOrders.map((order) => (
              <OrderRow key={order.id} order={order} onAccept={handleAccept} onUpdateStatus={handleUpdateStatus} />
            ))}
          </div>
        </div>
      )}
    </div>
  );
}

function OrderRow({ order, onAccept, onUpdateStatus }) {
  return (
    <div className="p-5">
      <div className="flex items-center justify-between mb-3">
        <div className="flex items-center gap-3">
          <div className="w-9 h-9 bg-gray-100 rounded-xl flex items-center justify-center">
            <MapPin className="w-4 h-4 text-gray-500" />
          </div>
          <div>
            <p className="text-sm font-medium text-gray-900">
              {order.fromAddress} <span className="text-gray-300 mx-1">→</span> {order.toAddress}
            </p>
            <p className="text-xs text-gray-400 flex items-center gap-1 mt-0.5">
              <Clock className="w-3 h-3" />
              {new Date(order.createdAt).toLocaleString()}
            </p>
          </div>
        </div>
        <p className="font-semibold text-gray-900 text-sm">
          {order.estimatedPrice.toLocaleString()} som
        </p>
      </div>

      <div className="flex gap-2">
        {order.status === 'Pending' && (
          <button
            onClick={() => onAccept(order.id)}
            className="flex items-center gap-1.5 px-4 py-2 bg-gray-900 text-white text-xs font-semibold rounded-lg hover:bg-gray-800"
          >
            <CheckCircle2 className="w-3.5 h-3.5" /> Qabul qilish
          </button>
        )}
        {order.status === 'Accepted' && (
          <button
            onClick={() => onUpdateStatus(order.id, 'DriverArrived')}
            className="flex items-center gap-1.5 px-4 py-2 bg-blue-600 text-white text-xs font-semibold rounded-lg hover:bg-blue-700"
          >
            <Navigation className="w-3.5 h-3.5" /> Yetib keldim
          </button>
        )}
        {order.status === 'DriverArrived' && (
          <button
            onClick={() => onUpdateStatus(order.id, 'InProgress')}
            className="flex items-center gap-1.5 px-4 py-2 bg-blue-600 text-white text-xs font-semibold rounded-lg hover:bg-blue-700"
          >
            Safarni boshlash
          </button>
        )}
        {order.status === 'InProgress' && (
          <button
            onClick={() => onUpdateStatus(order.id, 'Completed')}
            className="flex items-center gap-1.5 px-4 py-2 bg-green-600 text-white text-xs font-semibold rounded-lg hover:bg-green-700"
          >
            <CheckCircle2 className="w-3.5 h-3.5" /> Yakunlash
          </button>
        )}
        {(order.status === 'Completed' ||
          order.status === 'CancelledByCustomer' ||
          order.status === 'CancelledByDriver') && (
          <span className="text-xs text-gray-400 px-2 py-2">
            {statusLabels[order.status] || order.status}
          </span>
        )}
      </div>
    </div>
  );
}