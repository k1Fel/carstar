// ============================================
// CARSTAR — FavoritesPage
// ============================================
import { useEffect, useState } from 'react';
import { productApi } from '../api';
import ProductCard from '../components/ProductCard';
import { useFavorites } from '../hooks/useFavorites';
import type { ProductDto } from '../types';
import styles from './FavoritesPage.module.css';
import BackButton from '../components/BackButton';

interface Props {
  onAuthRequired: () => void;
  onProductClick: (product: ProductDto) => void;
  onBack: () => void;
}

export default function FavoritesPage({ onAuthRequired, onProductClick , onBack}: Props) {
  const { favorites } = useFavorites();
  const [products, setProducts] = useState<ProductDto[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    setLoading(true);
    productApi.filter({})
      .then(data => {
        setProducts(data.filter(x => favorites.includes(x.id)));
      })
      .catch(() => setProducts([]))
      .finally(() => setLoading(false));
  }, [favorites]);

  return (
    <div className={styles.page}>
      {/* Header */}
      <div className={styles.header}>
        <div className={styles.titleRow}>
          <BackButton onClick={onBack} />
          <span className={styles.titleDot} />
          <h1 className={styles.title}>Уподобані</h1>
          {!loading && (
            <span className={styles.count}>{products.length} товарів</span>
          )}
        </div>
      </div>

      {/* Content */}
      {loading ? (
        <div className={styles.empty}>
          <div className={styles.emptyText}>Завантаження...</div>
        </div>
      ) : products.length === 0 ? (
        <div className={styles.empty}>
          <div className={styles.emptyIcon}>
            <HeartEmptyIcon />
          </div>
          <div className={styles.emptyTitle}>Список порожній</div>
          <div className={styles.emptyText}>
            Додайте товари в улюблені, натиснувши ♥ на картці товару
          </div>
        </div>
      ) : (
        <div className={styles.grid}>
          {products.map(p => (
            <ProductCard
              key={p.id}
              product={p}
              onAuthRequired={onAuthRequired}
              onClick={onProductClick}
            />
          ))}
        </div>
      )}
    </div>
  );
}

const HeartEmptyIcon = () => (
  <svg width="48" height="48" viewBox="0 0 24 24" fill="none" opacity=".15">
    <path
      d="M12 21s-9-5.25-9-11.25C3 6.355 5.355 4 7.5 4c1.582 0 3 .75 3.75 2.25C12 4.75 13.418 4 15.5 4 17.645 4 20 6.355 20 9.75 20 15.75 12 21 12 21z"
      stroke="currentColor"
      strokeWidth="1.5"
    />
  </svg>
);
