import { useState } from 'react';
import { MapContainer, TileLayer, Marker, useMapEvents, useMap } from 'react-leaflet';
import 'leaflet/dist/leaflet.css';
import L from 'leaflet';
import toast from 'react-hot-toast';
import { MapPin, Navigation, Tag, Search, CheckCircle2 } from 'lucide-react';
import api from '../../services/api';
import { useSignalR } from '../../hooks/useSignalR';

delete L.Icon.Default.prototype._getIconUrl;
L.Icon.Default.mergeOptions({
  iconRetinaUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon-2x.png',
  iconUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png',
  shadowUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png',
});

const UZBEKISTAN_CENTER = [41.377491, 64.585262];
const DEFAULT_ZOOM = 6;

const statusMessages = {
  Accepted: 'Haydovchi buyurtmangizni qabul qildi!',
  DriverArrived: 'Haydovchi manzilga yetib keldi!',
  InProgress: 'Safaringiz boshlandi!',
  Completed: 'Safar yakunlandi. Rahmat!',
  CancelledByDriver: 'Afsuski, haydovchi buyurtmani bekor qildi.',
};

function LocationPicker({ onSelect }) {
  useMapEvents({
    click(e) {
      onSelect([e.latlng.lat, e.latlng.lng]);
    },
  });
  return null;
}

function FlyToLocation({ position }) {
  const map = useMap();
  if (position) {
    map.flyTo(position, 15);
  }
  return null;
}

function AddressSearchBox({ onResultSelect }) {
  const [query, setQuery] = useState('');
  const [results, setResults] = useState([]);
  const [searching, setSearching] = useState(false);

  const handleSearch = async () => {
    if (!query.trim()) return;
    setSearching(true);
    try {
      const searchQuery = query + ", O'zbekiston";
      const response = await fetch(
        'https://nominatim.openstreetmap.org/search?format=json&q=' +
          encodeURIComponent(searchQuery) +
          '&limit=5'
      );
      const data = await response.json();
      setResults(data);
      if (data.length === 0) {
        toast.error('Hech narsa topilmadi. Boshqacha qidirib koring.');
      }
    } catch (err) {
      setResults([]);
      toast.error('Qidirishda xatolik yuz berdi.');
    } finally {
      setSearching(false);
    }
  };

  const handleKeyDown = (e) => {
    if (e.key === 'Enter') {
      e.preventDefault();
      handleSearch();
    }
  };

  return (
    <div className="relative">
      <div className="flex gap-2">
        <input
          type="text"
          value={query}
          onChange={(e) => setQuery(e.target.value)}
          onKeyDown={handleKeyDown}
          placeholder="Shahar yoki kocha nomini qidiring"
          className="flex-1 px-3 py-2.5 bg-gray-50 border border-gray-200 rounded-lg text-sm text-gray-700 placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-gray-900 focus:bg-white transition-all"
        />
        <button
          type="button"
          onClick={handleSearch}
          disabled={searching}
          className="px-3 py-2.5 bg-gray-900 rounded-lg hover:bg-gray-800 transition-colors"
        >
          <Search className="w-4 h-4 text-white" />
        </button>
      </div>

      {results.length > 0 && (
        <ul className="absolute z-[1000] mt-1 w-full bg-white border border-gray-200 rounded-lg shadow-xl overflow-hidden">
          {results.map((r) => (
            <li
              key={r.place_id}
              onClick={() => {
                onResultSelect([parseFloat(r.lat), parseFloat(r.lon)], r.display_name);
                setResults([]);
                setQuery(r.display_name);
              }}
              className="px-3 py-2.5 text-sm text-gray-700 cursor-pointer hover:bg-gray-50 border-b border-gray-100 last:border-0"
            >
              {r.display_name}
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}

export default function OrderCreate() {
  const [fromAddress, setFromAddress] = useState('');
  const [toAddress, setToAddress] = useState('');
  const [fromCoords, setFromCoords] = useState(null);
  const [toCoords, setToCoords] = useState(null);
  const [pickingMode, setPickingMode] = useState('from');
  const [flyTarget, setFlyTarget] = useState(null);
  const [promoCode, setPromoCode] = useState('');
  const [result, setResult] = useState(null);
  const [loading, setLoading] = useState(false);

  // Real-time: buyurtma holati ozgarganda avtomatik bildirishnoma
  useSignalR(null, (data) => {
    setResult((prev) => {
      if (prev && prev.id === data.orderId) {
        return { ...prev, status: data.status };
      }
      return prev;
    });

    const message = statusMessages[data.status] || `Buyurtma holati: ${data.status}`;
    toast.success(message, { duration: 5000 });
  });

  const handleMapClick = (coords) => {
    if (pickingMode === 'from') {
      setFromCoords(coords);
    } else {
      setToCoords(coords);
    }
  };

  const handleFromSearchSelect = (coords, displayName) => {
    setFromCoords(coords);
    setFromAddress(displayName);
    setFlyTarget(coords);
  };

  const handleToSearchSelect = (coords, displayName) => {
    setToCoords(coords);
    setToAddress(displayName);
    setFlyTarget(coords);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setResult(null);

    if (!fromCoords || !toCoords) {
      toast.error('Iltimos, qayerdan va qayerga manzillarini belgilang.');
      return;
    }

    setLoading(true);
    try {
      const response = await api.post('/Orders', {
        fromAddress,
        fromLatitude: fromCoords[0],
        fromLongitude: fromCoords[1],
        toAddress,
        toLatitude: toCoords[0],
        toLongitude: toCoords[1],
        source: 'Website',
        promoCode: promoCode || null,
      });

      setResult(response.data);
      toast.success('Buyurtma muvaffaqiyatli yaratildi!');
    } catch (err) {
      const message = err.response?.data?.message || 'Buyurtma berishda xatolik yuz berdi.';
      toast.error(message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <div className="mb-8">
        <h1 className="text-2xl font-bold text-gray-900">Buyurtma berish</h1>
        <p className="text-gray-500 mt-1">Manzillaringizni belgilang va safarni boshlang</p>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-5 gap-6">
        <div className="lg:col-span-2 bg-white rounded-2xl border border-gray-200 p-6 h-fit">
          <form onSubmit={handleSubmit} className="space-y-5">
            <div>
              <div className="flex items-center gap-2 mb-2">
                <span className="w-2 h-2 rounded-full bg-gray-900" />
                <label className="text-xs font-semibold text-gray-500 uppercase tracking-wide">
                  Qayerdan
                </label>
              </div>
              <AddressSearchBox onResultSelect={handleFromSearchSelect} />
              <input
                type="text"
                value={fromAddress}
                onChange={(e) => setFromAddress(e.target.value)}
                onFocus={() => setPickingMode('from')}
                placeholder="Yoki manzilni qolda yozing"
                required
                className="w-full mt-2 px-3 py-2.5 bg-gray-50 border border-gray-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-gray-900 focus:bg-white transition-all"
              />
              {fromCoords && (
                <p className="flex items-center gap-1 mt-1.5 text-xs text-gray-500">
                  <CheckCircle2 className="w-3 h-3 text-gray-900" /> Xaritada belgilandi
                </p>
              )}
            </div>

            <div className="border-l-2 border-dashed border-gray-200 h-4 ml-1" />

            <div>
              <div className="flex items-center gap-2 mb-2">
                <span className="w-2 h-2 rounded-full bg-gray-400" />
                <label className="text-xs font-semibold text-gray-500 uppercase tracking-wide">
                  Qayerga
                </label>
              </div>
              <AddressSearchBox onResultSelect={handleToSearchSelect} />
              <input
                type="text"
                value={toAddress}
                onChange={(e) => setToAddress(e.target.value)}
                onFocus={() => setPickingMode('to')}
                placeholder="Yoki manzilni qolda yozing"
                required
                className="w-full mt-2 px-3 py-2.5 bg-gray-50 border border-gray-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-gray-900 focus:bg-white transition-all"
              />
              {toCoords && (
                <p className="flex items-center gap-1 mt-1.5 text-xs text-gray-500">
                  <CheckCircle2 className="w-3 h-3 text-gray-900" /> Xaritada belgilandi
                </p>
              )}
            </div>

            <div className="grid grid-cols-2 gap-2 pt-1">
              <button
                type="button"
                onClick={() => setPickingMode('from')}
                className={`flex items-center justify-center gap-1.5 py-2.5 rounded-lg text-xs font-semibold transition-all ${
                  pickingMode === 'from'
                    ? 'bg-gray-900 text-white'
                    : 'bg-gray-100 text-gray-500 hover:bg-gray-200'
                }`}
              >
                <MapPin className="w-3.5 h-3.5" /> Qayerdan
              </button>
              <button
                type="button"
                onClick={() => setPickingMode('to')}
                className={`flex items-center justify-center gap-1.5 py-2.5 rounded-lg text-xs font-semibold transition-all ${
                  pickingMode === 'to'
                    ? 'bg-gray-900 text-white'
                    : 'bg-gray-100 text-gray-500 hover:bg-gray-200'
                }`}
              >
                <Navigation className="w-3.5 h-3.5" /> Qayerga
              </button>
            </div>

            <div>
              <label className="flex items-center gap-1.5 text-xs font-semibold text-gray-500 uppercase tracking-wide mb-2">
                <Tag className="w-3.5 h-3.5" />
                Promo-kod
              </label>
              <input
                type="text"
                value={promoCode}
                onChange={(e) => setPromoCode(e.target.value)}
                placeholder="Masalan: SUMMER20"
                className="w-full px-3 py-2.5 bg-gray-50 border border-gray-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-gray-900 focus:bg-white transition-all"
              />
            </div>

            <button
              type="submit"
              disabled={loading}
              className="w-full py-3 bg-gray-900 text-white font-semibold rounded-lg hover:bg-gray-800 active:scale-[0.98] transition-all disabled:opacity-50"
            >
              {loading ? 'Yuborilmoqda...' : 'Buyurtma berish'}
            </button>
          </form>

          {result && (
            <div className="mt-5 p-4 bg-gray-900 rounded-xl text-white">
              <div className="flex items-center gap-2 mb-3">
                <CheckCircle2 className="w-4 h-4" />
                <h3 className="font-semibold text-sm">Buyurtma yaratildi</h3>
              </div>
              <div className="grid grid-cols-2 gap-3 text-sm">
                <div>
                  <p className="text-gray-400 text-xs">Holat</p>
                  <p className="font-medium">{result.status}</p>
                </div>
                <div>
                  <p className="text-gray-400 text-xs">Masofa</p>
                  <p className="font-medium">{result.distanceKm.toFixed(2)} km</p>
                </div>
                <div className="col-span-2 pt-2 border-t border-gray-700">
                  <p className="text-gray-400 text-xs">Narx</p>
                  <p className="font-bold text-xl">{result.estimatedPrice.toFixed(0)} som</p>
                </div>
              </div>
            </div>
          )}
        </div>

        <div className="lg:col-span-3 rounded-2xl overflow-hidden border border-gray-200 h-[640px]">
          <MapContainer center={UZBEKISTAN_CENTER} zoom={DEFAULT_ZOOM} style={{ height: '100%', width: '100%' }}>
            <TileLayer
              url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
              attribution="OpenStreetMap contributors"
            />
            <LocationPicker onSelect={handleMapClick} />
            <FlyToLocation position={flyTarget} />
            {fromCoords && <Marker position={fromCoords} />}
            {toCoords && <Marker position={toCoords} />}
          </MapContainer>
        </div>
      </div>
    </div>
  );
}