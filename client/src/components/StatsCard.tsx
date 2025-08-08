import React from 'react';

interface StatsCardProps {
  title: string;
  value: string | number;
  subtitle?: string;
  icon: React.ReactNode;
  color: 'blue' | 'green' | 'yellow' | 'purple' | 'red';
}

const StatsCard: React.FC<StatsCardProps> = ({ title, value, subtitle, icon, color }) => {
  const colorClasses = {
    blue: 'bg-blue-500 text-blue-600',
    green: 'bg-green-500 text-green-600',
    yellow: 'bg-yellow-500 text-yellow-600',
    purple: 'bg-purple-500 text-purple-600',
    red: 'bg-red-500 text-red-600',
  };

  return (
    <div className="bg-white rounded-lg shadow-sm border p-6">
      <div className="flex items-center justify-between">
        <div>
          <p className="text-sm font-medium text-gray-600">{title}</p>
          <p className="text-2xl font-bold text-gray-900 mt-1">{value}</p>
          {subtitle && <p className="text-sm text-gray-500 mt-1">{subtitle}</p>}
        </div>
        <div className={`p-3 rounded-full ${colorClasses[color].split(' ')[0]} bg-opacity-10`}>
          <div className={colorClasses[color].split(' ')[1]}>
            {icon}
          </div>
        </div>
      </div>
    </div>
  );
};

export default StatsCard;