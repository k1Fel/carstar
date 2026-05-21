import { useState, useEffect } from 'react';
import { adminApi, categoryApi, productApi } from '../api';
import { useAuth } from '../hooks/useAuth';
import type {
  ResponseOrderDto, ProductDto, CategoryDtoResponse, OrderStatus
} from '../types';
import BackButton from '../components/BackButton';
import styles from './AdminPage.module.css';

interface AdminPageProps {
  onBack: () => void;
}

type Section = 'orders' | 'products' | 'categories';

const STATUS_LABELS: Record<string, string> = {
  Pending: 'Очікує', Processing: 'Обробка',
  Shipped: 'Відправлено', Delivered: 'Доставлено', Cancelled: 'Скасовано',
};

const STATUS_NEXT: Record<string, string[]> = {
  Pending: ['Processing', 'Cancelled'],
  Processing: ['Shipped', 'Cancelled'],
  Shipped: ['Delivered'],
  Delivered: [], Cancelled: [],
};

export default function AdminPage({ onBack }: AdminPageProps) {
  const { account } = useAuth();
  const [section, setSection] = useState<Section>('orders');

  if (account?.role !== 'admin') {
    return (
      <div className={styles.page}>
        <div className={styles.denied}>
          <div className={styles.deniedTitle}>Доступ заборонено</div>
          <BackButton onClick={onBack} />
        </div>
      </div>
    );
  }

  return (
    <div className={styles.page}>
      <div className={styles.layout}>
        {/* Sidebar */}
        <aside className={styles.sidebar}>
          <div className={styles.sidebarHeader}>
            <BackButton onClick={onBack} />
            <div className={styles.adminBadge}>ADMIN</div>
          </div>
          <nav className={styles.nav}>
            {(['orders', 'products', 'categories'] as Section[]).map(s => (
              <button
                key={s}
                className={`${styles.navItem} ${section === s ? styles.navActive : ''}`}
                onClick={() => setSection(s)}
              >
                {s === 'orders' ? 'Замовлення' :
                 s === 'products' ? 'Товари' : 'Категорії'}
              </button>
            ))}
          </nav>
        </aside>

        {/* Main */}
        <main className={styles.main}>
          {section === 'orders'     && <OrdersSection />}
          {section === 'products'   && <ProductsSection />}
          {section === 'categories' && <CategoriesSection />}
        </main>
      </div>
    </div>
  );
}

// ===== ORDERS =====
function OrdersSection() {
  const [orders, setOrders] = useState<ResponseOrderDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [cancelReason, setCancelReason] = useState('');
  const [cancelingId, setCancelingId] = useState<number | null>(null);
  const [filter, setFilter] = useState<string>('all');

  useEffect(() => {
    adminApi.getAllOrders()
      .then(setOrders)
      .catch(() => {})
      .finally(() => setLoading(false));
  }, []);

  const updateStatus = async (id: number, status: string, reason?: string) => {
    try {
      const updated = await adminApi.updateOrderStatus(id, { status, cancellationReason: reason });
      setOrders(prev => prev.map(o => o.id === id ? updated : o));
      setCancelingId(null);
      setCancelReason('');
    } catch (err: any) {
      alert(err.message);
    }
  };

  const filtered = filter === 'all' ? orders : orders.filter(o => o.status === filter);

  return (
    <div>
      <div className={styles.sectionHeader}>
        <h2 className={styles.sectionTitle}>Замовлення</h2>
        <select
          className={styles.filterSelect}
          value={filter}
          onChange={e => setFilter(e.target.value)}
        >
          <option value="all">Всі</option>
          {Object.keys(STATUS_LABELS).map(s => (
            <option key={s} value={s}>{STATUS_LABELS[s]}</option>
          ))}
        </select>
      </div>

      {loading ? (
        <div className={styles.loading}>Завантаження...</div>
      ) : (
        <div className={styles.ordersList}>
          {filtered.map(order => (
            <div key={order.id} className={styles.orderCard}>
              <div className={styles.orderTop}>
                <div className={styles.orderMeta}>
                  <span className={styles.orderId}>
                    #{`ORD-${String(order.id).padStart(5, '0')}`}
                  </span>
                  <span className={styles.orderUser}>
                    {order.userName || `User #${order.accountId}`}
                  </span>
                  <span className={styles.orderDate}>
                    {new Date(order.createdAt).toLocaleDateString('uk-UA')}
                  </span>
                </div>
                <div className={styles.orderRight}>
                  <span className={`${styles.statusBadge} status-${order.status.toLowerCase()}`}>
                    {STATUS_LABELS[order.status]}
                  </span>
                  <span className={styles.orderAmount}>
                    ₴ {order.totalAmount.toLocaleString('uk-UA')}
                  </span>
                </div>
              </div>

              <div className={styles.orderAddress}>📍 {order.shippingAddress}</div>

              <div className={styles.orderItems}>
                {order.orderItems.map(item => (
                  <span key={item.id} className={styles.orderItem}>
                    #{item.productId} × {item.quantity} — ₴{item.price}
                  </span>
                ))}
              </div>

              {order.cancellationReason && (
                <div className={styles.cancelReason}>
                  Причина: {order.cancellationReason}
                </div>
              )}

              {/* Дії */}
              {STATUS_NEXT[order.status]?.length > 0 && (
                <div className={styles.orderActions}>
                  {STATUS_NEXT[order.status]
                    .filter(s => s !== 'Cancelled')
                    .map(nextStatus => (
                      <button
                        key={nextStatus}
                        className={styles.actionBtn}
                        onClick={() => updateStatus(order.id, nextStatus)}
                      >
                        → {STATUS_LABELS[nextStatus]}
                      </button>
                    ))}

                  {STATUS_NEXT[order.status].includes('Cancelled') && (
                    cancelingId === order.id ? (
                      <div className={styles.cancelForm}>
                        <input
                          className={styles.cancelInput}
                          placeholder="Причина відмови..."
                          value={cancelReason}
                          onChange={e => setCancelReason(e.target.value)}
                        />
                        <button
                          className={styles.cancelConfirmBtn}
                          onClick={() => updateStatus(order.id, 'Cancelled', cancelReason)}
                          disabled={!cancelReason.trim()}
                        >
                          Підтвердити
                        </button>
                        <button
                          className={styles.cancelAbortBtn}
                          onClick={() => { setCancelingId(null); setCancelReason(''); }}
                        >
                          Скасувати
                        </button>
                      </div>
                    ) : (
                      <button
                        className={styles.cancelBtn}
                        onClick={() => setCancelingId(order.id)}
                      >
                        Відхилити
                      </button>
                    )
                  )}
                </div>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

// ===== PRODUCTS =====
function ProductsSection() {
  const [products, setProducts] = useState<ProductDto[]>([]);
  const [categories, setCategories] = useState<CategoryDtoResponse[]>([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');
  const [showForm, setShowForm] = useState(false);
  const [editProduct, setEditProduct] = useState<ProductDto | null>(null);

  const [form, setForm] = useState({
    name: '', description: '', price: '', stock: '', imageUrl: '', categoryIds: [] as number[]
  });

  useEffect(() => {
    Promise.all([productApi.getAll(), categoryApi.getAll()])
      .then(([p, c]) => { setProducts(p as ProductDto[]); setCategories(c); })
      .finally(() => setLoading(false));
  }, []);

  const openCreate = () => {
    setEditProduct(null);
    setForm({ name: '', description: '', price: '', stock: '', imageUrl: '', categoryIds: [] });
    setShowForm(true);
  };

  const openEdit = (p: ProductDto) => {
    setEditProduct(p);
    setForm({
      name: p.name,
      description: p.description,
      price: String(p.price),
      stock: String(p.stock),
      imageUrl: p.imageUrl || '',
      categoryIds: p.categories.map(c => c.id)
    });
    setShowForm(true);
  };

  const handleSubmit = async () => {
    const dto = {
      name: form.name,
      description: form.description,
      price: Number(form.price),
      stock: Number(form.stock),
      imageUrl: form.imageUrl,
      categoryIds: form.categoryIds
    };

    try {
      if (editProduct) {
        const updated = await adminApi.updateProduct(editProduct.id, dto);
        setProducts(prev => prev.map(p => p.id === editProduct.id ? updated : p));
      } else {
        const created = await adminApi.createProduct(dto);
        setProducts(prev => [...prev, created]);
      }
      setShowForm(false);
    } catch (err: any) {
      alert(err.message);
    }
  };

  const handleDelete = async (id: number) => {
    if (!confirm('Видалити товар?')) return;
    try {
      await adminApi.deleteProduct(id);
      setProducts(prev => prev.filter(p => p.id !== id));
    } catch (err: any) {
      alert(err.message);
    }
  };

  const toggleCat = (id: number) => {
    setForm(prev => ({
      ...prev,
      categoryIds: prev.categoryIds.includes(id)
        ? prev.categoryIds.filter(x => x !== id)
        : [...prev.categoryIds, id]
    }));
  };

  // Flatten categories for display
  const flatCats = (cats: CategoryDtoResponse[]): CategoryDtoResponse[] =>
    cats.flatMap(c => [c, ...flatCats(c.children || [])]);

  const filtered = products.filter(p =>
    p.name.toLowerCase().includes(search.toLowerCase())
  );

  return (
    <div>
      <div className={styles.sectionHeader}>
        <h2 className={styles.sectionTitle}>Товари ({products.length})</h2>
        <div style={{ display: 'flex', gap: 8 }}>
          <input
            className={styles.searchInput}
            placeholder="Пошук..."
            value={search}
            onChange={e => setSearch(e.target.value)}
          />
          <button className={styles.addBtn} onClick={openCreate}>+ Додати</button>
        </div>
      </div>

      {/* Form */}
      {showForm && (
        <div className={styles.formCard}>
          <div className={styles.formTitle}>
            {editProduct ? `Редагувати: ${editProduct.name}` : 'Новий товар'}
          </div>
          <div className={styles.formGrid}>
            <div className={styles.formField}>
              <label>Назва</label>
              <input value={form.name} onChange={e => setForm(p => ({ ...p, name: e.target.value }))} />
            </div>
            <div className={styles.formField}>
              <label>Ціна</label>
              <input type="number" value={form.price} onChange={e => setForm(p => ({ ...p, price: e.target.value }))} />
            </div>
            <div className={styles.formField}>
              <label>Кількість</label>
              <input type="number" value={form.stock} onChange={e => setForm(p => ({ ...p, stock: e.target.value }))} />
            </div>
            <div className={styles.formField}>
              <label>URL зображення</label>
              <input value={form.imageUrl} onChange={e => setForm(p => ({ ...p, imageUrl: e.target.value }))} />
            </div>
            <div className={`${styles.formField} ${styles.formFieldFull}`}>
              <label>Опис</label>
              <textarea value={form.description} onChange={e => setForm(p => ({ ...p, description: e.target.value }))} rows={3} />
            </div>
            <div className={`${styles.formField} ${styles.formFieldFull}`}>
              <label>Категорії</label>
              <div className={styles.catCheckboxes}>
                {flatCats(categories).map(cat => (
                  <label key={cat.id} className={styles.catCheckbox}>
                    <input
                      type="checkbox"
                      checked={form.categoryIds.includes(cat.id)}
                      onChange={() => toggleCat(cat.id)}
                    />
                    <span style={{ paddingLeft: cat.parentId ? 12 : 0 }}>
                      {cat.name}
                      <span className={styles.catType}> [{cat.type}]</span>
                    </span>
                  </label>
                ))}
              </div>
            </div>
          </div>
          <div className={styles.formActions}>
            <button className={styles.submitBtn} onClick={handleSubmit}>
              {editProduct ? 'Зберегти' : 'Створити'}
            </button>
            <button className={styles.cancelAbortBtn} onClick={() => setShowForm(false)}>
              Скасувати
            </button>
          </div>
        </div>
      )}

      {/* Table */}
      {loading ? (
        <div className={styles.loading}>Завантаження...</div>
      ) : (
        <div className={styles.table}>
          <div className={styles.tableHeader}>
            <span>ID</span>
            <span>Назва</span>
            <span>Ціна</span>
            <span>Склад</span>
            <span>Категорії</span>
            <span>Дії</span>
          </div>
          {filtered.map(p => (
            <div key={p.id} className={styles.tableRow}>
              <span className={styles.tableId}>#{p.id}</span>
              <span className={styles.tableName}>{p.name}</span>
              <span className={styles.tablePrice}>₴{p.price.toLocaleString('uk-UA')}</span>
              <span className={p.stock === 0 ? styles.stockOut : styles.stockOk}>
                {p.stock} шт
              </span>
              <span className={styles.tableCats}>
                {p.categories.map(c => c.name).join(', ')}
              </span>
              <div className={styles.tableActions}>
                <button className={styles.editBtn} onClick={() => openEdit(p)}>✏</button>
                <button className={styles.deleteBtn} onClick={() => handleDelete(p.id)}>✕</button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

// ===== CATEGORIES =====
function CategoriesSection() {
  const [categories, setCategories] = useState<CategoryDtoResponse[]>([]);
  const [loading, setLoading] = useState(true);
  const [name, setName] = useState('');
  const [type, setType] = useState('part_type');
  const [parentId, setParentId] = useState<number | undefined>();

  const flatCats = (cats: CategoryDtoResponse[]): CategoryDtoResponse[] =>
    cats.flatMap(c => [c, ...flatCats(c.children || [])]);

  useEffect(() => {
    categoryApi.getAll()
      .then(setCategories)
      .finally(() => setLoading(false));
  }, []);

  const handleCreate = async () => {
    if (!name.trim()) return;
    try {
      await adminApi.createCategory(name, type, parentId);
      const updated = await categoryApi.getAll();
      setCategories(updated);
      setName('');
    } catch (err: any) {
      alert(err.message);
    }
  };

  const handleDelete = async (id: number) => {
    if (!confirm('Видалити категорію?')) return;
    try {
      await adminApi.deleteCategory(id);
      const updated = await categoryApi.getAll();
      setCategories(updated);
    } catch (err: any) {
      alert(err.message);
    }
  };

  return (
    <div>
      <div className={styles.sectionHeader}>
        <h2 className={styles.sectionTitle}>Категорії</h2>
      </div>

      {/* Create form */}
      <div className={styles.formCard}>
        <div className={styles.formTitle}>Нова категорія</div>
        <div className={styles.formGrid}>
          <div className={styles.formField}>
            <label>Назва</label>
            <input value={name} onChange={e => setName(e.target.value)} placeholder="Назва..." />
          </div>
          <div className={styles.formField}>
            <label>Тип</label>
            <select value={type} onChange={e => setType(e.target.value)}>
              <option value="part_type">Тип запчастини</option>
              <option value="brand">Бренд</option>
              <option value="model">Модель</option>
              <option value="country">Країна</option>
            </select>
          </div>
          <div className={styles.formField}>
            <label>Батьківська категорія</label>
            <select
              value={parentId ?? ''}
              onChange={e => setParentId(e.target.value ? Number(e.target.value) : undefined)}
            >
              <option value="">Немає (коренева)</option>
              {flatCats(categories).map(c => (
                <option key={c.id} value={c.id}>
                  {c.parentId ? `  └ ${c.name}` : c.name} [{c.type}]
                </option>
              ))}
            </select>
          </div>
        </div>
        <div className={styles.formActions}>
          <button className={styles.submitBtn} onClick={handleCreate}>Створити</button>
        </div>
      </div>

      {/* List */}
      {loading ? (
        <div className={styles.loading}>Завантаження...</div>
      ) : (
        <div className={styles.table}>
          <div className={styles.tableHeader}>
            <span>ID</span>
            <span>Назва</span>
            <span>Тип</span>
            <span>Батьківська</span>
            <span>Товарів</span>
            <span>Дії</span>
          </div>
          {flatCats(categories).map(cat => (
            <div key={cat.id} className={styles.tableRow}>
              <span className={styles.tableId}>#{cat.id}</span>
              <span style={{ paddingLeft: cat.parentId ? 16 : 0 }}>{cat.name}</span>
              <span className={styles.catType}>{cat.type}</span>
              <span className={styles.tableId}>
                {cat.parentId ? `#${cat.parentId}` : '—'}
              </span>
              <span>{cat.productCount}</span>
              <div className={styles.tableActions}>
                <button className={styles.deleteBtn} onClick={() => handleDelete(cat.id)}>✕</button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}