// ============================================
// CARSTAR — Header
// ============================================
import { useState } from 'react';
import { useAuth } from '../hooks/useAuth';
import { useCart } from '../hooks/useCart';
import styles from './Header.module.css';

interface HeaderProps {
  onAuthClick: () => void;
  onCartClick: () => void;
  currentPage: string;
  onNavigate: (page: string) => void;
}

export default function Header({
  onAuthClick, onCartClick, currentPage, onNavigate,
}: HeaderProps) {
  const { isAuthenticated, account, logout } = useAuth();
  const { itemsCount } = useCart();
  const [profileOpen, setProfileOpen] = useState(false);

  const links = [
    { id: 'catalog', label: 'Каталог' },
    { id: 'selector', label: 'Підбір за авто' },
  ];

  return (
    <header className={styles.header}>
      <nav className={styles.nav}>
        {/* Logo */}
        <button
          className={styles.logo}
          onClick={() => onNavigate('home')}
        >
          <span className={styles.logoDot} />
          CARSTAR
        </button>

        {/* Nav links */}
        <div className={styles.links}>
          {links.map(l => (
            <button
              key={l.id}
              className={`${styles.link} ${currentPage === l.id ? styles.active : ''}`}
              onClick={() => onNavigate(l.id)}
            >
              {l.label}
            </button>
          ))}
        </div>

        {/* Right icons */}
        <div className={styles.right}>
          {/* Search icon */}
          <button
            className={styles.iconBtn}
            title="Пошук"
            onClick={() => onNavigate('catalog')}
          >
            <SearchIcon />
          </button>

          <button
            className={styles.iconBtn}
            title="Уподобані"
            onClick={() => onNavigate('favorites')}
          >
            <HeartIcon />
          </button>

          <button
            className={styles.iconBtn}
            title="Кошик"
            onClick={onCartClick}
          >
            <CartIcon />
            {itemsCount > 0 && (
              <span className={styles.badge}>
                {itemsCount > 99 ? '99+' : itemsCount}
              </span>
            )}
          </button>

          {/* User Menu */}
          {isAuthenticated ? (
            <div className={styles.userMenu}>
              <button
                className={styles.userButton}
                onClick={() => setProfileOpen(p => !p)}
              >
                <div
                  className={`${styles.userAvatar} ${
                    account?.role === 'admin' ? styles.adminAvatar : ''
                  }`}
                >
                  {account?.userName?.[0]?.toUpperCase()}
                </div>

                <span className={styles.userName}>
                  {account?.userName}
                </span>

                <svg width="12" height="12" viewBox="0 0 12 12" fill="none">
                  <path
                    d="M3 4.5L6 7.5L9 4.5"
                    stroke="currentColor"
                    strokeWidth="2"
                    strokeLinecap="round"
                    strokeLinejoin="round"
                  />
                </svg>
              </button>

              {profileOpen && (
                <div className={styles.dropdown}>
                  {/* Admin badge */}
                  {account?.role === 'admin' && (
                    <div className={styles.dropdownAdmin}>
                      <span className={styles.adminBadgeText}>
                        ADMIN
                      </span>
                    </div>
                  )}

                  <button
                    className={styles.dropdownItem}
                    onClick={() => {
                      onNavigate('profile');
                      setProfileOpen(false);
                    }}
                  >
                   🏠︎ Профіль
                  </button>

                  <button
                    className={styles.dropdownItem}
                    onClick={() => {
                      onNavigate('orders');
                      setProfileOpen(false);
                    }}
                  >
                    ⛟ Замовлення
                  </button>

                  {account?.role === 'admin' && (
                    <button
                      className={styles.dropdownItem}
                      onClick={() => {
                        onNavigate('admin');
                        setProfileOpen(false);
                      }}
                    >
                      ᯓ★ Адмін панель
                    </button>
                  )}

                  <div className={styles.dropdownDivider}></div>

                  <button
                    className={styles.dropdownItem}
                    onClick={() => {
                      logout();
                      setProfileOpen(false);
                    }}
                  >
                    ➜] Вийти
                  </button>
                </div>
              )}
            </div>
          ) : (
            <button
              className={styles.loginButton}
              onClick={onAuthClick}
            >
              Увійти
            </button>
          )}
        </div>
      </nav>
    </header>
  );
}

// --- Inline SVG Icons ---
const SearchIcon = () => (
  <svg width="16" height="16" viewBox="0 0 16 16" fill="none">
    <circle cx="7" cy="7" r="4.5" stroke="currentColor" strokeWidth="1.2" />
    <path d="M10.5 10.5L13.5 13.5" stroke="currentColor" strokeWidth="1.2" strokeLinecap="round" />
  </svg>
);

const HeartIcon = () => (
  <svg width="16" height="16" viewBox="0 0 16 16" fill="none">
    <path d="M8 13s-6-3.5-6-7.5C2 3.57 3.57 2 5.5 2c1.054 0 2 .5 2.5 1.5C8.5 2.5 9.446 2 10.5 2 12.43 2 14 3.57 14 5.5 14 9.5 8 13 8 13z" stroke="currentColor" strokeWidth="1.2" />
  </svg>
);

const CartIcon = () => (
  <svg width="16" height="16" viewBox="0 0 16 16" fill="none">
    <path d="M2 2h1.5l2 7h6l1.5-5H5" stroke="currentColor" strokeWidth="1.2" strokeLinecap="round" strokeLinejoin="round" />
    <circle cx="7" cy="13" r="1" fill="currentColor" />
    <circle cx="11" cy="13" r="1" fill="currentColor" />
  </svg>
);