// ============================================
// CARSTAR — Auth Hook
// Manages JWT token + account state
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

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(
    () => localStorage.getItem(TOKEN_KEY)
  );
  const [account, setAccount] = useState<AccountResponseDto | null>(() => {
    const raw = localStorage.getItem(ACCOUNT_KEY);
    return raw ? JSON.parse(raw) : null;
  });

  const persist = (t: string, a: AccountResponseDto) => {
    localStorage.setItem(TOKEN_KEY, t);
    localStorage.setItem(ACCOUNT_KEY, JSON.stringify(a));
    setToken(t);
    setAccount(a);
  };

  const login = async (dto: LoginDto) => {
    const res = await accountApi.login(dto);
    persist(res.token, res.account);
  };

  const register = async (dto: RegisterDto) => {
    const res = await accountApi.register(dto);
    persist(res.token, res.account);
  };

  const logout = () => {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(ACCOUNT_KEY);
    setToken(null);
    setAccount(null);
  };

  return (
    <AuthContext.Provider value={{
      account, token,
      isAuthenticated: !!token && !!account,
      login, register, logout,
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
