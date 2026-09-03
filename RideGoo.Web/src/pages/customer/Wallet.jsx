import { useState, useEffect } from 'react';
import toast from 'react-hot-toast';
import { Wallet as WalletIcon, Plus, ArrowUpRight, ArrowDownRight } from 'lucide-react';
import api from '../../services/api';
import { SkeletonLine, SkeletonList } from '../../components/Skeleton';

export default function Wallet() {
  const [wallet, setWallet] = useState(null);
  const [transactions, setTransactions] = useState([]);
  const [amount, setAmount] = useState('');
  const [description, setDescription] = useState('');
  const [loading, setLoading] = useState(false);
  const [pageLoading, setPageLoading] = useState(true);

  const loadWalletData = async () => {
    try {
      const [walletRes, transactionsRes] = await Promise.all([
        api.get('/Wallet/my'),
        api.get('/Wallet/transactions'),
      ]);
      setWallet(walletRes.data);
      setTransactions(transactionsRes.data);
    } catch (err) {
      toast.error('Hamyon malumotlarini yuklashda xatolik yuz berdi.');
    } finally {
      setPageLoading(false);
    }
  };

  useEffect(() => {
    loadWalletData();
  }, []);

  const handleTopUp = async (e) => {
    e.preventDefault();
    setLoading(true);

    try {
      await api.post('/Wallet/top-up', {
        amount: Number(amount),
        description: description || null,
      });

      toast.success(`Hamyon ${Number(amount).toLocaleString()} som ga toldirildi!`);
      setAmount('');
      setDescription('');
      await loadWalletData();
    } catch (err) {
      const message = err.response?.data?.message || 'Toldirishda xatolik yuz berdi.';
      toast.error(message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <div className="mb-8">
        <h1 className="text-2xl font-bold text-gray-900">Hamyon</h1>
        <p className="text-gray-500 mt-1">Balansingizni boshqaring va tarixni koring</p>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="lg:col-span-1 space-y-6">
          {pageLoading ? (
            <div className="bg-gray-900 rounded-2xl p-6">
              <SkeletonLine width="w-24" height="h-3" />
              <div className="mt-6">
                <SkeletonLine width="w-32" height="h-8" />
              </div>
            </div>
          ) : (
            <div className="bg-gray-900 rounded-2xl p-6 text-white">
              <div className="flex items-center justify-between mb-6">
                <div className="w-10 h-10 bg-white/10 rounded-xl flex items-center justify-center">
                  <WalletIcon className="w-5 h-5" />
                </div>
                <span className="text-xs text-gray-400 uppercase tracking-wide">Balans</span>
              </div>
              <p className="text-3xl font-bold">{wallet?.balance.toLocaleString()}</p>
              <p className="text-gray-400 text-sm mt-1">{wallet?.currency}</p>
            </div>
          )}

          <div className="bg-white rounded-2xl border border-gray-200 p-6">
            <h3 className="text-sm font-semibold text-gray-900 mb-4">Hamyonni toldirish</h3>
            <form onSubmit={handleTopUp} className="space-y-3">
              <input
                type="number"
                value={amount}
                onChange={(e) => setAmount(e.target.value)}
                placeholder="Summa (som)"
                required
                min="1"
                className="w-full px-3 py-2.5 bg-gray-50 border border-gray-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-gray-900 focus:bg-white transition-all"
              />
              <input
                type="text"
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                placeholder="Izoh (ixtiyoriy)"
                className="w-full px-3 py-2.5 bg-gray-50 border border-gray-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-gray-900 focus:bg-white transition-all"
              />

              <button
                type="submit"
                disabled={loading}
                className="w-full flex items-center justify-center gap-1.5 py-2.5 bg-gray-900 text-white text-sm font-semibold rounded-lg hover:bg-gray-800 active:scale-[0.98] transition-all disabled:opacity-50"
              >
                <Plus className="w-4 h-4" />
                {loading ? 'Yuborilmoqda...' : 'Toldirish'}
              </button>
            </form>
          </div>
        </div>

        <div className="lg:col-span-2 bg-white rounded-2xl border border-gray-200">
          <div className="px-6 py-4 border-b border-gray-100">
            <h3 className="text-sm font-semibold text-gray-900">Tranzaksiyalar tarixi</h3>
          </div>

          {pageLoading ? (
            <SkeletonList rows={4} />
          ) : transactions.length === 0 ? (
            <p className="text-gray-400 text-sm p-6">Hali hech qanday tranzaksiya yoq.</p>
          ) : (
            <div className="divide-y divide-gray-100">
              {transactions.map((t) => (
                <div key={t.id} className="flex items-center justify-between px-6 py-4">
                  <div className="flex items-center gap-3">
                    <div
                      className={`w-9 h-9 rounded-full flex items-center justify-center ${
                        t.type === 'TopUp' ? 'bg-green-50' : 'bg-red-50'
                      }`}
                    >
                      {t.type === 'TopUp' ? (
                        <ArrowDownRight className="w-4 h-4 text-green-600" />
                      ) : (
                        <ArrowUpRight className="w-4 h-4 text-red-600" />
                      )}
                    </div>
                    <div>
                      <p className="text-sm font-medium text-gray-900">
                        {t.type === 'TopUp' ? 'Toldirish' : t.type}
                      </p>
                      <p className="text-xs text-gray-400">
                        {new Date(t.createdAt).toLocaleString()}
                        {t.description ? ` · ${t.description}` : ''}
                      </p>
                    </div>
                  </div>
                  <p
                    className={`text-sm font-semibold ${
                      t.type === 'TopUp' ? 'text-green-600' : 'text-red-600'
                    }`}
                  >
                    {t.type === 'TopUp' ? '+' : '-'}
                    {t.amount.toLocaleString()} som
                  </p>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}