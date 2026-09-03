import { useState, useEffect } from 'react';
import toast from 'react-hot-toast';
import { Tag, Plus, Percent } from 'lucide-react';
import api from '../../services/api';

export default function AdminPromoCodes() {
  const [promoCodes, setPromoCodes] = useState([]);
  const [loading, setLoading] = useState(true);

  const [code, setCode] = useState('');
  const [discountType, setDiscountType] = useState('Percentage');
  const [discountValue, setDiscountValue] = useState('');
  const [validFrom, setValidFrom] = useState('');
  const [validTo, setValidTo] = useState('');
  const [maxUsageCount, setMaxUsageCount] = useState('');
  const [submitting, setSubmitting] = useState(false);

  const loadPromoCodes = async () => {
    setLoading(true);
    try {
      const response = await api.get('/PromoCodes');
      setPromoCodes(response.data);
    } catch (err) {
      toast.error('Promo-kodlarni yuklashda xatolik yuz berdi.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadPromoCodes();
  }, []);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSubmitting(true);

    try {
      await api.post('/PromoCodes', {
        code,
        discountType,
        discountValue: Number(discountValue),
        validFrom: new Date(validFrom).toISOString(),
        validTo: new Date(validTo).toISOString(),
        maxUsageCount: Number(maxUsageCount),
      });

      toast.success(`"${code}" promo-kodi yaratildi!`);
      setCode('');
      setDiscountValue('');
      setValidFrom('');
      setValidTo('');
      setMaxUsageCount('');
      loadPromoCodes();
    } catch (err) {
      const message = err.response?.data?.message || 'Yaratishda xatolik yuz berdi.';
      toast.error(message);
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div>
      <div className="mb-8">
        <h1 className="text-2xl font-bold text-gray-900">Promo-kodlar</h1>
        <p className="text-gray-500 mt-1">Chegirma kodlarini yarating va boshqaring</p>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="lg:col-span-1 bg-white rounded-2xl border border-gray-200 p-6 h-fit">
          <h3 className="text-sm font-semibold text-gray-900 mb-4">Yangi promo-kod</h3>

          <form onSubmit={handleSubmit} className="space-y-3">
            <input
              type="text"
              value={code}
              onChange={(e) => setCode(e.target.value.toUpperCase())}
              placeholder="Kod (masalan: SUMMER20)"
              required
              className="w-full px-3 py-2.5 bg-gray-50 border border-gray-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-gray-900 focus:bg-white transition-all"
            />

            <select
              value={discountType}
              onChange={(e) => setDiscountType(e.target.value)}
              className="w-full px-3 py-2.5 bg-gray-50 border border-gray-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-gray-900 focus:bg-white transition-all"
            >
              <option value="Percentage">Foizli (%)</option>
              <option value="FixedAmount">Belgilangan summa (som)</option>
            </select>

            <input
              type="number"
              value={discountValue}
              onChange={(e) => setDiscountValue(e.target.value)}
              placeholder="Chegirma qiymati"
              required
              className="w-full px-3 py-2.5 bg-gray-50 border border-gray-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-gray-900 focus:bg-white transition-all"
            />

            <div className="grid grid-cols-2 gap-2">
              <div>
                <label className="text-xs text-gray-400 mb-1 block">Boshlanish</label>
                <input
                  type="date"
                  value={validFrom}
                  onChange={(e) => setValidFrom(e.target.value)}
                  required
                  className="w-full px-3 py-2 bg-gray-50 border border-gray-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-gray-900 focus:bg-white transition-all"
                />
              </div>
              <div>
                <label className="text-xs text-gray-400 mb-1 block">Tugash</label>
                <input
                  type="date"
                  value={validTo}
                  onChange={(e) => setValidTo(e.target.value)}
                  required
                  className="w-full px-3 py-2 bg-gray-50 border border-gray-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-gray-900 focus:bg-white transition-all"
                />
              </div>
            </div>

            <input
              type="number"
              value={maxUsageCount}
              onChange={(e) => setMaxUsageCount(e.target.value)}
              placeholder="Maksimal ishlatilish soni"
              required
              className="w-full px-3 py-2.5 bg-gray-50 border border-gray-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-gray-900 focus:bg-white transition-all"
            />

            <button
              type="submit"
              disabled={submitting}
              className="w-full flex items-center justify-center gap-1.5 py-2.5 bg-gray-900 text-white text-sm font-semibold rounded-lg hover:bg-gray-800 active:scale-[0.98] transition-all disabled:opacity-50"
            >
              <Plus className="w-4 h-4" />
              {submitting ? 'Yaratilmoqda...' : 'Yaratish'}
            </button>
          </form>
        </div>

        <div className="lg:col-span-2 bg-white rounded-2xl border border-gray-200">
          <div className="px-6 py-4 border-b border-gray-100">
            <h3 className="text-sm font-semibold text-gray-900">Mavjud promo-kodlar</h3>
          </div>

          {loading ? (
            <p className="text-gray-400 text-sm p-6">Yuklanmoqda...</p>
          ) : promoCodes.length === 0 ? (
            <p className="text-gray-400 text-sm p-6">Hali promo-kod yoq.</p>
          ) : (
            <div className="divide-y divide-gray-100">
              {promoCodes.map((p) => (
                <div key={p.id} className="flex items-center justify-between px-6 py-4">
                  <div className="flex items-center gap-3">
                    <div className="w-9 h-9 bg-gray-100 rounded-xl flex items-center justify-center">
                      <Tag className="w-4 h-4 text-gray-500" />
                    </div>
                    <div>
                      <p className="text-sm font-semibold text-gray-900">{p.code}</p>
                      <div className="flex items-center gap-3 text-xs text-gray-400 mt-0.5">
                        <span className="flex items-center gap-1">
                          <Percent className="w-3 h-3" />
                          {p.discountValue}{p.discountType === 'Percentage' ? '%' : ' som'}
                        </span>
                        <span>{p.currentUsageCount} / {p.maxUsageCount} ishlatilgan</span>
                      </div>
                    </div>
                  </div>

                  <span
                    className={`px-3 py-1 rounded-full text-xs font-medium ${
                      p.isActive ? 'bg-green-50 text-green-700' : 'bg-gray-100 text-gray-500'
                    }`}
                  >
                    {p.isActive ? 'Faol' : 'Faol emas'}
                  </span>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}