import { useState, useEffect } from 'react';
import toast from 'react-hot-toast';
import { Trash2, ChevronLeft, ChevronRight, UserPlus } from 'lucide-react';
import api from '../../services/api';
import { SkeletonList } from '../../components/Skeleton';

export default function AdminUsers() {
  const [users, setUsers] = useState([]);
  const [pageNumber, setPageNumber] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [loading, setLoading] = useState(true);

  const [fullName, setFullName] = useState('');
  const [phoneNumber, setPhoneNumber] = useState('');
  const [password, setPassword] = useState('');
  const [role, setRole] = useState('Customer');
  const [licenseNumber, setLicenseNumber] = useState('');
  const [submitting, setSubmitting] = useState(false);

  const loadUsers = async (page) => {
    setLoading(true);
    try {
      const response = await api.get('/Users', {
        params: { pageNumber: page, pageSize: 10 },
      });
      setUsers(response.data.items);
      setTotalPages(response.data.totalPages);
    } catch (err) {
      toast.error('Foydalanuvchilarni yuklashda xatolik yuz berdi.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadUsers(pageNumber);
  }, [pageNumber]);

  const handleDelete = async (id, name) => {
    if (!window.confirm(`"${name}" ni rostdan ham ochirmoqchimisiz?`)) return;
    try {
      await api.delete(`/Users/${id}`);
      toast.success(`"${name}" ochirildi.`);
      loadUsers(pageNumber);
    } catch (err) {
      toast.error(err.response?.data?.message || 'Ochirishda xatolik yuz berdi.');
    }
  };

  const handleCreate = async (e) => {
    e.preventDefault();
    setSubmitting(true);

    try {
      await api.post('/Users', {
        fullName,
        phoneNumber,
        password,
        role,
        licenseNumber: role === 'Driver' ? licenseNumber : null,
      });

      toast.success(`"${fullName}" muvaffaqiyatli yaratildi!`);
      setFullName('');
      setPhoneNumber('');
      setPassword('');
      setRole('Customer');
      setLicenseNumber('');
      loadUsers(1);
      setPageNumber(1);
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
        <h1 className="text-2xl font-bold text-gray-900">Foydalanuvchilar</h1>
        <p className="text-gray-500 mt-1">Tizimdagi barcha foydalanuvchilarni boshqaring</p>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="lg:col-span-1 bg-white rounded-2xl border border-gray-200 p-6 h-fit">
          <h3 className="text-sm font-semibold text-gray-900 mb-4 flex items-center gap-1.5">
            <UserPlus className="w-4 h-4" /> Yangi foydalanuvchi
          </h3>

          <form onSubmit={handleCreate} className="space-y-3">
            <input
              type="text"
              value={fullName}
              onChange={(e) => setFullName(e.target.value)}
              placeholder="Ism familiya"
              required
              className="w-full px-3 py-2.5 bg-gray-50 border border-gray-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-gray-900 focus:bg-white transition-all"
            />

            <input
              type="text"
              value={phoneNumber}
              onChange={(e) => setPhoneNumber(e.target.value)}
              placeholder="+998901234567"
              required
              className="w-full px-3 py-2.5 bg-gray-50 border border-gray-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-gray-900 focus:bg-white transition-all"
            />

            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="Parol"
              required
              minLength={6}
              className="w-full px-3 py-2.5 bg-gray-50 border border-gray-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-gray-900 focus:bg-white transition-all"
            />

            <select
              value={role}
              onChange={(e) => setRole(e.target.value)}
              className="w-full px-3 py-2.5 bg-gray-50 border border-gray-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-gray-900 focus:bg-white transition-all"
            >
              <option value="Customer">Mijoz (Customer)</option>
              <option value="Driver">Haydovchi (Driver)</option>
              <option value="Admin">Admin</option>
            </select>

            {role === 'Driver' && (
              <input
                type="text"
                value={licenseNumber}
                onChange={(e) => setLicenseNumber(e.target.value)}
                placeholder="Guvohnoma raqami"
                required
                className="w-full px-3 py-2.5 bg-gray-50 border border-gray-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-gray-900 focus:bg-white transition-all"
              />
            )}

            <button
              type="submit"
              disabled={submitting}
              className="w-full py-2.5 bg-gray-900 text-white text-sm font-semibold rounded-lg hover:bg-gray-800 active:scale-[0.98] transition-all disabled:opacity-50"
            >
              {submitting ? 'Yaratilmoqda...' : 'Yaratish'}
            </button>
          </form>
        </div>

        <div className="lg:col-span-2 bg-white rounded-2xl border border-gray-200">
          <div className="px-6 py-4 border-b border-gray-100">
            <h3 className="text-sm font-semibold text-gray-900">Barcha foydalanuvchilar</h3>
          </div>

          {loading ? (
            <SkeletonList rows={5} />
          ) : users.length === 0 ? (
            <p className="text-gray-400 text-sm p-6">Foydalanuvchilar topilmadi.</p>
          ) : (
            <>
              <div className="divide-y divide-gray-100">
                {users.map((u) => (
                  <div key={u.id} className="flex items-center justify-between px-6 py-4 hover:bg-gray-50 transition-colors">
                    <div className="flex items-center gap-3">
                      <div className="w-9 h-9 bg-gray-100 rounded-full flex items-center justify-center text-gray-600 text-sm font-semibold">
                        {u.fullName.charAt(0)}
                      </div>
                      <div>
                        <p className="text-sm font-medium text-gray-900">{u.fullName}</p>
                        <p className="text-xs text-gray-400">{u.phoneNumber}</p>
                      </div>
                    </div>

                    <button
                      onClick={() => handleDelete(u.id, u.fullName)}
                      className="p-2 rounded-lg text-gray-400 hover:bg-red-50 hover:text-red-600 transition-colors"
                    >
                      <Trash2 className="w-4 h-4" />
                    </button>
                  </div>
                ))}
              </div>

              {totalPages > 1 && (
                <div className="flex items-center justify-center gap-3 py-4">
                  <button
                    disabled={pageNumber === 1}
                    onClick={() => setPageNumber((p) => p - 1)}
                    className="p-2 rounded-lg border border-gray-200 disabled:opacity-30 hover:bg-gray-50"
                  >
                    <ChevronLeft className="w-4 h-4" />
                  </button>
                  <span className="text-sm text-gray-500">
                    {pageNumber} / {totalPages}
                  </span>
                  <button
                    disabled={pageNumber === totalPages}
                    onClick={() => setPageNumber((p) => p + 1)}
                    className="p-2 rounded-lg border border-gray-200 disabled:opacity-30 hover:bg-gray-50"
                  >
                    <ChevronRight className="w-4 h-4" />
                  </button>
                </div>
              )}
            </>
          )}
        </div>
      </div>
    </div>
  );
}