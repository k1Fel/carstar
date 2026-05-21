// ============================================
// CARSTAR — App.tsx
// ============================================
import { useState, useEffect } from 'react';
import { AuthProvider } from './hooks/useAuth';
import { CartProvider } from './hooks/useCart';
import { UnauthorizedError } from './api';
import type { ProductDto } from './types';


import Header      from './components/Header';
import HeroSection from './components/HeroSection';
import AuthModal   from './components/AuthModal';
import CartSidebar from './components/CartSidebar';
import CatalogPage from './pages/CatalogPage';
import ProductPage from './pages/ProductPage';
import ProfilePage from './pages/ProfilePage';
import FavoritesPage from './pages/FavoritesPage';
import AdminPage from './pages/AdminPage';
import './styles/tokens.css';

type Page = 'home' | 'catalog' | 'product' | 'profile' | 'orders' | 'selector' | 'favorites' | 'admin';

export default function App() {
  const [page, setPage]         = useState<Page>('home');
  const [authOpen, setAuthOpen] = useState(false);
  const [cartOpen, setCartOpen] = useState(false);
  const [selectedProductId, setSelectedProductId] = useState<number | null>(null);

  // Стек навігації для кнопки "Назад"
  const [historyStack, setHistoryStack] = useState<Page[]>([]);

  // Навігація з запам'ятовуванням
  const navigate = (p: Page) => {
    setHistoryStack(prev => [...prev, page]);
    setPage(p);
  };

  // Назад
  const goBack = () => {
    if (historyStack.length === 0) {
      setPage('home');
      return;
    }
    const prev = historyStack[historyStack.length - 1];
    setHistoryStack(h => h.slice(0, -1));
    setPage(prev);
  };

  // Перехід на продукт
  const goProduct = (product: ProductDto) => {
    setHistoryStack(prev => [...prev, page]);
    setSelectedProductId(product.id);
    setPage('product');
  };

  // Глобальний обробник 401
  useEffect(() => {
    const handleUnauthorized = (e: PromiseRejectionEvent) => {
      if (e.reason instanceof UnauthorizedError) {
        e.preventDefault();
        setAuthOpen(true);
      }
    };

    window.addEventListener('unhandledrejection', handleUnauthorized);
    return () => window.removeEventListener('unhandledrejection', handleUnauthorized);
  }, []);

  return (
    <AuthProvider>
      <CartProvider>
        <div style={{ minHeight: '100vh', background: 'var(--bg)' }}>
          <Header
            currentPage={page}
            onNavigate={(p) => navigate(p as Page)}
            onAuthClick={() => setAuthOpen(true)}
            onCartClick={() => setCartOpen(true)}
          />

          <main>
            {page === 'home' && (
              <>
                <HeroSection onCatalogClick={() => navigate('catalog')} />
                <CatalogPage
                  onBack={goBack}
                  onAuthRequired={() => setAuthOpen(true)}
                  onProductClick={goProduct}
                />
              </>
            )}

            {page === 'catalog' && (
              <CatalogPage
                onBack={goBack}
                onAuthRequired={() => setAuthOpen(true)}
                onProductClick={goProduct}
              />
            )}

            {page === 'product' && selectedProductId && (
              <ProductPage
                productId={selectedProductId}
                onBack={goBack}
                onAuthRequired={() => setAuthOpen(true)}
              />
            )}

            {(page === 'profile' || page === 'orders') && (
              <ProfilePage onBack={goBack} />
            )}

            {page === 'selector' && (
              <PlaceholderPage title="Підбір за авто" />
            )}

            {page === 'favorites' && (
              <FavoritesPage
                onBack={goBack}
                onAuthRequired={() => setAuthOpen(true)}
                onProductClick={goProduct}
              />
              
            )}
            {page === 'admin' && (
              <AdminPage onBack={goBack} />
            )}
          </main>

          {authOpen && <AuthModal onClose={() => setAuthOpen(false)} />}
          {cartOpen && <CartSidebar onClose={() => setCartOpen(false)} />}
        </div>
      </CartProvider>
    </AuthProvider>
  );
}

function PlaceholderPage({ title }: { title: string }) {
  return (
    <div style={{
      maxWidth: 1280, margin: '0 auto', padding: '100px 24px',
      textAlign: 'center', color: 'var(--t4)',
      fontFamily: 'var(--fc)', fontSize: 36, fontWeight: 700,
      textTransform: 'uppercase', letterSpacing: '.08em',
    }}>
      {title}
    </div>
  );
}