export function SkeletonLine({ width = 'w-full', height = 'h-4' }) {
  return <div className={`${width} ${height} bg-gray-200 rounded-md animate-pulse`} />;
}

export function SkeletonCircle({ size = 'w-9 h-9' }) {
  return <div className={`${size} bg-gray-200 rounded-full animate-pulse`} />;
}

export function SkeletonCard({ children }) {
  return <div className="bg-white rounded-2xl border border-gray-200 p-5">{children}</div>;
}

export function SkeletonListRow() {
  return (
    <div className="flex items-center justify-between px-6 py-4">
      <div className="flex items-center gap-3">
        <SkeletonCircle />
        <div className="space-y-2">
          <SkeletonLine width="w-32" />
          <SkeletonLine width="w-24" height="h-3" />
        </div>
      </div>
      <SkeletonLine width="w-16" />
    </div>
  );
}

export function SkeletonList({ rows = 4 }) {
  return (
    <div className="divide-y divide-gray-100">
      {Array.from({ length: rows }).map((_, i) => (
        <SkeletonListRow key={i} />
      ))}
    </div>
  );
}

export function SkeletonStatCard() {
  return (
    <div className="bg-white rounded-2xl border border-gray-200 p-5">
      <SkeletonLine width="w-20" height="h-3" />
      <div className="mt-2">
        <SkeletonLine width="w-12" height="h-7" />
      </div>
    </div>
  );
}