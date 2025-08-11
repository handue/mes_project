import React, { useState, useEffect } from 'react';
import { Wifi, WifiOff, AlertCircle } from 'lucide-react';
import { apiService } from '../services/api';

interface ConnectionStatusProps {
  className?: string;
}

const ConnectionStatus: React.FC<ConnectionStatusProps> = ({ className = '' }) => {
  const [isOnline, setIsOnline] = useState(true);
  const [apiStatus, setApiStatus] = useState<'online' | 'offline' | 'checking'>('checking');
  const [lastCheck, setLastCheck] = useState<Date | null>(null);

  useEffect(() => {
    // Check initial connection
    checkApiConnection();
    
    // Monitor browser online/offline status
    const handleOnline = () => {
      setIsOnline(true);
      checkApiConnection();
    };
    
    const handleOffline = () => {
      setIsOnline(false);
      setApiStatus('offline');
    };

    window.addEventListener('online', handleOnline);
    window.addEventListener('offline', handleOffline);

    // Periodic API health check
    const interval = setInterval(checkApiConnection, 60000); // Check every minute

    return () => {
      window.removeEventListener('online', handleOnline);
      window.removeEventListener('offline', handleOffline);
      clearInterval(interval);
    };
  }, []);

  const checkApiConnection = async () => {
    if (!navigator.onLine) {
      setIsOnline(false);
      setApiStatus('offline');
      return;
    }

    setApiStatus('checking');
    
    try {
      // Try to fetch a small amount of data to test connection
      await apiService.getMachines();
      setApiStatus('online');
      setLastCheck(new Date());
    } catch (error) {
      console.warn('API connection check failed:', error);
      setApiStatus('offline');
      setLastCheck(new Date());
    }
  };

  const getStatusInfo = () => {
    if (!isOnline) {
      return {
        icon: <WifiOff className="h-4 w-4" />,
        text: 'No Internet',
        color: 'text-red-500',
        bgColor: 'bg-red-100',
      };
    }

    switch (apiStatus) {
      case 'online':
        return {
          icon: <Wifi className="h-4 w-4" />,
          text: 'API Connected',
          color: 'text-green-600',
          bgColor: 'bg-green-100',
        };
      case 'offline':
        return {
          icon: <AlertCircle className="h-4 w-4" />,
          text: 'API Disconnected',
          color: 'text-red-500',
          bgColor: 'bg-red-100',
        };
      case 'checking':
        return {
          icon: <Wifi className="h-4 w-4 animate-pulse" />,
          text: 'Checking...',
          color: 'text-yellow-600',
          bgColor: 'bg-yellow-100',
        };
      default:
        return {
          icon: <AlertCircle className="h-4 w-4" />,
          text: 'Unknown',
          color: 'text-gray-500',
          bgColor: 'bg-gray-100',
        };
    }
  };

  const status = getStatusInfo();

  return (
    <div className={`flex items-center space-x-2 ${className}`}>
      <div className={`flex items-center space-x-2 px-3 py-1 rounded-full ${status.bgColor}`}>
        <div className={status.color}>
          {status.icon}
        </div>
        <span className={`text-sm font-medium ${status.color}`}>
          {status.text}
        </span>
      </div>
      {lastCheck && (
        <span className="text-xs text-gray-500">
          Last checked: {lastCheck.toLocaleTimeString()}
        </span>
      )}
    </div>
  );
};

export default ConnectionStatus;