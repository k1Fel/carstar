// ============================================
// CARSTAR — ProductCard
// Used in catalog grid
// ============================================
import type { ProductDto } from '../types';
import { useCart } from '../hooks/useCart';
import { useAuth } from '../hooks/useAuth';
import { useFavorites } from '../hooks/useFavorites';
import styles from './ProductCard.module.css';

interface ProductCardProps {
  product: ProductDto;
  onAuthRequired: () => void;
  onClick: (product: ProductDto) => void;
}

export default function ProductCard({ product, onAuthRequired, onClick }: ProductCardProps) {
  const { addItem } = useCart();
  const { isAuthenticated } = useAuth();
  const { toggleFavorite, isFavorite } = useFavorites();
  const stockStatus =
    product.stock === 0 ? 'out' :
    product.stock <= 10  ? 'low' : 'ok';
  const favorite = isFavorite(product.id);

  const handleAddToCart = async (e: React.MouseEvent) => {
    e.stopPropagation();
    if (!isAuthenticated) { onAuthRequired(); return; }
    if (product.stock === 0) return;
    try {
      await addItem(product.id, 1);
    } catch (err: any) {
      alert(err.message);
    }
  };

  const handleToggleFavorite = (e: React.MouseEvent) => {
    e.stopPropagation();
    toggleFavorite(product.id);
  };

  return (
    <div className={styles.card} onClick={() => onClick(product)}>
      {/* Image */}
      <div className={styles.imgWrap}>
        {/* Favorite button — top right, centered */}
        <button
          className={`${styles.actionIconBtn} ${favorite ? styles.favActive : ''}`}
          onClick={handleToggleFavorite}
          title={favorite ? 'Видалити з улюблених' : 'Додати в улюблені'}
        >
          <HeartIcon filled={favorite} />
        </button>

        {product.imageUrl ? (
          <img
            src={product.imageUrl}
            alt={product.name}
            className={styles.productImg}
            onError={e => {
              (e.currentTarget as HTMLImageElement).style.display = 'none';
              const fallback = e.currentTarget.parentElement?.querySelector('[data-fallback]') as HTMLElement;
              if (fallback) fallback.style.display = 'flex';
            }}
          />
        ) : (
          <span data-fallback className={styles.fallbackIcon}>
            <GearIcon />
          </span>
        )}
        <span
          data-fallback
          className={styles.fallbackIcon}
          style={{ display: 'none' }}
        >
          <GearIcon />
        </span>
        {stockStatus === 'out' && (
          <span className={`${styles.tag} ${styles.tagOut}`}>Немає</span>
        )}
      </div>

      <div className={styles.body}>
        <div className={styles.cat}>{product.categoryName}</div>
        <div className={styles.name}>{product.name}</div>
        <div className={styles.desc}>{product.description}</div>

        <div className={styles.foot}>
          <div>
            <div className={`${styles.stock} ${styles[`stock_${stockStatus}`]}`}>
              ● {stockStatus === 'ok'  ? `В наявності · ${product.stock} шт` :
                 stockStatus === 'low' ? `Залишок · ${product.stock} шт` :
                 'Немає · очікується'}
            </div>
            <div className={styles.price}>
              ₴ {product.price.toLocaleString('uk-UA')}
            </div>
          </div>

          {/* Add to cart button — centered */}
          <button
            className={styles.addBtn}
            onClick={handleAddToCart}
            disabled={product.stock === 0}
            title="Додати в кошик"
          >
            <CartPlusIcon />
          </button>
        </div>
      </div>
    </div>
  );
}

const GearIcon = () => (
  <svg width="36" height="36" viewBox="0 0 24 24" fill="none" opacity=".12">
    <path d="M12 15a3 3 0 100-6 3 3 0 000 6z" stroke="currentColor" strokeWidth="1.5"/>
    <path d="M19.4 15a1.65 1.65 0 00.33 1.82l.06.06a2 2 0 010 2.83 2 2 0 01-2.83 0l-.06-.06a1.65 1.65 0 00-1.82-.33 1.65 1.65 0 00-1 1.51V21a2 2 0 01-4 0v-.09A1.65 1.65 0 009 19.4a1.65 1.65 0 00-1.82.33l-.06.06a2 2 0 01-2.83-2.83l.06-.06A1.65 1.65 0 004.68 15a1.65 1.65 0 00-1.51-1H3a2 2 0 010-4h.09A1.65 1.65 0 004.6 9a1.65 1.65 0 00-.33-1.82l-.06-.06a2 2 0 012.83-2.83l.06.06A1.65 1.65 0 009 4.68a1.65 1.65 0 001-1.51V3a2 2 0 014 0v.09a1.65 1.65 0 001 1.51 1.65 1.65 0 001.82-.33l.06-.06a2 2 0 012.83 2.83l-.06.06A1.65 1.65 0 0019.4 9a1.65 1.65 0 001.51 1H21a2 2 0 010 4h-.09a1.65 1.65 0 00-1.51 1z" stroke="currentColor" strokeWidth="1.5"/>
  </svg>
);

const HeartIcon = ({ filled }: { filled: boolean }) => (
  <svg width="14" height="14" viewBox="0 0 16 16" fill={filled ? 'currentColor' : 'none'}>
    <path
      d="M8 13s-6-3.5-6-7.5C2 3.57 3.57 2 5.5 2c1.054 0 2 .5 2.5 1.5C8.5 2.5 9.446 2 10.5 2 12.43 2 14 3.57 14 5.5 14 9.5 8 13 8 13z"
      stroke="currentColor"
      strokeWidth="1.2"
    />
  </svg>
);

const CartPlusIcon = () => (
  <svg width="15" height="15" viewBox="0 0 16 16" fill="none">
    <path d="M2 2h1.5l2 7h6l1.5-5H5" stroke="currentColor" strokeWidth="1.2" strokeLinecap="round" strokeLinejoin="round"/>
    <circle cx="7" cy="13" r="1" fill="currentColor"/>
    <circle cx="11" cy="13" r="1" fill="currentColor"/>
    <path d="M13 3v4M11 5h4" stroke="currentColor" strokeWidth="1.2" strokeLinecap="round"/>
  </svg>
);
