// ============================================
// CARSTAR — ProductPage
// GET /api/products/{id}
// ============================================
import { useState, useEffect } from 'react';
import type { ProductDto } from '../types';
import { productApi } from '../api';
import { useCart } from '../hooks/useCart';
import { useAuth } from '../hooks/useAuth';
import styles from './ProductPage.module.css';

interface ProductPageProps {
  productId: number;
  onBack: () => void;
  onAuthRequired: () => void;
}

export default function ProductPage({ productId, onBack, onAuthRequired }: ProductPageProps) {
  const [product, setProduct] = useState<ProductDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [qty, setQty]         = useState(1);
  const [added, setAdded]     = useState(false);
  const { addItem } = useCart();
  const { isAuthenticated } = useAuth();

  useEffect(() => {
    setLoading(true);
    productApi.getById(productId)
      .then(setProduct)
      .catch(() => setProduct(null))
      .finally(() => setLoading(false));
  }, [productId]);

  const handleAdd = async () => {
    if (!isAuthenticated) { onAuthRequired(); return; }
    if (!product || product.stock === 0) return;
    try {
      await addItem(product.id, qty);
      setAdded(true);
      setTimeout(() => setAdded(false), 2000);
    } catch (err: any) {
      alert(err.message);
    }
  };

  const stockStatus =
    !product        ? 'out' :
    product.stock === 0 ? 'out' :
    product.stock <= 10  ? 'low' : 'ok';

  if (loading) return (
    <div className={styles.loadingWrap}>
      <div className={styles.loadingText}>Завантаження...</div>
    </div>
  );

  if (!product) return (
    <div className={styles.loadingWrap}>
      <div className={styles.loadingText}>Товар не знайдено</div>
      <button className={styles.backBtn} onClick={onBack}>← Назад</button>
    </div>
  );

  return (
    <div className={styles.page}>
      {/* Breadcrumb */}
      <div className={styles.breadcrumb}>
        <button className={styles.breadLink} onClick={onBack}>Каталог</button>
        <span className={styles.breadSep}>›</span>
        <span className={styles.breadCurrent}>
          {product.categories.find(c => c.type === 'part_type')?.name ?? ''}
        </span>
        <span className={styles.breadSep}>›</span>
        <span className={styles.breadCurrent}>{product.name}</span>
      </div>

      <div className={styles.layout}>
        {/* Gallery */}
        <div className={styles.gallery}>
          <div className={styles.mainImg}>
            {product?.imageUrl ? (
              <img
                src={product.imageUrl}
                alt={product.name}
                className={styles.mainImgPhoto}
                onError={e => { (e.currentTarget as HTMLImageElement).style.display = 'none'; }}
              />
            ) : (
              <GearSvg />
            )}
            {stockStatus === 'out' && (
              <span className={styles.outBadge}>Немає в наявності</span>
            )}
          </div>
          <div className={styles.thumbs}>
            {[0,1,2,3].map(i => (
              <div key={i} className={`${styles.thumb} ${i === 0 ? styles.thumbActive : ''}`} />
            ))}
          </div>
        </div>

        {/* Info */}
        <div className={styles.info}>
          <div className={styles.category}>
            {product.categories.map(c => c.name).join(' · ')}
          </div>
          <h1 className={styles.title}>{product.name}</h1>
          <div className={styles.sku}>
            SKU: {`CST-${String(product.id).padStart(6, '0')}`}
          </div>

          {/* Price */}
          <div className={styles.priceRow}>
            <span className={styles.price}>
              ₴ {product.price.toLocaleString('uk-UA')}
            </span>
          </div>

          {/* Stock */}
          <div className={styles.stockRow}>
            <span className={`${styles.stockDot} ${styles[`dot_${stockStatus}`]}`} />
            <span className={styles.stockText}>
              {stockStatus === 'ok'  ? 'В наявності' :
               stockStatus === 'low' ? 'Закінчується' :
               'Немає в наявності'}
            </span>
            {product.stock > 0 && (
              <span className={styles.stockQty}>· {product.stock} шт</span>
            )}
          </div>

          {/* Description */}
          {product.description && (
            <div className={styles.desc}>{product.description}</div>
          )}

          <div className={styles.divider} />

          {/* Qty + Add */}
          <div className={styles.actionRow}>
            <div className={styles.qtyCtrl}>
              <button
                className={styles.qBtn}
                onClick={() => setQty(q => Math.max(1, q - 1))}
                disabled={product.stock === 0}
              >−</button>
              <span className={styles.qNum}>{qty}</span>
              <button
                className={styles.qBtn}
                onClick={() => setQty(q => Math.min(product.stock, q + 1))}
                disabled={product.stock === 0}
              >+</button>
            </div>

            <button
              className={`${styles.addBtn} ${added ? styles.addBtnSuccess : ''}`}
              onClick={handleAdd}
              disabled={product.stock === 0}
            >
              {added ? '✓ Додано' : product.stock === 0 ? 'Немає в наявності' : 'Додати в кошик'}
            </button>
          </div>

          {/* Meta */}
          <div className={styles.meta}>
            <div className={styles.metaRow}>
              <span className={styles.metaLabel}>Категорія</span>
              <span className={styles.metaValue}>
                {product.categories.map(c => c.name).join(', ')}
              </span>
            </div>
            <div className={styles.metaRow}>
              <span className={styles.metaLabel}>ID товару</span>
              <span className={styles.metaValue}>#{product.id}</span>
            </div>
            <div className={styles.metaRow}>
              <span className={styles.metaLabel}>Залишок</span>
              <span className={styles.metaValue}>{product.stock} шт</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

const GearSvg = () => (
  <svg width="80" height="80" viewBox="0 0 24 24" fill="none" opacity=".08" color="currentColor">
    <path d="M12 15a3 3 0 100-6 3 3 0 000 6z" stroke="currentColor" strokeWidth="1.2"/>
    <path d="M19.4 15a1.65 1.65 0 00.33 1.82l.06.06a2 2 0 010 2.83 2 2 0 01-2.83 0l-.06-.06a1.65 1.65 0 00-1.82-.33 1.65 1.65 0 00-1 1.51V21a2 2 0 01-4 0v-.09A1.65 1.65 0 009 19.4a1.65 1.65 0 00-1.82.33l-.06.06a2 2 0 01-2.83-2.83l.06-.06A1.65 1.65 0 004.68 15a1.65 1.65 0 00-1.51-1H3a2 2 0 010-4h.09A1.65 1.65 0 004.6 9a1.65 1.65 0 00-.33-1.82l-.06-.06a2 2 0 012.83-2.83l.06.06A1.65 1.65 0 009 4.68a1.65 1.65 0 001-1.51V3a2 2 0 014 0v.09a1.65 1.65 0 001 1.51 1.65 1.65 0 001.82-.33l.06-.06a2 2 0 012.83 2.83l-.06.06A1.65 1.65 0 0019.4 9a1.65 1.65 0 001.51 1H21a2 2 0 010 4h-.09a1.65 1.65 0 00-1.51 1z" stroke="currentColor" strokeWidth="1.2"/>
  </svg>
);
