import { useState, useEffect } from 'react';
import { MapPin, Clock, ChevronLeft, ChevronRight } from 'lucide-react';
import api from '../../services/api';
import { SkeletonList } from '../../components/Skeleton';

const statusLabels = {
  Pending: 'Kutilmoqda',
  Accepted: 'Qabul qilindi',
  DriverArrived: 'Haydovchi yetib keldi',
  InProgress: 'Yolda',
  Completed: 'Yakunlandi',
  CancelledByCustomer: 'Bekor qilindi',
  CancelledByDriver: 'Bekor qilindi',
};

const statusStyles = {
  Pending: 'bg-amber-50 text-amber-700',
  Accepted: 'bg-blue-50 text-blue-700',
  DriverArrived: 'bg-blue-50 text-blue-700',
  InProgress: 'bg-gray-900 text-white',
  Completed: 'bg-green-50 text-green-700',
  CancelledByCustomer: 'bg-red-50 text-red-700',
  CancelledByDriver: 'bg-red-50 text-red-700',
};

export default function MyOrders() {
  const [orders, setOrders] = useState([]);
  const [pageNumber, setPageNumber] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [loading, setLoading] = useState(true);

  const loadOrders = async (page) => {
    setLoading(true);
    try {
      const response = await api.get('/Orders/my-orders', {
        params: { pageNumber: page, pageSize: 10 },
      });
      setOrders(response.data.items);
      setTotalPages(response.data.totalPages);
    } catch (err) {
      // xato bo'lsa ham davom etamiz
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadOrders(pageNumber);
  }, [pageNumber]);

  return (
    <div>
      <div className="mb-8">
        <h1 className="text-2xl font-bold text-gray-900">Buyurtmalarim</h1>
        <p className="text-gray-500 mt-1">Barcha safarlaringiz tarixi</p>
      </div>

      {loading ? (
        <div className="bg-white rounded-2xl border border-gray-200">
          <SkeletonList rows={5} />
        </div>
      ) : orders.length === 0 ? (
        <div className="bg-white rounded-2xl border border-gray-200 p-12 text-center">
          <p className="text-gray-400">Hali hech qanday buyurtma yoq.</p>
        </div>
      ) : (
        <>
          <div className="bg-white rounded-2xl border border-gray-200 divide-y divide-gray-100">
            {orders.map((order) => (
              <div key={order.id} className="p-5 flex items-center justify-between hover:bg-gray-50 transition-colors">
                <div className="flex items-start gap-4">
                  <div className="w-10 h-10 bg-gray-100 rounded-xl flex items-center justify-center shrink-0">
                    <MapPin className="w-4 h-4 text-gray-500" />
                  </div>
                  <div>
                    <p className="font-medium text-gray-900 text-sm">
                      {order.fromAddress} <span className="text-gray-300 mx-1">→</span> {order.toAddress}
                    </p>
                    <div className="flex items-center gap-1.5 mt-1 text-xs text-gray-400">
                      <Clock className="w-3 h-3" />
                      {new Date(order.createdAt).toLocaleString()}
                    </div>
                  </div>
                </div>

                <div className="flex items-center gap-4">
                  <p className="font-semibold text-gray-900 text-sm">
                    {order.estimatedPrice.toLocaleString()} som
                  </p>
                  <span
                    className={`px-3 py-1 rounded-full text-xs font-medium whitespace-nowrap ${
                      statusStyles[order.status] || 'bg-gray-100 text-gray-600'
                    }`}
                  >
                    {statusLabels[order.status] || order.status}
                  </span>
                </div>
              </div>
            ))}
          </div>

          {totalPages > 1 && (
            <div className="flex items-center justify-center gap-3 mt-6">
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
  );
}