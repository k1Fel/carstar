// ============================================
// CARSTAR — CatalogPage (Multiple Categories)
// ============================================
import { useState, useEffect, useCallback } from 'react';
import type { ProductDto, CategoryDtoResponse } from '../types';
import { productApi, categoryApi } from '../api';
import ProductCard from '../components/ProductCard';
import BackButton from '../components/BackButton';
import styles from './CatalogPage.module.css';

interface CatalogPageProps {
    onAuthRequired: () => void;
    onProductClick: (product: ProductDto) => void;
    onBack: () => void;
}

// ===== Рекурсивне дерево категорій =====
function CategoryTree({
  categories,
  selectedIds,
  onSelect,
  depth = 0,
}: {
  categories: CategoryDtoResponse[];
  selectedIds: number[];
  onSelect: (id: number) => void;
  depth?: number;
}) {
  const [expanded, setExpanded] = useState<Set<number>>(new Set());

  const toggle = (id: number) => {
    setExpanded(prev => {
      const next = new Set(prev);
      next.has(id) ? next.delete(id) : next.add(id);
      return next;
    });
  };

  return (
    <>
      {categories.map(cat => {
        const hasChildren = cat.children && cat.children.length > 0;
        const isExpanded = expanded.has(cat.id);
        const isActive = selectedIds.includes(cat.id);

        return (
          <div key={cat.id}>
            <div
              className={`${styles.catItem} ${isActive ? styles.catItemActive : ''}`}
              style={{ paddingLeft: 8 + depth * 14 }}
            >
              {/* Expand toggle */}
              {hasChildren ? (
                <button
                  className={styles.catExpand}
                  onClick={() => toggle(cat.id)}
                >
                  {isExpanded ? '▾' : '▸'}
                </button>
              ) : (
                <span className={styles.catExpandPlaceholder} />
              )}

              {/* Category name */}
              <button
                className={styles.catName}
                onClick={() => onSelect(cat.id)}
              >
                {cat.name}
              </button>

              <span className={styles.catCount}>{cat.productCount}</span>
            </div>

            {/* Children */}
            {hasChildren && isExpanded && (
              <CategoryTree
                categories={cat.children!}
                selectedIds={selectedIds}
                onSelect={onSelect}
                depth={depth + 1}
              />
            )}
          </div>
        );
      })}
    </>
  );
}

// ===== Головний компонент =====
export default function CatalogPage({ onAuthRequired, onProductClick, onBack }: CatalogPageProps) {
  const [products, setProducts]     = useState<ProductDto[]>([]);
  const [categories, setCategories] = useState<CategoryDtoResponse[]>([]);
  const [loading, setLoading]       = useState(true);
  const [error, setError]           = useState('');
  const [sidebarOpen, setSidebarOpen] = useState(true);

  // Filters
  const [categoryIds, setCategoryIds] = useState<number[]>([]);
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

  // Handle category selection (toggle)
  const handleCategorySelect = (id: number) => {
    setCategoryIds(prev => 
      prev.includes(id) 
        ? prev.filter(x => x !== id)
        : [...prev, id]
    );
  };

  // Load products
  const fetchProducts = useCallback(async () => {
    setLoading(true);
    setError('');
    try {
      const data = await productApi.filter({
        categoryIds: categoryIds.length > 0 ? categoryIds : undefined,
        search:   search || undefined,
        minPrice: minPrice ? Number(minPrice) : undefined,
        maxPrice: maxPrice ? Number(maxPrice) : undefined,
        inStock:  inStock || undefined,
      });
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
  }, [categoryIds, search, minPrice, maxPrice, inStock, sortBy]);

  useEffect(() => {
    const t = setTimeout(fetchProducts, 300);
    return () => clearTimeout(t);
  }, [fetchProducts]);

  const hasActiveFilters = !!(categoryIds.length > 0 || search || minPrice || maxPrice || inStock);

  const resetFilters = () => {
    setCategoryIds([]);
    setSearch('');
    setMinPrice('');
    setMaxPrice('');
    setInStock(false);
  };

  // Знайти назву вибраної категорії
  const findCategoryName = (cats: CategoryDtoResponse[], id: number): string | undefined => {
    for (const cat of cats) {
      if (cat.id === id) return cat.name;
      if (cat.children) {
        const found = findCategoryName(cat.children, id);
        if (found) return found;
      }
    }
    return undefined;
  };

  return (
    <div className={styles.page}>
      <div className={styles.layout}>

        {/* ===== SIDEBAR ===== */}
        <aside className={`${styles.sidebar} ${sidebarOpen ? styles.sidebarOpen : styles.sidebarClosed}`}>
          <div className={styles.sidebarHeader}>
            {sidebarOpen && <span className={styles.sidebarTitle}>Фільтри</span>}
            <button
              className={styles.sidebarToggle}
              onClick={() => setSidebarOpen(p => !p)}
            >
              {sidebarOpen ? '‹' : '›'}
            </button>
          </div>

          {sidebarOpen && (
            <>
              {/* Search */}
              <div className={styles.filterSection}>
                <div className={styles.filterLabel}>Пошук</div>
                <div className={styles.searchWrap}>
                  <svg className={styles.searchIcon} width="13" height="13" viewBox="0 0 16 16" fill="none">
                    <circle cx="7" cy="7" r="4.5" stroke="currentColor" strokeWidth="1.3"/>
                    <path d="M10.5 10.5L13.5 13.5" stroke="currentColor" strokeWidth="1.3" strokeLinecap="round"/>
                  </svg>
                  <input
                    className={styles.searchInput}
                    placeholder="Назва або опис..."
                    value={search}
                    onChange={e => setSearch(e.target.value)}
                  />
                  {search && (
                    <button className={styles.clearSearch} onClick={() => setSearch('')}>✕</button>
                  )}
                </div>
              </div>

              {/* Categories tree */}
              <div className={styles.filterSection}>
                <div className={styles.filterLabel}>
                  Категорія
                  {categoryIds.length > 0 && (
                    <span className={styles.selectedCount}>({categoryIds.length})</span>
                  )}
                </div>
                <div className={styles.catList}>
                  {/* Всі */}
                  <div className={`${styles.catItem} ${categoryIds.length === 0 ? styles.catItemActive : ''}`}>
                    <span className={styles.catExpandPlaceholder} />
                    <button
                      className={styles.catName}
                      onClick={() => setCategoryIds([])}
                    >
                      Всі категорії
                    </button>
                    <span className={styles.catCount}>
                      {products.length > 0 || !loading ? products.length : '—'}
                    </span>
                  </div>

                  <CategoryTree
                    categories={categories}
                    selectedIds={categoryIds}
                    onSelect={handleCategorySelect}
                  />
                </div>
              </div>

              {/* Price */}
              <div className={styles.filterSection}>
                <div className={styles.filterLabel}>Ціна (₴)</div>
                <div className={styles.priceRow}>
                  <input
                    className={styles.priceInput}
                    type="number"
                    placeholder="Від"
                    value={minPrice}
                    onChange={e => setMinPrice(e.target.value)}
                  />
                  <span className={styles.priceSep}>—</span>
                  <input
                    className={styles.priceInput}
                    type="number"
                    placeholder="До"
                    value={maxPrice}
                    onChange={e => setMaxPrice(e.target.value)}
                  />
                </div>
              </div>

              {/* In stock toggle */}
              <div className={styles.filterSection}>
                <label className={styles.toggleLabel}>
                  <div
                    className={`${styles.toggle} ${inStock ? styles.toggleOn : ''}`}
                    onClick={() => setInStock(p => !p)}
                  />
                  <span>Тільки в наявності</span>
                </label>
              </div>

              {/* Reset */}
              {hasActiveFilters && (
                <div className={styles.filterSection}>
                  <button className={styles.resetBtn} onClick={resetFilters}>
                    Скинути фільтри
                  </button>
                </div>
              )}
            </>
          )}
        </aside>

        {/* ===== CONTENT ===== */}
        <div className={styles.content}>

          {/* Top bar */}
          <div className={styles.topBar}>
            <div className={styles.topBarLeft}>
                <BackButton onClick={onBack} />              
                <div className={styles.resultsInfo}>
                {loading ? (
                  <span className={styles.loading}>Завантаження...</span>
                ) : error ? (
                  <span className={styles.errText}>{error}</span>
                ) : (
                  <span>{products.length} товарів</span>
                )}
              </div>
            </div>

            <div className={styles.topBarRight}>
              {/* Active filter chips */}
              {categoryIds.map(id => {
                const name = findCategoryName(categories, id);
                return name ? (
                  <span key={id} className={styles.chip}>
                    {name}
                    <button onClick={() => handleCategorySelect(id)}>✕</button>
                  </span>
                ) : null;
              })}
              {search && (
                <span className={styles.chip}>
                  «{search}»
                  <button onClick={() => setSearch('')}>✕</button>
                </span>
              )}
              {(minPrice || maxPrice) && (
                <span className={styles.chip}>
                  ₴ {minPrice || '0'} — {maxPrice || '∞'}
                  <button onClick={() => { setMinPrice(''); setMaxPrice(''); }}>✕</button>
                </span>
              )}
              {inStock && (
                <span className={styles.chip}>
                  В наявності
                  <button onClick={() => setInStock(false)}>✕</button>
                </span>
              )}

              <select
                className={styles.sortSelect}
                value={sortBy}
                onChange={e => setSortBy(e.target.value)}
              >
                <option value="">Сортування</option>
                <option value="price_asc">Дешевше</option>
                <option value="price_desc">Дорожче</option>
              </select>
            </div>
          </div>

          {/* Grid */}
          {!loading && !error && (
            products.length === 0 ? (
              <div className={styles.noResults}>
                <p>Товарів не знайдено</p>
                {hasActiveFilters && (
                  <button className={styles.noResultsBtn} onClick={resetFilters}>
                    Скинути фільтри
                  </button>
                )}
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
      </div>
    </div>
  );
}