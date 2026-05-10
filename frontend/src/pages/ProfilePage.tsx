// ============================================
// CARSTAR — ProfilePage
// GET /api/account/profile
// GET /api/orders/user/{accountId}
// DELETE /api/orders/{id}/cancel
// ============================================
import { useState, useEffect } from 'react';
import type { ResponseOrderDto, OrderStatus } from '../types';
import { orderApi } from '../api';
import { useAuth } from '../hooks/useAuth';
import styles from './ProfilePage.module.css';

const STATUS_LABELS: Record<OrderStatus, string> = {
  Pending:    'Очікує',
  Processing: 'Обробка',
  Shipped:    'Відправлено',
  Delivered:  'Доставлено',
  Cancelled:  'Скасовано',
};

type Section = 'profile' | 'orders';

export default function ProfilePage() {
  const { account, logout } = useAuth();
  const [section, setSection]   = useState<Section>('profile');
  const [orders, setOrders]     = useState<ResponseOrderDto[]>([]);
  const [ordersLoading, setOrdersLoading] = useState(false);

  useEffect(() => {
    if (section === 'orders' && account) {
      setOrdersLoading(true);
      orderApi.getMyOrders()
        .then(setOrders)
        .catch(() => setOrders([]))
        .finally(() => setOrdersLoading(false));
    }
  }, [section, account]);

  const handleCancelOrder = async (orderId: number) => {
    if (!account) return;
    if (!window.confirm('Скасувати замовлення?')) return;
    try {
      await orderApi.cancel(orderId);
      setOrders(prev =>
        prev.map(o => o.id === orderId ? { ...o, status: 'Cancelled' as OrderStatus } : o)
      );
    } catch (err: any) {
      alert(err.message);
    }
  };

  return (
    <div className={styles.page}>
      <div className={styles.layout}>
        {/* Sidebar */}
        <aside className={styles.sidebar}>
          <div className={styles.avatar}>
            {account?.userName.slice(0, 2).toUpperCase()}
          </div>
          <div className={styles.userName}>{account?.userName}</div>
          <div className={styles.userEmail}>{account?.email}</div>

          <nav className={styles.nav}>
            <button
              className={`${styles.navItem} ${section === 'profile' ? styles.navActive : ''}`}
              onClick={() => setSection('profile')}
            >
              Профіль
            </button>
            <button
              className={`${styles.navItem} ${section === 'orders' ? styles.navActive : ''}`}
              onClick={() => setSection('orders')}
            >
              Мої замовлення
              {orders.length > 0 && (
                <span className={styles.navBadge}>{orders.length}</span>
              )}
            </button>
            <div className={styles.navSep} />
            <button
              className={`${styles.navItem} ${styles.navLogout}`}
              onClick={logout}
            >
              Вийти
            </button>
          </nav>
        </aside>

        {/* Main */}
        <main className={styles.main}>
          {section === 'profile' && (
            <div>
              <h2 className={styles.sectionTitle}>Особисті дані</h2>
              <div className={styles.infoGrid}>
                <div className={styles.field}>
                  <label>Username</label>
                  <input defaultValue={account?.userName} readOnly />
                </div>
                <div className={styles.field}>
                  <label>Email</label>
                  <input defaultValue={account?.email} readOnly />
                </div>
                <div className={styles.field}>
                  <label>ID аккаунту</label>
                  <input value={`#${account?.id}`} readOnly />
                </div>
              </div>
              <div className={styles.note}>
                Для зміни даних скористайтесь відповідним ендпоінтом API.
              </div>
            </div>
          )}

          {section === 'orders' && (
            <div>
              <h2 className={styles.sectionTitle}>Мої замовлення</h2>
              {ordersLoading ? (
                <div className={styles.loading}>Завантаження...</div>
              ) : orders.length === 0 ? (
                <div className={styles.empty}>Замовлень ще немає</div>
              ) : (
                <div className={styles.ordersList}>
                  {orders.map(order => (
                    <div key={order.id} className={styles.orderRow}>
                      <div className={styles.orderLeft}>
                        <div className={styles.orderId}>
                          #{`ORD-${String(order.id).padStart(5, '0')}`}
                        </div>
                        <div className={styles.orderDate}>
                          {new Date(order.createdAt).toLocaleDateString('uk-UA', {
                            day: '2-digit', month: 'short', year: 'numeric'
                          })}
                        </div>
                        <div className={styles.orderAddress}>
                          {order.shippingAddress}
                        </div>
                      </div>

                      <div className={styles.orderMeta}>
                        <span
                          className={`${styles.status} status-${order.status.toLowerCase()}`}
                        >
                          {STATUS_LABELS[order.status]}
                        </span>
                        <span className={styles.orderAmount}>
                          ₴ {order.totalAmount.toLocaleString('uk-UA')}
                        </span>
                      </div>

                      <div className={styles.orderActions}>
                        <div className={styles.orderItems}>
                          {order.orderItems.length} поз.
                        </div>
                        {order.status === 'Pending' && (
                          <button
                            className={styles.cancelBtn}
                            onClick={() => handleCancelOrder(order.id)}
                          >
                            Скасувати
                          </button>
                        )}
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          )}
        </main>
      </div>
    </div>
  );
}
