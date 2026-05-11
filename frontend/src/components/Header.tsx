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
    { id: 'catalog',  label: 'Каталог' },
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
          <button
              className={styles.iconBtn}
              title="Пошук"
              onClick={() => onNavigate('catalog')}
            >
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

          {isAuthenticated ? (
            <div className={styles.profileWrap}>
              <button
                className={styles.iconBtn}
                onClick={() => setProfileOpen(p => !p)}
                title={account?.userName}
              >
                <span className={styles.avatarLetters}>
                  {account?.userName.slice(0, 2).toUpperCase()}
                </span>
              </button>
              {profileOpen && (
                <div className={styles.dropdown}>
                  <div className={styles.dropHeader}>
                    <div className={styles.dropName}>{account?.userName}</div>
                    <div className={styles.dropEmail}>{account?.email}</div>
                  </div>
                  <button
                    className={styles.dropItem}
                    onClick={() => { onNavigate('profile'); setProfileOpen(false); }}
                  >
                    Профіль
                  </button>
                  <button
                    className={styles.dropItem}
                    onClick={() => { onNavigate('orders'); setProfileOpen(false); }}
                  >
                    Мої замовлення
                  </button>
                  <div className={styles.dropSep} />
                  <button
                    className={`${styles.dropItem} ${styles.dropLogout}`}
                    onClick={() => { logout(); setProfileOpen(false); }}
                  >
                    Вийти
                  </button>
                </div>
              )}
            </div>
          ) : (
            <button
              className={styles.authBtn}
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
    <circle cx="7" cy="7" r="4.5" stroke="currentColor" strokeWidth="1.2"/>
    <path d="M10.5 10.5L13.5 13.5" stroke="currentColor" strokeWidth="1.2" strokeLinecap="round"/>
  </svg>
);
const HeartIcon = () => (
  <svg width="16" height="16" viewBox="0 0 16 16" fill="none">
    <path d="M8 13s-6-3.5-6-7.5C2 3.57 3.57 2 5.5 2c1.054 0 2 .5 2.5 1.5C8.5 2.5 9.446 2 10.5 2 12.43 2 14 3.57 14 5.5 14 9.5 8 13 8 13z" stroke="currentColor" strokeWidth="1.2"/>
  </svg>
);
const CartIcon = () => (
  <svg width="16" height="16" viewBox="0 0 16 16" fill="none">
    <path d="M2 2h1.5l2 7h6l1.5-5H5" stroke="currentColor" strokeWidth="1.2" strokeLinecap="round" strokeLinejoin="round"/>
    <circle cx="7" cy="13" r="1" fill="currentColor"/>
    <circle cx="11" cy="13" r="1" fill="currentColor"/>
  </svg>
);
