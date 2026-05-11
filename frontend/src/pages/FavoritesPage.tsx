import { useEffect, useState } from 'react';
import { productApi } from '../api';
import ProductCard from '../components/ProductCard';
import { useFavorites } from '../hooks/useFavorites';
import type { ProductDto } from '../types';

interface Props {
  onAuthRequired: () => void;
  onProductClick: (product: ProductDto) => void;
}

export default function FavoritesPage({
  onAuthRequired,
  onProductClick,
}: Props) {
  const { favorites } = useFavorites();

  const [products, setProducts] = useState<ProductDto[]>([]);

  useEffect(() => {
    productApi.filter({})
      .then(data => {
        setProducts(
          data.filter(x => favorites.includes(x.id))
        );
      });
  }, [favorites]);

  return (
    <div style={{ padding: 24 }}>
      <h2>Уподобані товари</h2>

      <div
        style={{
          display: 'grid',
          gridTemplateColumns: 'repeat(auto-fill,minmax(260px,1fr))',
          gap: 20,
        }}
      >
        {products.map(p => (
          <ProductCard
            key={p.id}
            product={p}
            onAuthRequired={onAuthRequired}
            onClick={onProductClick}
          />
        ))}
      </div>
    </div>
  );
}