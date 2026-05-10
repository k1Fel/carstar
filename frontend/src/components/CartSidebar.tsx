// ============================================
// CARSTAR — CartSidebar
// GET /api/cart/{accountId}
// POST /api/orders  (checkout)
// ============================================
import { useState } from 'react';
import { useCart } from '../hooks/useCart';
import { useAuth } from '../hooks/useAuth';
import { orderApi } from '../api';
import styles from './CartSidebar.module.css';

interface CartSidebarProps {
  onClose: () => void;
}

export default function CartSidebar({ onClose }: CartSidebarProps) {
  const { cart, updateItem, removeItem, clearCart, loading } = useCart();
  const { account } = useAuth();
  const [address, setAddress] = useState('');
  const [ordering, setOrdering] = useState(false);
  const [orderSuccess, setOrderSuccess] = useState<number | null>(null);

  const handleCheckout = async () => {
    if (!account || !cart || !address.trim()) {
      alert('Вкажіть адресу доставки');
      return;
    }
    setOrdering(true);
    try {
      const order = await orderApi.create({
        accountId: account.id,
        totalAmount: cart.totalAmount,
        status: 'Pending',
        shippingAddress: address,
      });
      setOrderSuccess(order.id);
    } catch (err: any) {
      alert(err.message);
    } finally {
      setOrdering(false);
    }
  };

  return (
    <div className={styles.overlay} onClick={e => e.target === e.currentTarget && onClose()}>
      <div className={styles.drawer}>
        {/* Header */}
        <div className={styles.header}>
          <span className={styles.title}>
            Кошик
            {cart && <span className={styles.count}> · {cart.itemsCount}</span>}
          </span>
          <div style={{ display: 'flex', gap: 8 }}>
            {cart && cart.itemsCount > 0 && (
              <button className={styles.clearBtn} onClick={() => clearCart()}>
                Очистити
              </button>
            )}
            <button className={styles.closeBtn} onClick={onClose}>✕</button>
          </div>
        </div>

        {/* Success state */}
        {orderSuccess ? (
          <div className={styles.success}>
            <div className={styles.successIcon}>✓</div>
            <div className={styles.successTitle}>Замовлення оформлено!</div>
            <div className={styles.successId}>#{`ORD-${String(orderSuccess).padStart(5, '0')}`}</div>
            <p className={styles.successText}>
              Статус: <strong>Pending</strong> — ми зв'яжемося з вами для підтвердження.
            </p>
            <button className={styles.closeOrderBtn} onClick={onClose}>
              Закрити
            </button>
          </div>
        ) : !cart || cart.itemsCount === 0 ? (
          <div className={styles.empty}>
            <div className={styles.emptyIcon}>⊡</div>
            <p>Кошик порожній</p>
          </div>
        ) : (
          <>
            {/* Items */}
            <div className={styles.items}>
              {cart.items?.map(item => (
                <div key={item.id} className={styles.item}>
                  <div className={styles.itemImg}>⚙</div>
                  <div className={styles.itemInfo}>
                    <div className={styles.itemName}>{item.productName}</div>
                    <div className={styles.itemPrice}>
                      ₴ {item.price?.toLocaleString('uk-UA')} / шт
                    </div>
                  </div>
                  <div className={styles.itemQty}>
                    <button
                      className={styles.qBtn}
                      onClick={() => updateItem(item.id, item.quantity - 1)}
                      disabled={loading}
                    >−</button>
                    <span className={styles.qNum}>{item.quantity}</span>
                    <button
                      className={styles.qBtn}
                      onClick={() => updateItem(item.id, item.quantity + 1)}
                      disabled={loading}
                    >+</button>
                  </div>
                  <div className={styles.itemSubtotal}>
                    ₴ {item.subtotal.toLocaleString('uk-UA')}
                  </div>
                  <button
                    className={styles.removeBtn}
                    onClick={() => removeItem(item.id)}
                    disabled={loading}
                  >✕</button>
                </div>
              ))}
            </div>

            {/* Summary */}
            <div className={styles.summary}>
              <div className={styles.summaryRow}>
                <span>Товарів</span>
                <span>{cart.itemsCount} шт</span>
              </div>
              <div className={styles.totalRow}>
                <span>Разом</span>
                <span className={styles.totalAmount}>
                  ₴ {cart.totalAmount.toLocaleString('uk-UA')}
                </span>
              </div>

              <input
                className={styles.addressInput}
                placeholder="Адреса доставки (місто, вулиця, будинок)"
                value={address}
                onChange={e => setAddress(e.target.value)}
              />

              <button
                className={styles.checkoutBtn}
                onClick={handleCheckout}
                disabled={ordering || !address.trim()}
              >
                {ordering ? 'Оформлення...' : 'Оформити замовлення'}
              </button>
            </div>
          </>
        )}
      </div>
    </div>
  );
}
