import React from 'react';

interface LoadingSkeletonProps {
  type: 'card' | 'list' | 'table';
  count?: number;
}

const LoadingSkeleton: React.FC<LoadingSkeletonProps> = ({ type, count = 1 }) => {
  const renderCardSkeleton = () => (
    <div className="bg-white rounded-lg shadow-sm border p-6 animate-pulse">
      <div className="flex items-center justify-between">
        <div className="space-y-2">
          <div className="h-4 bg-gray-200 rounded w-24"></div>
          <div className="h-8 bg-gray-200 rounded w-16"></div>
          <div className="h-3 bg-gray-200 rounded w-20"></div>
        </div>
        <div className="h-12 w-12 bg-gray-200 rounded-full"></div>
      </div>
    </div>
  );

  const renderListSkeleton = () => (
    <div className="bg-white rounded-lg shadow-sm border">
      <div className="px-6 py-4 border-b animate-pulse">
        <div className="h-6 bg-gray-200 rounded w-32 mb-2"></div>
        <div className="h-4 bg-gray-200 rounded w-48"></div>
      </div>
      <div className="p-6 space-y-4">
        {Array.from({ length: count }).map((_, i) => (
          <div key={i} className="flex items-center justify-between p-4 border rounded-lg animate-pulse">
            <div className="flex items-center space-x-3">
              <div className="h-5 w-5 bg-gray-200 rounded-full"></div>
              <div className="space-y-2">
                <div className="h-4 bg-gray-200 rounded w-24"></div>
                <div className="h-3 bg-gray-200 rounded w-32"></div>
              </div>
            </div>
            <div className="flex items-center space-x-3">
              <div className="h-6 bg-gray-200 rounded-full w-16"></div>
              <div className="h-3 bg-gray-200 rounded w-12"></div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );

  const renderTableSkeleton = () => (
    <div className="bg-white rounded-lg shadow-sm border animate-pulse">
      <div className="px-6 py-4 border-b">
        <div className="h-6 bg-gray-200 rounded w-32 mb-2"></div>
        <div className="h-4 bg-gray-200 rounded w-48"></div>
      </div>
      <div className="p-6">
        {Array.from({ length: count }).map((_, i) => (
          <div key={i} className="border rounded-lg p-4 mb-4 last:mb-0">
            <div className="flex items-center justify-between mb-3">
              <div className="flex items-center space-x-2">
                <div className="h-4 bg-gray-200 rounded w-20"></div>
                <div className="h-6 bg-gray-200 rounded-full w-16"></div>
              </div>
              <div className="h-3 bg-gray-200 rounded w-16"></div>
            </div>
            <div className="grid grid-cols-2 gap-4">
              {Array.from({ length: 4 }).map((_, j) => (
                <div key={j} className="flex items-center space-x-2">
                  <div className="h-4 w-4 bg-gray-200 rounded"></div>
                  <div className="h-3 bg-gray-200 rounded w-24"></div>
                </div>
              ))}
            </div>
          </div>
        ))}
      </div>
    </div>
  );

  switch (type) {
    case 'card':
      return renderCardSkeleton();
    case 'list':
      return renderListSkeleton();
    case 'table':
      return renderTableSkeleton();
    default:
      return renderCardSkeleton();
  }
};

export default React.memo(LoadingSkeleton);