import { useState, useEffect } from 'react';
import toast from 'react-hot-toast';
import { ArrowUpFromLine, Check, X, CreditCard } from 'lucide-react';
import api from '../../services/api';
import { SkeletonList } from '../../components/Skeleton';

export default function AdminWithdrawals() {
  const [requests, setRequests] = useState([]);
  const [loading, setLoading] = useState(true);
  const [processingId, setProcessingId] = useState(null);

  const loadRequests = async () => {
    setLoading(true);
    try {
      const response = await api.get('/Withdrawals/pending');
      setRequests(response.data);
    } catch (err) {
      toast.error('Sorovlarni yuklashda xatolik yuz berdi.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadRequests();
  }, []);

  const handleProcess = async (id, approve) => {
    setProcessingId(id);
    try {
      await api.put(`/Withdrawals/${id}/process`, {
        approve,
        adminComment: approve ? 'Tasdiqlandi' : 'Rad etildi',
      });
      toast.success(approve ? 'Sorov tasdiqlandi!' : 'Sorov rad etildi.');
      loadRequests();
    } catch (err) {
      toast.error(err.response?.data?.message || 'Amalni bajarishda xatolik.');
    } finally {
      setProcessingId(null);
    }
  };

  return (
    <div>
      <div className="mb-8">
        <h1 className="text-2xl font-bold text-gray-900">Pul yechish sorovlari</h1>
        <p className="text-gray-500 mt-1">Haydovchilarning kutilayotgan sorovlarini korib chiqing</p>
      </div>

      <div className="bg-white rounded-2xl border border-gray-200">
        {loading ? (
          <SkeletonList rows={4} />
        ) : requests.length === 0 ? (
          <div className="p-12 text-center">
            <ArrowUpFromLine className="w-8 h-8 text-gray-300 mx-auto mb-2" />
            <p className="text-gray-400">Hozircha kutilayotgan sorov yoq.</p>
          </div>
        ) : (
          <div className="divide-y divide-gray-100">
            {requests.map((r) => (
              <div key={r.id} className="flex items-center justify-between px-6 py-4">
                <div className="flex items-center gap-3">
                  <div className="w-10 h-10 bg-amber-50 rounded-xl flex items-center justify-center">
                    <ArrowUpFromLine className="w-4 h-4 text-amber-600" />
                  </div>
                  <div>
                    <p className="text-sm font-semibold text-gray-900">{r.driverName}</p>
                    <p className="text-xs text-gray-400">
                      {new Date(r.createdAt).toLocaleString()}
                      {r.cardNumber && (
                        <span className="inline-flex items-center gap-1 ml-2">
                          <CreditCard className="w-3 h-3" /> {r.cardNumber}
                        </span>
                      )}
                    </p>
                  </div>
                </div>

                <div className="flex items-center gap-3">
                  <p className="font-bold text-gray-900 text-sm">{r.amount.toLocaleString()} som</p>

                  <button
                    onClick={() => handleProcess(r.id, true)}
                    disabled={processingId === r.id}
                    className="flex items-center gap-1 px-3 py-2 bg-green-600 text-white text-xs font-semibold rounded-lg hover:bg-green-700 disabled:opacity-50"
                  >
                    <Check className="w-3.5 h-3.5" /> Tasdiqlash
                  </button>

                  <button
                    onClick={() => handleProcess(r.id, false)}
                    disabled={processingId === r.id}
                    className="flex items-center gap-1 px-3 py-2 bg-red-50 text-red-600 text-xs font-semibold rounded-lg hover:bg-red-100 disabled:opacity-50"
                  >
                    <X className="w-3.5 h-3.5" /> Rad etish
                  </button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}