import { useEffect, useRef } from 'react';
import * as signalR from '@microsoft/signalr';

const HUB_URL = 'https://localhost:7001/hubs/ride';

export function useSignalR(onNewOrder, onOrderUpdated) {
  const connectionRef = useRef(null);

  useEffect(() => {
    const token = localStorage.getItem('token');
    if (!token) return;

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(HUB_URL, {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect()
      .build();

    if (onNewOrder) {
      connection.on('NewOrderAvailable', onNewOrder);
    }

    if (onOrderUpdated) {
      connection.on('OrderStatusUpdated', onOrderUpdated);
    }

    connection
      .start()
      .catch((err) => console.error('SignalR ulanish xatosi:', err));

    connectionRef.current = connection;

    return () => {
      connection.stop();
    };
  }, []);

  return connectionRef;
}