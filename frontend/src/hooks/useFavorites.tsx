import { useState, useEffect } from 'react';
import { favoriteApi } from '../api';
import { useAuth } from './useAuth';

export function useFavorites() {
  const { isAuthenticated } = useAuth();
  const [favorites, setFavorites] = useState<number[]>([]);
  const [loading, setLoading] = useState(false);

  // Завантажуємо улюблені тільки якщо авторизовані
  useEffect(() => {
    if (!isAuthenticated) {
      setFavorites([]);
      return;
    }

    setLoading(true);
    favoriteApi.getIds()
      .then(setFavorites)
      .catch(() => setFavorites([]))
      .finally(() => setLoading(false));
  }, [isAuthenticated]);

  const toggleFavorite = async (productId: number) => {
    if (!isAuthenticated) return;

    const isFav = favorites.includes(productId);

    // Оптимістичне оновлення
    setFavorites(prev =>
      isFav ? prev.filter(id => id !== productId) : [...prev, productId]
    );

    try {
      if (isFav) {
        await favoriteApi.remove(productId);
      } else {
        await favoriteApi.add(productId);
      }
    } catch {
      // Відкатити якщо помилка
      setFavorites(prev =>
        isFav ? [...prev, productId] : prev.filter(id => id !== productId)
      );
    }
  };

  const isFavorite = (productId: number) => favorites.includes(productId);

  return { favorites, toggleFavorite, isFavorite, loading };
}