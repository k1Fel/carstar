import { useEffect, useState } from 'react';

const STORAGE_KEY = 'favorites';

export function useFavorites() {
  const [favorites, setFavorites] = useState<number[]>([]);

  useEffect(() => {
    const data = localStorage.getItem(STORAGE_KEY);

    if (data) {
      setFavorites(JSON.parse(data));
    }
  }, []);

  const save = (items: number[]) => {
    setFavorites(items);
    localStorage.setItem(STORAGE_KEY, JSON.stringify(items));
  };

  const toggleFavorite = (id: number) => {
    if (favorites.includes(id)) {
      save(favorites.filter(x => x !== id));
    } else {
      save([...favorites, id]);
    }
  };

  const isFavorite = (id: number) => {
    return favorites.includes(id);
  };

  return {
    favorites,
    toggleFavorite,
    isFavorite,
  };
}