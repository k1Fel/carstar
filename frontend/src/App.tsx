// ============================================
// CARSTAR — App (full)
// ============================================
import { useState } from 'react';
import { AuthProvider } from './hooks/useAuth';
import { CartProvider } from './hooks/useCart';
import type { ProductDto } from './types';

import Header      from './components/Header';
import HeroSection from './components/HeroSection';
import AuthModal   from './components/AuthModal';
import CartSidebar from './components/CartSidebar';
import CatalogPage from './pages/CatalogPage';
import ProductPage from './pages/ProductPage';
import ProfilePage from './pages/ProfilePage';
import FavoritesPage from './pages/FavoritesPage';
import './styles/tokens.css';

type Page = 'home' | 'catalog' | 'product' | 'profile' | 'orders'  | 'selector' | 'favorites';

export default function App() {
  const [page, setPage]         = useState<Page>('home');
  const [authOpen, setAuthOpen] = useState(false);
  const [cartOpen, setCartOpen] = useState(false);
  const [selectedProductId, setSelectedProductId] = useState<number | null>(null);

  const goProduct = (product: ProductDto) => {
    setSelectedProductId(product.id);
    setPage('product');
  };

  return (
    <AuthProvider>
      <CartProvider>
        <div style={{ minHeight: '100vh', background: 'var(--bg)' }}>
          <Header
            currentPage={page}
            onNavigate={(p) => setPage(p as Page)}
            onAuthClick={() => setAuthOpen(true)}
            onCartClick={() => setCartOpen(true)}
          />

          <main>
            {page === 'home' && (
              <>
                <HeroSection onCatalogClick={() => setPage('catalog')} />
                <CatalogPage
                  onAuthRequired={() => setAuthOpen(true)}
                  onProductClick={goProduct}
                />
              </>
            )}

            {page === 'catalog' && (
              <CatalogPage
                onAuthRequired={() => setAuthOpen(true)}
                onProductClick={goProduct}
              />
            )}

            {page === 'product' && selectedProductId && (
              <ProductPage
                productId={selectedProductId}
                onBack={() => setPage('catalog')}
                onAuthRequired={() => setAuthOpen(true)}
              />
            )}

            {(page === 'profile' || page === 'orders') && <ProfilePage />}

            {(page === 'selector') && (
              <PlaceholderPage title={
                page === 'selector' ? 'Підбір за авто' : 'Улюблені'} />
            )}
            {page === 'favorites' && (
              <FavoritesPage
                onAuthRequired={() => setAuthOpen(true)}
                onProductClick={goProduct}
              />
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
