// ============================================
// CARSTAR — API Client
// Base: http://localhost:5253
// All endpoints from feature-api branch
// ============================================

import type {
  LoginDto, RegisterDto, AuthResponse, AccountResponseDto,
  CategoryDtoResponse,
  ProductDto, CreateProductDto, UpdateProductDto, ProductFilterParams,
  CartResponseDto, AddToCartDto, UpdateCartItemDto,
  ResponseOrderDto, CreateOrderDto, UpdateOrderStatusDto,
} from '../types';

const BASE = 'http://localhost:5253/api';

function getToken(): string | null {
  return localStorage.getItem('carstar_token');
}

async function request<T>(
  path: string,
  options: RequestInit = {}
): Promise<T> {
  const token = getToken();
  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
    ...(options.headers as Record<string, string> ?? {}),
  };
  if (token) headers['Authorization'] = `Bearer ${token}`;

  const res = await fetch(`${BASE}${path}`, { ...options, headers });

  if (!res.ok) {
    const err = await res.json().catch(() => ({ message: `HTTP ${res.status}` }));
    throw new Error(err.message ?? `HTTP ${res.status}`);
  }

  // 204 No Content
  if (res.status === 204) return undefined as T;
  return res.json();
}

// ============================================
// Account  →  /api/account
// ============================================
export const accountApi = {
  register: (dto: RegisterDto) =>
    request<AuthResponse>('/account/register', {
      method: 'POST',
      body: JSON.stringify(dto),
    }),

  login: (dto: LoginDto) =>
    request<AuthResponse>('/account/login', {
      method: 'POST',
      body: JSON.stringify(dto),
    }),

  // Requires JWT: Authorization: Bearer <token>
  getProfile: () =>
    request<AccountResponseDto>('/account/profile'),
};

// ============================================
// Categories  →  /api/categories
// ============================================
export const categoryApi = {
  getAll: () =>
    request<CategoryDtoResponse[]>('/categories'),

  getById: (id: number) =>
    request<CategoryDtoResponse>(`/categories/${id}`),

  create: (name: string) =>
    request<CategoryDtoResponse>('/categories', {
      method: 'POST',
      body: JSON.stringify({ name }),
    }),

  update: (id: number, name: string) =>
    request<CategoryDtoResponse>(`/categories/${id}`, {
      method: 'PUT',
      body: JSON.stringify({ id, name }),
    }),

  delete: (id: number) =>
    request<{ message: string }>(`/categories/${id}`, { method: 'DELETE' }),
};

// ============================================
// Products  →  /api/products
// ============================================
export const productApi = {
  getAll: () =>
    request<ProductDto[]>('/products'),

  getById: (id: number) =>
    request<ProductDto>(`/products/${id}`),

  filter: (params: ProductFilterParams) => {
    const q = new URLSearchParams();
    if (params.categoryId !== undefined) q.set('categoryId', String(params.categoryId));
    if (params.minPrice   !== undefined) q.set('minPrice',   String(params.minPrice));
    if (params.maxPrice   !== undefined) q.set('maxPrice',   String(params.maxPrice));
    if (params.search)                   q.set('search',     params.search);
    if (params.inStock    !== undefined) q.set('inStock',    String(params.inStock));
    if (params.similar)                  q.set('similar',    params.similar);
    return request<ProductDto[]>(`/products/filter?${q}`);
  },

  create: (dto: CreateProductDto) =>
    request<ProductDto>('/products', {
      method: 'POST',
      body: JSON.stringify(dto),
    }),

  update: (id: number, dto: UpdateProductDto) =>
    request<ProductDto>(`/products/${id}`, {
      method: 'PUT',
      body: JSON.stringify(dto),
    }),

  delete: (id: number) =>
    request<{ message: string }>(`/products/${id}`, { method: 'DELETE' }),
};

// ============================================
// Cart  →  /api/cart
// ============================================
export const cartApi = {
  getCart: () =>
    request<CartResponseDto>('/cart'),

  addItem: (dto: AddToCartDto) =>
    request<CartResponseDto>('/cart', {
      method: 'POST',
      body: JSON.stringify(dto),
    }),

  updateItem: (dto: UpdateCartItemDto) =>
    request<CartResponseDto>('/cart', {
      method: 'PUT',
      body: JSON.stringify(dto),
    }),

  deleteItem: (cartItemId: number) =>
    request<CartResponseDto>(`/cart/${cartItemId}`, {
      method: 'DELETE',
    }),

  clearCart: () =>
    request<CartResponseDto>('/cart/clear', {
      method: 'DELETE',
    }),
};

// ============================================
// Orders  →  /api/orders
// ============================================
export const orderApi = {
  create: (dto: CreateOrderDto) =>
    request<ResponseOrderDto>('/orders', {
      method: 'POST',
      body: JSON.stringify(dto),
    }),

  getById: (id: number) =>
    request<ResponseOrderDto>(`/orders/${id}`),

  getMyOrders: () =>
    request<ResponseOrderDto[]>('/orders'),          // ← було getByUser

  getAll: () =>
    request<ResponseOrderDto[]>('/orders/all'),

  updateStatus: (id: number, dto: UpdateOrderStatusDto) =>
    request<ResponseOrderDto>(`/orders/${id}/status`, {
      method: 'PATCH',                               // ← бекенд використовує HttpPatch
      body: JSON.stringify(dto),
    }),

  cancel: (id: number) =>
    request<{ message: string }>(`/orders/${id}/cancel`, {
      method: 'DELETE',
    }),
};
