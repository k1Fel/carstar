// ============================================
// CARSTAR — Cart Hook
// Syncs with GET/POST /api/cart/{accountId}
// ============================================

import { useState, useEffect, createContext, useContext, ReactNode } from 'react';
import type { CartResponseDto } from '../types';
import { cartApi } from '../api';
import { useAuth } from './useAuth';

interface CartContextType {
  cart: CartResponseDto | null;
  itemsCount: number;
  loading: boolean;
  addItem: (productId: number, quantity?: number) => Promise<void>;
  updateItem: (cartItemId: number, quantity: number) => Promise<void>;
  removeItem: (productId: number) => Promise<void>;
  clearCart: () => Promise<void>;
  refresh: () => Promise<void>;
}

const CartContext = createContext<CartContextType | null>(null);

export function CartProvider({ children }: { children: ReactNode }) {
  const { account, isAuthenticated } = useAuth();
  const [cart, setCart] = useState<CartResponseDto | null>(null);
  const [loading, setLoading] = useState(false);

  const refresh = async () => {
    if (!account) return;
    try {
      const data = await cartApi.getCart();
      setCart(data);
    } catch {
      setCart(null);
    }
  };

  useEffect(() => {
    if (isAuthenticated) refresh();
    else setCart(null);
  }, [isAuthenticated, account?.id]);

  const wrap = async (fn: () => Promise<CartResponseDto>) => {
    setLoading(true);
    try {
      const data = await fn();
      setCart(data);
    } finally {
      setLoading(false);
    }
  };

  const addItem = (productId: number, quantity = 1) =>
  wrap(() => cartApi.addItem({ productId, quantity }));

const updateItem = (cartItemId: number, quantity: number) =>
  wrap(() => cartApi.updateItem({ cartItemId, quantity }));

const removeItem = (cartItemId: number) =>
  wrap(() => cartApi.deleteItem(cartItemId));

const clearCart = () =>
  wrap(() => cartApi.clearCart());

  return (
    <CartContext.Provider value={{
      cart,
      itemsCount: cart?.itemsCount ?? 0,
      loading,
      addItem, updateItem, removeItem, clearCart, refresh,
    }}>
      {children}
    </CartContext.Provider>
  );
}

export function useCart() {
  const ctx = useContext(CartContext);
  if (!ctx) throw new Error('useCart must be inside CartProvider');
  return ctx;
}
