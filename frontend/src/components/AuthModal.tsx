// ============================================
// CARSTAR — AuthModal
// POST /api/account/login
// POST /api/account/register
// ============================================
import { useState } from 'react';
import { useAuth } from '../hooks/useAuth';
import styles from './AuthModal.module.css';

interface AuthModalProps {
  onClose: () => void;
}

type Tab = 'login' | 'register';

export default function AuthModal({ onClose }: AuthModalProps) {
  const { login, register } = useAuth();
  const [tab, setTab] = useState<Tab>('login');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  // Login fields
  const [loginEmail, setLoginEmail] = useState('');
  const [loginPassword, setLoginPassword] = useState('');

  // Register fields
  const [regEmail, setRegEmail]       = useState('');
  const [regPassword, setRegPassword] = useState('');
  const [regUserName, setRegUserName] = useState('');

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      await login({ email: loginEmail, password: loginPassword });
      onClose();
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleRegister = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    // Client-side mirrors backend validation
    if (regPassword.length < 6) {
      setError('Пароль повинен містити не менше 6 символів');
      return;
    }
    if (!regEmail.includes('@')) {
      setError('Невірний формат email');
      return;
    }
    if (!regUserName.trim()) {
      setError("Ім'я користувача не може бути порожнім");
      return;
    }
    setLoading(true);
    try {
      await register({ email: regEmail, password: regPassword, userName: regUserName });
      onClose();
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className={styles.overlay} onClick={e => e.target === e.currentTarget && onClose()}>
      <div className={styles.modal}>
        {/* Header */}
        <div className={styles.header}>
          <span className={styles.title}>
            <span className={styles.dot} />
            CARSTAR
          </span>
          <button className={styles.close} onClick={onClose}>✕</button>
        </div>

        {/* Tabs */}
        <div className={styles.tabs}>
          <button
            className={`${styles.tab} ${tab === 'login' ? styles.tabActive : ''}`}
            onClick={() => { setTab('login'); setError(''); }}
          >
            Увійти
          </button>
          <button
            className={`${styles.tab} ${tab === 'register' ? styles.tabActive : ''}`}
            onClick={() => { setTab('register'); setError(''); }}
          >
            Реєстрація
          </button>
        </div>

        <div className={styles.body}>
          {tab === 'login' ? (
            <form onSubmit={handleLogin}>
              <div className={styles.field}>
                <label>Email</label>
                <input
                  type="email"
                  placeholder="your@email.com"
                  value={loginEmail}
                  onChange={e => setLoginEmail(e.target.value)}
                  required
                  autoFocus
                />
              </div>
              <div className={styles.field}>
                <label>Пароль</label>
                <input
                  type="password"
                  placeholder="••••••••"
                  value={loginPassword}
                  onChange={e => setLoginPassword(e.target.value)}
                  required
                />
              </div>
              <div className={styles.footer}>
                <label className={styles.remember}>
                  <input type="checkbox" /> Запам'ятати
                </label>
                <span className={styles.forgot}>Забули пароль?</span>
              </div>
              {error && <div className={styles.error}>{error}</div>}
              <button type="submit" className={styles.submit} disabled={loading}>
                {loading ? 'Завантаження...' : 'Увійти'}
              </button>
            </form>
          ) : (
            <form onSubmit={handleRegister}>
              <div className={styles.field}>
                <label>Ім'я користувача</label>
                <input
                  type="text"
                  placeholder="john_doe"
                  value={regUserName}
                  onChange={e => setRegUserName(e.target.value)}
                  required
                  autoFocus
                />
              </div>
              <div className={styles.field}>
                <label>Email</label>
                <input
                  type="email"
                  placeholder="your@email.com"
                  value={regEmail}
                  onChange={e => setRegEmail(e.target.value)}
                  required
                />
              </div>
              <div className={styles.field}>
                <label>Пароль (мін. 6 символів)</label>
                <input
                  type="password"
                  placeholder="••••••••"
                  value={regPassword}
                  onChange={e => setRegPassword(e.target.value)}
                  required
                  minLength={6}
                />
              </div>
              {error && <div className={styles.error}>{error}</div>}
              <button type="submit" className={styles.submit} disabled={loading}>
                {loading ? 'Реєстрація...' : 'Зареєструватись'}
              </button>
            </form>
          )}
        </div>
      </div>
    </div>
  );
}
