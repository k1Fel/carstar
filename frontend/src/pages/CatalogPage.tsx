// ============================================
// CARSTAR — CatalogPage
// GET /api/categories
// GET /api/products/filter
// ============================================
import { useState, useEffect, useCallback } from 'react';
import type { ProductDto, CategoryDtoResponse } from '../types';
import { productApi, categoryApi } from '../api';
import ProductCard from '../components/ProductCard';
import styles from './CatalogPage.module.css';

interface CatalogPageProps {
  onAuthRequired: () => void;
  onProductClick: (product: ProductDto) => void;
}

export default function CatalogPage({ onAuthRequired, onProductClick }: CatalogPageProps) {
  const [products, setProducts]     = useState<ProductDto[]>([]);
  const [categories, setCategories] = useState<CategoryDtoResponse[]>([]);
  const [loading, setLoading]       = useState(true);
  const [error, setError]           = useState('');

  // Filters
  const [categoryId, setCategoryId] = useState<number | undefined>();
  const [search, setSearch]         = useState('');
  const [minPrice, setMinPrice]     = useState('');
  const [maxPrice, setMaxPrice]     = useState('');
  const [inStock, setInStock]       = useState(false);
  const [sortBy, setSortBy]         = useState('');

  // Load categories once
  useEffect(() => {
    categoryApi.getAll()
      .then(setCategories)
      .catch(() => {});
  }, []);

  // Load products with filters
  const fetchProducts = useCallback(async () => {
    setLoading(true);
    setError('');
    try {
      const data = await productApi.filter({
        categoryId,
        search:   search || undefined,
        minPrice: minPrice ? Number(minPrice) : undefined,
        maxPrice: maxPrice ? Number(maxPrice) : undefined,
        inStock:  inStock || undefined,
      });
      // Client-side sort
      let sorted = [...data];
      if (sortBy === 'price_asc')  sorted.sort((a, b) => a.price - b.price);
      if (sortBy === 'price_desc') sorted.sort((a, b) => b.price - a.price);
      setProducts(sorted);
    } catch (err: any) {
      if (err.message?.includes('не знайдено')) {
        setProducts([]);
      } else {
        setError(err.message);
      }
    } finally {
      setLoading(false);
    }
  }, [categoryId, search, minPrice, maxPrice, inStock, sortBy]);

  useEffect(() => {
    const t = setTimeout(fetchProducts, 300);
    return () => clearTimeout(t);
  }, [fetchProducts]);

  return (
    <div className={styles.page}>
      {/* Categories */}
      <div className={styles.catRow}>
        <button
          className={`${styles.catChip} ${!categoryId ? styles.catActive : ''}`}
          onClick={() => setCategoryId(undefined)}
        >
          Всі
        </button>
        {categories.map(cat => (
          <button
            key={cat.id}
            className={`${styles.catChip} ${categoryId === cat.id ? styles.catActive : ''}`}
            onClick={() => setCategoryId(cat.id === categoryId ? undefined : cat.id)}
          >
            {cat.name}
            <span className={styles.catCount}> · {cat.productCount}</span>
          </button>
        ))}
      </div>

      {/* Filter bar */}
      <div className={styles.filterBar}>
        <input
          className={styles.searchInput}
          placeholder="Пошук за назвою або описом..."
          value={search}
          onChange={e => setSearch(e.target.value)}
        />
        <input
          className={styles.priceInput}
          type="number"
          placeholder="Ціна від"
          value={minPrice}
          onChange={e => setMinPrice(e.target.value)}
        />
        <input
          className={styles.priceInput}
          type="number"
          placeholder="Ціна до"
          value={maxPrice}
          onChange={e => setMaxPrice(e.target.value)}
        />
        <select
          className={styles.sortSelect}
          value={sortBy}
          onChange={e => setSortBy(e.target.value)}
        >
          <option value="">Сортування</option>
          <option value="price_asc">Дешевше</option>
          <option value="price_desc">Дорожче</option>
        </select>
        <div className={styles.sep} />
        <label className={styles.inStockLabel}>
          <div
            className={`${styles.toggle} ${inStock ? styles.toggleOn : ''}`}
            onClick={() => setInStock(p => !p)}
          />
          В наявності
        </label>
      </div>

      {/* Results info */}
      <div className={styles.resultsInfo}>
        {loading ? (
          <span className={styles.loading}>Завантаження...</span>
        ) : error ? (
          <span className={styles.errText}>{error}</span>
        ) : (
          <span>{products.length} товарів</span>
        )}
      </div>

      {/* Grid */}
      {!loading && !error && (
        products.length === 0 ? (
          <div className={styles.noResults}>
            <p>Товарів не знайдено</p>
            <button
              className={styles.resetBtn}
              onClick={() => {
                setCategoryId(undefined);
                setSearch('');
                setMinPrice('');
                setMaxPrice('');
                setInStock(false);
              }}
            >
              Скинути фільтри
            </button>
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
        )
      )}
    </div>
  );
}
