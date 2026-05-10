// ============================================
// CARSTAR — TypeScript Types
// Mirrors backend DTOs from feature-api branch
// ============================================

// --- Account ---
export interface AccountResponseDto {
  id: number;
  email: string;
  userName: string;
}

export interface LoginDto {
  email: string;
  password: string;
}

export interface RegisterDto {
  email: string;
  password: string;
  userName: string;
}

export interface AuthResponse {
  message: string;
  token: string;
  account: AccountResponseDto;
}

// --- Category ---
export interface CategoryDtoResponse {
  id: number;
  name: string;
  productCount: number;
}

// --- Product ---
export interface ProductDto {
  id: number;
  name: string;
  description: string;
  price: number;
  stock: number;
  categoryId: number;
  categoryName: string;
  imageUrl?: string; // optional — якщо бекенд повертає URL картинки
}

export interface CreateProductDto {
  name: string;
  description: string;
  price: number;
  stock: number;
  categoryId: number;
}

export interface UpdateProductDto {
  name: string;
  description: string;
  price: number;
  stock: number;
  categoryId: number;
}

// Filter params → GET /api/products/filter
export interface ProductFilterParams {
  categoryId?: number;
  minPrice?: number;
  maxPrice?: number;
  search?: string;
  inStock?: boolean;
  similar?: string;
}

// --- Cart ---
export interface CartItemResponseDto {
  id: number;
  productId: number;
  productName: string;
  price: number;
  quantity: number;
  subtotal: number;
}

export interface CartResponseDto {
  id: number;
  accountId: number;
  itemsCount: number;
  totalAmount: number;
  items: CartItemResponseDto[];
}

export interface AddToCartDto {
  productId: number;
  quantity: number;
}

export interface UpdateCartItemDto {
  cartItemId: number;
  quantity: number;
}

// --- Order ---
export type OrderStatus = 'Pending' | 'Processing' | 'Shipped' | 'Delivered' | 'Cancelled';

export interface OrderItemResponseDto {
  id: number;
  productId: number;
  quantity: number;
  price: number;
}

export interface ResponseOrderDto {
  id: number;
  accountId: number;
  totalAmount: number;
  status: OrderStatus;
  shippingAddress: string;
  createdAt: string;
  orderItems: OrderItemResponseDto[];
}

export interface CreateOrderDto {
  accountId: number;
  totalAmount: number;
  status: string;
  shippingAddress: string;
}

export interface UpdateOrderStatusDto {
  status: string;
}

// --- Auth context ---
export interface AuthState {
  account: AccountResponseDto | null;
  token: string | null;
  isAuthenticated: boolean;
}
