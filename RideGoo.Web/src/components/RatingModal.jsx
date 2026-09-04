import { useState } from 'react';
import { Star, X } from 'lucide-react';
import toast from 'react-hot-toast';
import api from '../services/api';

export default function RatingModal({ orderId, onClose, onSuccess }) {
  const [score, setScore] = useState(0);
  const [hoverScore, setHoverScore] = useState(0);
  const [comment, setComment] = useState('');
  const [submitting, setSubmitting] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (score === 0) {
      toast.error('Iltimos, baho tanlang.');
      return;
    }

    setSubmitting(true);
    try {
      await api.post('/Ratings', {
        orderId,
        score,
        comment: comment || null,
      });
      toast.success('Bahoyingiz uchun rahmat!');
      onSuccess();
      onClose();
    } catch (err) {
      toast.error(err.response?.data?.message || 'Baho qoyishda xatolik yuz berdi.');
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black/50 z-50 flex items-center justify-center p-4">
      <div className="bg-white rounded-2xl w-full max-w-sm p-6 relative">
        <button
          onClick={onClose}
          className="absolute top-4 right-4 text-gray-400 hover:text-gray-600"
        >
          <X className="w-5 h-5" />
        </button>

        <h3 className="text-lg font-bold text-gray-900 mb-1">Safarni baholang</h3>
        <p className="text-sm text-gray-500 mb-5">Haydovchi xizmatidan mamnunmisiz?</p>

        <form onSubmit={handleSubmit}>
          <div className="flex items-center justify-center gap-2 mb-5">
            {[1, 2, 3, 4, 5].map((value) => (
              <button
                key={value}
                type="button"
                onClick={() => setScore(value)}
                onMouseEnter={() => setHoverScore(value)}
                onMouseLeave={() => setHoverScore(0)}
                className="transition-transform hover:scale-110"
              >
                <Star
                  className={`w-9 h-9 ${
                    value <= (hoverScore || score)
                      ? 'fill-amber-400 text-amber-400'
                      : 'text-gray-200'
                  }`}
                />
              </button>
            ))}
          </div>

          <textarea
            value={comment}
            onChange={(e) => setComment(e.target.value)}
            placeholder="Izoh qoldiring (ixtiyoriy)"
            rows={3}
            className="w-full px-3 py-2.5 bg-gray-50 border border-gray-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-gray-900 focus:bg-white transition-all resize-none"
          />

          <button
            type="submit"
            disabled={submitting}
            className="w-full mt-4 py-2.5 bg-gray-900 text-white text-sm font-semibold rounded-lg hover:bg-gray-800 active:scale-[0.98] transition-all disabled:opacity-50"
          >
            {submitting ? 'Yuborilmoqda...' : 'Baho qoyish'}
          </button>
        </form>
      </div>
    </div>
  );
}