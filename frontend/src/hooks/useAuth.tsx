// ============================================
// CARSTAR — useAuth.tsx
// ============================================
import { useState, useEffect, createContext, useContext, ReactNode } from 'react';
import type { AccountResponseDto, LoginDto, RegisterDto } from '../types';
import { accountApi } from '../api';

interface AuthContextType {
  account: AccountResponseDto | null;
  token: string | null;
  isAuthenticated: boolean;
  login: (dto: LoginDto) => Promise<void>;
  register: (dto: RegisterDto) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | null>(null);

const TOKEN_KEY = 'carstar_token';
const ACCOUNT_KEY = 'carstar_account';
const REFRESH_KEY = 'carstar_refresh_token';

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(
    () => localStorage.getItem(TOKEN_KEY)
  );
  const [account, setAccount] = useState<AccountResponseDto | null>(() => {
    const raw = localStorage.getItem(ACCOUNT_KEY);
    return raw ? JSON.parse(raw) : null;
  });

  const persist = (t: string, rt: string, a: AccountResponseDto) => {
      localStorage.setItem(TOKEN_KEY, t);
      localStorage.setItem(REFRESH_KEY, rt);
      localStorage.setItem(ACCOUNT_KEY, JSON.stringify(a));
      setToken(t);
      setAccount(a);
  };

  const clear = () => {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(ACCOUNT_KEY);
    localStorage.removeItem(REFRESH_KEY);
    localStorage.removeItem('favorites');  
    setToken(null);
    setAccount(null);
  };

  const login = async (dto: LoginDto) => {
      const res = await accountApi.login(dto);
      persist(res.token, res.refreshToken, res.account);
  };

  const register = async (dto: RegisterDto) => {
    const res = await accountApi.register(dto);
    persist(res.token, res.refreshToken, res.account);
  };

  const logout = () => clear();

  // Слухаємо storage — якщо api/index.ts видалив токен через 401
  useEffect(() => {
    const handleStorage = (e: StorageEvent) => {
      if (e.key === TOKEN_KEY && !e.newValue) {
        setToken(null);
        setAccount(null);
      }
    };

    window.addEventListener('storage', handleStorage);
    return () => window.removeEventListener('storage', handleStorage);
  }, []);

  // Перевіряємо токен при старті — якщо протух очищаємо
  useEffect(() => {
    if (!token) return;

    accountApi.getProfile().catch(() => {
      clear();
    });
  }, []); // тільки при монтуванні

  return (
    <AuthContext.Provider value={{
      account,
      token,
      isAuthenticated: !!token && !!account,
      login,
      register,
      logout,
    }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be inside AuthProvider');
  return ctx;
}